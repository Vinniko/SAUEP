namespace SAUEP.DeviceClient.Interfaces
{
    public interface IWriter
    {
        void Write<T>(T data);
    }
}
