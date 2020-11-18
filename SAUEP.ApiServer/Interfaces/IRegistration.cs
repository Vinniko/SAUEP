using SAUEP.ApiServer.Models;

namespace SAUEP.ApiServer.Interfaces
{
    public interface IRegistration
    {
        bool Registration(UserModel userModel);
    }
}
