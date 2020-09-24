﻿using Core.Interfaces;
using System;
using System.Collections.Generic;

namespace Core.Services
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