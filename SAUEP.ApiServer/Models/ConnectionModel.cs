using SAUEP.ApiServer.Interfaces;

namespace SAUEP.ApiServer.Models
{
    public sealed class ConnectionModel : IModel
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
    }
}
