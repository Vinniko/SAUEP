using System.Text.Json;
using SAUEP.DeviceClient.Interfaces;


namespace SAUEP.DeviceClient.Services
{
    public sealed class JsonParser : IParser
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
