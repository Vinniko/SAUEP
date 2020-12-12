using System.Net;
using System.IO;
using System.Threading.Tasks;
using SAUEP.Core.Models;
using SAUEP.Core.Interfaces;
using SAUEP.Core.Exceptions;
using SAUEP.Core.Connections;
using System;

namespace SAUEP.Core.Services
{
    public sealed class AuthorizationService : IAuthorization
    {
        #region Constructors

        public AuthorizationService(IConnection connection, IParser jsonParser)
        {
            _connection = connection;
            _jsonParser = jsonParser;
        }

        #endregion



        #region Main Logic

        public async Task<AuthorizationResponseModel> Authorize(string login, string password)
        {
            try
            {
                WebRequest request = WebRequest.Create((_connection as ServerConnection).ConnectionUrl + "auth");
                request.Method = "POST";
                string data = $"username={login}&password={password}";
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(data);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;
                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
                string token = string.Empty;
                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            return _jsonParser.Pars<AuthorizationResponseModel>(reader.ReadToEnd());
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                if((int)httpResponse.StatusCode == 400)
                    throw new AuthorizationException("Неправильный логин или пароль");
                else throw new AuthorizationException("Неизвестная ошибка авторизации");
            }
        }

        #endregion



        #region Fields

        private IConnection _connection;
        private IParser _jsonParser;

        #endregion
    }
}
