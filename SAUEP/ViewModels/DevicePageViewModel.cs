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
using System.Net.Sockets;
using LiveCharts;
using LiveCharts.Wpf;

namespace SAUEP.WPF.ViewModels
{
    public sealed class DevicePageViewModel : BindableBase
    {
        #region Constructors

        public DevicePageViewModel(IEventAggregator eventAggregator, InternetConnectionChecker internetConnectionChecker,
            DeviceGroupRepository deviceGroupRepository, DeviceRepository deviceRepository, AuthorizationService authorizationService,
            IGuardian guardian, IListener listener, PollRepository pollRepository, IModel user)
        {
            _eventAggregator = eventAggregator;
            _internetConnectionChecker = internetConnectionChecker;
            _deviceGroupRepository = deviceGroupRepository;
            _deviceRepository = deviceRepository;
            _pollRepository = pollRepository;
            _authorizationService = authorizationService;
            _guardian = guardian;
            _listener = listener as SocketListener;
            _userModel = user as UserModel;
            _deviceGroupModels = new List<DeviceGroupModel>();
            _lastDevicePolls = new Dictionary<string, Dictionary<string, PollModel>>();
            _lastPower = new Dictionary<string, Queue<double>>();
            _cartesianChartSeries = new SeriesCollection();
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
                .GetEvent<CloseEvent>()
                .Subscribe(() =>
                {
                    _workFlag = false;
                    if (_listenThread.IsAlive)
                        _listenThread.Join();
                }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
                .GetEvent<SelectDeviceModelEvent>()
                .Subscribe((details) =>
                {
                    if (_internetConnectionChecker.Check())
                    {
                        _deviceModel = details;
                        _listenThread = new Thread(new ThreadStart(Listen));
                        _workFlag = true;
                        Dispatcher.CurrentDispatcher.Invoke(() =>
                        {
                            DeviceTitle = details.Title;
                        });
                        _deviceStatus = details.Status;
                        DeviceTitle = details.Title;
                        DeviceSerial = details.Serial;
                        DeviceGroup = details.DeviceGroup;
                        DeviceIp = details.Ip;
                        DevicePort = details.Port;
                        DeviceMinPower = details.MinPower;
                        DeviceMaxPower = details.MaxPower;
                        OnPropertyChanged(nameof(DeviceStatus));
                        _deviceId = details.Id;
                        GetDeviceGroupsAndDeviceModels();
                    }
                    else
                        _eventAggregator
                               .GetEvent<ExceptionEvent>()
                               .Publish(new InternetException(_internetError));
                }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<GoOnDeviceGroupHistoryPageEvent>()
               .Subscribe(() =>
               {
                   _workFlag = false;
                   if(_listenThread != null)
                       if (_listenThread.IsAlive)
                           _listenThread.Join();
                   SetDefault();
               }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<GoOnExpensesPageEvent>()
               .Subscribe(() =>
               {
                   _workFlag = false;
                   if (_listenThread != null)
                       if (_listenThread.IsAlive)
                           _listenThread.Join();
                   SetDefault();
               }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<GoOnCreateDeviceGroupPageEvent>()
               .Subscribe(() =>
               {
                   _workFlag = false;
                   if (_listenThread != null)
                       if (_listenThread.IsAlive)
                           _listenThread.Join();
                   SetDefault();
               }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<GoOnUpdateDevicePageEvent>()
               .Subscribe((details) =>
               {
                   _workFlag = false;
                   if (_listenThread != null)
                       if (_listenThread.IsAlive)
                           _listenThread.Join();
                   SetDefault();
               }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<GoOnCreateDevicePageEvent>()
               .Subscribe(() =>
               {
                   _workFlag = false;
                   if (_listenThread != null)
                       if (_listenThread.IsAlive)
                           _listenThread.Join();
                   SetDefault();
               }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<GoOnUpdateDeviceGroupPageEvent>()
               .Subscribe((details) =>
               {
                   _workFlag = false;
                   if (_listenThread != null)
                       if (_listenThread.IsAlive)
                           _listenThread.Join();
                   SetDefault();
               }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<GoOnCreateDevicePollPageEvent>()
               .Subscribe(() =>
               {
                   _workFlag = false;
                   if (_listenThread != null)
                       if (_listenThread.IsAlive)
                           _listenThread.Join();
                   SetDefault();
               }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<GoOnUsersPage>()
               .Subscribe(() =>
               {
                   _workFlag = false;
                   if (_listenThread != null)
                       if (_listenThread.IsAlive)
                           _listenThread.Join();
                   SetDefault();
               }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<GoOnCreateUserPageEvent>()
               .Subscribe(() =>
               {
                   _workFlag = false;
                   if (_listenThread != null)
                       if (_listenThread.IsAlive)
                           _listenThread.Join();
                   SetDefault();
               }, ThreadOption.UIThread);
        }

        #endregion



        #region Commands

        public RelayCommand OnOff
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    DeviceStatus = !DeviceStatus;
                });
            }
        }
        public RelayCommand Update
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    FillCharts();
                });
            }
        }
        public RelayCommand Back
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    _workFlag = false;
                    while (_listenThread.IsAlive) { }
                    SetDefault();
                    _eventAggregator
                        .GetEvent<ChangePageEvent>()
                        .Publish("DeviceDispatcherPage.xaml");
                    _eventAggregator
                           .GetEvent<GoOnDeviceDispatcherPageEvent>()
                           .Publish();
                });
            }
        }
        public RelayCommand Edit
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    _eventAggregator
                        .GetEvent<GoOnUpdateDevicePageEvent>()
                        .Publish(_deviceModel);
                    _eventAggregator
                        .GetEvent<ChangePageEvent>()
                        .Publish("UpdateDevicePage.xaml");
                });
            }
        }
        public RelayCommand Delete
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    DeleteDeviceModel();
                });
            }
        }

        #endregion



        #region Main Logic

        private async void GetDeviceGroupsAndDeviceModels()
        {
            try
            {
                _deviceGroupModels = await _deviceGroupRepository.Get<DeviceGroupModel>(_userModel.Token) as List<DeviceGroupModel>;
                var deviceModels = (await _deviceRepository.Get<DeviceModel>((_userModel as UserModel).Token) as List<DeviceModel>).ToList();
                for (var i = 0; i < _deviceGroupModels.Count; i++)
                {
                    _deviceGroupModels[i].DeviceModels.AddRange(deviceModels.Where(device => device.DeviceGroup.Equals(_deviceGroupModels[i].Title)).ToList());
                    if (_deviceGroupModels[i].DeviceModels.Count == 0)
                        _deviceGroupModels.RemoveAt(i--);
                }
                CartesianChartSeries.Add(new LineSeries { Title = $"{_deviceTitle} {_deviceSerial}", Values = new ChartValues<double>() });
                if (!_reloadFlag)
                    _lastPower.Add(_deviceTitle, new Queue<double>());
                await GetLastPolls();
            }
            catch (TokenLifetimeException ex)
            {
                await UpdateToken();
                _deviceGroupModels = await _deviceGroupRepository.Get<DeviceGroupModel>(_userModel.Token) as List<DeviceGroupModel>;
                var deviceModels = (await _deviceRepository.Get<DeviceModel>((_userModel as UserModel).Token) as List<DeviceModel>).ToList();
                for (var i = 0; i < _deviceGroupModels.Count; i++)
                {
                    _deviceGroupModels[i].DeviceModels.AddRange(deviceModels.Where(device => device.DeviceGroup.Equals(_deviceGroupModels[i].Title)).ToList());
                    if (_deviceGroupModels[i].DeviceModels.Count == 0)
                        _deviceGroupModels.RemoveAt(i--);
                }
                var deviceModel = _deviceGroupModels.Where(model => model.Title.Equals(_deviceGroup)).Last().DeviceModels.ToList().Find(device => device.Serial.Equals(_deviceSerial));
                CartesianChartSeries.Add(new LineSeries { Title = $"{_deviceTitle} {_deviceSerial}", Values = new ChartValues<double>() });
                if (!_reloadFlag)
                    _lastPower.Add(_deviceTitle, new Queue<double>());
                await GetLastPolls();
            }
            finally
            {
                _guardian.Secure(() => Calculation(), ref _exception);
                if (_exception != null)
                {
                    _eventAggregator
                               .GetEvent<ExceptionEvent>()
                               .Publish(new CalculationException(_calculationException));
                    _exception = null;
                }
                FillCharts();
                _listenThread.Start();
                _reloadFlag = false;
            }
        }
        private async Task<bool> GetLastPolls()
        {
            List<PollModel> polls = new List<PollModel>();
            for (var i = 0; i < _deviceGroupModels.Count; i++)
            {
                var tmpDictionary = new Dictionary<string, PollModel>();
                for (var j = 0; j < _deviceGroupModels[i].DeviceModels.Count; j++)
                {
                    var response = await _pollRepository.Get<PollModel>(1, DateTime.Now, _deviceGroupModels[i].DeviceModels[j].Serial, _userModel.Token) as List<PollModel>;
                    if(response.Count > 0)
                    {
                        var poll = response[0];
                        if (_deviceGroupModels[i].DeviceModels[j].Status)
                            polls.Add(poll);
                        else
                            polls.Add(new PollModel(poll.Serial, poll.Ip, 0, poll.ElectricityConsumption, DateTime.Now));
                        tmpDictionary.Add(_deviceGroupModels[i].DeviceModels[j].Serial, poll);
                    }
                    else
                    {
                        var poll = new PollModel(_deviceGroupModels[i].DeviceModels[j].Serial, _deviceGroupModels[i].DeviceModels[j].Ip, 0, 0, DateTime.Now);
                        if (_deviceGroupModels[i].DeviceModels[j].Status)
                            polls.Add(poll);
                        else
                            polls.Add(new PollModel(poll.Serial, poll.Ip, 0, poll.ElectricityConsumption, DateTime.Now));
                        tmpDictionary.Add(_deviceGroupModels[i].DeviceModels[j].Serial, poll);
                    }
                }
                _lastDevicePolls.Add(_deviceGroupModels[i].Title, tmpDictionary);
            }
            _eventAggregator
                .GetEvent<DataUpdateEvent>()
                .Publish(_lastDevicePolls.Last().Value.Last().Value.Date);
            return true;
        }
        private void Calculation()
        {
            double totalPower = 0;
            for (var i = 0; i < _deviceGroupModels.Count; i++)
                for (var j = 0; j < _lastDevicePolls[_deviceGroupModels[i].Title].Count; j++)
                {
                    var poll = _lastDevicePolls[_deviceGroupModels[i].Title][_lastDevicePolls[_deviceGroupModels[i].Title].Keys.ElementAt(j)];
                    totalPower += poll.Power;
                    if (_deviceGroupModels[i].Title.Equals(_deviceGroup) && poll.Serial.Equals(_deviceSerial))
                    {
                        Power = poll.Power;
                        Electricityconsumption = poll.ElectricityConsumption;
                        SetNewPowerInLineSeries(_deviceTitle, poll.Power);
                    }
                }
            PowerPercent = _power / totalPower * 100;
        }
        private void Listen()
        {
            while (_workFlag)
            {
                try
                {
                    _listener.Socket = new SocketModel();
                    SetNewPoll(_listener.Listen() as PollModel);
                    _guardian.Secure(() => Calculation(), ref _exception);
                    if (_exception != null)
                    {
                        _eventAggregator
                                   .GetEvent<ExceptionEvent>()
                                   .Publish(new CalculationException(_calculationException));
                        _exception = null;
                    }
                    _eventAggregator
                        .GetEvent<DataUpdateEvent>()
                        .Publish(DateTime.Now);
                }
                catch (SocketException ex)
                {
                    _eventAggregator
                               .GetEvent<ExceptionEvent>()
                               .Publish(new InternetException(_tcpServerError));
                    _workFlag = false;
                }
            }
        }
        private void SetNewPoll(PollModel pollModel)
        {
            for (var i = 0; i < _deviceGroupModels.Count; i++)
            {
                if (_lastDevicePolls[_deviceGroupModels[i].Title].ContainsKey(pollModel.Serial))
                    _lastDevicePolls[_deviceGroupModels[i].Title][pollModel.Serial] = pollModel;
            }
        }
        private void SetNewPowerInLineSeries(string title, double power)
        {
            if (_lastPower[title].Count == 0)
            {
                _lastPower[title].Enqueue(power);
            }
            else if (_lastPower[title].Count <= _maxChartPoints && _lastPower[title].Count > 0)
            {
                if (_lastPower[title].Last() != power)
                    _lastPower[title].Enqueue(power);
            }
            else
            {
                if (_lastPower[title].Last() != power)
                {
                    _lastPower[title].Dequeue();
                    _lastPower[title].Enqueue(power);
                }
            }
        }
        private void FillCharts()
        {
            for (var i = 0; i < _cartesianChartSeries.Count; i++)
            {
                var tmpArray = _lastPower[_deviceTitle].ToArray();
                if (_cartesianChartSeries[i].Values.Count > 0)
                {
                    CartesianChartSeries[i].Values = new ChartValues<double>(tmpArray);
                }
                else
                    for (var j = 0; j < tmpArray.Length; j++)
                        CartesianChartSeries[i].Values.Add(tmpArray[j]);
            }
        }
        private async void ChangeStatus()
        {
            try
            {
                if (_internetConnectionChecker.Check())
                    await _deviceRepository.Update(_deviceId, new DeviceModel(_deviceGroup, _deviceSerial, _deviceTitle, _deviceIp, _devicePort, !_deviceStatus, _deviceMaxPower, _deviceMinPower), _userModel.Token);
                else
                    _eventAggregator
                           .GetEvent<ExceptionEvent>()
                           .Publish(new InternetException(_internetError));
            }
            catch (TokenLifetimeException ex)
            {
                await UpdateToken();
                if (_internetConnectionChecker.Check())
                    await _deviceRepository.Update(_deviceId, new DeviceModel(_deviceGroup, _deviceSerial, _deviceTitle, _deviceIp, _devicePort, !_deviceStatus, _deviceMaxPower, _deviceMinPower), _userModel.Token);
                else
                    _eventAggregator
                           .GetEvent<ExceptionEvent>()
                           .Publish(new InternetException(_internetError));
            }
        }
        private void SetDefault()
        {
            _deviceGroupModels = new List<DeviceGroupModel>();
            _lastDevicePolls = new Dictionary<string, Dictionary<string, PollModel>>();
            CartesianChartSeries = new SeriesCollection();
            _lastPower = new Dictionary<string, Queue<double>>();
        }
        private async void DeleteDeviceModel()
        {
            try
            {
                await _deviceRepository.Remove<DeviceModel>(_deviceModel.Id, _userModel.Token);
            }
            catch (TokenLifetimeException ex)
            {
                await UpdateToken();
                await _deviceRepository.Remove<DeviceModel>(_deviceModel.Id, _userModel.Token);
            }
            finally
            {
                _workFlag = false;
                if (_listenThread != null)
                    if (_listenThread.IsAlive)
                        _listenThread.Join();
                SetDefault();
                _eventAggregator
                .GetEvent<ChangePageEvent>()
                .Publish("DispatcherPage.xaml");
                _eventAggregator
                    .GetEvent<DeleteDeviceEvent>()
                    .Publish();
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

        public string DeviceTitle
        {
            get => _deviceTitle;
            set => SetProperty(ref _deviceTitle, value);
        }
        public string DeviceSerial
        {
            get => _deviceSerial;
            set => SetProperty(ref _deviceSerial, value);
        }
        public string DeviceGroup
        {
            get => _deviceGroup;
            set => SetProperty(ref _deviceGroup, value);
        }
        public string DeviceIp
        {
            get => _deviceIp;
            set => SetProperty(ref _deviceIp, value);
        }
        public string DevicePort
        {
            get => _devicePort;
            set => SetProperty(ref _devicePort, value);
        }
        public double DeviceMinPower
        {
            get => _deviceMinPower;
            set => SetProperty(ref _deviceMinPower, value);
        }
        public double DeviceMaxPower
        {
            get => _deviceMaxPower;
            set => SetProperty(ref _deviceMaxPower, value);
        }
        public double Power
        {
            get => Math.Round(_power, 2);
            set
            {
                if (DeviceStatus)
                    SetProperty(ref _power, value);
                else
                    SetProperty(ref _power, 0);
            }
        }
        public double Electricityconsumption
        {
            get => Math.Round(_electricityConsumption, 2);
            set => SetProperty(ref _electricityConsumption, value);
        }
        public double PowerPercent
        {
            get => Math.Round(_powerPercent, 2);
            set => SetProperty(ref _powerPercent, value);
        }
        public bool DeviceStatus
        {
            get => _deviceStatus;
            set
            {
                if(value != _deviceStatus)
                    ChangeStatus();
                SetProperty(ref _deviceStatus, value);
                if (!_deviceStatus)
                    Power = 0;
            }
        }
        public SeriesCollection CartesianChartSeries
        {
            get => _cartesianChartSeries;
            set => SetProperty(ref _cartesianChartSeries, value);
        }


        #endregion



        #region Destructors

        ~DevicePageViewModel()
        {
            _workFlag = false;
            _listenThread.Join();
        }

        #endregion



        #region Fields

        private IEventAggregator _eventAggregator;
        private IGuardian _guardian;
        private UserModel _userModel;
        private InternetConnectionChecker _internetConnectionChecker;
        private DeviceGroupRepository _deviceGroupRepository;
        private DeviceRepository _deviceRepository;
        private PollRepository _pollRepository;
        private AuthorizationService _authorizationService;
        private SocketListener _listener;
        private List<DeviceGroupModel> _deviceGroupModels; // список групп устройств
        private Dictionary<string, Dictionary<string, PollModel>> _lastDevicePolls; // список последних отчётов по группам устройств
        private Dictionary<string, Queue<double>> _lastPower;
        private SeriesCollection _cartesianChartSeries;
        private Thread _listenThread;
        private Exception _exception;
        private DeviceModel _deviceModel;
        private const string _internetError = "Отсутствует подключение к интернету";
        private const string _tcpServerError = "TCP сервер выключен";
        private const string _calculationException = "Ошибка расчётов";
        private string _deviceTitle;
        private string _deviceSerial;
        private string _deviceGroup;
        private string _deviceIp;
        private string _devicePort;
        private double _deviceMinPower;
        private double _deviceMaxPower;
        private double _power;
        private double _electricityConsumption;
        private double _powerPercent;
        private bool _deviceStatus;
        private bool _workFlag = true;
        private bool _reloadFlag = false;
        private int _maxChartPoints = 50;
        private int _deviceId = 0;

        #endregion
    }
}
