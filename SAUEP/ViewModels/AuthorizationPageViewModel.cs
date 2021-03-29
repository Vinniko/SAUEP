using System;
using Prism.Events;
using Prism.Mvvm;
using SAUEP.Core.Interfaces;
using SAUEP.WPF.Configs;
using SAUEP.Core.Services;
using SAUEP.WPF.Events;
using SAUEP.Core.Exceptions;

namespace SAUEP.WPF.ViewModels
{
    public sealed class AuthorizationPageViewModel : BindableBase
    {
        #region Constructors

        public AuthorizationPageViewModel(IEventAggregator eventAggregator, IGuardian guardian, ILogger logger, 
            IAuthorization authorization, LoginValidation loginValidation, PasswordValidation passwordValidation, 
            InternetConnectionChecker internetConnectionChecker)
        {
            _eventAggregator = eventAggregator;
            _guardian = guardian;
            _logger = logger;
            _authorization = authorization;
            _loginValidation = loginValidation;
            _passwordValidation = passwordValidation;
            _internetChecker = internetConnectionChecker;
        }

        #endregion



        #region Commands

        public RelayCommand Authorize
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    if(_internetChecker.Check())
                        AuthorizeAsync();
                    else
                        _eventAggregator
                        .GetEvent<ExceptionEvent>()
                        .Publish(new InternetException(_internetError));
                });
            }
        }

        #endregion



        #region Main Logic

        public async void AuthorizeAsync()
        {
            try
            {
                if (_loginValidation.Validate(_login) && _passwordValidation.Validate(_password))
                {
                    var authorizationResponseModel = await _authorization.Authorize(_login, _password);
                    _eventAggregator
                       .GetEvent<AuthorizeEvent>()
                       .Publish(authorizationResponseModel);
                }
            }
            catch(Exception ex)
            {
                Error = ex.Message;
            }
        }

        #endregion



        #region Get/Set

        public string Login
        {
            get => _login;
            set => SetProperty(ref _login, value);
        }
        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value);
            }
        }
        public string Error
        {
            get => _error;
            set => SetProperty(ref _error, value);
        }

        #endregion



        #region Fields

        private string _login = string.Empty;
        private string _password = string.Empty;
        private string _error = string.Empty;
        private IEventAggregator _eventAggregator;
        private IGuardian _guardian;
        private ILogger _logger;
        private IAuthorization _authorization;
        private LoginValidation _loginValidation;
        private PasswordValidation _passwordValidation;
        public const short MaxLoginLength = 8;
        public const string LoginToolTip = "Введите свой логин";
        public const short MaxPasswordLength = 32;
        public const string PasswordToolTip = "Введите свой пароль";
        private IChecker _internetChecker;
        private const string _internetError = "Отсутствует подключение к интернету";

        #endregion
    }
}
