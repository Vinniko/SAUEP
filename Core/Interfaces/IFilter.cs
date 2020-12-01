using System.Collections.Generic;

namespace SAUEP.Core.Interfaces
{
    public interface IFilter
    {
        ICollection<T> Filtering<T>(ICollection<T> list);
    }
}
