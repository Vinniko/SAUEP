﻿using System;
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
using LiveCharts;
using LiveCharts.Wpf;

namespace SAUEP.WPF.ViewModels
{
    public sealed class DeviceHistoryPageViewModel : BindableBase
    {
        #region Constructros

        public DeviceHistoryPageViewModel(IEventAggregator eventAggregator, InternetConnectionChecker internetConnectionChecker,
            DeviceRepository deviceRepository, AuthorizationService authorizationService, IGuardian guardian, 
            PollRepository pollRepository, IModel user, DeviceModelsSorter deviceModelsSorter)
        {
            _eventAggregator = eventAggregator;
            _checker = internetConnectionChecker;
            _deviceRepository = deviceRepository;
            _authorizationService = authorizationService;
            _guardian = guardian;
            _pollRepository = pollRepository;
            _deviceModelsSorter = deviceModelsSorter;
            _userModel = user as UserModel;
            _deviceModels = new ObservableCollection<DeviceModel>();
            _lineChartSeries = new SeriesCollection();
            _labels = new List<string>();
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
                .GetEvent<GoOnDeviceHistoryPage>()
                .Subscribe((details) =>
                {
                    if (!_workFlag)
                    {
                        DeviceGroupTitle = details.Title;
                        GetDeviceModels();
                        _visibleType = 0;
                        OnPropertyChanged(nameof(VisibleType));
                    }

                }, ThreadOption.UIThread);
        }

        #endregion



        #region Commands
        public RelayCommand BuildChart
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    _lineChartSeries.Clear();
                    GetPolls();
                });
            }
        }
        public RelayCommand Back
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    _eventAggregator
                        .GetEvent<ChangePageEvent>()
                        .Publish("DeviceGroupHistoryPage.xaml");
                    _eventAggregator
                           .GetEvent<GoOnDeviceGroupHistoryPageEvent>()
                           .Publish();
                    ClearCollections();
                    _workFlag = false;
                });
            }
        }
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
                    SortDeviceModels();
                });
            }
        }

        #endregion



        #region Main Logic

        private async void GetDeviceModels()
        {
            try
            {
                var deviceModels = (await _deviceRepository.Get<DeviceModel>((_userModel as UserModel).Token) as List<DeviceModel>).ToList();
                for (var i = 0; i < deviceModels.Count; i++)
                    if (deviceModels[i].DeviceGroup.Equals(DeviceGroupTitle))
                        DeviceModels.Add(deviceModels[i]);

            }
            catch (TokenLifetimeException ex)
            {
                await UpdateToken();
                var deviceModels = (await _deviceRepository.Get<DeviceModel>((_userModel as UserModel).Token) as List<DeviceModel>).ToList();
                for (var i = 0; i < deviceModels.Count; i++)
                    if (deviceModels[i].DeviceGroup.Equals(DeviceGroupTitle))
                        DeviceModels.Add(deviceModels[i]);
            }
            finally
            {
                SortDeviceModels();
                GetPolls();
                _workFlag = true;
            }
        }
        private async void GetPolls()
        {
            try
            {
                for (var i = 0; i < _deviceModels.Count; i++)
                {
                    var date = _startDate - _timeSpan;
                    List<double> deviceElectrecityConsumptions = new List<double>();
                    while (date < _endDate)
                    {
                        var poll = await _pollRepository.Get<PollModel>(1, date, _deviceModels[i].Serial, _userModel.Token);
                        if (poll.Count == 0) deviceElectrecityConsumptions.Add(0);
                        else deviceElectrecityConsumptions.Add(poll.Last().ElectricityConsumption);
                        date += _timeSpan;
                    }
                    LineChartSeries.Add(new LineSeries { Title = _deviceModels[i].Title, Values = new ChartValues<double>(CalculateElectricityConsumptionDiff(deviceElectrecityConsumptions)) });
                }
            }
            catch (TokenLifetimeException ex)
            {
                await UpdateToken();
                for (var i = 0; i < _deviceModels.Count; i++)
                {
                    var date = _startDate - _timeSpan;
                    List<double> deviceElectrecityConsumptions = new List<double>();
                    while (date < _endDate)
                    {
                        var poll = await _pollRepository.Get<PollModel>(1, date, _deviceModels[i].Serial, _userModel.Token);
                        if (poll.Count == 0) deviceElectrecityConsumptions.Add(0);
                        else deviceElectrecityConsumptions.Add(poll.Last().ElectricityConsumption);
                        date += _timeSpan;
                    }
                    LineChartSeries.Add(new LineSeries { Title = _deviceModels[i].Title, Values = new ChartValues<double>(CalculateElectricityConsumptionDiff(deviceElectrecityConsumptions)) });
                }
            }
        }
        private void ChangeTimeSpan(int type)
        {
            switch (type)
            {
                case 0:
                    _timeSpanKey = "All";
                    EnableDateSpan = false;
                    StartDate = new DateTime(1990, 1, 1);
                    EndDate = DateTime.Today;
                    _timeSpan = TimeSpan.FromDays(365);
                    LableTitle = "Years";
                    Labels.Clear();
                    var date = _startDate;
                    while (date < _endDate)
                    {
                        Labels.Add(date.Year.ToString());
                        date += _timeSpan;
                    }
                    break;
                case 1:
                    _timeSpanKey = "Year";
                    EnableDateSpan = false;
                    if (DateTime.IsLeapYear(DateTime.Now.Year))
                        StartDate = DateTime.Today - TimeSpan.FromDays(366);
                    else
                        StartDate = DateTime.Today - TimeSpan.FromDays(365);
                    EndDate = DateTime.Today;
                    _timeSpan = TimeSpan.FromDays(28);
                    LableTitle = "Months";
                    Labels.Clear();
                    date = _startDate;
                    while (date < _endDate)
                    {
                        Labels.Add(date.Month.ToString());
                        date += _timeSpan;
                    }
                    break;
                case 2:
                    _timeSpanKey = "Month";
                    EnableDateSpan = false;
                    StartDate = DateTime.Today - TimeSpan.FromDays(28);
                    EndDate = DateTime.Today;
                    _timeSpan = TimeSpan.FromDays(7);
                    LableTitle = "Weeks";
                    Labels.Clear();
                    date = _startDate;
                    var count = 1;
                    while (date < _endDate)
                    {
                        Labels.Add(count.ToString());
                        date += _timeSpan;
                        count++;
                    }
                    break;
                case 3:
                    _timeSpanKey = "Week";
                    EnableDateSpan = false;
                    StartDate = DateTime.Today - TimeSpan.FromDays(7);
                    EndDate = DateTime.Today;
                    _timeSpan = TimeSpan.FromDays(1);
                    LableTitle = "Days";
                    date = _startDate;
                    while (date < _endDate)
                    {
                        Labels.Add(date.Day.ToString());
                        date += _timeSpan;
                    }
                    break;
                case 4:
                    _timeSpanKey = "Day";
                    EnableDateSpan = false;
                    StartDate = DateTime.Today - TimeSpan.FromDays(1);
                    EndDate = DateTime.Today;
                    _timeSpan = TimeSpan.FromHours(1);
                    LableTitle = "Hours";
                    date = _startDate;
                    while (date < _endDate)
                    {
                        Labels.Add(date.Hour.ToString());
                        date += _timeSpan;
                    }
                    break;
                case 5:
                    _timeSpanKey = "Select";
                    EnableDateSpan = true;
                    _timeSpan = TimeSpan.FromDays(1);
                    LableTitle = "Days";
                    date = _startDate;
                    while (date < _endDate)
                    {
                        Labels.Add(date.Day.ToString());
                        date += _timeSpan;
                    }
                    break;
            }
        }
        private void SortDeviceModels()
        {
            _deviceModels = new ObservableCollection<DeviceModel>(_deviceModelsSorter.Sort(_deviceModels.ToList(), _sortKey, _sortDirection).ToList());
            OnPropertyChanged(nameof(DeviceModels));
        }
        private void ClearCollections()
        {
            _deviceModels.Clear();
            _lineChartSeries.Clear();
            _labels.Clear();
        }
        private List<double> CalculateElectricityConsumptionDiff(List<double> list)
        {
            var result = new List<double>();
            for (var i = 1; i < list.Count; i++)
            {
                result.Add(Math.Abs(list[i] - list[i - 1]));
            }
            return result;
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



        #region Get\Set

        public int VisibleType
        {
            get => _visibleType;
            set
            {
                SetProperty(ref _visibleType, value);
                ChangeTimeSpan(_visibleType);
                _lineChartSeries.Clear();
                if (!_visibleType.Equals(5)) GetPolls();
            }
        }
        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }
        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }
        public bool EnableDateSpan
        {
            get => _enableDateSpan;
            set => SetProperty(ref _enableDateSpan, value);
        }
        public ObservableCollection<DeviceModel> DeviceModels
        {
            get => _deviceModels;
            set => SetProperty(ref _deviceModels, value);
        }
        public SeriesCollection LineChartSeries
        {
            get => _lineChartSeries;
            set => SetProperty(ref _lineChartSeries, value);
        }
        public string LableTitle
        {
            get => _lableTitle;
            set => SetProperty(ref _lableTitle, value);
        }
        public List<string> Labels
        {
            get => _labels;
            set => SetProperty(ref _labels, value);
        }
        public string DeviceGroupTitle
        {
            get => _deviceGroupTitle;
            set => SetProperty(ref _deviceGroupTitle, value);
        }
        public DeviceModel SelectDeviceModel
        {
            get => _selectDeviceModel;
            set
            {
                SetProperty(ref _selectDeviceModel, value);
                if (_selectDeviceModel != null)
                {
                    _eventAggregator
                         .GetEvent<ChangePageEvent>()
                         .Publish("OneDeviceHistoryPage.xaml");
                    _eventAggregator
                           .GetEvent<GoOnOneDeviceHistoryPage>()
                           .Publish(value);
                    _workFlag = false;
                }
            }
        }

        #endregion



        #region Fields

        private IEventAggregator _eventAggregator;
        private IGuardian _guardian;
        private InternetConnectionChecker _checker;
        private UserModel _userModel;
        private DeviceRepository _deviceRepository;
        private PollRepository _pollRepository;
        private AuthorizationService _authorizationService;
        private DeviceModelsSorter _deviceModelsSorter;
        private DeviceModel _selectDeviceModel;
        private int _visibleType;
        private DateTime _startDate = new DateTime(1990, 1, 1);
        private DateTime _endDate = DateTime.Now;
        private TimeSpan _timeSpan = TimeSpan.FromDays(365);
        private bool _enableDateSpan;
        private ObservableCollection<DeviceModel> _deviceModels;
        private SeriesCollection _lineChartSeries;
        private List<string> _labels;
        private string _timeSpanKey = "All";
        private string _lableTitle = "Years";
        private string _sortKey = "Id";
        private string _deviceGroupTitle = string.Empty;
        private bool _sortDirection = true;
        private bool _workFlag = false;

        #endregion
    }
}
