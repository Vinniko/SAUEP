using System.Threading.Tasks;
using SAUEP.Core.Models;

namespace SAUEP.Core.Interfaces
{
    public interface IAuthorization
    {
        Task<AuthorizationResponseModel> Authorize(string login, string password);
    }
}
