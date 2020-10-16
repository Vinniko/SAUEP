namespace SAUEP.TCPServer.Interfaces
{
    public interface IObservable
    {
        void AddObserver(IObserver observer);
        void NotifyObservers();
        void RemoveObserver(IObserver observer);
    }
}
