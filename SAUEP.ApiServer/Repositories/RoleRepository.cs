using System;
using System.Collections.Generic;
using SAUEP.ApiServer.Interfaces;
using SAUEP.ApiServer.Models;
using SAUEP.ApiServer.Connections;
using SAUEP.ApiServer.Exceptions;
using Npgsql;

namespace SAUEP.ApiServer.Repositories
{
    public sealed class RoleRepository : IRepository
    {
        #region Constructors

        public RoleRepository(IConnection connection)
        {
            _connection = connection;
        }

        #endregion



        #region Main Logic

        public void Set<T>(T data)
        {
            string query = String.Format("INSERT INTO Roles(title) VALUES('{0}') ON CONFLICT DO NOTHING;",
                (data as RoleModel).Title);
            using (var command = new NpgsqlCommand(query, (_connection as DataBaseConnection).Connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public ICollection<T> Get<T>()
        {
            ICollection<T> roles = new List<T>();
            string allFromRolesQuery = "SELECT Id, Title FROM Roles;";
            using (var command = new NpgsqlCommand(allFromRolesQuery, (_connection as DataBaseConnection).Connection))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                        (roles as List<RoleModel>).Add(new RoleModel(reader.GetString(1), reader.GetInt32(0)));
                    return roles;
                }
            }
        }

        public IModel GetById(int id)
        {
            string getRoleQuery = String.Format("SELECT Id, Title FROM Roles;", id);
            using (var command = new NpgsqlCommand(getRoleQuery, (_connection as DataBaseConnection).Connection))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read()) return new RoleModel(reader.GetString(1), reader.GetInt32(0));
                    else throw new DataBaseNullSelectException(String.Format("В таблице Roles не существует записи с Id = {0}", id));
                }
            }
        }

        public void Update<T>(int id, T data)
        {
            string query = String.Format("UPDATE Roles SET Title = '{0}' WHERE Id = {1};",
                (data as RoleModel).Title, id);
            using (var command = new NpgsqlCommand(query, (_connection as DataBaseConnection).Connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void Remove<T>(int id)
        {
            string query = String.Format("DELETE FROM Roles WHERE Id = {0};", id);
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
