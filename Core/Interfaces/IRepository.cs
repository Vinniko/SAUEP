using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    interface IRepository
    {
        void Set<T>(T data);
        object GetAll();
        object GetByIndex(int index);
        void Clear();
        void Remove<T>(T obj);
    }
}
