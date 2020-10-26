using SAUEP.DeviceClient.Models;

namespace SAUEP.DeviceClient.Interfaces
{
    public interface IObserver
    {
        void Update(PollModel pollModel);
    }
}
