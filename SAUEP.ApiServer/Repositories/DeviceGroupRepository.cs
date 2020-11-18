using System;
using System.Collections.Generic;
using SAUEP.ApiServer.Interfaces;
using SAUEP.ApiServer.Models;
using SAUEP.ApiServer.Connections;
using SAUEP.ApiServer.Exceptions;
using Npgsql;

namespace SAUEP.ApiServer.Repositories
{
    public sealed class DeviceGroupRepository : IRepository
    {
        #region Constructors

        public DeviceGroupRepository(IConnection connection)
        {
            _connection = connection;
        }

        #endregion



        #region Main Logic

        public void Set<T>(T data)
        {
            string query = String.Format("INSERT INTO DeviceGroup(title) VALUES('{0}') ON CONFLICT DO NOTHING;",
                (data as DeviceGroupModel).Title);
            using (var command = new NpgsqlCommand(query, (_connection as DataBaseConnection).Connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public ICollection<T> Get<T>()
        {
            ICollection<T> deviceGroups = new List<T>();
            string allFromDeviceGroupsQuery = "SELECT Id, Title FROM DeviceGroup;";
            using (var command = new NpgsqlCommand(allFromDeviceGroupsQuery, (_connection as DataBaseConnection).Connection))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var deviceGroup = new DeviceGroupModel(reader.GetString(1), reader.GetInt32(0));
                        (deviceGroups as List<DeviceGroupModel>).Add(deviceGroup);
                    }
                    return deviceGroups;
                }
            }
        }

        public IModel GetById(int id)
        {
            string getDeviceGroupQuery = String.Format("SELECT Id, Title FROM DeviceGroup WHERE Id = {0};", id);
            using (var command = new NpgsqlCommand(getDeviceGroupQuery, (_connection as DataBaseConnection).Connection))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var deviceGroup = new DeviceGroupModel(reader.GetString(1),reader.GetInt32(0));
                        return deviceGroup;
                    }
                    else throw new DataBaseNullSelectException(String.Format("В таблице DeviceGroup не существует записи с Id = {0}", id));
                }
            }
        }

        public void Update<T>(int id, T data)
        {
            string query = String.Format("UPDATE DeviceGroup SET Title = '{0}' WHERE Id = {1};",
                (data as DeviceGroupModel).Title, id);
            using (var command = new NpgsqlCommand(query, (_connection as DataBaseConnection).Connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void Remove<T>(int id)
        {
            string query = String.Format("DELETE FROM DeviceGroup WHERE Id = {0};", id);
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
