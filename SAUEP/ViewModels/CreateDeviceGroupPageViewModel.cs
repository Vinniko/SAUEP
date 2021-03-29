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
    public sealed class CreateDeviceGroupPageViewModel : BindableBase
    {
        #region Constructors

        public CreateDeviceGroupPageViewModel(IEventAggregator eventAggregator, InternetConnectionChecker internetConnectionChecker, AuthorizationService authorizationService, IGuardian guardian,
            DeviceGroupRepository deviceGroupRepository, IModel user)
        {
            _eventAggregator = eventAggregator;
            _internetConnectionChecker = internetConnectionChecker;
            _authorizationService = authorizationService;
            _guardian = guardian;
            _deviceGroupRepository = deviceGroupRepository;
            _userModel = user as UserModel;
            _deviceGroupModels = new List<DeviceGroupModel>();
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
                .GetEvent<GoOnCreateDeviceGroupPageEvent>()
                .Subscribe(() =>
                {
                    if (_internetConnectionChecker.Check())
                    {
                        GetDeviceGroups();
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
        private async void CreateAsync()
        {
            if (await CheckDeviceGroupTitleExist(_deviceGroupModelTitle) && await CheckDeviceGroupTitleNotEmpty(_deviceGroupModelTitle) && await CheckDeviceGroupTitleNotBigger(_deviceGroupModelTitle))
                CreateDeviceGroupModel(_deviceGroupModelTitle);
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
        private async Task<bool> CreateDeviceGroupModel(string deviceGroupTitle)
        {
            try
            {
                _deviceGroupRepository.Set(new DeviceGroupModel(deviceGroupTitle), _userModel.Token);
            }
            catch (TokenLifetimeException ex)
            {
                await UpdateToken();
                _deviceGroupRepository.Set(new DeviceGroupModel(deviceGroupTitle), _userModel.Token);
            }
            catch(Exception ex)
            {
                return false;
            }
            finally
            {
                _deviceGroupModelTitle = string.Empty;
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
            set => SetProperty(ref _deviceGroupModelTitle, value);
        }

        #endregion



        #region Fields

        IEventAggregator _eventAggregator;
        InternetConnectionChecker _internetConnectionChecker;
        AuthorizationService _authorizationService;
        IGuardian _guardian;
        DeviceGroupRepository _deviceGroupRepository;
        UserModel _userModel;
        private List<DeviceGroupModel> _deviceGroupModels;
        private string _deviceGroupModelTitle;
        private const string _internetError = "Отсутствует подключение к интернету";
        private const string _deviceGroupModelTitleDublicateError = "Ошибка создания!";

        #endregion
    }
}
