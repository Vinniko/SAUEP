using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SAUEP.DeviceClient.Interfaces;
using SAUEP.DeviceClient.Models;

namespace SAUEP.DeviceClient.Services
{
    public sealed class ResultGenerator : IObservable
    {
        #region Constructors

        public ResultGenerator(IRepository repository)
        {
            _deviceRepository = repository;
            _observers = new List<IObserver>();
            
        }

        #endregion




        #region Main Logic

        public void Generate()
        {
            while (true)
            {
                ICollection<Thread> threads = new List<Thread>();
                foreach(var device in _deviceRepository.Get<DeviceModel>())
                {
                    threads.Add(new Thread(new ParameterizedThreadStart(CreateRandomPoll)));
                    threads.ElementAt(threads.Count - 1).Start(device);
                }
                foreach(var thread in threads)
                {
                    thread.Join();
                }
            }
        }
        private void CreateRandomPoll(object device)
        {
            Random random = new Random();
            NotifyObservers(new PollModel(0, (device as DeviceModel).Serial, (device as DeviceModel).Ip, random.Next((int)(device as DeviceModel).MinPower, (int)(device as DeviceModel).MaxPower),
                random.Next((int)(device as DeviceModel).MinElecticityConsumption, (int)(device as DeviceModel).MaxElecticityConsumption), DateTime.Now));
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

        private IRepository _deviceRepository;
        private ICollection<IObserver> _observers;

        #endregion
    }
}
