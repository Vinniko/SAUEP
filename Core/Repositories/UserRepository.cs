using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using SAUEP.Core.Interfaces;
using SAUEP.Core.Models;
using SAUEP.Core.Exceptions;

namespace SAUEP.Core.Repositories
{
    public sealed class UserRepository : IRepository
    {
        #region Constructors

        public UserRepository(IConnection connection, IParser jsonParser)
        {
            _connection = connection;
            _jsonParser = jsonParser;
        }

        #endregion



        #region Main Logic

        public async void Set<T>(T data, string token)
        {
            try
            {
                WebRequest request = WebRequest.Create(_connection + "reg");
                request.Method = "POST";
                string requestData = $"login={(data as UserModel).Login}&password={(data as UserModel).Password}&email={(data as UserModel).Email}";
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(requestData);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                request.Headers.Add("Authorization", "Bearer " + token);
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(byteArray, 0, byteArray.Length);
                }
                WebResponse response = await request.GetResponseAsync();
            }
            catch (WebException ex)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                if ((int)httpResponse.StatusCode == 400)
                    throw new RegistrationException("Такой логин уже существует");
                else if ((int)httpResponse.StatusCode == 401)
                    throw new TokenLifetimeException("Время жизни токена авторизации истекло");
                else throw new RegistrationException("Неизвестная ошибка регистрации");
            }
        }

        public async Task<ICollection<T>> Get<T>(string token)
        {
            try
            {
                WebRequest request = WebRequest.Create(_connection + "api/user/getUsers");
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
                throw new Exception(((int)httpResponse.StatusCode).ToString()); 
            }
        }

        public async Task<IModel> GetById(int id, string token)
        {
            try
            {
                WebRequest request = WebRequest.Create(_connection + $"api/user/getUser?id={id}");
                request.Method = "GET";
                request.Headers.Add("Authorization", "Bearer " + token);
                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            return _jsonParser.Pars<UserModel>(reader.ReadToEnd());
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                throw new Exception(((int)httpResponse.StatusCode).ToString());
            }
        }

        public async void Update<T>(int id, T data, string token)
        {
            try
            {
                WebRequest request = WebRequest.Create(_connection + $"api/user/updateUser?id={id}&login={(data as UserModel).Login}" +
                    $"&password={(data as UserModel).Password}&email={(data as UserModel).Email}");
                request.Method = "PUT";
                request.Headers.Add("Authorization", "Bearer " + token);
                WebResponse response = await request.GetResponseAsync();
            }
            catch (WebException ex)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                if ((int)httpResponse.StatusCode == 400)
                    throw new RegistrationException("Такой логин уже существует");
                else if ((int)httpResponse.StatusCode == 401)
                    throw new TokenLifetimeException("Время жизни токена авторизации истекло");
                else throw new RegistrationException("Неизвестная ошибка обновления пользователя");
            }
        }

        public async void Remove<T>(int id, string token)
        {
            try
            {
                WebRequest request = WebRequest.Create(_connection + $"api/user/deleteUser?id={id}");
                request.Method = "DELETE";
                request.Headers.Add("Authorization", "Bearer " + token);
                WebResponse response = await request.GetResponseAsync();
            }
            catch (WebException ex)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                if ((int)httpResponse.StatusCode == 400)
                    throw new RegistrationException("Такого пользователя не существует");
                else if ((int)httpResponse.StatusCode == 401)
                    throw new TokenLifetimeException("Время жизни токена авторизации истекло");
                else throw new RegistrationException("Неизвестная ошибка удаление пользователя");
            }
        }

        #endregion



        #region Fields

        private IConnection _connection;
        private IParser _jsonParser;

        #endregion
    }
}
