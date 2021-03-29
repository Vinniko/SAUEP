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
    public sealed class UpdateDevicePageViewModel : BindableBase
    {
        #region Constructors

        public UpdateDevicePageViewModel(IEventAggregator eventAggregator, InternetConnectionChecker internetConnectionChecker, AuthorizationService authorizationService,
            IGuardian guardian, DeviceGroupRepository deviceGroupRepository, DeviceRepository deviceRepository,
            IModel user)
        {
            _eventAggregator = eventAggregator;
            _internetConnectionChecker = internetConnectionChecker;
            _authorizationService = authorizationService;
            _guardian = guardian;
            _deviceGroupRepository = deviceGroupRepository;
            _deviceRepository = deviceRepository;
            _userModel = user as UserModel;
            _deviceGroups = new List<string>();
            _deviceModels = new List<DeviceModel>();
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
                .GetEvent<GoOnUpdateDevicePageEvent>()
                .Subscribe((details) =>
                {
                    if (_internetConnectionChecker.Check())
                    {
                        _deviceId = details.Id;
                        _deviceGroup = details.DeviceGroup;
                        _startDeviceGroup = details.DeviceGroup;
                        _deviceSerial = details.Serial;
                        _startDeviceSerial = details.Serial;
                        _deviceTitle = details.Title;
                        _startDeviceTitle = details.Title;
                        _deviceIp = details.Ip;
                        _startDeviceIp = details.Ip;
                        _devicePort = details.Port;
                        _startDevicePort = details.Port;
                        _deviceStatus = details.Status;
                        _startDeviceStatus = details.Status;
                        _deviceMinPower = details.MinPower.ToString();
                        _startDeviceMinPower = details.MinPower;
                        _deviceMaxPower = details.MaxPower.ToString();
                        _startDeviceMaxPower = details.MaxPower;
                        _deviceModel = details;
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
                    _deviceGroups.Clear();
                    _deviceModels.Clear();
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
                    DeviceGroup = _startDeviceGroup;
                    DeviceSerial = _startDeviceSerial;
                    DeviceTitle = _startDeviceTitle;
                    DeviceIp = _startDeviceIp;
                    DevicePort = _startDevicePort;
                    DeviceStatus = _startDeviceStatus;
                    DeviceMaxPower = _startDeviceMaxPower.ToString();
                    DeviceMinPower = _startDeviceMinPower.ToString();
                    ChangeFlag = false;
                });
            }
        }

        #endregion



        #region Main Logic
        private async void GetWrapper()
        {
            await GetDeviceGroups();
            await GetDevices();
        }
        private async Task<bool> GetDeviceGroups()
        {
            List<DeviceGroupModel> deviceGroupModels = new List<DeviceGroupModel>();
            try
            {
                deviceGroupModels = (await _deviceGroupRepository.Get<DeviceGroupModel>(_userModel.Token)).ToList();
            }
            catch (TokenLifetimeException ex)
            {
                await UpdateToken();
                deviceGroupModels = (await _deviceGroupRepository.Get<DeviceGroupModel>(_userModel.Token)).ToList();
            }
            finally
            {
                for (var i = 0; i < deviceGroupModels.Count; i++)
                {
                    DeviceGroups.Add(deviceGroupModels[i].Title);
                }
                OnPropertyChanged(nameof(DeviceGroups));
            }
            return true;
        }
        private async Task<bool> GetDevices()
        {
            try
            {
                _deviceModels = (await _deviceRepository.Get<DeviceModel>(_userModel.Token)).ToList();
            }
            catch (TokenLifetimeException ex)
            {
                await UpdateToken();
                _deviceModels = (await _deviceRepository.Get<DeviceModel>(_userModel.Token)).ToList();
            }
            for(var i = 0; i < _deviceModels.Count; i++)
            {
                if (_deviceModels[i].Serial.Equals(_deviceModel.Serial))
                {
                    _deviceModels.RemoveAt(i);
                    break;
                }    
            }
            return true;
        }
        private async void UpdateAsync()
        {
            if (await CheckDeviceGroupNotEmpty(_deviceGroup) && await CheckDeviceSerialNotEmpty(_deviceSerial) && await CheckDeviceSerialNotExist(_deviceSerial)
                && await CheckDeviceSerialNotBigger(_deviceSerial) && await CheckDeviceTitleNotEmpty(_deviceTitle) && await CheckDeviceTitleNotExist(_deviceTitle)
                && await CheckDeviceTitleNotBigger(_deviceTitle) && await CheckDeviceIpNotEmpty(_deviceIp) && await CheckDeviceIpNotBigger(_deviceIp)
                && await CheckDevicePortNotEmpty(_devicePort) && await CheckDevicePortNotBigger(_devicePort) && await CheckDeviceMaxPowerNotEmpty(_deviceMaxPower)
                && await CheckDeviceMinPowerNotEmpty(_deviceMinPower))
            {
                await UpdateDeviceModel(_deviceGroup, _deviceSerial, _deviceTitle, _deviceIp, _devicePort, _deviceStatus, _deviceMaxPower, _deviceMinPower);
            }
            else
                _eventAggregator
                            .GetEvent<ExceptionEvent>()
                            .Publish(new UpdateDeviceException(_deviceModelError));
        }
        private async Task<bool> CheckDeviceGroupNotEmpty(string deviceGroup)
        {
            if (deviceGroup != null)
                return true;
            else
                return false;
        }
        private async Task<bool> CheckDeviceSerialNotEmpty(string deviceSerial)
        {
            if (deviceSerial != null && !deviceSerial.Equals(string.Empty))
                return true;
            else
                return false;
        }
        private async Task<bool> CheckDeviceSerialNotExist(string deviceSerial)
        {
            if (_deviceModels.Find(deviceModel => deviceModel.Serial.Equals(deviceSerial)) == null)
                return true;
            else
                return false;
        }
        private async Task<bool> CheckDeviceSerialNotBigger(string deviceSerial)
        {
            if (deviceSerial.Length <= 16)
                return true;
            else
                return false;
        }
        private async Task<bool> CheckDeviceTitleNotEmpty(string deviceTitle)
        {
            if (deviceTitle != null && !deviceTitle.Equals(string.Empty))
                return true;
            else
                return false;
        }
        private async Task<bool> CheckDeviceTitleNotExist(string deviceTitle)
        {
            if (_deviceModels.Find(deviceModel => deviceModel.Title.Equals(deviceTitle)) == null)
                return true;
            else
                return false;
        }
        private async Task<bool> CheckDeviceTitleNotBigger(string deviceTitle)
        {
            if (deviceTitle.Length <= 32)
                return true;
            else
                return false;
        }
        private async Task<bool> CheckDeviceIpNotEmpty(string deviceIp)
        {
            if (deviceIp != null && !deviceIp.Equals(string.Empty))
                return true;
            else
                return false;
        }
        private async Task<bool> CheckDeviceIpNotBigger(string deviceIp)
        {
            if (deviceIp.Length <= 15)
                return true;
            else
                return false;
        }
        private async Task<bool> CheckDevicePortNotEmpty(string devicePort)
        {
            if (devicePort != null && !devicePort.Equals(string.Empty))
                return true;
            else
                return false;
        }
        private async Task<bool> CheckDevicePortNotBigger(string devicePort)
        {
            if (devicePort.Length <= 5)
                return true;
            else
                return false;
        }
        private async Task<bool> CheckDeviceMaxPowerNotEmpty(string deviceMaxPower)
        {
            double tmp;
            if (deviceMaxPower != null && !deviceMaxPower.Equals(string.Empty) && double.TryParse(deviceMaxPower, out tmp))
                return true;
            else
                return false;
        }
        private async Task<bool> CheckDeviceMinPowerNotEmpty(string deviceMinPower)
        {
            double tmp;
            if (deviceMinPower != null && !deviceMinPower.Equals(string.Empty) && double.TryParse(deviceMinPower, out tmp))
                return true;
            else
                return false;
        }
        private async Task<bool> UpdateDeviceModel(string devcieGroup, string deviceSerial, string deviceTitle, string deviceIp, string devicePort, bool deviceStatus, string deviceMaxPower, string deviceMinPower)
        {
            try
            {
                _deviceRepository.Update(_deviceId ,new DeviceModel(devcieGroup, deviceSerial, deviceTitle, deviceIp, devicePort, deviceStatus, double.Parse(deviceMaxPower), double.Parse(deviceMinPower)), _userModel.Token);
            }
            catch (TokenLifetimeException ex)
            {
                await UpdateToken();
                _deviceRepository.Update(_deviceId,new DeviceModel(devcieGroup, deviceSerial, deviceTitle, deviceIp, devicePort, deviceStatus, double.Parse(deviceMaxPower), double.Parse(deviceMinPower)), _userModel.Token);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                _eventAggregator
                    .GetEvent<UpdateDeviceEvent>()
                    .Publish();
                _eventAggregator
                    .GetEvent<ChangePageEvent>()
                    .Publish("DispatcherPage.xaml");
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

        public List<string> DeviceGroups
        {
            get => _deviceGroups;
            set => SetProperty(ref _deviceGroups, value);
        }
        public string DeviceGroup
        {
            get => _deviceGroup;
            set
            {
                SetProperty(ref _deviceGroup, value);
                ChangeFlag = true;
            }
        }
        public string DeviceSerial
        {
            get => _deviceSerial;
            set
            {
                SetProperty(ref _deviceSerial, value);
                ChangeFlag = true;
            }
        }
        public string DeviceTitle
        {
            get => _deviceTitle;
            set
            {
                SetProperty(ref _deviceTitle, value);
                ChangeFlag = true;
            }
        }
        public string DeviceIp
        {
            get => _deviceIp;
            set
            {
                SetProperty(ref _deviceIp, value);
                ChangeFlag = true;
            }
        }
        public string DevicePort
        {
            get => _devicePort;
            set
            {
                SetProperty(ref _devicePort, value);
                ChangeFlag = true;
            }
        }
        public bool DeviceStatus
        {
            get => _deviceStatus;
            set
            {
                SetProperty(ref _deviceStatus, value);
                ChangeFlag = true;
            }
        }
        public string DeviceMaxPower
        {
            get => _deviceMaxPower;
            set
            {
                SetProperty(ref _deviceMaxPower, value);
                ChangeFlag = true;
            }
        }
        public string DeviceMinPower
        {
            get => _deviceMinPower;
            set
            {
                SetProperty(ref _deviceMinPower, value);
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
        DeviceRepository _deviceRepository;
        UserModel _userModel;
        private DeviceModel _deviceModel; 
        private List<string> _deviceGroups;
        private List<DeviceModel> _deviceModels;
        private int _deviceId;
        private string _deviceGroup;
        private string _startDeviceGroup;
        private string _deviceSerial;
        private string _startDeviceSerial;
        private string _deviceTitle;
        private string _startDeviceTitle;
        private string _deviceIp;
        private string _startDeviceIp;
        private string _devicePort;
        private string _startDevicePort;
        private bool _deviceStatus;
        private bool _startDeviceStatus;
        private string _deviceMinPower;
        private double _startDeviceMinPower;
        private string _deviceMaxPower;
        private double _startDeviceMaxPower;
        private const string _internetError = "Отсутствует подключение к интернету";
        private const string _deviceModelError = "Ошибка изменения!";
        private bool _changeFlag;

        #endregion
    }
}
