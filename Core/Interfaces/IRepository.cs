using System.Threading.Tasks;
using System.Collections.Generic;

namespace SAUEP.Core.Interfaces
{
    public interface IRepository
    {
        void Set<T>(T data, string token);
        Task<ICollection<T>> Get<T>(string token);
        Task<IModel> GetById(int id, string token);
        void Update<T>(int id, T data, string token);
        void Remove<T>(int id, string token);
    }
}
