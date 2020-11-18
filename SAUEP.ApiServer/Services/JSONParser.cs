﻿using System.Text.Json;
using SAUEP.ApiServer.Interfaces;


namespace SAUEP.ApiServer.Services
{
    public sealed class JSONParser : IParser
    {
        #region Main Logic

        public T Pars<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }
        public string Depars<T>(T data)
        {
            return JsonSerializer.Serialize<T>(data);
        }

        #endregion
    }
}
