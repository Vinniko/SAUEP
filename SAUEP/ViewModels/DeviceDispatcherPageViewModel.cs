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
using SAUEP.WPF.Models;
using SAUEP.Core.Exceptions;
using SAUEP.WPF.Events;
using SAUEP.WPF.Configs;
using System.Windows.Threading;
using System.Net.Sockets;
using SAUEP.WPF.Services;
using LiveCharts;
using LiveCharts.Wpf;

namespace SAUEP.WPF.ViewModels
{
    public sealed class DeviceDispatcherPageViewModel : BindableBase
    {
        #region Constructors

        public DeviceDispatcherPageViewModel(IEventAggregator eventAggregator, InternetConnectionChecker internetConnectionChecker,
            DeviceGroupRepository deviceGroupRepository, DeviceRepository deviceRepository, AuthorizationService authorizationService, 
            IGuardian guardian, IListener listener, PollRepository pollRepository, IModel user, DeviceModelViewingSorter deviceModelViewingSorter, DeviceModelViewingFilter deviceModelViewingFilter)
        {
            _eventAggregator = eventAggregator;
            _internetConnectionChecker = internetConnectionChecker;
            _deviceGroupRepository = deviceGroupRepository;
            _deviceRepository = deviceRepository;
            _pollRepository = pollRepository;
            _authorizationService = authorizationService;
            _guardian = guardian;
            _deviceModelViewingSorter = deviceModelViewingSorter;
            _deviceModelViewingFilter = deviceModelViewingFilter;
            _listener = listener as SocketListener;
            _userModel = user as UserModel;
            DeviceModelViewings = new ObservableCollection<DeviceModelViewing>();
            _deviceGroupModels = new List<DeviceGroupModel>();
            _lastDevicePolls = new Dictionary<string, Dictionary<string, PollModel>>();
            _lastPower = new Dictionary<string, Queue<double>>();
            _lineChartSeries = new SeriesCollection();
            _pieChartSeries = new SeriesCollection();
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
                    if(_listenThread.IsAlive)
                        _listenThread.Join();
                }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
                .GetEvent<SelectDeviceGroupModelEvent>()
                .Subscribe((details) =>
                {
                    if (_internetConnectionChecker.Check())
                    {
                        _listenThread = new Thread(new ThreadStart(Listen));
                        _workFlag = true;
                        DeviceGropModelTitle = details.Title;
                        _deviceGroupModel = details;
                        GetDeviceGroupsAndDeviceModels();
                    }
                    else
                        _eventAggregator
                               .GetEvent<ExceptionEvent>()
                               .Publish(new InternetException(_internetError));
                }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
                .GetEvent<GoOnDeviceDispatcherPageEvent>()
                .Subscribe(() =>
                {
                    _workFlag = true;
                    _dinamizationFlag = true;
                    _listenThread = new Thread(new ThreadStart(Listen));
                    ReloadList();
                }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<GoOnDeviceGroupHistoryPageEvent>()
               .Subscribe(() =>
               {
                   _workFlag = false;
                   _sortDirection = false;
                   _sortKey = "PowerPercent";
                   TitleFilter = string.Empty;
                   SerialFilter = string.Empty;
                   PowerLowFilter = 0;
                   PowerUpFilter = 0;
                   PowerPercentLowFilter = 0;
                   PowerPercentUpFilter = 0;
                   ElectricityConsumptionLowFilter = 0;
                   ElectricityConsumptionUpFilter = 0;
                   StatusFilter = false;
                   _statusFilter = 0;
                   SetDefault();
                   if (_listenThread != null)
                       if (_listenThread.IsAlive)
                           _listenThread.Join();
               }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<GoOnExpensesPageEvent>()
               .Subscribe(() =>
               {
                   _workFlag = false;
                   _sortDirection = false;
                   _sortKey = "PowerPercent";
                   TitleFilter = string.Empty;
                   SerialFilter = string.Empty;
                   PowerLowFilter = 0;
                   PowerUpFilter = 0;
                   PowerPercentLowFilter = 0;
                   PowerPercentUpFilter = 0;
                   ElectricityConsumptionLowFilter = 0;
                   ElectricityConsumptionUpFilter = 0;
                   StatusFilter = false;
                   _statusFilter = 0;
                   SetDefault();
                   if (_listenThread != null)
                       if (_listenThread.IsAlive)
                           _listenThread.Join();
               }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<GoOnCreateDeviceGroupPageEvent>()
               .Subscribe(() =>
               {
                   _workFlag = false;
                   _sortDirection = false;
                   _sortKey = "PowerPercent";
                   TitleFilter = string.Empty;
                   SerialFilter = string.Empty;
                   PowerLowFilter = 0;
                   PowerUpFilter = 0;
                   PowerPercentLowFilter = 0;
                   PowerPercentUpFilter = 0;
                   ElectricityConsumptionLowFilter = 0;
                   ElectricityConsumptionUpFilter = 0;
                   StatusFilter = false;
                   _statusFilter = 0;
                   SetDefault();
                   if (_listenThread != null)
                       if (_listenThread.IsAlive)
                           _listenThread.Join();
               }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<GoOnUpdateDeviceGroupPageEvent>()
               .Subscribe((details) =>
               {
                   _workFlag = false;
                   _sortDirection = false;
                   _sortKey = "PowerPercent";
                   TitleFilter = string.Empty;
                   SerialFilter = string.Empty;
                   PowerLowFilter = 0;
                   PowerUpFilter = 0;
                   PowerPercentLowFilter = 0;
                   PowerPercentUpFilter = 0;
                   ElectricityConsumptionLowFilter = 0;
                   ElectricityConsumptionUpFilter = 0;
                   StatusFilter = false;
                   _statusFilter = 0;
                   SetDefault();
                   if (_listenThread != null)
                       if (_listenThread.IsAlive)
                           _listenThread.Join();
               }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<GoOnCreateDevicePageEvent>()
               .Subscribe(() =>
               {
                   _workFlag = false;
                   _sortDirection = false;
                   _sortKey = "PowerPercent";
                   TitleFilter = string.Empty;
                   SerialFilter = string.Empty;
                   PowerLowFilter = 0;
                   PowerUpFilter = 0;
                   PowerPercentLowFilter = 0;
                   PowerPercentUpFilter = 0;
                   ElectricityConsumptionLowFilter = 0;
                   ElectricityConsumptionUpFilter = 0;
                   StatusFilter = false;
                   _statusFilter = 0;
                   SetDefault();
                   if (_listenThread != null)
                       if (_listenThread.IsAlive)
                           _listenThread.Join();
               }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<GoOnUpdateDevicePageEvent>()
               .Subscribe((details) =>
               {
                   _workFlag = false;
                   _sortDirection = false;
                   _sortKey = "PowerPercent";
                   TitleFilter = string.Empty;
                   SerialFilter = string.Empty;
                   PowerLowFilter = 0;
                   PowerUpFilter = 0;
                   PowerPercentLowFilter = 0;
                   PowerPercentUpFilter = 0;
                   ElectricityConsumptionLowFilter = 0;
                   ElectricityConsumptionUpFilter = 0;
                   StatusFilter = false;
                   _statusFilter = 0;
                   SetDefault();
                   if (_listenThread != null)
                       if (_listenThread.IsAlive)
                           _listenThread.Join();
               }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<GoOnCreateDevicePollPageEvent>()
               .Subscribe(() =>
               {
                   _workFlag = false;
                   _sortDirection = false;
                   _sortKey = "PowerPercent";
                   TitleFilter = string.Empty;
                   SerialFilter = string.Empty;
                   PowerLowFilter = 0;
                   PowerUpFilter = 0;
                   PowerPercentLowFilter = 0;
                   PowerPercentUpFilter = 0;
                   ElectricityConsumptionLowFilter = 0;
                   ElectricityConsumptionUpFilter = 0;
                   StatusFilter = false;
                   _statusFilter = 0;
                   SetDefault();
                   if (_listenThread != null)
                       if (_listenThread.IsAlive)
                           _listenThread.Join();
               }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<GoOnUsersPage>()
               .Subscribe(() =>
               {
                   _workFlag = false;
                   _sortDirection = false;
                   _sortKey = "PowerPercent";
                   TitleFilter = string.Empty;
                   SerialFilter = string.Empty;
                   PowerLowFilter = 0;
                   PowerUpFilter = 0;
                   PowerPercentLowFilter = 0;
                   PowerPercentUpFilter = 0;
                   ElectricityConsumptionLowFilter = 0;
                   ElectricityConsumptionUpFilter = 0;
                   StatusFilter = false;
                   _statusFilter = 0;
                   SetDefault();
                   if (_listenThread != null)
                       if (_listenThread.IsAlive)
                           _listenThread.Join();
               }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<GoOnCreateUserPageEvent>()
               .Subscribe(() =>
               {
                   _workFlag = false;
                   _sortDirection = false;
                   _sortKey = "PowerPercent";
                   TitleFilter = string.Empty;
                   SerialFilter = string.Empty;
                   PowerLowFilter = 0;
                   PowerUpFilter = 0;
                   PowerPercentLowFilter = 0;
                   PowerPercentUpFilter = 0;
                   ElectricityConsumptionLowFilter = 0;
                   ElectricityConsumptionUpFilter = 0;
                   StatusFilter = false;
                   _statusFilter = 0;
                   SetDefault();
                   if (_listenThread != null)
                       if (_listenThread.IsAlive)
                           _listenThread.Join();
               }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<DeleteDeviceEvent>()
               .Subscribe(() =>
               {
                   _reloadFlag = false;
                   _deviceGroupModels = new List<DeviceGroupModel>();
                   DeviceModelViewings = new ObservableCollection<DeviceModelViewing>();
                   _lastDevicePolls = new Dictionary<string, Dictionary<string, PollModel>>();
                   _listenThread = new Thread(new ThreadStart(Listen));
                   LineChartSeries = new SeriesCollection();
                   PieChartSeries = new SeriesCollection();
                   _lastPower = new Dictionary<string, Queue<double>>();
               }, ThreadOption.UIThread);
        }

        #endregion



        #region Commands

        public RelayCommand Sort
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    if (_sortKey.Equals(obj.ToString()))
                    {
                        _sortDirection = !_sortDirection;
                    }
                    else
                    {
                        _sortDirection = true;
                        _sortKey = obj.ToString();
                    }
                    Dispatcher.CurrentDispatcher.Invoke(() =>
                    {
                        DeviceModelViewings = new ObservableCollection<DeviceModelViewing>(
                                _deviceModelViewingSorter.Sort(DeviceModelViewings.ToList(), _sortKey, _sortDirection));
                    });
                });
            }
        }
        public RelayCommand Reset
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    _sortDirection = false;
                    _sortKey = "PowerPercent";
                    TitleFilter = string.Empty;
                    SerialFilter = string.Empty;
                    PowerLowFilter = 0;
                    PowerUpFilter = 0;
                    PowerPercentLowFilter = 0;
                    PowerPercentUpFilter = 0;
                    ElectricityConsumptionLowFilter = 0;
                    ElectricityConsumptionUpFilter = 0;
                    StatusFilter = false;
                    _statusFilter = 0;
                    ReloadList();
                });
            }
        }
        public RelayCommand Filter
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    if (!_titleFilter.Equals(string.Empty))
                        Dispatcher.CurrentDispatcher.Invoke(() =>
                        {
                            DeviceModelViewings = new ObservableCollection<DeviceModelViewing>(
                                _deviceModelViewingFilter.Filtering(DeviceModelViewings.ToList(), "Title", _titleFilter));
                        });
                    if (!_serialFilter.Equals(string.Empty))
                        Dispatcher.CurrentDispatcher.Invoke(() =>
                        {
                            DeviceModelViewings = new ObservableCollection<DeviceModelViewing>(
                                _deviceModelViewingFilter.Filtering(DeviceModelViewings.ToList(), "Serial", _serialFilter));
                        });
                    if (_powerUpFilter != 0)
                        Dispatcher.CurrentDispatcher.Invoke(() =>
                        {
                            DeviceModelViewings = new ObservableCollection<DeviceModelViewing>(
                                _deviceModelViewingFilter.Filtering(DeviceModelViewings.ToList(), "Power", _powerUpFilter, _powerLowFilter));
                        });
                    if (_powerPercentUpFilter != 0)
                        Dispatcher.CurrentDispatcher.Invoke(() =>
                        {
                            DeviceModelViewings = new ObservableCollection<DeviceModelViewing>(
                                _deviceModelViewingFilter.Filtering(DeviceModelViewings.ToList(), "PowerPercent", _powerPercentUpFilter, _powerPercentLowFilter));
                        });
                    if (_electricityConsumptionUpFilter != 0)
                        Dispatcher.CurrentDispatcher.Invoke(() =>
                        {
                            DeviceModelViewings = new ObservableCollection<DeviceModelViewing>(
                                _deviceModelViewingFilter.Filtering(DeviceModelViewings.ToList(), "ElectricityConsumption", _electricityConsumptionUpFilter, _electricityConsumptionLowFilter));
                        });
                    if (_statusFilter != 0)
                        Dispatcher.CurrentDispatcher.Invoke(() =>
                        {
                            DeviceModelViewings = new ObservableCollection<DeviceModelViewing>(
                                _deviceModelViewingFilter.Filtering(DeviceModelViewings.ToList(), "Status", _statusFilter));
                        });
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
                        .Publish("DispatcherPage.xaml");
                    _eventAggregator
                           .GetEvent<GoOnDispatcherPageEvent>()
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
                       .GetEvent<GoOnUpdateDeviceGroupPageEvent>()
                       .Publish(_deviceGroupModel);
                    _eventAggregator
                        .GetEvent<ChangePageEvent>()
                        .Publish("UpdateDeviceGroupPage.xaml");
                });
            }
        }
        public RelayCommand Delete
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    DeleteDeviceGroupModel();
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
                deviceModels = _deviceGroupModels.Where(model => model.Title.Equals(_deviceGropModelTitle)).Last().DeviceModels.ToList();
                for (var i = 0; i < deviceModels.Count; i++)
                {
                    DeviceModelViewings.Add(new DeviceModelViewing(deviceModels[i]));
                    LineChartSeries.Add(new LineSeries { Title = deviceModels[i].Title, Values = new ChartValues<double>() });
                    PieChartSeries.Add(new PieSeries { Title = deviceModels[i].Title, Values = new ChartValues<double>(), DataLabels = true }); 
                    if (!_reloadFlag)
                        _lastPower.Add(deviceModels[i].Title, new Queue<double>());
                }
                await GetLastPolls();
            }
            catch(TokenLifetimeException ex)
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
                deviceModels = _deviceGroupModels.Where(model => model.Title.Equals(_deviceGropModelTitle)).Last().DeviceModels.ToList();
                for (var i = 0; i < deviceModels.Count; i++)
                {
                    DeviceModelViewings.Add(new DeviceModelViewing(deviceModels[i]));
                    LineChartSeries.Add(new LineSeries { Title = deviceModels[i].Title, Values = new ChartValues<double>() });
                    PieChartSeries.Add(new PieSeries { Title = deviceModels[i].Title, Values = new ChartValues<double>(), DataLabels = true });
                    if (!_reloadFlag) 
                        _lastPower.Add(deviceModels[i].Title, new Queue<double>());
                }
                await GetLastPolls();
            }
            finally
            {
                _guardian.Secure(() => Calculation(), ref _exception);
                if(_exception != null)
                {
                    _eventAggregator
                               .GetEvent<ExceptionEvent>()
                               .Publish(new CalculationException(_calculationException));
                    _exception = null;
                }
                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    DeviceModelViewings = new ObservableCollection<DeviceModelViewing>(
                            _deviceModelViewingSorter.Sort(DeviceModelViewings.ToList(), _sortKey, _sortDirection));
                });
                FillCharts();
                _listenThread.Start();
                if (!_reloadFlag) DinamizationFlag = true;
                _reloadFlag = false;
            }
        }
        private async Task<bool> GetLastPolls()
        {
            List<PollModel> polls = new List<PollModel>();
            for(var i = 0; i < _deviceGroupModels.Count; i++)
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
                        {
                            poll.Date = DateTime.Now;
                            poll.Power = 0;
                            polls.Add(poll);
                        }
                        tmpDictionary.Add(_deviceGroupModels[i].DeviceModels[j].Serial, poll);
                    }
                    else
                    {
                        var poll = new PollModel(_deviceGroupModels[i].DeviceModels[j].Serial, _deviceGroupModels[i].DeviceModels[j].Ip, 0, 0, DateTime.Now);
                        if (_deviceGroupModels[i].DeviceModels[j].Status)
                            polls.Add(poll);
                        else
                        {
                            poll.Date = DateTime.Now;
                            poll.Power = 0;
                            polls.Add(poll);
                        }
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
            {
                double deviceGroupPower = 0, allElectricytiConsumption = 0;
                for (var j = 0; j < _lastDevicePolls[_deviceGroupModels[i].Title].Count; j++)
                {
                    var poll = _lastDevicePolls[_deviceGroupModels[i].Title][_lastDevicePolls[_deviceGroupModels[i].Title].Keys.ElementAt(j)];
                    totalPower += poll.Power;
                    deviceGroupPower += poll.Power;
                    allElectricytiConsumption += poll.ElectricityConsumption;
                    if (_deviceGroupModels[i].Title.Equals(_deviceGropModelTitle))
                    {
                        DeviceModelViewings.Where(model => model.Serial == poll.Serial).ToList().Last().Power = poll.Power;
                        DeviceModelViewings.Where(model => model.Serial == poll.Serial).ToList().Last().ElectricityConsumption = poll.ElectricityConsumption;
                        SetNewPowerInLineSeries(DeviceModelViewings.Where(model => model.Serial == poll.Serial).ToList().Last().Title, poll.Power);
                    }
                }
                if (_deviceGroupModels[i].Title.Equals(_deviceGropModelTitle))
                {
                    DeviceGroupModelElectricityconsumption = allElectricytiConsumption;
                    DeviceGroupModelPower = deviceGroupPower;
                }
            }
            for (var i = 0; i < _deviceGroupModels.Where(model => model.Title.Equals(_deviceGropModelTitle)).Last().DeviceModels.Count; i++)
            {
                var deviceModel = _deviceGroupModels.Where(model => model.Title.Equals(_deviceGropModelTitle)).Last().DeviceModels[i];
                DeviceModelViewings.Where(
                    model => model.Serial.Equals(deviceModel.Serial)
                    ).Last().PowerPercent = DeviceModelViewings.Where(
                        model => model.Serial.Equals(deviceModel.Serial)
                        ).Last().Power / totalPower * 100;
            }
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
                    Dispatcher.CurrentDispatcher.Invoke(() =>
                    {
                        DeviceModelViewings = new ObservableCollection<DeviceModelViewing>(
                            _deviceModelViewingSorter.Sort(DeviceModelViewings.ToList(), _sortKey, _sortDirection));
                    });
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
        private void ReloadList()
        {
            _reloadFlag = true;
            _deviceGroupModels = new List<DeviceGroupModel>();
            DeviceModelViewings = new ObservableCollection<DeviceModelViewing>();
            LineChartSeries = new SeriesCollection();
            PieChartSeries = new SeriesCollection();
            _lastDevicePolls = new Dictionary<string, Dictionary<string, PollModel>>();
            _listenThread = new Thread(new ThreadStart(Listen));
            GetDeviceGroupsAndDeviceModels();
        }
        private void SetNewPowerInLineSeries(string title, double power)
        {
            if(_lastPower[title].Count == 0)
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
            for (var i = 0; i < _lineChartSeries.Count; i++)
            {
                var tmpArray = _lastPower[_lineChartSeries[i].Title].ToArray();
                if (LineChartSeries[i].Values.Count > 0)
                {
                    LineChartSeries[i].Values = new ChartValues<double>(tmpArray);
                }
                else
                    for (var j = 0; j < tmpArray.Length; j++)
                        LineChartSeries[i].Values.Add(tmpArray[j]);
                PieChartSeries[i].Values = new ChartValues<double> { Math.Round(tmpArray.Average(), 3) };
            }
        }
        private void SetDefault()
        {
            _deviceGroupModels = new List<DeviceGroupModel>();
            DeviceModelViewings = new ObservableCollection<DeviceModelViewing>();
            _lastDevicePolls = new Dictionary<string, Dictionary<string, PollModel>>();
            LineChartSeries = new SeriesCollection();
            PieChartSeries = new SeriesCollection();
            _lastPower = new Dictionary<string, Queue<double>>();
        }
        private async void DeleteDeviceGroupModel()
        {
            try
            {
                await _deviceGroupRepository.Remove<DeviceGroupModel>(_deviceGroupModel.Id, _userModel.Token);
            }
            catch (TokenLifetimeException ex)
            {
                await UpdateToken();
                await _deviceGroupRepository.Remove<DeviceGroupModel>(_deviceGroupModel.Id, _userModel.Token);
            }
            finally
            {
                _workFlag = false;
                _sortDirection = false;
                _sortKey = "PowerPercent";
                TitleFilter = string.Empty;
                SerialFilter = string.Empty;
                PowerLowFilter = 0;
                PowerUpFilter = 0;
                PowerPercentLowFilter = 0;
                PowerPercentUpFilter = 0;
                ElectricityConsumptionLowFilter = 0;
                ElectricityConsumptionUpFilter = 0;
                StatusFilter = false;
                _statusFilter = 0;
                SetDefault();
                if (_listenThread != null)
                    if (_listenThread.IsAlive)
                        _listenThread.Join();
                _eventAggregator
                .GetEvent<ChangePageEvent>()
                .Publish("DispatcherPage.xaml");
                _eventAggregator
                    .GetEvent<UpdateDeviceGroupModelEvent>()
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

        public ObservableCollection<DeviceModelViewing> DeviceModelViewings
        {
            get => _deviceModelViewings;
            set => SetProperty(ref _deviceModelViewings, value);
        }
        public string TitleFilter
        {
            get => _titleFilter;
            set => SetProperty(ref _titleFilter, value);
        }
        public string SerialFilter
        {
            get => _serialFilter;
            set => SetProperty(ref _serialFilter, value);
        }
        public double PowerUpFilter
        {
            get => _powerUpFilter;
            set => SetProperty(ref _powerUpFilter, value);
        }
        public double PowerLowFilter
        {
            get => _powerLowFilter;
            set => SetProperty(ref _powerLowFilter, value);
        }
        public double PowerPercentUpFilter
        {
            get => _powerPercentUpFilter;
            set => SetProperty(ref _powerPercentUpFilter, value);
        }
        public double PowerPercentLowFilter
        {
            get => _powerPercentLowFilter;
            set => SetProperty(ref _powerPercentLowFilter, value);
        }
        public double ElectricityConsumptionUpFilter
        {
            get => _electricityConsumptionUpFilter;
            set => SetProperty(ref _electricityConsumptionUpFilter, value);
        }
        public double ElectricityConsumptionLowFilter
        {
            get => _electricityConsumptionLowFilter;
            set => SetProperty(ref _electricityConsumptionLowFilter, value);
        }
        public bool StatusFilter
        {
            get
            {
                if (_statusFilter == 1) return true;
                else return false;
            }
            set
            {
                if (value) _statusFilter = 1;
                else _statusFilter = 2;
            }
        }
        public bool DinamizationFlag
        {
            get => _dinamizationFlag;
            set
            {
                if (value)
                {
                    _workFlag = true;
                    if (!_listenThread.IsAlive)
                    {
                        _listenThread = new Thread(new ThreadStart(Listen));
                        _listenThread.Start();
                    }
                }
                else _workFlag = false;
                SetProperty(ref _dinamizationFlag, value);
                OnPropertyChanged(nameof(FilterEnableFlag));
            }
        }
        public bool FilterEnableFlag
        {
            get => !_dinamizationFlag;
        }
        public SeriesCollection LineChartSeries
        {
            get => _lineChartSeries;
            set => SetProperty(ref _lineChartSeries, value);
        }
        public SeriesCollection PieChartSeries
        {
            get => _pieChartSeries;
            set => SetProperty(ref _pieChartSeries, value);
        }
        public string DeviceGropModelTitle
        {
            get => _deviceGropModelTitle;
            set => SetProperty(ref _deviceGropModelTitle, value);
        }
        public double DeviceGroupModelElectricityconsumption
        {
            get => Math.Round(_deviceGroupModelElectricityconsumption, 2);
            set => SetProperty(ref _deviceGroupModelElectricityconsumption, value);
        }
        public double DeviceGroupModelPower
        {
            get => Math.Round(_deviceGroupModePower, 2);
            set => SetProperty(ref _deviceGroupModePower, value);
        }
        public DeviceModelViewing SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                SetProperty(ref _selectedDevice, value);
                if (_selectedDevice != null)
                {
                    _workFlag = false;
                    _eventAggregator
                         .GetEvent<SelectDeviceModelEvent>()
                         .Publish(_deviceGroupModels.Find(model => 
                            model.Title.Equals(_deviceGropModelTitle)).DeviceModels.Where(device => 
                                device.Serial.Equals(value.Serial)).Last());
                    _eventAggregator
                        .GetEvent<ChangePageEvent>()
                        .Publish("DevicePage.xaml");
                }
            }
        }

        #endregion



        #region Destructors

        ~DeviceDispatcherPageViewModel()
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
        private DeviceModelViewingSorter _deviceModelViewingSorter;
        private DeviceModelViewingFilter _deviceModelViewingFilter;
        private SocketListener _listener;
        private List<DeviceGroupModel> _deviceGroupModels; // список групп устройств
        private Dictionary<string, Dictionary<string, PollModel>> _lastDevicePolls; // список последних отчётов по группам устройств
        private ObservableCollection<DeviceModelViewing> _deviceModelViewings; 
        private Dictionary<string, Queue<double>> _lastPower;
        private SeriesCollection _lineChartSeries;
        private SeriesCollection _pieChartSeries;
        private Thread _listenThread;
        private Exception _exception;
        private DeviceModelViewing _selectedDevice;
        private DeviceGroupModel _deviceGroupModel;
        private const string _internetError = "Отсутствует подключение к интернету";
        private const string _tcpServerError = "TCP сервер выключен";
        private const string _calculationException = "Ошибка расчётов";
        private string _sortKey = "PowerPercent";
        private string _titleFilter = string.Empty;
        private string _serialFilter = string.Empty;
        private string _deviceGropModelTitle = string.Empty;
        private double _powerUpFilter = 0;
        private double _powerLowFilter = 0;
        private double _powerPercentUpFilter = 0;
        private double _powerPercentLowFilter = 0;
        private double _electricityConsumptionUpFilter = 0;
        private double _electricityConsumptionLowFilter = 0;
        private double _deviceGroupModelElectricityconsumption = 0;
        private double _deviceGroupModePower = 0;
        private bool _sortDirection = false;
        private bool _workFlag = true;
        private bool _dinamizationFlag = true;
        private bool _reloadFlag = false;
        private int _statusFilter = 0;
        private int _maxChartPoints = 50;

        #endregion
    }
}
