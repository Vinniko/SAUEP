using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using SAUEP.Core.Interfaces;
using SAUEP.Core.Models;
using SAUEP.Core.Exceptions;
using SAUEP.Core.Connections;

namespace SAUEP.Core.Repositories
{
    public sealed class DeviceRepository : IRepository
    {
        #region Constructors

        public DeviceRepository(IConnection connection, IParser jsonParser)
        {
            _connection = connection;
            _jsonParser = jsonParser;
        }

        #endregion



        #region Main Logic

        public async Task Set<T>(T data, string token)
        {
            try
            {
                WebRequest request = WebRequest.Create((_connection as ServerConnection).ConnectionUrl + $"api/device/setDevice?deviceGroup={(data as DeviceModel).DeviceGroup}" +
                    $"&serial={(data as DeviceModel).Serial}&title={(data as DeviceModel).Title}&ip={(data as DeviceModel).Ip}&port={(data as DeviceModel).Port}&status={(data as DeviceModel).Status}" +
                    $"&maxPower={(data as DeviceModel).MaxPower}&minPower={(data as DeviceModel).MinPower}&maxElecticityConsumption={(data as DeviceModel).MaxElecticityConsumption}" +
                    $"&minElecticityConsumption={(data as DeviceModel).MinElecticityConsumption}");
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = 0;
                request.Headers.Add("Authorization", "Bearer " + token);
                WebResponse response = await request.GetResponseAsync();
            }
            catch (WebException ex)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                if ((int)httpResponse.StatusCode == 400)
                    throw new SetDeviceException("Такое устройство уже существует");
                else if ((int)httpResponse.StatusCode == 401)
                    throw new TokenLifetimeException("Время жизни токена авторизации истекло");
                else throw new SetDeviceException("Неизвестная ошибка создания устройства");
            }
        }

        public async Task<ICollection<T>> Get<T>(string token)
        {
            try
            {
                WebRequest request = WebRequest.Create((_connection as ServerConnection).ConnectionUrl + "api/device/getDevices");
                request.Method = "GET";
                request.Headers.Add("Authorization", "Bearer " + token);
                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            return _jsonParser.Pars<ICollection<T>>(reader.ReadToEnd());
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                if ((int)httpResponse.StatusCode == 401)
                    throw new TokenLifetimeException("Время жизни токена авторизации истекло");
                else throw new GetDevicesException("Неизвестная ошибка получения списка устройств, с кодом: " + ((int)httpResponse.StatusCode).ToString());
            }
        }

        public async Task<IModel> GetById(int id, string token)
        {
            try
            {
                WebRequest request = WebRequest.Create((_connection as ServerConnection).ConnectionUrl + $"api/device/getDevice?id={id}");
                request.Method = "GET";
                request.Headers.Add("Authorization", "Bearer " + token);
                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            return _jsonParser.Pars<DeviceModel>(reader.ReadToEnd());
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                if ((int)httpResponse.StatusCode == 401)
                    throw new TokenLifetimeException("Время жизни токена авторизации истекло");
                else throw new GetDeviceException("Неизвестная ошибка получения устройства, с кодом: " + ((int)httpResponse.StatusCode).ToString());
            }
        }

        public async Task Update<T>(int id, T data, string token)
        {
            try
            {
                WebRequest request = WebRequest.Create((_connection as ServerConnection).ConnectionUrl + $"api/device/updateDevice?id={id}&deviceGroup={(data as DeviceModel).DeviceGroup}" +
                    $"&serial={(data as DeviceModel).Serial}&title={(data as DeviceModel).Title}&ip={(data as DeviceModel).Ip}&port={(data as DeviceModel).Port}&status={(data as DeviceModel).Status}" +
                    $"&maxPower={(data as DeviceModel).MaxPower}&minPower={(data as DeviceModel).MinPower}&maxElecticityConsumption={(data as DeviceModel).MaxElecticityConsumption}" +
                    $"&minElecticityConsumption={(data as DeviceModel).MinElecticityConsumption}");
                request.Method = "PUT";
                request.ContentLength = 0;
                request.Headers.Add("Authorization", "Bearer " + token);
                WebResponse response = await request.GetResponseAsync();
            }
            catch (WebException ex)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                if ((int)httpResponse.StatusCode == 400)
                    throw new UpdateDeviceException("Такое устройство уже существует");
                else if ((int)httpResponse.StatusCode == 401)
                    throw new TokenLifetimeException("Время жизни токена авторизации истекло");
                else throw new UpdateDeviceException("Неизвестная ошибка обновления устройства");
            }
        }

        public async Task Remove<T>(int id, string token)
        {
            try
            {
                WebRequest request = WebRequest.Create((_connection as ServerConnection).ConnectionUrl + $"api/device/deleteDevice?id={id}");
                request.Method = "DELETE";
                request.Headers.Add("Authorization", "Bearer " + token);
                WebResponse response = await request.GetResponseAsync();
            }
            catch (WebException ex)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                if ((int)httpResponse.StatusCode == 400)
                    throw new RemoveDeviceException("Такого устройства не существует");
                else if ((int)httpResponse.StatusCode == 401)
                    throw new TokenLifetimeException("Время жизни токена авторизации истекло");
                else throw new RemoveDeviceException("Неизвестная ошибка удаления устройства");
            }
        }

        #endregion



        #region Fields

        private IConnection _connection;
        private IParser _jsonParser;

        #endregion
    }
}
