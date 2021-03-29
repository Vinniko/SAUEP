using System;
using System.Collections.Generic;
using SAUEP.ApiServer.Interfaces;
using SAUEP.ApiServer.Models;
using SAUEP.ApiServer.Connections;
using SAUEP.ApiServer.Exceptions;
using Npgsql;

namespace SAUEP.ApiServer.Repositories
{
    public class PollRepository : IRepository
    {
        #region Constructors

        public PollRepository(IConnection connection)
        {
            _connection = connection;
        }

        #endregion



        #region Main Logic

        public void Set<T>(T data)
        {
            string query = String.Format("INSERT INTO Polls(deviceid, power, electricityConsumption, date) VALUES((SELECT Id FROM devices WHERE Serial = '{0}'), " +
                "{1}, {2}, '{3}') ON CONFLICT DO NOTHING;",
                (data as PollModel).Serial, (data as PollModel).Power, (data as PollModel).ElectricityConsumption,
                (data as PollModel).Date);
            using (var command = new NpgsqlCommand(query, (_connection as DataBaseConnection).Connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public ICollection<T> Get<T>()
        {
            ICollection<T> polls = new List<T>();
            string allFromPollsQuery = "SELECT Polls.Id, Devices.Serial, Devices.Ip, Polls.Power, Polls.ElectricityConsumption, Polls.Date FROM Polls " +
                "INNER JOIN Devices On Devices.Id = Polls.DeviceId;";
            using (var command = new NpgsqlCommand(allFromPollsQuery, (_connection as DataBaseConnection).Connection))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var poll = new PollModel(reader.GetString(1), reader.GetString(2),
                            reader.GetDouble(3), reader.GetDouble(4), reader.GetDateTime(5), reader.GetInt32(0));
                        (polls as List<PollModel>).Add(poll);
                    }
                    return polls;
                }
            }
        }

        public ICollection<T> Get<T>(int qty, DateTime dateTime, string serial)
        {
            ICollection<T> polls = new List<T>();
            string allFromPollsQuery = $"SELECT Polls.Id, Devices.Serial, Devices.Ip, Polls.Power, Polls.ElectricityConsumption, Polls.Date FROM Polls " +
                $"INNER JOIN Devices On Devices.Id = Polls.DeviceId WHERE date < '{dateTime.ToString("u")}' and serial = '{serial}' ORDER BY id DESC LIMIT {qty};";
            using (var command = new NpgsqlCommand(allFromPollsQuery, (_connection as DataBaseConnection).Connection))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var poll = new PollModel(reader.GetString(1), reader.GetString(2),
                            reader.GetDouble(3), reader.GetDouble(4), reader.GetDateTime(5), reader.GetInt32(0));
                        (polls as List<PollModel>).Add(poll);
                    }
                    return polls;
                }
            }
        }

        public IModel GetById(int id)
        {
            string getPollQuery = String.Format("SELECT Polls.Id, Devices.Serial, Devices.Ip, Polls.Power, Polls.ElectricityConsumption, Polls.Date FROM Polls " +
                "INNER JOIN Devices On Devices.Id = Polls.DeviceId WHERE Polls.Id = {0};", id);
            using (var command = new NpgsqlCommand(getPollQuery, (_connection as DataBaseConnection).Connection))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var poll = new PollModel(reader.GetString(1), reader.GetString(2),
                            reader.GetDouble(3), reader.GetDouble(4), reader.GetDateTime(5), reader.GetInt32(0));
                        return poll;
                    }
                    else throw new DataBaseNullSelectException(String.Format("В таблице Polls не существует записи с Id = {0}", id));
                }
            }
        }

        public void Update<T>(int id, T data)
        {
            string query = String.Format("UPDATE Polls SET deviceid = (SELECT Id FROM devices WHERE Serial = '{0}'), power = {1}, " +
                "electricityConsumption = {2}, date = '{3}' WHERE id = {4};",
                (data as PollModel).Serial, (data as PollModel).Power, (data as PollModel).ElectricityConsumption,
                (data as PollModel).Date, id);
            using (var command = new NpgsqlCommand(query, (_connection as DataBaseConnection).Connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void Remove<T>(int id)
        {
            string query = String.Format("DELETE FROM Polls WHERE Id = {0};", id);
            using (var command = new NpgsqlCommand(query, (_connection as DataBaseConnection).Connection))
            {
                command.ExecuteNonQuery();
            }
        }

        #endregion



        #region Fields

        private IConnection _connection;

        #endregion
    }
}
