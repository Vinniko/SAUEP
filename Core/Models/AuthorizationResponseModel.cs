using SAUEP.Core.Interfaces;

namespace SAUEP.Core.Models
{
    public sealed class AuthorizationResponseModel : IModel
    {
        public string acces_token { get; set; }
        public int id { get; set; }
    }
}
