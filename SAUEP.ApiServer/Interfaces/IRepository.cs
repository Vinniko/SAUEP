using System.Collections.Generic;

namespace SAUEP.ApiServer.Interfaces
{
    public interface IRepository
    {
        void Set<T>(T data);
        ICollection<T> Get<T>();
        IModel GetById(int id);
        void Update<T>(int id, T data);
        void Remove<T>(int id);
    }
}
