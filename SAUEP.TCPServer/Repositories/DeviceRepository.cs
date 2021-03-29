using System;
using System.Collections.Generic;
using SAUEP.TCPServer.Interfaces;
using SAUEP.TCPServer.Models;
using SAUEP.TCPServer.Connections;
using SAUEP.TCPServer.Exceptions;
using Npgsql;

namespace SAUEP.TCPServer.Repositories
{
    public sealed class DeviceRepository : IRepository
    {
        #region Constructors

        public DeviceRepository(IConnection connection)
        {
            _connection = connection;
        }

        #endregion



        #region Main Logic

        public void Set<T>(T data)
        {
            string query = String.Format("INSERT INTO Devices(devicegroupid, serial, title, ip, port, status, maxpower, minpower, maxelecticityconsumption, minelecticityconsumption) " +
                "VALUES((SELECT Id FROM devicegroup WHERE title = '{0}'), '{1}', '{2}', '{3}', '{4}', {5}, {6}, {7}) ON CONFLICT DO NOTHING;",
                (data as DeviceModel).DeviceGroup, (data as DeviceModel).Serial, (data as DeviceModel).Title, (data as DeviceModel).Ip, (data as DeviceModel).Port, (data as DeviceModel).Status,
                (data as DeviceModel).MaxPower, (data as DeviceModel).MinPower);
            using (var command = new NpgsqlCommand(query, (_connection as DataBaseConnection).Connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public ICollection<T> Get<T>()
        {
            ICollection<T> devices = new List<T>();
            string allFromDevicesQuery = "SELECT Devices.Id, DeviceGroup.Title, Devices.Serial, Devices.Title, Devices.Ip, Devices.Port, Devices.Status, Devices.MaxPower, Devices.MinPower " +
                "FROM Devices INNER JOIN DeviceGroup On Devices.devicegroupid = devicegroup.Id;";
            using (var command = new NpgsqlCommand(allFromDevicesQuery, (_connection as DataBaseConnection).Connection))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var device = new DeviceModel(reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), 
                            reader.GetBoolean(6), reader.GetDouble(7), reader.GetDouble(8), reader.GetInt32(0));
                        (devices as List<DeviceModel>).Add(device);
                    }
                    return devices;
                }
            }
        }

        public IModel GetById(int id)
        {
            string getDeviceQuery = String.Format("SELECT Devices.Id, DeviceGroup.Title, Devices.Serial, Devices.Title, Devices.Ip, Devices.Port, Devices.Status, Devices.MaxPower, " +
                "Devices.MinPower FROM Devices INNER JOIN DeviceGroup On Devices.devicegroupid = devicegroup.Id " +
                "WHERE Devices.Id = {0};", id);
            using (var command = new NpgsqlCommand(getDeviceQuery, (_connection as DataBaseConnection).Connection))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var device = new DeviceModel(reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5),
                            reader.GetBoolean(6), reader.GetDouble(7), reader.GetDouble(8), reader.GetInt32(0));
                        return device;
                    }
                    else throw new DataBaseNullSelectException(String.Format("В таблице Devices не существует записи с Id = {0}", id));
                }
            }
        }

        public void Update<T>(int id, T data)
        {
            string query = String.Format("UPDATE Devices SET devicegroupid = (SELECT Id FROM devicegroup WHERE Title = '{0}'), serial = '{1}', title = '{2}', ip = '{3}', port = '{4}', " +
                "status = {5}, maxpower = {6}, minpower = {7} WHERE Id = {10}",
                (data as DeviceModel).DeviceGroup, (data as DeviceModel).Serial, (data as DeviceModel).Title, (data as DeviceModel).Ip, (data as DeviceModel).Port, (data as DeviceModel).Status,
                (data as DeviceModel).MaxPower, (data as DeviceModel).MinPower, id);
            using (var command = new NpgsqlCommand(query, (_connection as DataBaseConnection).Connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void Remove<T>(int id)
        {
            string query = String.Format("DELETE FROM Devices WHERE Id = {0};", id);
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
