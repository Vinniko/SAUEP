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
    public sealed class DeviceGroupRepository : IRepository
    {
        #region Constructors

        public DeviceGroupRepository(IConnection connection, IParser jsonParser)
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
                WebRequest request = WebRequest.Create((_connection as ServerConnection).ConnectionUrl + $"api/devicesGroup/setDeviceGroup?title={(data as DeviceGroupModel).Title}");
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
                    throw new SetDeviceGroupException("Такая группа устройств уже существует");
                else if ((int)httpResponse.StatusCode == 401)
                    throw new TokenLifetimeException("Время жизни токена авторизации истекло");
                else throw new SetDeviceGroupException("Неизвестная ошибка создания группы устройств");
            }
        }

        public async Task<ICollection<T>> Get<T>(string token)
        {
            try
            {
                WebRequest request = WebRequest.Create((_connection as ServerConnection).ConnectionUrl + "api/devicesGroup/getDeviceGroups");
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
                else throw new GetDeviceGroupsException("Неизвестная ошибка получения списка групп устройств, с кодом: " + ((int)httpResponse.StatusCode).ToString());
            }
        }

        public async Task<IModel> GetById(int id, string token)
        {
            try
            {
                WebRequest request = WebRequest.Create((_connection as ServerConnection).ConnectionUrl + $"api/devicesGroup/getDeviceGroup?id={id}");
                request.Method = "GET";
                request.Headers.Add("Authorization", "Bearer " + token);
                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            return _jsonParser.Pars<DeviceGroupModel>(reader.ReadToEnd());
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                if ((int)httpResponse.StatusCode == 401)
                    throw new TokenLifetimeException("Время жизни токена авторизации истекло");
                else throw new GetDeviceGroupException("Неизвестная ошибка получения группы устройств, с кодом: " + ((int)httpResponse.StatusCode).ToString());
            }
        }

        public async Task Update<T>(int id, T data, string token)
        {
            try
            {
                WebRequest request = WebRequest.Create((_connection as ServerConnection).ConnectionUrl + $"api/devicesGroup/updateDeviceGroup?id={id}&title={(data as DeviceGroupModel).Title}");
                request.Method = "PUT";
                request.ContentLength = 0;
                request.Headers.Add("Authorization", "Bearer " + token);
                WebResponse response = await request.GetResponseAsync();
            }
            catch (WebException ex)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                if ((int)httpResponse.StatusCode == 400)
                    throw new UpdateDeviceGroupException("Такая группа устройств уже существует");
                else if ((int)httpResponse.StatusCode == 401)
                    throw new TokenLifetimeException("Время жизни токена авторизации истекло");
                else throw new UpdateDeviceGroupException("Неизвестная ошибка обновления группы устройств");
            }
        }

        public async Task Remove<T>(int id, string token)
        {
            try
            {
                WebRequest request = WebRequest.Create((_connection as ServerConnection).ConnectionUrl + $"api/devicesGroup/deleteDeviceGroup?id={id}");
                request.Method = "DELETE";
                request.Headers.Add("Authorization", "Bearer " + token);
                WebResponse response = await request.GetResponseAsync();
            }
            catch (WebException ex)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                if ((int)httpResponse.StatusCode == 400)
                    throw new RemoveDeviceGroupException("Такой группы устройств не существует");
                else if ((int)httpResponse.StatusCode == 401)
                    throw new TokenLifetimeException("Время жизни токена авторизации истекло");
                else throw new RemoveDeviceGroupException("Неизвестная ошибка удаление группы устройств");
            }
        }

        #endregion



        #region Fields

        private IConnection _connection;
        private IParser _jsonParser;

        #endregion
    }
}
