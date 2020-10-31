using System;
using System.Collections.Generic;
using SAUEP.DeviceClient.Interfaces;
using SAUEP.DeviceClient.Models;

namespace SAUEP.DeviceClient.Services
{
    public sealed class ResultGenerator : IObservable
    {
        #region Constructors

        public ResultGenerator()
        {
            _observers = new List<IObserver>();
            
        }

        #endregion




        #region Main Logic

        public void Generate(object device)
        {
            Random random = new Random();
            NotifyObservers(new PollModel(0, (device as DeviceModel).Serial, (device as DeviceModel).Ip, random.Next((int)(device as DeviceModel).MinPower - 2, (int)(device as DeviceModel).MaxPower + 2),
                random.Next((int)(device as DeviceModel).MinElecticityConsumption - 2, (int)(device as DeviceModel).MaxElecticityConsumption + 2), DateTime.Now));
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

        #endregion
    }
}
