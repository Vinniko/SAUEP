using SAUEP.Core.Services;
using System;

namespace SAUEP.Core.Interfaces
{
    public interface IGuardian
    {
        Result Secure(Action action, ref Exception ex);
        Result<T> Secure<T>(Func<T> func, ref Exception ex);

    }
}
