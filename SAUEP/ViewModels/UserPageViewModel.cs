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
using SAUEP.Core.Exceptions;
using SAUEP.WPF.Events;
using SAUEP.WPF.Configs;
using System.Windows.Threading;

namespace SAUEP.WPF.ViewModels
{
    public sealed class UserPageViewModel : BindableBase
    {
        #region Constructors

        public UserPageViewModel(IEventAggregator eventAggregator, InternetConnectionChecker internetConnectionChecker,
            UserRepository userRepository, AuthorizationService authorizationService, IGuardian guardian, 
            IModel user)
        {
            _eventAggregator = eventAggregator;
            _internetConnectionChecker = internetConnectionChecker;
            _userRepository = userRepository;
            _authorizationService = authorizationService;
            _guardian = guardian;
            _userModel = user as UserModel;
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
                .GetEvent<SelectUserModelEvent>()
                .Subscribe((details) =>
                {
                    if (_internetConnectionChecker.Check())
                    {
                        _user = details;
                        Login = details.Login;
                        Email = details.Email;
                        Role = details.Role;
                    }
                    else
                        _eventAggregator
                               .GetEvent<ExceptionEvent>()
                               .Publish(new InternetException(_internetError));
                }, ThreadOption.UIThread);
        }

        #endregion



        #region Commands

        public RelayCommand Update
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    _eventAggregator
                       .GetEvent<GoOnUpdateUserEvent>()
                       .Publish(_user);
                    _eventAggregator
                        .GetEvent<ChangePageEvent>()
                        .Publish("UpdateUserPage.xaml");
                });
            }
        }
        public RelayCommand Back
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    _eventAggregator
                        .GetEvent<ChangePageEvent>()
                        .Publish("UsersPage.xaml");
                    _eventAggregator
                           .GetEvent<GoOnUsersPage>()
                           .Publish();
                });
            }
        }
        public RelayCommand Delete
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    DeleteUserAsync();
                });
            }
        }

        #endregion



        #region Main Logic

        private async void DeleteUserAsync()
        {
            try
            {
                await _userRepository.Remove<UserModel>(_user.Id, _userModel.Token);
            }
            catch(TokenLifetimeException ex)
            {
                await UpdateToken(); 
                await _userRepository.Remove<UserModel>(_user.Id, _userModel.Token);
            }
            finally
            {
                _eventAggregator
                       .GetEvent<GoOnUsersPage>()
                       .Publish();
                _eventAggregator
                        .GetEvent<ChangePageEvent>()
                        .Publish("UsersPage.xaml");
            }
        }

        #endregion



        #region Staff

        private async Task<bool> UpdateToken()
        {
            var authorizationResponseModel = await _authorizationService.Authorize((_userModel as UserModel).Login, (_userModel as UserModel).Password);
            _eventAggregator
                          .GetEvent<ChangeTokenEvent>()
                          .Publish(authorizationResponseModel.access_token);
            (_userModel as UserModel).Token = authorizationResponseModel.access_token;
            return true;
        }

        #endregion



        #region Get/Set

        public string Login 
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }
        public string Role
        {
            get => _role;
            set => SetProperty(ref _role, value);
        }

        #endregion



        #region Fields

        private IEventAggregator _eventAggregator;
        private IGuardian _guardian;
        private UserModel _userModel;
        private InternetConnectionChecker _internetConnectionChecker;
        private UserRepository _userRepository;
        private AuthorizationService _authorizationService;
        private UserModel _user;
        private string _login;
        private string _email;
        private string _role;
        private const string _internetError = "Отсутствует подключение к интернету";

        #endregion
    }
}
