namespace SAUEP.TCPServer.Interfaces
{
    public interface IWriter
    {
        void Write<T>(T data);
    }
}
