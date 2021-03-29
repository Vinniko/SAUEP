namespace SAUEP.TCPServer.Interfaces
{
    public interface ISender
    {
        void Send<T>(T data);
    }
}
