using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Events;
using SAUEP.WPF.Configs;
using SAUEP.WPF.Events;
using SAUEP.Core.Interfaces;
using SAUEP.Core.Repositories;
using SAUEP.Core.Services;
using SAUEP.Core.Models;
using SAUEP.Core.Exceptions;
using System.Collections.ObjectModel;

namespace SAUEP.WPF.ViewModels
{
    public sealed class CreateUserPageViewModel : BindableBase
    {
        #region Constructors

        public CreateUserPageViewModel(IEventAggregator eventAggregator, InternetConnectionChecker internetConnectionChecker, AuthorizationService authorizationService,
            IGuardian guardian, UserRepository userRepository, RoleRepository roleRepository,
            IModel user)
        {
            _eventAggregator = eventAggregator;
            _internetConnectionChecker = internetConnectionChecker;
            _authorizationService = authorizationService;
            _guardian = guardian;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userModel = user as UserModel;
            _roles = new ObservableCollection<string>();
            _users = new List<UserModel>();
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
                .GetEvent<GoOnCreateUserPageEvent>()
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

        public RelayCommand Back
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    _roles.Clear();
                    _eventAggregator
                        .GetEvent<ChangePageEvent>()
                        .Publish("DispatcherPage.xaml");
                    _eventAggregator
                           .GetEvent<GoOnDispatcherPageEvent>()
                           .Publish();
                });
            }
        }
        public RelayCommand Create
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    CreateAsync();
                });
            }
        }

        #endregion



        #region Main Logic
        private async void GetWrapper()
        {
            await GetRoles();
            await GetUsers();
        }
        private async Task<bool> GetRoles()
        {
            List<RoleModel> roles = new List<RoleModel>();
            try
            {
                roles = (await _roleRepository.Get<RoleModel>(_userModel.Token)).ToList();
            }
            catch (TokenLifetimeException ex)
            {
                await UpdateToken();
                roles = (await _roleRepository.Get<RoleModel>(_userModel.Token)).ToList();
            }
            finally
            {
                for (var i = 0; i < roles.Count; i++)
                {
                    Roles.Add(roles[i].Title);
                }
                OnPropertyChanged(nameof(Roles));
            }
            return true;
        }
        private async Task<bool> GetUsers()
        {
            try
            {
                _users = (await _userRepository.Get<UserModel>(_userModel.Token)).ToList();
            }
            catch (TokenLifetimeException ex)
            {
                await UpdateToken();
                _users = (await _userRepository.Get<UserModel>(_userModel.Token)).ToList();
            }
            return true;
        }
        private async void CreateAsync()
        {
            if (await CheckLoginNotEmpty(_login) && await CheckLoginNotExists(_login) && await CheckLoginNotSmaller(_login)
                && await CheckLoginNotBigger(_login) && await CheckFirstPasswordNotEmpty(_firstPassword) && await CheckSecondPasswordNotEmpty(_secondPassword)
                && await CheckPasswordsEquals(_firstPassword, _secondPassword) && await CheckPasswordNotSmaller(_firstPassword) && await CheckPasswordNotBigger(_firstPassword)
                && await CheckEmailNotEmpty(_email) && await CheckRoleNotEmpty(_role))
            {
                await CreateUserModel(_login, _firstPassword, _email, _role);
            }
            else
                _eventAggregator
                            .GetEvent<ExceptionEvent>()
                            .Publish(new UserCreateException(_userCreateError));
        }
        private async Task<bool> CheckLoginNotEmpty(string login)
        {
            if (login != null)
                return true;
            else
                return false;
        }
        private async Task<bool> CheckLoginNotExists(string login)
        {
            if (_users.Find(user => user.Login.Equals(login)) == null)
                return true;
            else
                return false;
        }
        private async Task<bool> CheckLoginNotSmaller(string login)
        {
            if (login.Length >= 4)
                return true;
            else
                return false;
        }
        private async Task<bool> CheckLoginNotBigger(string login)
        {
            if (login.Length <= 12) 
                return true;
            else
                return false;
        }
        private async Task<bool> CheckFirstPasswordNotEmpty(string password)
        {
            if (password != null)
                return true;
            else
                return false;
        }
        private async Task<bool> CheckSecondPasswordNotEmpty(string password)
        {
            if (password != null)
                return true;
            else
                return false;
        }
        private async Task<bool> CheckPasswordsEquals(string firstPassword, string secondPassword)
        {
            if (firstPassword.Equals(secondPassword))
                return true;
            else
                return false;
        }
        private async Task<bool> CheckPasswordNotSmaller(string password)
        {
            if (password.Length >= 8)
                return true;
            else
                return false;
        }
        private async Task<bool> CheckPasswordNotBigger(string password)
        {
            if (password.Length <= 32)
                return true;
            else
                return false;
        }
        private async Task<bool> CheckEmailNotEmpty(string email)
        {
            if (email != null)
                return true;
            else
                return false;
        }
        private async Task<bool> CheckRoleNotEmpty(string role)
        {
            if (role != null)
                return true;
            else
                return false;
        }
        private async Task<bool> CreateUserModel(string login, string password, string email, string role)
        {
            try
            {
                await _userRepository.Set(new UserModel(login, password, email, default, role), _userModel.Token);
            }
            catch (TokenLifetimeException ex)
            {
                await UpdateToken();
                await _userRepository.Set(new UserModel(login, password, email, default, role), _userModel.Token);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                Login = string.Empty;
                FirstPassword = string.Empty;
                SecondPassword = string.Empty;
                Email = string.Empty;
                Roles.Clear();
                _role = string.Empty;
                _users.Clear();
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

        public ObservableCollection<string> Roles
        {
            get => _roles;
            set => SetProperty(ref _roles, value);
        }
        public string Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }
        public string FirstPassword
        {
            get => _firstPassword;
            set => SetProperty(ref _firstPassword, value);
        }
        public string SecondPassword
        {
            get => _secondPassword;
            set => SetProperty(ref _secondPassword, value);
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

        IEventAggregator _eventAggregator;
        InternetConnectionChecker _internetConnectionChecker;
        AuthorizationService _authorizationService;
        IGuardian _guardian;
        UserRepository _userRepository;
        RoleRepository _roleRepository;
        UserModel _userModel;
        private ObservableCollection<string> _roles;
        private List<UserModel> _users;
        private string _login;
        private string _firstPassword;
        private string _secondPassword;
        private string _email;
        private string _role;
        private const string _internetError = "Отсутствует подключение к интернету";
        private const string _userCreateError = "Ошибка создания!";

        #endregion
    }
}
