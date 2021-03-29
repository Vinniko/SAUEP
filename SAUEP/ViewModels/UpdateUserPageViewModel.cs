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
    public sealed class UpdateUserPageViewModel : BindableBase
    {
        #region Constructors

        public UpdateUserPageViewModel(IEventAggregator eventAggregator, InternetConnectionChecker internetConnectionChecker, AuthorizationService authorizationService,
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
                .GetEvent<GoOnUpdateUserEvent>()
                .Subscribe((details) =>
                {
                    if (_internetConnectionChecker.Check())
                    {
                        _userId = details.Id;
                        _login = details.Login;
                        _startLogin = details.Login;
                        _oldPassword = details.Password;
                        _email = details.Email;
                        _startEmail = details.Email;
                        _role = details.Role;
                        _startRole = details.Role;
                        _user = details;
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
                    _users.Clear();
                    FirstPassword = string.Empty;
                    SecondPassword = string.Empty;
                    OldPassword = string.Empty;
                    _eventAggregator
                        .GetEvent<ChangePageEvent>()
                        .Publish("UsersPage.xaml");
                    _eventAggregator
                           .GetEvent<GoOnUsersPage>()
                           .Publish();
                });
            }
        }
        public RelayCommand Update
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    UpdateAsync();
                });
            }
        }
        public RelayCommand SetStart
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    Login = _startLogin;
                    Email = _startEmail;
                    Role = _startRole;
                    FirstPassword = string.Empty;
                    SecondPassword = string.Empty;
                    ChangeFlag = false;
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
            finally
            {
                for (var i = 0; i < _users.Count; i++)
                    if (_users[i].Login.Equals(_user.Login))
                    {
                        _users.RemoveAt(i);
                        break;
                    }
            }
            return true;
        }
        private async void UpdateAsync()
        {
            if (await CheckLoginNotEmpty(_login) && await CheckLoginNotExists(_login) && await CheckLoginNotSmaller(_login)
                && await CheckLoginNotBigger(_login) && await CheckFirstPasswordNotEmpty(_firstPassword) && await CheckSecondPasswordNotEmpty(_secondPassword)
                && await CheckPasswordsEquals(_firstPassword, _secondPassword) && await CheckPasswordNotEqualsOldPassword(_firstPassword, _oldPassword) && await CheckPasswordNotSmaller(_firstPassword) 
                && await CheckPasswordNotBigger(_firstPassword) && await CheckEmailNotEmpty(_email) && await CheckRoleNotEmpty(_role))
            {
                try
                {
                    await UpdateUser(_login, _firstPassword, _email, _role);
                }
                catch(TokenLifetimeException ex)
                {
                    await UpdateToken(); 
                    await UpdateUser(_login, _firstPassword, _email, _role);
                }
                finally
                {
                    FirstPassword = string.Empty;
                    SecondPassword = string.Empty;
                }   
            }
            else
                _eventAggregator
                            .GetEvent<ExceptionEvent>()
                            .Publish(new UpdateUserException(_userUpdateError));
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
        private async Task<bool> CheckPasswordNotEqualsOldPassword(string password, string oldPassword)
        {
            if (password != oldPassword)
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
        private async Task<bool> UpdateUser(string login, string password, string email, string role)
        {
            try
            {
                await _userRepository.Update(_userId, new UserModel(login, password, email, default, role), _userModel.Token);
            }
            catch (TokenLifetimeException ex)
            {
                await UpdateToken();
                await _userRepository.Update(_userId, new UserModel(login, password, email, default, role), _userModel.Token);
            }
            catch (Exception ex)
            {
                return false;
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
            set
            {
                SetProperty(ref _login, value);
                ChangeFlag = true;
            }
        }
        public string FirstPassword
        {
            get => _firstPassword;
            set
            {
                SetProperty(ref _firstPassword, value);
                ChangeFlag = true;
            }
        }
        public string SecondPassword
        {
            get => _secondPassword;
            set
            {
                SetProperty(ref _secondPassword, value);
                ChangeFlag = true;
            }
        }
        public string Email
        {
            get => _email;
            set
            {
                SetProperty(ref _email, value);
                ChangeFlag = true;
            }
        }
        public string Role
        {
            get => _role;
            set
            {
                SetProperty(ref _role, value);
                ChangeFlag = true;
            }
        }
        public string OldPassword
        {
            get => _oldPassword;
            set => SetProperty(ref _oldPassword, value);
        }
        public bool ChangeFlag
        {
            get => _changeFlag;
            set => SetProperty(ref _changeFlag, value);
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
        UserModel _user;
        private ObservableCollection<string> _roles;
        private List<UserModel> _users;
        private string _login;
        private string _startLogin;
        private string _firstPassword;
        private string _secondPassword;
        private string _email;
        private string _startEmail;
        private string _role;
        private string _startRole;
        private string _oldPassword;
        private int _userId;
        private const string _internetError = "Отсутствует подключение к интернету";
        private const string _userUpdateError = "Ошибка измененения!";
        private bool _changeFlag;

        #endregion
    }
}
