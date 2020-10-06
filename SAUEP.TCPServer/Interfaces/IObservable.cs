namespace SAUEP.TCPServer.Interfaces
{
    interface IObservable
    {
        void AddObserver(IObserver observer);
        void NotifyObservers();
        void RemoveObserver(IObserver observer);
    }
}
