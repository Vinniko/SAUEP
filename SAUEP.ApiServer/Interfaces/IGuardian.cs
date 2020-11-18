using SAUEP.ApiServer.Services;
using System;

namespace SAUEP.ApiServer.Interfaces
{
    public interface IGuardian
    {
        Result Secure(Action action);
        Result<T> Secure<T>(Func<T> func);
    }
}
