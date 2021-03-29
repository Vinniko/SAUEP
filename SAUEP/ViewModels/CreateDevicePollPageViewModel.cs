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
    public sealed class CreateDevicePollPageViewModel : BindableBase
    {
        #region Constructors

        public CreateDevicePollPageViewModel(IEventAggregator eventAggregator, InternetConnectionChecker internetConnectionChecker, AuthorizationService authorizationService,
            IGuardian guardian, DeviceGroupRepository deviceGroupRepository, DeviceRepository deviceRepository,
            IModel user, PollRepository pollRepository)
        {
            _eventAggregator = eventAggregator;
            _internetConnectionChecker = internetConnectionChecker;
            _authorizationService = authorizationService;
            _guardian = guardian;
            _deviceGroupRepository = deviceGroupRepository;
            _deviceRepository = deviceRepository;
            _pollRepository = pollRepository;
            _userModel = user as UserModel;
            _deviceGroups = new ObservableCollection<string>();
            _deviceModels = new ObservableCollection<string>();
            _deviceGroupSelectFlag = false;
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
                .GetEvent<GoOnCreateDevicePollPageEvent>()
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
            await GetDeviceGroups();
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
            List<DeviceModel> deviceModels = new List<DeviceModel>();
            try
            {
                deviceModels = (await _deviceRepository.Get<DeviceModel>(_userModel.Token)).ToList();
            }
            catch (TokenLifetimeException ex)
            {
                await UpdateToken();
                deviceModels = (await _deviceRepository.Get<DeviceModel>(_userModel.Token)).ToList();
            }
            finally
            {
                deviceModels = deviceModels.FindAll(deviceModel => deviceModel.DeviceGroup.Equals(DeviceGroup));
                for (var i = 0; i < deviceModels.Count; i++)
                {
                    DeviceModels.Add(deviceModels[i].Serial);
                }
                DeviceModels = _deviceModels;
            }
            return true;
        }
        private async void CreateAsync()
        {
            if (await CheckDeviceGroupNotEmpty(_deviceGroup) && await CheckDeviceModelNotEmpty(_deviceSerial) && await CheckPollPowerNotEmpty(_power)
                && await CheckPollElecticityConsumptionNotEmpty(_electricityConsumption) && await CheckPollElecticityConsumptionNotSmallerThenLast(_electricityConsumption, _deviceSerial)
                && await CheckPollDateNotBiggerThenToday(_dateTime))
            {
                await CreateDevicePollModel(_deviceSerial, _power, _electricityConsumption, DateTime.Parse(DateTimeField));
            }
            else
                _eventAggregator
                            .GetEvent<ExceptionEvent>()
                            .Publish(new DevicePollCreateException(_devicePollCreateError));
        }
        private async Task<bool> CheckDeviceGroupNotEmpty(string deviceGroup)
        {
            if (deviceGroup != null)
                return true;
            else
                return false;
        }
        private async Task<bool> CheckDeviceModelNotEmpty(string device)
        {
            if (device != null)
                return true;
            else
                return false;
        }
        private async Task<bool> CheckPollPowerNotEmpty(string power)
        {
            double tmp = 0;
            if (power != null && !power.Equals(string.Empty) && double.TryParse(power, out tmp))
                return true;
            else
                return false;
        }
        private async Task<bool> CheckPollElecticityConsumptionNotEmpty(string electicityConsumption)
        {
            double tmp = 0;
            if (electicityConsumption != null && !electicityConsumption.Equals(string.Empty) && double.TryParse(electicityConsumption, out tmp))
                return true;
            else
                return false;
        }
        private async Task<bool> CheckPollElecticityConsumptionNotSmallerThenLast(string electicityConsumption, string deviceSerial)
        {
            PollModel pollModel = new PollModel();
            try
            {
                var response = await _pollRepository.Get<PollModel>(1, DateTime.Parse(_dateTime), deviceSerial, _userModel.Token);
                if (response.Count > 0)
                    pollModel = response.ElementAt(0);
                else
                    pollModel.ElectricityConsumption = 0;
            }
            catch (TokenLifetimeException ex)
            {
                UpdateToken();
                var response = await _pollRepository.Get<PollModel>(1, DateTime.Parse(_dateTime), deviceSerial, _userModel.Token);
                if (response.Count > 0)
                    pollModel = response.ElementAt(0);
                else
                    pollModel.ElectricityConsumption = 0;
            }
            if (pollModel.ElectricityConsumption > double.Parse(electicityConsumption))
                return false;
            else
                return true;
        }
        private async Task<bool> CheckPollDateNotBiggerThenToday(string dateTime)
        {

            if (DateTime.Parse(dateTime) > DateTime.Now)
                return false;
            else
                return true;
        }
        private async Task<bool> CreateDevicePollModel(string deviceSerial, string power, string electricityConsumption, DateTime dateTime)
        {
            List<DeviceModel> deviceModels = new List<DeviceModel>();
            try
            {
                deviceModels = await _deviceRepository.Get<DeviceModel>(_userModel.Token) as List<DeviceModel>;
                var device = deviceModels.Find(deviceModel => deviceModel.Serial.Equals(deviceSerial));
                await _pollRepository.Set(new PollModel(deviceSerial, device.Ip, double.Parse(power), double.Parse(electricityConsumption), dateTime), _userModel.Token);
            }
            catch (TokenLifetimeException ex)
            {
                await UpdateToken();
                deviceModels = await _deviceRepository.Get<DeviceModel>(_userModel.Token) as List<DeviceModel>;
                var device = deviceModels.Find(deviceModel => deviceModel.Serial.Equals(deviceSerial));
                await _pollRepository.Set(new PollModel(deviceSerial, device.Ip, double.Parse(power), double.Parse(electricityConsumption), dateTime), _userModel.Token);
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                DeviceGroup = string.Empty;
                DeviceModels.Clear();
                DeviceGroupSelectFlag = false;
                DeviceSerial = string.Empty;
                Power = string.Empty;
                ElectricityConsumption = string.Empty;
                DateTimeField = string.Empty;
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

        public ObservableCollection<string> DeviceGroups
        {
            get => _deviceGroups;
            set => SetProperty(ref _deviceGroups, value);
        }
        public ObservableCollection<string> DeviceModels
        {
            get => _deviceModels;
            set => SetProperty(ref _deviceModels, value);
        }
        public string DeviceGroup
        {
            get => _deviceGroup;
            set
            {
                SetProperty(ref _deviceGroup, value);
                DeviceModels.Clear();
                GetDevices();
                DeviceGroupSelectFlag = true;
            }
        }
        public string DeviceSerial
        {
            get => _deviceSerial;
            set => SetProperty(ref _deviceSerial, value);
        }
        public string Power
        {
            get => _power;
            set => SetProperty(ref _power, value);
        }
        public string ElectricityConsumption
        {
            get => _electricityConsumption;
            set => SetProperty(ref _electricityConsumption, value);
        }
        public string DateTimeField
        {
            get => _dateTime;
            set => SetProperty(ref _dateTime, value);
        }
        public bool DeviceGroupSelectFlag
        {
            get => _deviceGroupSelectFlag;
            set => SetProperty(ref _deviceGroupSelectFlag, value);
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
        private PollRepository _pollRepository;
        private ObservableCollection<string> _deviceGroups;
        private ObservableCollection<string> _deviceModels;
        private string _deviceGroup;
        private string _deviceSerial;
        private string _power;
        private string _electricityConsumption;
        private string _dateTime;
        private bool _deviceGroupSelectFlag;
        private const string _internetError = "Отсутствует подключение к интернету";
        private const string _devicePollCreateError = "Ошибка создания!";

        #endregion
    }
}
