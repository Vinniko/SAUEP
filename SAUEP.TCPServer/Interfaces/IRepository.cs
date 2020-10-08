using System.Collections.Generic;

namespace SAUEP.TCPServer.Interfaces
{
    public interface IRepository
    {
        void Set<T>(T data);
        ICollection<T> GetAll<T>();
        T GetById<T>(int id);
        void Update<T>(int id);
        void Remove<T>(int id);
    }
}
