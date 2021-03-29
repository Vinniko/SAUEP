using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using SAUEP.Core.Interfaces;
using SAUEP.Core.Models;
using SAUEP.Core.Exceptions;
using SAUEP.Core.Connections;
using System;

namespace SAUEP.Core.Repositories
{
    public sealed class PollRepository : IRepository
    {
        #region Constructors

        public PollRepository(IConnection connection, IParser jsonParser)
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
                WebRequest request = WebRequest.Create((_connection as ServerConnection).ConnectionUrl + $"api/poll/setPoll?serial={(data as PollModel).Serial}&ip={(data as PollModel).Ip}" +
                    $"&power={(data as PollModel).Power}&electricityConsumption={(data as PollModel).ElectricityConsumption}&date={(data as PollModel).Date.ToString()}");
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = 0;
                request.Headers.Add("Authorization", "Bearer " + token);
                WebResponse response = await request.GetResponseAsync();
            }
            catch (WebException ex)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                if ((int)httpResponse.StatusCode == 401)
                    throw new TokenLifetimeException("Время жизни токена авторизации истекло");
                else throw new SetPollException("Неизвестная ошибка создания отчёта");
            }
        }

        public async Task<ICollection<T>> Get<T>(string token)
        {
            try
            {
                WebRequest request = WebRequest.Create((_connection as ServerConnection).ConnectionUrl + "api/poll/getPolls");
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
                else throw new GetPollsException("Неизвестная ошибка получения списка отчётов, с кодом: " + ((int)httpResponse.StatusCode).ToString());
            }
        }

        public async Task<ICollection<T>> Get<T>(int qty, DateTime dateTime, string serial, string token)
        {
            try
            {
                WebRequest request = WebRequest.Create((_connection as ServerConnection).ConnectionUrl + $"api/poll/getLastPolls?qty={qty}&dateTime={dateTime.ToString("u")}&serial={serial}");
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
                else throw new GetPollsException("Неизвестная ошибка получения списка отчётов, с кодом: " + ((int)httpResponse.StatusCode).ToString());
            }
        }

        public async Task<IModel> GetById(int id, string token)
        {
            try
            {
                WebRequest request = WebRequest.Create((_connection as ServerConnection).ConnectionUrl + $"api/poll/getPoll?id={id}");
                request.Method = "GET";
                request.Headers.Add("Authorization", "Bearer " + token);
                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            return _jsonParser.Pars<PollModel>(reader.ReadToEnd());
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                if ((int)httpResponse.StatusCode == 401)
                    throw new TokenLifetimeException("Время жизни токена авторизации истекло");
                else throw new GetPollException("Неизвестная ошибка получения отчёта, с кодом: " + ((int)httpResponse.StatusCode).ToString());
            }
        }

        public async Task Update<T>(int id, T data, string token)
        {
            try
            {
                WebRequest request = WebRequest.Create((_connection as ServerConnection).ConnectionUrl + $"api/poll/updatePoll?id={id}&serial={(data as PollModel).Serial}&ip={(data as PollModel).Ip}" +
                    $"&power={(data as PollModel).Power}&electricityConsumption={(data as PollModel).ElectricityConsumption}&date={(data as PollModel).Date}");
                request.Method = "PUT";
                request.ContentLength = 0;
                request.Headers.Add("Authorization", "Bearer " + token);
                WebResponse response = await request.GetResponseAsync();
            }
            catch (WebException ex)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                if ((int)httpResponse.StatusCode == 401)
                    throw new TokenLifetimeException("Время жизни токена авторизации истекло");
                else throw new UpdatePollException("Неизвестная ошибка обновления отчёта");
            }
        }

        public async Task Remove<T>(int id, string token)
        {
            try
            {
                WebRequest request = WebRequest.Create((_connection as ServerConnection).ConnectionUrl + $"api/poll/deletePoll?id={id}");
                request.Method = "DELETE";
                request.Headers.Add("Authorization", "Bearer " + token);
                WebResponse response = await request.GetResponseAsync();
            }
            catch (WebException ex)
            {
                HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                if ((int)httpResponse.StatusCode == 400)
                    throw new RemovePollException("Такого отчёта не существует");
                else if ((int)httpResponse.StatusCode == 401)
                    throw new TokenLifetimeException("Время жизни токена авторизации истекло");
                else throw new RemovePollException("Неизвестная ошибка удаления отчёта");
            }
        }

        #endregion



        #region Fields

        private IConnection _connection;
        private IParser _jsonParser;

        #endregion
    }
}
