using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAUEP.TCPServer.Interfaces
{
    interface IObservable
    {
        void AddObserver(IObserver observer);
        void NotifyObservers();
        void RemoveObserver(IObserver observer);
    }
}
