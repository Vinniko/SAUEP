using SAUEP.Core.Services;
using System;

namespace SAUEP.Core.Interfaces
{
    public interface IGuardian
    {
        Result Secure(Action action);
        Result<T> Secure<T>(Func<T> func);
    }
}
