namespace SAUEP.TCPServer.Interfaces
{
    interface IWriter
    {
        void Write<T>(T data);
    }
}
