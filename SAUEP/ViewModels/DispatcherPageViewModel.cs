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
    public sealed class DispatcherPageViewModel : BindableBase
    {
        #region Constructors

        public DispatcherPageViewModel(IEventAggregator eventAggregator, InternetConnectionChecker internetConnectionChecker,
            DeviceGroupRepository deviceGroupRepository, DeviceRepository deviceRepository, AuthorizationService authorizationService, 
            IGuardian guardian, IListener listener, PollRepository pollRepository, IModel user, DeviceGroupModelViewingSorter deviceGroupModelViewingSorter, DeviceGroupModelViewingFilter deviceGroupModelViewingFilter)
        {
            _eventAggregator = eventAggregator;
            _internetConnectionChecker = internetConnectionChecker;
            _deviceGroupRepository = deviceGroupRepository;
            _deviceRepository = deviceRepository;
            _pollRepository = pollRepository;
            _authorizationService = authorizationService;
            _guardian = guardian;
            _deviceGroupModelViewingSorter = deviceGroupModelViewingSorter;
            _deviceGroupModelViewingFilter = deviceGroupModelViewingFilter;
            _listener = listener as SocketListener;
            _userModel = user as UserModel;
            DeviceGroupModelViewings = new ObservableCollection<DeviceGroupModelViewing>();
            _deviceGroupModels = new List<DeviceGroupModel>();
            _lastDevicePolls = new Dictionary<string, Dictionary<string, PollModel>>();
            _lastPower = new Dictionary<string, Queue<double>>();
            _listenThread = new Thread(new ThreadStart(Listen));
            _lineChartSeries = new SeriesCollection();
            _pieChartSeries = new SeriesCollection();
            SubscriptionToken subscriptionToken = _eventAggregator
                .GetEvent<UserCompliteEvent>()
                .Subscribe((details) =>
                {
                    _userModel = details;
                    if (_internetConnectionChecker.Check())
                    {   
                        GetDeviceGroupsAndDeviceModels();
                    }
                    else
                        _eventAggregator
                               .GetEvent<ExceptionEvent>()
                               .Publish(new InternetException(_internetError));
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
                .GetEvent<GoOnDispatcherPageEvent>()
                .Subscribe(() =>
                {
                    _workFlag = true;
                    _dinamizationFlag = true;
                    _listenThread = new Thread(new ThreadStart(Listen));
                    if(!_createFlag) ReloadList();
                    else
                    {
                        _reloadFlag = false;
                        _deviceGroupModels = new List<DeviceGroupModel>();
                        DeviceGroupModelViewings = new ObservableCollection<DeviceGroupModelViewing>();
                        _lastDevicePolls = new Dictionary<string, Dictionary<string, PollModel>>();
                        _listenThread = new Thread(new ThreadStart(Listen));
                        LineChartSeries = new SeriesCollection();
                        PieChartSeries = new SeriesCollection();
                        _lastPower = new Dictionary<string, Queue<double>>();
                        if (_internetConnectionChecker.Check())
                        {
                            GetDeviceGroupsAndDeviceModels();
                        }
                        else
                            _eventAggregator
                                   .GetEvent<ExceptionEvent>()
                                   .Publish(new InternetException(_internetError));
                    }
                });
            subscriptionToken = _eventAggregator
               .GetEvent<GoOnDeviceGroupHistoryPageEvent>()
               .Subscribe(() =>
               {
                   _workFlag = false;
                   if (_listenThread != null)
                       if (_listenThread.IsAlive)
                           _listenThread.Join();
               }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<GoOnExpensesPageEvent>()
               .Subscribe(() =>
               {
                   _workFlag = false;
                   if (_listenThread != null)
                       if (_listenThread.IsAlive)
                           _listenThread.Join();
               }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<GoOnCreateDeviceGroupPageEvent>()
               .Subscribe(() =>
               {
                   _workFlag = false;
                   if (_listenThread != null)
                       if (_listenThread.IsAlive)
                           _listenThread.Join();
               }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<GoOnUpdateDeviceGroupPageEvent>()
               .Subscribe((details) =>
               {
                   _workFlag = false;
                   if (_listenThread != null)
                       if (_listenThread.IsAlive)
                           _listenThread.Join();
               }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<UpdateDeviceGroupModelEvent>()
               .Subscribe(() =>
               {
                   _reloadFlag = false;
                   _deviceGroupModels = new List<DeviceGroupModel>();
                   DeviceGroupModelViewings = new ObservableCollection<DeviceGroupModelViewing>();
                   _lastDevicePolls = new Dictionary<string, Dictionary<string, PollModel>>();
                   _listenThread = new Thread(new ThreadStart(Listen));
                   LineChartSeries = new SeriesCollection();
                   PieChartSeries = new SeriesCollection();
                   _lastPower = new Dictionary<string, Queue<double>>();
                   if (_internetConnectionChecker.Check())
                   {
                       _workFlag = true;
                       _dinamizationFlag = true;
                       GetDeviceGroupsAndDeviceModels();
                   }
                   else
                       _eventAggregator
                              .GetEvent<ExceptionEvent>()
                              .Publish(new InternetException(_internetError));
               }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<GoOnCreateDevicePageEvent>()
               .Subscribe(() =>
               {
                   _workFlag = false;
                   if (_listenThread != null)
                       if (_listenThread.IsAlive)
                           _listenThread.Join();
               }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<CreateDeviceEvent>()
               .Subscribe(() =>
               {
                   _createFlag = true;
               }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<GoOnUpdateDevicePageEvent>()
               .Subscribe((details) =>
               {
                   _workFlag = false;
                   if (_listenThread != null)
                       if (_listenThread.IsAlive)
                           _listenThread.Join();
               }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<UpdateDeviceEvent>()
               .Subscribe(() =>
               {
                   _reloadFlag = false;
                   _deviceGroupModels = new List<DeviceGroupModel>();
                   DeviceGroupModelViewings = new ObservableCollection<DeviceGroupModelViewing>();
                   _lastDevicePolls = new Dictionary<string, Dictionary<string, PollModel>>();
                   _listenThread = new Thread(new ThreadStart(Listen));
                   LineChartSeries = new SeriesCollection();
                   PieChartSeries = new SeriesCollection();
                   _lastPower = new Dictionary<string, Queue<double>>();
                   if (_internetConnectionChecker.Check())
                   {
                       _workFlag = true;
                       _dinamizationFlag = true;
                       GetDeviceGroupsAndDeviceModels();
                   }
                   else
                       _eventAggregator
                              .GetEvent<ExceptionEvent>()
                              .Publish(new InternetException(_internetError));
               }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<GoOnCreateDevicePollPageEvent>()
               .Subscribe(() =>
               {
                   _workFlag = false;
                   if (_listenThread != null)
                       if (_listenThread.IsAlive)
                           _listenThread.Join();
               }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<GoOnUsersPage>()
               .Subscribe(() =>
               {
                   _workFlag = false;
                   if (_listenThread != null)
                       if (_listenThread.IsAlive)
                           _listenThread.Join();
               }, ThreadOption.UIThread);
            subscriptionToken = _eventAggregator
               .GetEvent<GoOnCreateUserPageEvent>()
               .Subscribe(() =>
               {
                   _workFlag = false;
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
                   DeviceGroupModelViewings = new ObservableCollection<DeviceGroupModelViewing>();
                   _lastDevicePolls = new Dictionary<string, Dictionary<string, PollModel>>();
                   _listenThread = new Thread(new ThreadStart(Listen));
                   LineChartSeries = new SeriesCollection();
                   PieChartSeries = new SeriesCollection();
                   _lastPower = new Dictionary<string, Queue<double>>();
                   if (_internetConnectionChecker.Check())
                   {
                       _workFlag = true;
                       _dinamizationFlag = true;
                       GetDeviceGroupsAndDeviceModels();
                   }
                   else
                       _eventAggregator
                              .GetEvent<ExceptionEvent>()
                              .Publish(new InternetException(_internetError));
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
                        DeviceGroupModelViewings = new ObservableCollection<DeviceGroupModelViewing>(
                                _deviceGroupModelViewingSorter.Sort<DeviceGroupModelViewing>(DeviceGroupModelViewings.ToList(), _sortKey, _sortDirection));
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
                    PowerLowFilter = 0;
                    PowerUpFilter = 0;
                    PowerPercentLowFilter = 0;
                    PowerPercentUpFilter = 0;
                    ElectricityConsumptionLowFilter = 0;
                    ElectricityConsumptionUpFilter = 0;
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
                            DeviceGroupModelViewings = new ObservableCollection<DeviceGroupModelViewing>(
                                _deviceGroupModelViewingFilter.Filtering(DeviceGroupModelViewings.ToList(), "Title", _titleFilter));
                        });
                    if (_powerUpFilter != 0)
                        Dispatcher.CurrentDispatcher.Invoke(() =>
                        {
                            DeviceGroupModelViewings = new ObservableCollection<DeviceGroupModelViewing>(
                                _deviceGroupModelViewingFilter.Filtering(DeviceGroupModelViewings.ToList(), "Power", _powerUpFilter, _powerLowFilter));
                        });
                    if (_powerPercentUpFilter != 0)
                        Dispatcher.CurrentDispatcher.Invoke(() =>
                        {
                            DeviceGroupModelViewings = new ObservableCollection<DeviceGroupModelViewing>(
                                _deviceGroupModelViewingFilter.Filtering(DeviceGroupModelViewings.ToList(), "PowerPercent", _powerPercentUpFilter, _powerPercentLowFilter));
                        });
                    if (_electricityConsumptionUpFilter != 0)
                        Dispatcher.CurrentDispatcher.Invoke(() =>
                        {
                            DeviceGroupModelViewings = new ObservableCollection<DeviceGroupModelViewing>(
                                _deviceGroupModelViewingFilter.Filtering(DeviceGroupModelViewings.ToList(), "ElectricityConsumption", _electricityConsumptionUpFilter, _electricityConsumptionLowFilter));
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
                    _deviceGroupModels[i].DeviceModels.AddRange(deviceModels.Where(device => device.DeviceGroup == _deviceGroupModels[i].Title).ToList());
                    if (_deviceGroupModels[i].DeviceModels.Count == 0)
                        _deviceGroupModels.RemoveAt(i--);
                    else
                    {
                        var deviceGroupModelViewing = new DeviceGroupModelViewing();
                        deviceGroupModelViewing.Title = _deviceGroupModels[i].Title;
                        DeviceGroupModelViewings.Add(deviceGroupModelViewing);
                        LineChartSeries.Add(new LineSeries { Title = _deviceGroupModels[i].Title, Values = new ChartValues<double>() });
                        PieChartSeries.Add(new PieSeries { Title = _deviceGroupModels[i].Title, Values = new ChartValues<double>(), DataLabels = true });
                        if (!_reloadFlag)
                        {
                            _lastPower.Add(_deviceGroupModels[i].Title, new Queue<double>());

                        }
                    }
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
                    _deviceGroupModels[i].DeviceModels.AddRange(deviceModels.Where(device => device.DeviceGroup == _deviceGroupModels[i].Title).ToList());
                    if (_deviceGroupModels[i].DeviceModels.Count == 0)
                        _deviceGroupModels.RemoveAt(i--);
                    else
                    {
                        var deviceGroupModelViewing = new DeviceGroupModelViewing();
                        deviceGroupModelViewing.Title = _deviceGroupModels[i].Title;
                        DeviceGroupModelViewings.Add(deviceGroupModelViewing);
                        LineChartSeries.Add(new LineSeries { Title = _deviceGroupModels[i].Title, Values = new ChartValues<double>() });
                        PieChartSeries.Add(new PieSeries { Title = _deviceGroupModels[i].Title, Values = new ChartValues<double>(), DataLabels = true });
                        if (!_reloadFlag)
                        {
                            _lastPower.Add(_deviceGroupModels[i].Title, new Queue<double>());

                        }
                    }
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
                    DeviceGroupModelViewings = new ObservableCollection<DeviceGroupModelViewing>(
                            _deviceGroupModelViewingSorter.Sort<DeviceGroupModelViewing>(DeviceGroupModelViewings.ToList(), _sortKey, _sortDirection));
                });
                FillCharts();
                _listenThread.Start();
                _reloadFlag = false;
                _createFlag = false;
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
                if(tmpDictionary.Count != 0)
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
            for (var i = 0; i < DeviceGroupModelViewings.Count; i++)
            {
                double deviceGroupPower = 0, allElectricytiConsumption = 0;
                for (var j = 0; j < _lastDevicePolls[DeviceGroupModelViewings[i].Title].Count; j++)
                {
                    totalPower += _lastDevicePolls[DeviceGroupModelViewings[i].Title][_lastDevicePolls[DeviceGroupModelViewings[i].Title].Keys.ElementAt(j)].Power;
                    deviceGroupPower += _lastDevicePolls[DeviceGroupModelViewings[i].Title][_lastDevicePolls[DeviceGroupModelViewings[i].Title].Keys.ElementAt(j)].Power;
                    allElectricytiConsumption += _lastDevicePolls[DeviceGroupModelViewings[i].Title][_lastDevicePolls[DeviceGroupModelViewings[i].Title].Keys.ElementAt(j)].ElectricityConsumption;
                }
                DeviceGroupModelViewings.Where(model => model.Title == DeviceGroupModelViewings[i].Title).ToList()[0].Power = deviceGroupPower;
                DeviceGroupModelViewings.Where(model => model.Title == DeviceGroupModelViewings[i].Title).ToList()[0].ElectricityConsumption = allElectricytiConsumption;
                SetNewPowerInLineSeries(DeviceGroupModelViewings[i].Title, deviceGroupPower);
            }
            for (var i = 0; i < _deviceGroupModels.Count; i++)
            {
                DeviceGroupModelViewings.Where(
                    model => model.Title == _deviceGroupModels[i].Title
                    ).ToList()[0].PowerPercent = DeviceGroupModelViewings.Where(
                        model => model.Title == _deviceGroupModels[i].Title
                        ).ToList()[0].Power / totalPower * 100;
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
                        DeviceGroupModelViewings = new ObservableCollection<DeviceGroupModelViewing>(
                            _deviceGroupModelViewingSorter.Sort<DeviceGroupModelViewing>(DeviceGroupModelViewings.ToList(), _sortKey, _sortDirection));
                    }); 
                    _eventAggregator
                        .GetEvent<DataUpdateEvent>()
                        .Publish(DateTime.Now);
                }
                catch(SocketException ex)
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
            DeviceGroupModelViewings = new ObservableCollection<DeviceGroupModelViewing>();
            _lastDevicePolls = new Dictionary<string, Dictionary<string, PollModel>>();
            _listenThread = new Thread(new ThreadStart(Listen));
            LineChartSeries = new SeriesCollection();
            PieChartSeries = new SeriesCollection();
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

        public ObservableCollection<DeviceGroupModelViewing> DeviceGroupModelViewings
        {
            get => _deviceGroupModelViewings;
            set => SetProperty(ref _deviceGroupModelViewings, value);
        }
        public string TitleFilter
        {
            get => _titleFilter;
            set => SetProperty(ref _titleFilter, value);
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
        public bool DinamizationFlag
        {
            get => _dinamizationFlag;
            set
            {
                if (value) {
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
        public DeviceGroupModelViewing SelectedDeviceGroupModel
        {
            get => _selectedDeviceGroupModelViewing;
            set
            {
                SetProperty(ref _selectedDeviceGroupModelViewing, value);
                if(_selectedDeviceGroupModelViewing != null)
                {
                    _workFlag = false; 
                    _eventAggregator
                         .GetEvent<SelectDeviceGroupModelEvent>()
                         .Publish(_deviceGroupModels.Find(model => model.Title.Equals(_selectedDeviceGroupModelViewing.Title)));
                    _eventAggregator
                        .GetEvent<ChangePageEvent>()
                        .Publish("DeviceDispatcherPage.xaml");
                }
            }
        }

        #endregion



        #region Destructors

        ~DispatcherPageViewModel()
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
        private DeviceGroupModelViewingSorter _deviceGroupModelViewingSorter;
        private DeviceGroupModelViewingFilter _deviceGroupModelViewingFilter;
        private SocketListener _listener;
        private ObservableCollection<DeviceGroupModelViewing> _deviceGroupModelViewings;
        private List<DeviceGroupModel> _deviceGroupModels;
        private Dictionary<string, Dictionary<string, PollModel>> _lastDevicePolls;
        private Dictionary<string, Queue<double>> _lastPower;
        private SeriesCollection _lineChartSeries;
        private SeriesCollection _pieChartSeries;
        private Thread _listenThread;
        private Exception _exception;
        private DeviceGroupModelViewing _selectedDeviceGroupModelViewing;
        private const string _internetError = "Отсутствует подключение к интернету";
        private const string _tcpServerError = "TCP сервер выключен";
        private const string _calculationException = "Ошибка расчётов";
        private string _sortKey = "PowerPercent";
        private string _titleFilter = string.Empty;
        private double _powerUpFilter = 0;
        private double _powerLowFilter = 0;
        private double _powerPercentUpFilter = 0;
        private double _powerPercentLowFilter = 0;
        private double _electricityConsumptionUpFilter = 0;
        private double _electricityConsumptionLowFilter = 0;
        private bool _sortDirection = false;
        private bool _workFlag = true;
        private bool _dinamizationFlag = true;
        private bool _reloadFlag = false;
        private bool _createFlag = false;
        private int _maxChartPoints = 50;

        #endregion
    }
}
