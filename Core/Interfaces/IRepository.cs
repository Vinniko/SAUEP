using System.Threading.Tasks;
using System.Collections.Generic;

namespace SAUEP.Core.Interfaces
{
    public interface IRepository
    {
        Task Set<T>(T data, string token);
        Task<ICollection<T>> Get<T>(string token);
        Task<IModel> GetById(int id, string token);
        Task Update<T>(int id, T data, string token);
        Task Remove<T>(int id, string token);
    }
}
