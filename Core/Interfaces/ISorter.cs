using System.Collections.Generic;

namespace SAUEP.Core.Interfaces
{
    public interface ISorter
    {
        ICollection<T> Sort<T>(ICollection<T> list, string key, bool direction = true);
    }
}
