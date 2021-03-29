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

namespace SAUEP.WPF.ViewModels
{
    public sealed class UpdateDeviceGroupPageViewModel : BindableBase
    {
        #region Constructors

        public UpdateDeviceGroupPageViewModel(IEventAggregator eventAggregator, InternetConnectionChecker internetConnectionChecker, AuthorizationService authorizationService, IGuardian guardian,
            DeviceGroupRepository deviceGroupRepository, IModel user)
        {
            _eventAggregator = eventAggregator;
            _internetConnectionChecker = internetConnectionChecker;
            _authorizationService = authorizationService;
            _guardian = guardian;
            _deviceGroupRepository = deviceGroupRepository;
            _userModel = user as UserModel;
            _deviceGroupModels = new List<DeviceGroupModel>();
            _changeFlag = false;
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
                .GetEvent<GoOnUpdateDeviceGroupPageEvent>()
                .Subscribe((details) =>
                {
                    if (_internetConnectionChecker.Check())
                    {
                        GetDeviceGroups();
                        _deviceGroupModelId = details.Id;
                        _deviceGroupModelTitle = details.Title;
                        _startDeviceGroupTitle = details.Title;
                        _deviceGroupModel = details;
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
                    _deviceGroupModels.Clear();
                    _eventAggregator
                        .GetEvent<ChangePageEvent>()
                        .Publish("DispatcherPage.xaml");
                    _eventAggregator
                           .GetEvent<GoOnDispatcherPageEvent>()
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
                    DeviceGroupModelTitle = _startDeviceGroupTitle;
                    ChangeFlag = false;
                });
            }
        }

        #endregion



        #region Main Logic
        private async void GetDeviceGroups()
        {
            try
            {
                _deviceGroupModels = (await _deviceGroupRepository.Get<DeviceGroupModel>(_userModel.Token)).ToList();
            }
            catch (TokenLifetimeException ex)
            {
                await UpdateToken();
                _deviceGroupModels = (await _deviceGroupRepository.Get<DeviceGroupModel>(_userModel.Token)).ToList();
            }
        }
        private async void UpdateAsync()
        {
            if (await CheckDeviceGroupTitleExist(_deviceGroupModelTitle) && await CheckDeviceGroupTitleNotEmpty(_deviceGroupModelTitle) && await CheckDeviceGroupTitleNotBigger(_deviceGroupModelTitle) && _deviceGroupModelTitle != _startDeviceGroupTitle)
                UpdateDeviceGroupModel(_deviceGroupModelTitle, _deviceGroupModelId);
            else
                _eventAggregator
                            .GetEvent<ExceptionEvent>()
                            .Publish(new DeviceGroupModelDublicateException(_deviceGroupModelTitleDublicateError));
        }
        private async Task<bool> CheckDeviceGroupTitleExist(string deviceGroupTitle)
        {
            if (_deviceGroupModels.Find(groupModel => groupModel.Title.Equals(deviceGroupTitle)) == null)
                return true;
            else
                return false;
        }
        private async Task<bool> CheckDeviceGroupTitleNotEmpty(string deviceGroupTitle)
        {
            if (!deviceGroupTitle.Equals(string.Empty))
                return true;
            else
                return false;
        }
        private async Task<bool> CheckDeviceGroupTitleNotBigger(string deviceGroupTitle)
        {
            if (deviceGroupTitle.Length <= 16)
                return true;
            else
                return false;
        }
        private async Task<bool> UpdateDeviceGroupModel(string deviceGroupTitle, int deviceGroupModelId)
        {
            try
            {
                _deviceGroupRepository.Update(deviceGroupModelId, new DeviceGroupModel(deviceGroupTitle), _userModel.Token);
                _deviceGroupModel = await _deviceGroupRepository.GetById(deviceGroupModelId, _userModel.Token) as DeviceGroupModel;
            }
            catch (TokenLifetimeException ex)
            {
                await UpdateToken();
                _deviceGroupRepository.Update(deviceGroupModelId, new DeviceGroupModel(deviceGroupTitle), _userModel.Token);
                _deviceGroupModel = await _deviceGroupRepository.GetById(deviceGroupModelId, _userModel.Token) as DeviceGroupModel;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                _startDeviceGroupTitle = _deviceGroupModelTitle;
                _deviceGroupModels.Clear();
                _eventAggregator
                    .GetEvent<ChangePageEvent>()
                    .Publish("DispatcherPage.xaml");
                _eventAggregator
                    .GetEvent<UpdateDeviceGroupModelEvent>()
                    .Publish();
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

        public string DeviceGroupModelTitle
        {
            get => _deviceGroupModelTitle;
            set
            {
                SetProperty(ref _deviceGroupModelTitle, value);
                ChangeFlag = true;
            }
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
        DeviceGroupRepository _deviceGroupRepository;
        UserModel _userModel;
        private DeviceGroupModel _deviceGroupModel;
        private List<DeviceGroupModel> _deviceGroupModels;
        private string _deviceGroupModelTitle;
        private string _startDeviceGroupTitle;
        private const string _internetError = "Отсутствует подключение к интернету";
        private const string _deviceGroupModelTitleDublicateError = "Ошибка изменения!";
        private bool _changeFlag;
        private int _deviceGroupModelId;

        #endregion
    }
}
