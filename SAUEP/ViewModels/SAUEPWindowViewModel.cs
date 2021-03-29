using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using Prism.Mvvm;
using SAUEP.WPF.Events;
using SAUEP.Core.Interfaces;
using SAUEP.Core.Repositories;
using SAUEP.Core.Models;
using System.Windows;
using SAUEP.Core.Services;
using SAUEP.Core.Exceptions;
using SAUEP.WPF.Configs;

namespace SAUEP.WPF.ViewModels
{
    public sealed class SAUEPWindowViewModel : BindableBase
    {
        #region Constructors

        public SAUEPWindowViewModel(IEventAggregator eventAggregator, IModel user, UserRepository userRepository,
            InternetConnectionChecker internetConnectionChecker, DeviceGroupRepository deviceGroupRepository,
            DeviceRepository deviceRepository, AuthorizationService authorizationService, DispatcherPageViewModel dispatcherPageViewModel, 
            IGuardian guardian, DeviceDispatcherPageViewModel deviceDispatcherPageViewModel, DevicePageViewModel devicePageViewModel, 
            DeviceGroupHistoryPageViewModel deviceGroupHistoryPageViewModel, DeviceHistoryPageViewModel deviceHistoryPageViewModel,
            OneDeviceHistoryPageViewModel oneDevicePageViewModel, ExpensesPageViewModel expensesPageViewModel, CreateDeviceGroupPageViewModel createDeviceGroupPageViewModel,
            UpdateDeviceGroupPageViewModel updateDeviceGroupPageViewModel, CreateDevicePageViewModel createDevicePageViewModel, UpdateDevicePageViewModel updateDevicePageViewModel,
            CreateDevicePollPageViewModel createDevicePollPageViewModel, UsersPageViewModel usersPageViewModel, UserPageViewModel userPageViewModel,
            CreateUserPageViewModel createUserPageViewModel, UpdateUserPageViewModel updateUserPageViewModel)
        {
            _eventAggregator = eventAggregator;
            _userModel = user;
            _userRepository = userRepository;
            _checker = internetConnectionChecker;
            _deviceGroupRepository = deviceGroupRepository;
            _deviceRepository = deviceRepository;
            _authorizationService = authorizationService;
            _guardian = guardian;
            PagePath = "AuthorizationPage.xaml";
            DeviceGroupModels = new ObservableCollection<DeviceGroupModel>();
            SubscriptionToken subscriptionToken = _eventAggregator
                                        .GetEvent<AuthorizeEvent>()
                                        .Subscribe((details) =>
                                        {
                                            if (_checker.Check())
                                                Start(details);
                                            else
                                                Error = _internetError;
                                            
                                        }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
                .GetEvent<ExceptionEvent>()
                .Subscribe((details) =>
                {
                    Error = details.Message;
                }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
                .GetEvent<ChangePageEvent>()
                .Subscribe((details) =>
                {
                    PagePath = details;
                }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
                .GetEvent<ChangeTokenEvent>()
                .Subscribe((details) =>
                {
                    (_userModel as UserModel).Token = details;
                }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
                .GetEvent<ReloadEvent>()
                .Subscribe((details) =>
                {
                    PagePath = "AuthorizationPage.xaml";
                    PagePath = details;
                }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
                .GetEvent<DataUpdateEvent>()
                .Subscribe((details) =>
                {
                    Update = string.Concat("Дата последнего обновления данных: ", details.ToString("u"));
                }, ThreadOption.UIThread);
        }

        #endregion



        #region Commands

        public RelayCommand Closing
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    _eventAggregator
                       .GetEvent<CloseEvent>()
                       .Publish();
                });
            }
        }
        public RelayCommand GoOnHistoryPage
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    _eventAggregator
                       .GetEvent<GoOnDeviceGroupHistoryPageEvent>()
                       .Publish();
                    _eventAggregator
                        .GetEvent<ChangePageEvent>()
                        .Publish("DeviceGroupHistoryPage.xaml");
                });
            }
        }
        public RelayCommand GoOnExpensesPage
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    _eventAggregator
                       .GetEvent<GoOnExpensesPageEvent>()
                       .Publish();
                    _eventAggregator
                        .GetEvent<ChangePageEvent>()
                        .Publish("ExpensesPage.xaml");
                });
            }
        }
        public RelayCommand GoOnCreateDeviceGroupPage
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    _eventAggregator
                       .GetEvent<GoOnCreateDeviceGroupPageEvent>()
                       .Publish();
                    _eventAggregator
                        .GetEvent<ChangePageEvent>()
                        .Publish("CreateDeviceGroupPage.xaml");
                });
            }
        }
        public RelayCommand GoOnCreateDevicePage
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    _eventAggregator
                       .GetEvent<GoOnCreateDevicePageEvent>()
                       .Publish();
                    _eventAggregator
                        .GetEvent<ChangePageEvent>()
                        .Publish("CreateDevicePage.xaml");
                });
            }
        }
        public RelayCommand GoOnCreateDevicePollPage
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    _eventAggregator
                       .GetEvent<GoOnCreateDevicePollPageEvent>()
                       .Publish();
                    _eventAggregator
                        .GetEvent<ChangePageEvent>()
                        .Publish("CreateDevicePollPage.xaml");
                });
            }
        }
        public RelayCommand GoOnUsersPage
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    _eventAggregator
                       .GetEvent<GoOnUsersPage>()
                       .Publish();
                    _eventAggregator
                        .GetEvent<ChangePageEvent>()
                        .Publish("UsersPage.xaml");
                });
            }
        }
        public RelayCommand GoOnCreateUserPage
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    _eventAggregator
                       .GetEvent<GoOnCreateUserPageEvent>()
                       .Publish();
                    _eventAggregator
                        .GetEvent<ChangePageEvent>()
                        .Publish("CreateUserPage.xaml");
                });
            }
        }


        #endregion




        #region Main Logic


        private async void Start(IModel authorizationResponseModel)
        {
            _userModel = await _userRepository.GetById(int.Parse((authorizationResponseModel as AuthorizationResponseModel).id), (authorizationResponseModel as AuthorizationResponseModel).access_token);
            (_userModel as UserModel).Token = (authorizationResponseModel as AuthorizationResponseModel).access_token + 2;
            if ((_userModel as UserModel).Role.Equals("Администратор"))
            {
                AdminVisibility = Visibility.Visible;
                ModeratorVisibility = Visibility.Visible;
                UserVisibility = Visibility.Visible;
                AdminFlag = true;
                ModeratorFlag = true;
                UserFlag = true;
            }
            else if ((_userModel as UserModel).Role.Equals("Модератор"))
            {
                ModeratorVisibility = Visibility.Visible;
                UserVisibility = Visibility.Visible;
                ModeratorFlag = true;
                UserFlag = true;
            }
            else
            {
                UserVisibility = Visibility.Visible;
                UserFlag = true;
            }
            try
            {
                DeviceGroupModels.AddRange(await _deviceGroupRepository.Get<DeviceGroupModel>((_userModel as UserModel).Token) as List<DeviceGroupModel>);
                var deviceModels = (await _deviceRepository.Get<DeviceModel>((_userModel as UserModel).Token) as List<DeviceModel>).ToList();
                for (var i = 0; i < DeviceGroupModels.Count; i++)
                {
                    DeviceGroupModels[i].DeviceModels.AddRange(deviceModels.Where(device => device.DeviceGroup == DeviceGroupModels[i].Title).ToList());
                }
            }
            catch (TokenLifetimeException ex)
            {
                await UpdateToken();
                DeviceGroupModels.AddRange(await _deviceGroupRepository.Get<DeviceGroupModel>((_userModel as UserModel).Token) as List<DeviceGroupModel>);
                var deviceModels = (await _deviceRepository.Get<DeviceModel>((_userModel as UserModel).Token) as List<DeviceModel>).ToList();
                for (var i = 0; i < DeviceGroupModels.Count; i++)
                {
                    DeviceGroupModels[i].DeviceModels.AddRange(deviceModels.Where(device => device.DeviceGroup == DeviceGroupModels[i].Title).ToList());
                }
            }
            finally
            {
                OnPropertyChanged(nameof(DeviceGroupModels));
                PagePath = "DispatcherPage.xaml";
                _eventAggregator
                       .GetEvent<UserCompliteEvent>()
                       .Publish(_userModel as UserModel);
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

        public Visibility AdminVisibility
        {
            get => _adminVisibility;
            set => SetProperty(ref _adminVisibility, value);
        }
        public Visibility ModeratorVisibility
        {
            get => _moderatorVisibility;
            set => SetProperty(ref _moderatorVisibility, value);
        }
        public Visibility UserVisibility
        {
            get => _userVisibility;
            set => SetProperty(ref _userVisibility, value);
        }
        public bool AdminFlag
        {
            get => _adminFlag;
            set => SetProperty(ref _adminFlag, value);
        }
        public bool ModeratorFlag
        {
            get => _moderatorFlag;
            set => SetProperty(ref _moderatorFlag, value);
        }
        public bool UserFlag
        {
            get => _userFlag;
            set => SetProperty(ref _userFlag, value);
        }
        public string PagePath
        {
            get => _pagePath;
            set => SetProperty(ref _pagePath, value);
        }
        public string Error
        {
            get => _error;
            set => SetProperty(ref _error, value);
        }
        public string Update
        {
            get => _update;
            set => SetProperty(ref _update, value);
        }
        public ObservableCollection<DeviceGroupModel> DeviceGroupModels
        {
            get => _deviceGroupModels;
            set => SetProperty(ref _deviceGroupModels, value);
        }
        public DeviceGroupModel SelectedDeviceGroupModel
        {
            get => _selectedDeviceGroupModel;
            set
            {
                SetProperty(ref _selectedDeviceGroupModel, value);
                if(_selectedDeviceGroupModel != null)
                {
                    _eventAggregator
                         .GetEvent<SelectDeviceGroupModelEvent>()
                         .Publish(_selectedDeviceGroupModel);
                    PagePath = "DeviceDispatcherPageView.xaml";
                }
            }
        }

        #endregion



        #region Fields

        private IEventAggregator _eventAggregator;
        private IModel _userModel;
        private IRepository _userRepository;
        private IChecker _checker;
        private IGuardian _guardian;
        private IRepository _deviceGroupRepository;
        private DeviceRepository _deviceRepository;
        private AuthorizationService _authorizationService;
        private Visibility _adminVisibility = Visibility.Collapsed;
        private Visibility _moderatorVisibility = Visibility.Collapsed;
        private Visibility _userVisibility = Visibility.Collapsed;
        private bool _adminFlag = false;
        private bool _moderatorFlag = false;
        private bool _userFlag = false;
        private string _pagePath;
        private string _error;
        private string _update;
        private const string _internetError = "Отсутствует подключение к интернету";
        private ObservableCollection<DeviceGroupModel> _deviceGroupModels;
        private DeviceGroupModel _selectedDeviceGroupModel;

        #endregion



    }
}
