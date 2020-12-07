using SAUEP.Core.Interfaces;

namespace SAUEP.Core.Models
{
    public sealed class AuthorizationResponseModel : IModel
    {
        public string access_token { get; set; }
        public string id { get; set; }
    }
}
