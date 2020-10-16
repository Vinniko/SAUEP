using SAUEP.TCPServer.Models;

namespace SAUEP.TCPServer.Interfaces
{
    public interface IObserver
    {
        void Update(PollModel pollModel);
    }
}
