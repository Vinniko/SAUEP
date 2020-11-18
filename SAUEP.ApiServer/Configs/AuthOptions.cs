using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace SAUEP.ApiServer.Configs
{
    public sealed class AuthOptions
    {
        public const string ISSUER = "SauepApiServer"; // издатель токена
        public const string AUDIENCE = "SauepClient"; // потребитель токена
        const string KEY = "RyeUSwd97uPRghrd";   // ключ для шифрации
        public const int LIFETIME = 1; // время жизни токена - 1 минута
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
