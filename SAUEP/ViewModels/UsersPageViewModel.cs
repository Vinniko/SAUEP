using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Prism.Mvvm;
using Prism.Events;
using SAUEP.Core.Interfaces;
using SAUEP.Core.Models;
using SAUEP.Core.Services;
using SAUEP.Core.Repositories;
using System.Collections.ObjectModel;
using SAUEP.WPF.Models;
using SAUEP.Core.Exceptions;
using SAUEP.WPF.Events;
using SAUEP.WPF.Configs;
using System.Windows.Threading;
using System.Net.Sockets;
using SAUEP.WPF.Services;

namespace SAUEP.WPF.ViewModels
{
    public sealed class UsersPageViewModel : BindableBase
    {
        #region Constructors

        public UsersPageViewModel(IEventAggregator eventAggregator, InternetConnectionChecker internetConnectionChecker, AuthorizationService authorizationService,
            IGuardian guardian, IModel user, UsersSorter usersSorter, UserRepository userRepository, UsersFilter usersFilter)
        {
            _eventAggregator = eventAggregator;
            _internetConnectionChecker = internetConnectionChecker;
            _authorizationService = authorizationService;
            _guardian = guardian;
            _userModel = user as UserModel;
            _userRepository = userRepository;
            _users = new ObservableCollection<UserModel>();
            _usersSorter = usersSorter;
            _usersFilter = usersFilter;
            SubscriptionToken subscriptionToken = _eventAggregator
                .GetEvent<UserCompliteEvent>()
                .Subscribe((details) =>
                {
                    _userModel = details;
                }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
                .GetEvent<ChangeTokenEvent>()
                .Subscribe((details) =>
                {
                    _userModel.Token = details;
                }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
                .GetEvent<GoOnUsersPage>()
                .Subscribe(() =>
                {
                    if (_internetConnectionChecker.Check())
                    {
                        GetWrapper();
                    }
                    else
                        _eventAggregator
                            .GetEvent<ExceptionEvent>()
                            .Publish(new InternetException(_internetError));
                }, ThreadOption.UIThread);
        }

        #endregion



        #region Commands

        public RelayCommand Sort
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    if (_sortKey.Equals(obj.ToString()))
                    {
                        _sortDirection = !_sortDirection;
                    }
                    else
                    {
                        _sortDirection = true;
                        _sortKey = obj.ToString();
                    }
                    Dispatcher.CurrentDispatcher.Invoke(() =>
                    {
                        Users = new ObservableCollection<UserModel>(
                                _usersSorter.Sort(Users.ToList(), _sortKey, _sortDirection));
                    });
                });
            }
        }
        public RelayCommand Reset
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    _sortDirection = false;
                    _sortKey = "Login";
                    LoginFilter = string.Empty;
                    EmailFilter = string.Empty;
                    RoleFilter = string.Empty;
                    _users.Clear();
                    GetWrapper();
                });
            }
        }
        public RelayCommand Filter
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    if (!_loginFilter.Equals(string.Empty))
                        Dispatcher.CurrentDispatcher.Invoke(() =>
                        {
                            Users = new ObservableCollection<UserModel>(
                                _usersFilter.Filtering(Users.ToList(), "Login", _loginFilter));
                        });
                    if (!_emailFilter.Equals(string.Empty))
                        Dispatcher.CurrentDispatcher.Invoke(() =>
                        {
                            Users = new ObservableCollection<UserModel>(
                                _usersFilter.Filtering(Users.ToList(), "Email", _emailFilter));
                        });
                    if (!_roleFilter.Equals(string.Empty))
                        Dispatcher.CurrentDispatcher.Invoke(() =>
                        {
                            Users = new ObservableCollection<UserModel>(
                                _usersFilter.Filtering(Users.ToList(), "Role", _roleFilter));
                        });
                });
            }
        }
        public RelayCommand Back
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    _users.Clear();
                    _eventAggregator
                        .GetEvent<ChangePageEvent>()
                        .Publish("DispatcherPage.xaml");
                    _eventAggregator
                           .GetEvent<GoOnDispatcherPageEvent>()
                           .Publish();
                });
            }
        }

        #endregion



        #region Main Logic

        private async void GetWrapper()
        {
            await GetUsers();
        }
        private async Task<bool> GetUsers()
        {
            var users = new List<UserModel>();
            try
            {
                users = await _userRepository.Get<UserModel>(_userModel.Token) as List<UserModel>;
            }
            catch(TokenLifetimeException ex)
            {
                UpdateToken();
                users = await _userRepository.Get<UserModel>(_userModel.Token) as List<UserModel>;
            }
            finally
            {
                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    Users = new ObservableCollection<UserModel>(
                            _usersSorter.Sort(users, _sortKey, _sortDirection));
                });
            }
            return true;
        }

        #endregion



        #region Staff

        private async Task<bool> UpdateToken()
        {
            var authorizationResponseModel = await _authorizationService.Authorize(_userModel.Login, _userModel.Password);
            _eventAggregator
                          .GetEvent<ChangeTokenEvent>()
                          .Publish(authorizationResponseModel.access_token);
            _userModel.Token = authorizationResponseModel.access_token;
            return true;
        }

        #endregion



        #region Get/Set

        public ObservableCollection<UserModel> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }
        public UserModel User
        {
            get => _user;
            set
            {
                SetProperty(ref _user, value);
                if (_user != null)
                {
                    _users.Clear();
                    _eventAggregator
                         .GetEvent<SelectUserModelEvent>()
                         .Publish(value);
                    _eventAggregator
                        .GetEvent<ChangePageEvent>()
                        .Publish("UserPage.xaml");
                }
            }
        }
        public string LoginFilter
        {
            get => _loginFilter;
            set => SetProperty(ref _loginFilter, value);
        }
        public string EmailFilter
        {
            get => _emailFilter;
            set => SetProperty(ref _emailFilter, value);
        }
        public string RoleFilter
        {
            get => _roleFilter;
            set => SetProperty(ref _roleFilter, value);
        }

        #endregion



        #region Fields

        private IEventAggregator _eventAggregator;
        private InternetConnectionChecker _internetConnectionChecker;
        private AuthorizationService _authorizationService;
        private IGuardian _guardian;
        private UserModel _userModel;
        private UserRepository _userRepository;
        private ObservableCollection<UserModel> _users;
        private UserModel _user;
        private UsersSorter _usersSorter;
        private UsersFilter _usersFilter;
        private string _sortKey = "Login";
        private string _loginFilter = string.Empty;
        private string _emailFilter = string.Empty;
        private string _roleFilter = string.Empty;
        private bool _sortDirection = false;
        private const string _internetError = "Отсутствует подключение к интернету";

        #endregion
    }
}
