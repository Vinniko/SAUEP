using System.Security.Claims;

namespace SAUEP.ApiServer.Interfaces
{
    public interface IAuthorization
    {
        public ClaimsIdentity Authorize(string username, string password);
    }
}
