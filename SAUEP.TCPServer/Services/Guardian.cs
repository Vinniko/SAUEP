using SAUEP.TCPServer.Interfaces;
using System;
using System.Collections.Generic;

namespace SAUEP.TCPServer.Services
{
    public sealed class Guardian
    {
        #region Constructors

        public Guardian(ILogger logger)
        {
            _logger = logger;
        }

        #endregion



        #region Main Logic

        public Result Secure(Action action)
        {
            Result result = null;

            try
            {
                action();

                result = Result.Successful();
            }
            catch (Exception exception)
            {
                string logText = exception.Message;
                _logger.Logg(logText);
                result = Result.Failed(exception);
            }

            return result;
        }

        public Result<T> Secure<T>(Func<T> func)
        {
            Result<T> result = null;

            try
            {
                var value = func();
                result = Result<T>.Successful(value);
            }
            catch (Exception exception)
            {
                string logText = exception.Message;
                _logger.Logg(logText);
                result = Result<T>.Failed(exception);
            }

            return result;
        }

        #endregion



        #region Fields

        private ILogger _logger;

        #endregion
    }
}
