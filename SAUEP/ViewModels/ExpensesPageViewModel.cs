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
using LiveCharts;
using LiveCharts.Wpf;

namespace SAUEP.WPF.ViewModels
{
    public sealed class ExpensesPageViewModel : BindableBase
    {
        #region Constructors

        public ExpensesPageViewModel(IEventAggregator eventAggregator, InternetConnectionChecker internetConnectionChecker, AuthorizationService authorizationService, IGuardian guardian,
            PollRepository pollRepository, IModel user, DeviceGroupRepository deviceGroupRepository, DeviceRepository deviceRepository)
        {
            _eventAggregator = eventAggregator;
            _internetConnectionChecker = internetConnectionChecker;
            _authorizationService = authorizationService;
            _guardian = guardian;
            _pollRepository = pollRepository;
            _deviceGroupRepository = deviceGroupRepository;
            _deviceRepository = deviceRepository;
            _userModel = user as UserModel;
            _lineChartSeries = new SeriesCollection();
            _labels = new List<string>();
            _deviceGroupModels = new List<DeviceGroupModel>();
            _standartPrice = 0;
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
                .GetEvent<GoOnExpensesPageEvent>()
                .Subscribe(() =>
                {
                    if (!_workFlag)
                    {
                        if (_internetConnectionChecker.Check())
                        {
                            GetDeviceGroups();
                            ChangeTimeSpan(0);
                            VisibleType = 0;
                            _workFlag = true;
                        }
                        else
                            _eventAggregator
                                .GetEvent<ExceptionEvent>()
                                .Publish(new InternetException(_internetError));
                    }
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
                    _eventAggregator
                        .GetEvent<ChangePageEvent>()
                        .Publish("DispatcherPage.xaml");
                    _eventAggregator
                           .GetEvent<GoOnDispatcherPageEvent>()
                           .Publish();
                    ClearCollections();
                    _workFlag = false;
                });
            }
        }
        public RelayCommand Calculate
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    ChangeTimeSpan(_visibleType);
                    CalculationWrapper();
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
            catch(TokenLifetimeException ex)
            {
                await UpdateToken();
                _deviceGroupModels = (await _deviceGroupRepository.Get<DeviceGroupModel>(_userModel.Token)).ToList();
            }
            finally
            {
                GetDevices();
            }
        }
        private async void GetDevices()
        {
            try
            {
                var devices = await _deviceRepository.Get<DeviceModel>(_userModel.Token);
                for (var i = 0; i < _deviceGroupModels.Count; i++)
                    for (var j = 0; j < devices.Count; j++)
                        if (devices.ElementAt(j).DeviceGroup.Equals(_deviceGroupModels[i].Title))
                            _deviceGroupModels[i].DeviceModels.Add(devices.ElementAt(j));
            }
            catch(TokenLifetimeException ex)
            {
                await UpdateToken();
                var devices = await _deviceRepository.Get<DeviceModel>(_userModel.Token);
                for (var i = 0; i < _deviceGroupModels.Count; i++)
                    for (var j = 0; j < devices.Count; j++)
                        if (devices.ElementAt(j).DeviceGroup.Equals(_deviceGroupModels[i].Title))
                            _deviceGroupModels[i].DeviceModels.Add(devices.ElementAt(j));
            }
        }
        private void ChangeTimeSpan(int type)
        {
            switch (type)
            {
                case 0:
                    _timeSpanKey = "Three";
                    EnableDateSpan = false;
                    StartDate = DateTime.Today - TimeSpan.FromDays(84);
                    EndDate = DateTime.Today;
                    _timeSpan = TimeSpan.FromDays(28);
                    LableTitle = "Month";
                    Labels.Clear();
                    var date = _startDate;
                    while (date <= _endDate)
                    {
                        Labels.Add(date.Month.ToString());
                        date += _timeSpan;
                    }
                    break;
                case 1:
                    _timeSpanKey = "Month";
                    EnableDateSpan = false;
                    StartDate = DateTime.Today - TimeSpan.FromDays(28);
                    EndDate = DateTime.Today;
                    _timeSpan = TimeSpan.FromDays(7);
                    LableTitle = "Weeks";
                    Labels.Clear();
                    date = _startDate;
                    var count = 1;
                    while (date <= _endDate)
                    {
                        Labels.Add(count.ToString());
                        date += _timeSpan;
                        count++;
                    }
                    break;
                case 2:
                    _timeSpanKey = "Week";
                    EnableDateSpan = false;
                    StartDate = DateTime.Today - TimeSpan.FromDays(7);
                    EndDate = DateTime.Today;
                    _timeSpan = TimeSpan.FromDays(1);
                    LableTitle = "Days";
                    Labels.Clear();
                    date = _startDate;
                    while (date <= _endDate)
                    {
                        Labels.Add(date.DayOfWeek.ToString());
                        date += _timeSpan;
                    }
                    break;
                case 3:
                    _timeSpanKey = "Day";
                    EnableDateSpan = false;
                    StartDate = DateTime.Now - TimeSpan.FromDays(1);
                    EndDate = DateTime.Now;
                    _timeSpan = TimeSpan.FromHours(1);
                    LableTitle = "Hours";
                    Labels.Clear();
                    for(var i = 0; i < 24; i++)
                    {
                        Labels.Add((_endDate - TimeSpan.FromHours(24-i)).Hour.ToString());
                    }
                    break;
                case 4:
                    _timeSpanKey = "Select";
                    EnableDateSpan = true;
                    _timeSpan = TimeSpan.FromDays(1);
                    LableTitle = "Days";
                    Labels.Clear();
                    date = _startDate;
                    while (date <= _endDate)
                    {
                        Labels.Add(date.Day.ToString());
                        date += _timeSpan;
                    }
                    break;
            }
        }
        private void ChangeExpenseType(int type)
        {
            switch (type)
            {
                case 0:
                    StandartEnable = true;
                    TwoFactorsEnable = false;
                    ThreeFactorEnable = false;
                    break;
                case 1:
                    StandartEnable = false;
                    TwoFactorsEnable = true;
                    ThreeFactorEnable = false;
                    break;
                case 2:
                    StandartEnable = false;
                    TwoFactorsEnable = true;
                    ThreeFactorEnable = true;
                    break;
            }
        }
        private void ClearCollections()
        {
            _lineChartSeries.Clear();
            _labels.Clear();
            _deviceGroupModels.Clear();
        }
        private async void CalculationWrapper()
        {
            _lineChartSeries.Clear();
            var firstDate = _startDate;
            var lastDate = _startDate + _timeSpan;
            ChartValues<double> chartCollection = new ChartValues<double>();
            if (_expenseType == 0 && _standartPrice >= 0)
                while (lastDate <= _endDate)
                {
                    chartCollection.Add((double)CalculateForFirstExpenseType((await GetPollsForExpense(firstDate, lastDate))));
                    firstDate += _timeSpan;
                    lastDate += _timeSpan;
                }
            else if(_expenseType == 1 && _firstSpanStartTime == _secondSpanEndTime && _firstSpanEndTime == _secondSpanStartTime && Math.Abs(_firstSpanEndTime.Hour - _firstSpanStartTime.Hour) + (24 - Math.Abs(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour)) == 24)
            {
                while (lastDate <= _endDate)
                {
                    double firstPrice = 0;
                    double secondPrice = 0;
                    if (_visibleType != 3)
                    {
                        var date = firstDate;
                        TimeSpan lastSpan;
                        if(_firstSpanEndTime.Hour > _firstSpanStartTime.Hour)
                        {
                            if (date.Hour < _firstSpanEndTime.Hour && date.Hour >= _firstSpanStartTime.Hour)
                            {
                                lastSpan = (_firstSpanEndTime - _firstSpanStartTime) - TimeSpan.FromHours(Math.Abs(_firstSpanEndTime.Hour - date.Hour));
                                firstPrice += (double)CalculateForSecondExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_firstSpanEndTime.Hour - date.Hour))),0);
                                secondPrice += (double)CalculateForSecondExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(24 - Math.Abs(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour))),1);
                                while (date < lastDate - lastSpan)
                                {
                                    firstPrice += (double)CalculateForSecondExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_firstSpanEndTime.Hour - _firstSpanStartTime.Hour))), 0);
                                    secondPrice += (double)CalculateForSecondExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(24 - Math.Abs(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour))), 1);
                                }
                                firstPrice += (double)CalculateForSecondExpenseType(await GetPollsForExpense(date, date += lastSpan), 0);
                            }
                            else
                            {
                                if (date.Hour >= _secondSpanStartTime.Hour)
                                {
                                    lastSpan = TimeSpan.FromHours((_secondSpanEndTime.Hour - _secondSpanStartTime.Hour) - (24 - date.Hour) + _secondSpanEndTime.Hour);
                                    secondPrice += (double)CalculateForSecondExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(24 + _secondSpanEndTime.Hour - date.Hour))), 1);
                                }
                                else
                                {
                                    lastSpan = TimeSpan.FromHours((_secondSpanEndTime.Hour - _secondSpanStartTime.Hour) - (_secondSpanEndTime.Hour - date.Hour));
                                    secondPrice += (double)CalculateForSecondExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_secondSpanEndTime.Hour - date.Hour))), 1);
                                }
                                while (date < lastDate - lastSpan)
                                {
                                    firstPrice += (double)CalculateForSecondExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_firstSpanEndTime.Hour - _firstSpanStartTime.Hour))), 0);
                                    secondPrice += (double)CalculateForSecondExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(24 - Math.Abs(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour))), 1);
                                }
                                secondPrice += (double)CalculateForSecondExpenseType(await GetPollsForExpense(date, date += lastSpan), 1);
                            }
                        }
                        else if(_firstSpanStartTime.Hour > _firstSpanEndTime.Hour)
                        {
                            if (date.Hour < _firstSpanEndTime.Hour)
                            {
                                lastSpan = TimeSpan.FromHours((24 -(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour)) - (_firstSpanEndTime.Hour - date.Hour));
                                firstPrice += (double)CalculateForSecondExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(_firstSpanEndTime.Hour - date.Hour)),0);
                                secondPrice += (double)CalculateForSecondExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour))),1);
                                while (date < lastDate - lastSpan)
                                {
                                    firstPrice += (double)CalculateForSecondExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(24 - Math.Abs(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour))), 0);
                                    secondPrice += (double)CalculateForSecondExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour))), 1);
                                }
                                firstPrice += (double)CalculateForSecondExpenseType(await GetPollsForExpense(date, date += lastSpan), 0);
                            }
                            else if (date.Hour >= _firstSpanStartTime.Hour)
                            {
                                lastSpan = TimeSpan.FromHours(date.Hour - _firstSpanStartTime.Hour);
                                firstPrice += (double)CalculateForSecondExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours((24 - (_secondSpanEndTime.Hour - _secondSpanStartTime.Hour)) - lastSpan.Hours)), 0);
                                secondPrice += (double)CalculateForSecondExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour))), 1);
                                while (date < lastDate - lastSpan)
                                {
                                    firstPrice += (double)CalculateForSecondExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(24 - Math.Abs(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour))), 0);
                                    secondPrice += (double)CalculateForSecondExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour))), 1);
                                }
                                firstPrice += (double)CalculateForSecondExpenseType(await GetPollsForExpense(date, date += lastSpan), 0);
                            }
                            else
                            {
                                lastSpan = TimeSpan.FromHours((_secondSpanEndTime.Hour - _secondSpanStartTime.Hour) - (_secondSpanEndTime.Hour - date.Hour));
                                secondPrice += (double)CalculateForSecondExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_secondSpanEndTime.Hour - date.Hour))),1);
                                while (date < lastDate - lastSpan)
                                {
                                    firstPrice += (double)CalculateForSecondExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(24 - Math.Abs(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour))), 0);
                                    secondPrice += (double)CalculateForSecondExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour))), 1);
                                }
                                secondPrice += (double)CalculateForSecondExpenseType(await GetPollsForExpense(date, date += lastSpan), 1);
                            }
                        }
                    }
                    else
                    {
                        var date = firstDate - TimeSpan.FromHours(3);
                        TimeSpan lastSpan;
                        if (_firstSpanEndTime.Hour > _firstSpanStartTime.Hour)
                        {
                            if(date.Hour + 1 >= _firstSpanStartTime.Hour && date.Hour + 1 < _firstSpanEndTime.Hour)
                            {
                                firstPrice += (double)CalculateForSecondExpenseType(await GetPollsForExpense(date, date + TimeSpan.FromHours(1)), 0);
                            }
                            else
                            {
                                secondPrice += (double)CalculateForSecondExpenseType(await GetPollsForExpense(date, date + TimeSpan.FromHours(1)), 1);
                            }
                        }
                        else if (_firstSpanStartTime.Hour > _firstSpanEndTime.Hour)
                        {
                            if (date.Hour + 1 >= _secondSpanStartTime.Hour && date.Hour + 1 < _secondSpanEndTime.Hour)
                            {
                                secondPrice += (double)CalculateForSecondExpenseType(await GetPollsForExpense(date, date + TimeSpan.FromHours(1)), 1);
                            }
                            else
                            {
                                firstPrice += (double)CalculateForSecondExpenseType(await GetPollsForExpense(date, date + TimeSpan.FromHours(1)), 0);
                            }
                        }
                    }
                    chartCollection.Add(firstPrice + secondPrice);
                    firstDate += _timeSpan;
                    lastDate += _timeSpan;
                }
            }
            else if (_expenseType == 2 && _firstSpanStartTime == _thirdSpanEndTime && _firstSpanEndTime == _secondSpanStartTime && _secondSpanEndTime == _thirdSpanStartTime && _firstSpanStartTime - _secondSpanStartTime != TimeSpan.FromHours(0))
            {
                while (lastDate <= _endDate)
                {
                    double firstPrice = 0;
                    double secondPrice = 0;
                    double thirdPrice = 0;
                    if (_visibleType != 3)
                    {
                        var date = firstDate;
                        TimeSpan lastSpan;
                        if (_firstSpanStartTime < _firstSpanEndTime | _firstSpanStartTime == _firstSpanEndTime)
                        {
                            if(_secondSpanStartTime < _secondSpanEndTime)
                            {
                                if(date.Hour >= _firstSpanStartTime.Hour && date.Hour < _firstSpanEndTime.Hour)
                                {
                                    lastSpan = (_firstSpanEndTime - _firstSpanStartTime) - TimeSpan.FromHours(Math.Abs(_firstSpanEndTime.Hour - date.Hour));
                                    firstPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_firstSpanEndTime.Hour - date.Hour))), 0);
                                    secondPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour))), 1);
                                    thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(24 - Math.Abs(_thirdSpanEndTime.Hour - _thirdSpanStartTime.Hour))), 2);
                                    while (date < lastDate - lastSpan)
                                    {
                                        firstPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_firstSpanEndTime.Hour - _firstSpanStartTime.Hour))), 0);
                                        secondPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour))), 1);
                                        thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(24 - Math.Abs(_thirdSpanEndTime.Hour - _thirdSpanStartTime.Hour))), 2);

                                    }
                                    firstPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += lastSpan), 0);
                                }
                                else if(date.Hour >= _secondSpanStartTime.Hour && date.Hour < _secondSpanEndTime.Hour)
                                {
                                    lastSpan = (_secondSpanEndTime - _secondSpanStartTime) - TimeSpan.FromHours(Math.Abs(_secondSpanEndTime.Hour - date.Hour));
                                    secondPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_secondSpanEndTime.Hour - date.Hour))), 1);
                                    thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(24 - Math.Abs(_thirdSpanEndTime.Hour - _thirdSpanStartTime.Hour))), 2);
                                    while (date < lastDate - lastSpan)
                                    {
                                        firstPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_firstSpanEndTime.Hour - _firstSpanStartTime.Hour))), 0);
                                        secondPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour))), 1);
                                        thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(24 - Math.Abs(_thirdSpanEndTime.Hour - _thirdSpanStartTime.Hour))), 2);

                                    }
                                    secondPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += lastSpan), 0);
                                }
                                else
                                {
                                    if (date.Hour >= _thirdSpanStartTime.Hour)
                                    {
                                        lastSpan = TimeSpan.FromHours((24 - _thirdSpanStartTime.Hour) - (24 - date.Hour));
                                        thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(24 - date.Hour + _thirdSpanEndTime.Hour)), 2);
                                        while (date < lastDate - lastSpan)
                                        {
                                            firstPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_firstSpanEndTime.Hour - _firstSpanStartTime.Hour))), 0);
                                            secondPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour))), 1);
                                            thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(24 - Math.Abs(_thirdSpanEndTime.Hour - _thirdSpanStartTime.Hour))), 2);

                                        }
                                        thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += lastSpan), 0);
                                    }
                                    else
                                    {
                                        if (_visibleType != 2)
                                        {
                                            lastSpan = TimeSpan.FromHours(((24 - _thirdSpanStartTime.Hour) + _thirdSpanEndTime.Hour) - (_thirdSpanEndTime.Hour - date.Hour));
                                            thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(_thirdSpanEndTime.Hour - date.Hour)), 2);
                                            while (date < lastDate - lastSpan)
                                            {
                                                firstPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_firstSpanEndTime.Hour - _firstSpanStartTime.Hour))), 0);
                                                secondPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour))), 1);
                                                thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(24 - Math.Abs(_thirdSpanEndTime.Hour - _thirdSpanStartTime.Hour))), 2);

                                            }
                                            thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += lastSpan), 0);
                                        }
                                        else
                                        {
                                            lastSpan = TimeSpan.FromHours(((24 - _thirdSpanStartTime.Hour) + _thirdSpanEndTime.Hour) - (_thirdSpanEndTime.Hour - date.Hour));
                                            thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(_thirdSpanEndTime.Hour - date.Hour)), 2);
                                            firstPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_firstSpanEndTime.Hour - _firstSpanStartTime.Hour))), 0);
                                            secondPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour))), 1);
                                            thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += lastSpan), 0);
                                        }   
                                    }
                                }
                            }
                            else if(_secondSpanStartTime > _secondSpanEndTime)
                            {
                                if (_thirdSpanStartTime < _thirdSpanEndTime)
                                {
                                    if (date.Hour >= _firstSpanStartTime.Hour && date.Hour < _firstSpanEndTime.Hour)
                                    {
                                        lastSpan = (_firstSpanEndTime - _firstSpanStartTime) - TimeSpan.FromHours(Math.Abs(_firstSpanEndTime.Hour - date.Hour));
                                        firstPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_firstSpanEndTime.Hour - date.Hour))), 0);
                                        secondPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour))), 1);
                                        thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(24 - Math.Abs(_thirdSpanEndTime.Hour - _thirdSpanStartTime.Hour))), 2);
                                        while (date < lastDate - lastSpan)
                                        {
                                            firstPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_firstSpanEndTime.Hour - _firstSpanStartTime.Hour))), 0);
                                            secondPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour))), 1);
                                            thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(24 - Math.Abs(_thirdSpanEndTime.Hour - _thirdSpanStartTime.Hour))), 2);
                                        }
                                        firstPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += lastSpan), 0);
                                    }
                                    else if (date.Hour >= _secondSpanStartTime.Hour || date.Hour < _secondSpanEndTime.Hour)
                                    {
                                        if(date.Hour >= _secondSpanStartTime.Hour)
                                        {
                                            lastSpan = TimeSpan.FromHours(date.Hour - _secondSpanStartTime.Hour);
                                            secondPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(24 - date.Hour + _secondSpanEndTime.Hour)), 1);
                                            thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_thirdSpanEndTime.Hour - _thirdSpanStartTime.Hour))), 2);
                                            while (date < lastDate - lastSpan)
                                            {
                                                firstPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_firstSpanEndTime.Hour - _firstSpanStartTime.Hour))), 0);
                                                secondPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(24 - Math.Abs(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour))), 1);
                                                thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_thirdSpanEndTime.Hour - _thirdSpanStartTime.Hour))), 2);
                                            }
                                            secondPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += lastSpan), 0);
                                        }
                                        else
                                        {
                                            lastSpan = TimeSpan.FromHours((24 - _secondSpanStartTime.Hour + _secondSpanEndTime.Hour) - (24 - _secondSpanStartTime.Hour + date.Hour));
                                            secondPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(_secondSpanEndTime.Hour - date.Hour)), 1);
                                            thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(_thirdSpanEndTime.Hour - _thirdSpanStartTime.Hour)), 2);
                                            while (date < lastDate - lastSpan)
                                            {
                                                firstPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_firstSpanEndTime.Hour - _firstSpanStartTime.Hour))), 0);
                                                secondPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(24 - Math.Abs(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour))), 1);
                                                thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_thirdSpanEndTime.Hour - _thirdSpanStartTime.Hour))), 2);
                                            }
                                            secondPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += lastSpan), 0);
                                        }
                                    }
                                    else
                                    {
                                        lastSpan = TimeSpan.FromHours(date.Hour - _thirdSpanStartTime.Hour);
                                        thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(_thirdSpanEndTime.Hour- date.Hour)), 2);
                                        while (date < lastDate - lastSpan)
                                        {
                                            firstPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_firstSpanEndTime.Hour - _firstSpanStartTime.Hour))), 0);
                                            secondPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(24 - Math.Abs(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour))), 1);
                                            thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_thirdSpanEndTime.Hour - _thirdSpanStartTime.Hour))), 2);

                                        }
                                        thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += lastSpan), 0);
                                    }
                                }
                            }
                        }
                        else if (_firstSpanStartTime > _firstSpanEndTime)
                        {
                            if (date.Hour >= _firstSpanStartTime.Hour || date.Hour < _firstSpanEndTime.Hour)
                            {
                                if (date.Hour >= _firstSpanStartTime.Hour)
                                {
                                    lastSpan = TimeSpan.FromHours(date.Hour - _firstSpanStartTime.Hour);
                                    firstPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(24 - date.Hour + _firstSpanStartTime.Hour)), 0);
                                    secondPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour))), 1);
                                    thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_thirdSpanEndTime.Hour - _thirdSpanStartTime.Hour))), 2);
                                    while (date < lastDate - lastSpan)
                                    {
                                        firstPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(24 - Math.Abs(_firstSpanEndTime.Hour - _firstSpanStartTime.Hour))), 0);
                                        secondPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour))), 1);
                                        thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_thirdSpanEndTime.Hour - _thirdSpanStartTime.Hour))), 2);
                                    }
                                    firstPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += lastSpan), 0);
                                }
                                else
                                {
                                    lastSpan = TimeSpan.FromHours(24 - _firstSpanStartTime.Hour + date.Hour);
                                    firstPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(_firstSpanEndTime.Hour - date.Hour)), 0);
                                    secondPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour))), 1);
                                    thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_thirdSpanEndTime.Hour - _thirdSpanStartTime.Hour))), 2);
                                    while (date < lastDate - lastSpan)
                                    {
                                        firstPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(24 - Math.Abs(_firstSpanEndTime.Hour - _firstSpanStartTime.Hour))), 0);
                                        secondPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour))), 1);
                                        thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_thirdSpanEndTime.Hour - _thirdSpanStartTime.Hour))), 2);
                                    }
                                    firstPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += lastSpan), 0);
                                }
                            }
                            else if (date.Hour >= _secondSpanStartTime.Hour && date.Hour < _secondSpanEndTime.Hour)
                            {
                                lastSpan = TimeSpan.FromHours(date.Hour - _secondSpanStartTime.Hour);
                                secondPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(_secondSpanEndTime.Hour - date.Hour)), 1);
                                thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(_thirdSpanEndTime.Hour - _thirdSpanStartTime.Hour)), 2);
                                while (date < lastDate - lastSpan)
                                {
                                    firstPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(24 - Math.Abs(_firstSpanEndTime.Hour - _firstSpanStartTime.Hour))), 0);
                                    secondPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour))), 1);
                                    thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_thirdSpanEndTime.Hour - _thirdSpanStartTime.Hour))), 2);
                                }
                                secondPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += lastSpan), 0);
                            }
                            else
                            {
                                lastSpan = TimeSpan.FromHours(date.Hour - _thirdSpanStartTime.Hour);
                                thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(_thirdSpanEndTime.Hour - date.Hour)), 2);
                                while (date < lastDate - lastSpan)
                                {
                                    firstPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(24 - Math.Abs(_firstSpanEndTime.Hour - _firstSpanStartTime.Hour))), 0);
                                    secondPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_secondSpanEndTime.Hour - _secondSpanStartTime.Hour))), 1);
                                    thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += TimeSpan.FromHours(Math.Abs(_thirdSpanEndTime.Hour - _thirdSpanStartTime.Hour))), 2);

                                }
                                thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date += lastSpan), 0);
                            }
                        }
                    }
                    else
                    {
                        var date = firstDate - TimeSpan.FromHours(3);
                        if (_firstSpanEndTime.Hour >= _firstSpanStartTime.Hour)
                        {
                            if(_secondSpanEndTime > _secondSpanStartTime)
                            {
                                if (date.Hour + 1 >= _firstSpanStartTime.Hour && date.Hour + 1 < _firstSpanEndTime.Hour)
                                {
                                    firstPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date + TimeSpan.FromHours(1)), 0);
                                }
                                else if(date.Hour + 1 >= _secondSpanStartTime.Hour && date.Hour + 1 < _secondSpanEndTime.Hour)
                                {
                                    secondPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date + TimeSpan.FromHours(1)), 1);
                                }
                                else
                                {
                                    thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date + TimeSpan.FromHours(1)), 2);
                                }
                            }
                            else
                            {
                                if (date.Hour + 1 >= _firstSpanStartTime.Hour && date.Hour + 1 < _firstSpanEndTime.Hour)
                                {
                                    firstPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date + TimeSpan.FromHours(1)), 0);
                                }
                                else if (date.Hour + 1 >= _secondSpanStartTime.Hour || date.Hour + 1 < _secondSpanEndTime.Hour)
                                {
                                    secondPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date + TimeSpan.FromHours(1)), 1);
                                }
                                else
                                {
                                    thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date + TimeSpan.FromHours(1)), 2);
                                }
                            }
                        }
                        else if (_firstSpanStartTime.Hour > _firstSpanEndTime.Hour)
                        {
                            if (date.Hour + 1 >= _firstSpanStartTime.Hour || date.Hour + 1 < _firstSpanEndTime.Hour)
                            {
                                firstPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date + TimeSpan.FromHours(1)), 0);
                            }
                            else if (date.Hour + 1 >= _secondSpanStartTime.Hour && date.Hour + 1 < _secondSpanEndTime.Hour)
                            {
                                secondPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date + TimeSpan.FromHours(1)), 1);
                            }
                            else
                            {
                                thirdPrice += (double)CalculateForThirdExpenseType(await GetPollsForExpense(date, date + TimeSpan.FromHours(1)), 2);
                            }
                        }
                    }
                    chartCollection.Add(firstPrice + secondPrice + thirdPrice);
                    firstDate += _timeSpan;
                    lastDate += _timeSpan;
                }
            }
            LineChartSeries.Add(new LineSeries { Title = "Затраты на электроэнергию", Values = chartCollection });
        }
        private decimal CalculateForFirstExpenseType(List<double> electricityConsumptions)
        {
            return (decimal)(electricityConsumptions[1] - electricityConsumptions[0]) * _standartPrice;
        }
        private decimal CalculateForSecondExpenseType(List<double> electricityConsumptions, byte type)
        {
            if(type == 0)
                return (decimal)(electricityConsumptions[1] - electricityConsumptions[0]) * _firstSpanPrice;
            else
                return (decimal)(electricityConsumptions[1] - electricityConsumptions[0]) * _secondSpanPrice;
        }
        private decimal CalculateForThirdExpenseType(List<double> electricityConsumptions, byte type)
        {
            if (type == 0)
                return (decimal)(electricityConsumptions[1] - electricityConsumptions[0]) * _firstSpanPrice;
            else if(type == 1)
                return (decimal)(electricityConsumptions[1] - electricityConsumptions[0]) * _secondSpanPrice;
            else
                return (decimal)(electricityConsumptions[1] - electricityConsumptions[0]) * _thirdSpanPrice;
        }
        private async Task<List<double>> GetPollsForExpense(DateTime startDate, DateTime endDate)
        {
            List<double> electricityConsumptions = new List<double>();
            double startElectricityConsumption = 0;
            double endElectricityConsumption = 0;
            for(var i = 0; i < _deviceGroupModels.Count; i++)
                for (var j = 0; j < _deviceGroupModels[i].DeviceModels.Count; j++)
                {
                    try
                    {
                        var poll = await _pollRepository.Get<PollModel>(1, startDate, _deviceGroupModels[i].DeviceModels[j].Serial, _userModel.Token);
                        if (poll.Count == 0) startElectricityConsumption += 0;
                        else startElectricityConsumption += poll.Last().ElectricityConsumption;
                        poll = await _pollRepository.Get<PollModel>(1, endDate, _deviceGroupModels[i].DeviceModels[j].Serial, _userModel.Token);
                        if (poll.Count == 0) startElectricityConsumption += 0;
                        else endElectricityConsumption += poll.Last().ElectricityConsumption;
                    }
                    catch(TokenLifetimeException ex)
                    {
                        UpdateToken();
                        var poll = await _pollRepository.Get<PollModel>(1, startDate, _deviceGroupModels[i].DeviceModels[j].Serial, _userModel.Token);
                        if (poll.Count == 0) startElectricityConsumption += 0;
                        else startElectricityConsumption += poll.Last().ElectricityConsumption;
                        poll = await _pollRepository.Get<PollModel>(1, endDate, _deviceGroupModels[i].DeviceModels[j].Serial, _userModel.Token);
                        if (poll.Count == 0) startElectricityConsumption += 0;
                        else endElectricityConsumption += poll.Last().ElectricityConsumption;
                    }
                }
            electricityConsumptions.Add(startElectricityConsumption);
            electricityConsumptions.Add(endElectricityConsumption);
            return electricityConsumptions;
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

        public int VisibleType
        {
            get => _visibleType;
            set
            {
                SetProperty(ref _visibleType, value);
                ChangeTimeSpan(_visibleType);
                if (value != 4) CalculationWrapper();
            }
        }
        public int ExpenseType
        {
            get => _expenseType;
            set
            {
                SetProperty(ref _expenseType, value);
                ChangeExpenseType(value);
            }
        }
        public decimal StandartPrice
        {
            get => _standartPrice;
            set => SetProperty(ref _standartPrice, value);
        }
        public decimal FirstSpanPrice
        {
            get => _firstSpanPrice;
            set => SetProperty(ref _firstSpanPrice, value);
        }
        public decimal SecondSpanPrice
        {
            get => _secondSpanPrice;
            set => SetProperty(ref _secondSpanPrice, value);
        }
        public decimal ThirdSpanPrice
        {
            get => _thirdSpanPrice;
            set => SetProperty(ref _thirdSpanPrice, value);
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
        public DateTime FirstSpanStartTime
        {
            get => _firstSpanStartTime;
            set => SetProperty(ref _firstSpanStartTime, value);
        }
        public DateTime SecondSpanStartTime
        {
            get => _secondSpanStartTime;
            set => SetProperty(ref _secondSpanStartTime, value);
        }
        public DateTime ThirdSpanStartTime
        {
            get => _thirdSpanStartTime;
            set => SetProperty(ref _thirdSpanStartTime, value);
        }
        public DateTime FirstSpanEndTime
        {
            get => _firstSpanEndTime;
            set => SetProperty(ref _firstSpanEndTime, value);
        }
        public DateTime SecondSpanEndTime
        {
            get => _secondSpanEndTime;
            set => SetProperty(ref _secondSpanEndTime, value);
        }
        public DateTime ThirdSpanEndTime
        {
            get => _thirdSpanEndTime;
            set => SetProperty(ref _thirdSpanEndTime, value);
        }
        public bool EnableDateSpan
        {
            get => _enableDateSpan;
            set => SetProperty(ref _enableDateSpan, value);
        }
        public bool StandartEnable
        {
            get => _standartEnable;
            set => SetProperty(ref _standartEnable, value);
        }
        public bool TwoFactorsEnable
        {
            get => _twoFactorsEnable;
            set => SetProperty(ref _twoFactorsEnable, value);
        }
        public bool ThreeFactorEnable
        {
            get => _threeFactorEnable;
            set => SetProperty(ref _threeFactorEnable, value);
        }
        public SeriesCollection LineChartSeries
        {
            get => _lineChartSeries;
            set => SetProperty(ref _lineChartSeries, value);
        }
        public List<string> Labels
        {
            get => _labels;
            set => SetProperty(ref _labels, value);
        }
        public string LableTitle
        {
            get => _lableTitle;
            set => SetProperty(ref _lableTitle, value);
        }

        #endregion



        #region Fields

        IEventAggregator _eventAggregator;
        InternetConnectionChecker _internetConnectionChecker;
        AuthorizationService _authorizationService;
        IGuardian _guardian;
        PollRepository _pollRepository;
        DeviceRepository _deviceRepository;
        DeviceGroupRepository _deviceGroupRepository;
        UserModel _userModel;
        private int _visibleType;
        private int _expenseType = 0;
        private decimal _standartPrice;
        private decimal _firstSpanPrice;
        private decimal _secondSpanPrice;
        private decimal _thirdSpanPrice;
        private DateTime _startDate = new DateTime(1990, 1, 1);
        private DateTime _endDate = DateTime.Now;
        private DateTime _firstSpanStartTime;
        private DateTime _secondSpanStartTime;
        private DateTime _thirdSpanStartTime;
        private DateTime _firstSpanEndTime;
        private DateTime _secondSpanEndTime;
        private DateTime _thirdSpanEndTime;
        private TimeSpan _firstTimeSpan;
        private TimeSpan _secondTimeSpan;
        private TimeSpan _thirdTimeSpan;
        private TimeSpan _timeSpan = TimeSpan.FromDays(365);
        private bool _enableDateSpan = false;
        private bool _standartEnable = true;
        private bool _twoFactorsEnable = false;
        private bool _threeFactorEnable = false;
        private bool _workFlag = false;
        private SeriesCollection _lineChartSeries;
        private List<string> _labels;
        private List<DeviceGroupModel> _deviceGroupModels;
        private string _timeSpanKey = "All";
        private string _lableTitle = "Years";
        private const string _internetError = "Отсутствует подключение к интернету";

        #endregion
    }
}
