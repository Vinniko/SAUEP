using SAUEP.Core.Interfaces;
using System.Net.NetworkInformation;

namespace SAUEP.Core.Services
{
    public sealed class InternetConnectionChecker : IChecker
    {
        #region Constructors

        public InternetConnectionChecker()
        {
            _ping = new Ping();
            _buffer = new byte[32];
            _pingOptions = new PingOptions();
        }

        #endregion



        #region Main Logic

        public bool Check()
        {
            try
            {
                var reply = _ping.Send(_host, _timeout, _buffer, _pingOptions);
                return reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                return false;
            }
        }

        #endregion



        #region Fields

        private Ping _ping;
        private const string _host = "google.com";
        private const int _timeout = 1000;
        private byte[] _buffer;
        private PingOptions _pingOptions;

        #endregion
    }
}
