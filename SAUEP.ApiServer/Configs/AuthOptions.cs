using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace SAUEP.ApiServer.Configs
{
    public sealed class AuthOptions
    {
        public const string ISSUER = "SauepApiServer"; 
        public const string AUDIENCE = "SauepClient"; 
        const string KEY = "RyeUSwd97uPRghrd";   
        public const int LIFETIME = 30; 
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
