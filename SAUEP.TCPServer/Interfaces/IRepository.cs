using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAUEP.TCPServer.Interfaces
{
    interface IRepository
    {
        void Set<T>(T data);
        ICollection<T> GetAll<T>();
        T GetById<T>(int id);
        void Update<T>(int id);
        void Remove<T>(int id);
    }
}
