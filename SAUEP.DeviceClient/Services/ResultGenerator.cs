using System;
using System.Collections.Generic;
using SAUEP.DeviceClient.Interfaces;
using SAUEP.DeviceClient.Models;
using SAUEP.DeviceClient.Repositories;

namespace SAUEP.DeviceClient.Services
{
    public sealed class ResultGenerator : IObservable
    {
        #region Constructors

        public ResultGenerator(PollRepository pollRepository)
        {
            _observers = new List<IObserver>();
            _pollRepository = pollRepository;
        }

        #endregion



        #region Main Logic

        public void Generate(object device)
        {
            Random random = new Random();
            double electricityConsumption = 0;
            var poll = _pollRepository.Get<PollModel>(1, DateTime.Now, (device as DeviceModel).Serial) as List<PollModel>;
            if (poll.Count > 0)
                electricityConsumption = poll[0].ElectricityConsumption;
            else
                electricityConsumption = 0;
            NotifyObservers(new PollModel(0, (device as DeviceModel).Serial, (device as DeviceModel).Ip, random.Next((int)(device as DeviceModel).MinPower - 2, (int)(device as DeviceModel).MaxPower + 2),
                electricityConsumption + random.Next(300), DateTime.Now));
        }
        public void AddObserver(IObserver observer)
        {
            _observers.Add(observer);
        }
        public void RemoveObserver(IObserver observer)
        {
            _observers.Remove(observer);
        }
        public void NotifyObservers()
        {
            
        }
        public void NotifyObservers(PollModel pollModel)
        {
            foreach (var observer in _observers)
                observer.Update(pollModel);
        }

        #endregion



        #region Fields

        private ICollection<IObserver> _observers;
        private PollRepository _pollRepository;

        #endregion
    }
}
