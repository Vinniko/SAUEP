using System;
using System.Collections.Generic;
using SAUEP.TCPServer.Interfaces;
using SAUEP.TCPServer.Models;
using SAUEP.TCPServer.Connections;
using SAUEP.TCPServer.Exceptions;
using Npgsql;

namespace SAUEP.TCPServer.Repositories
{
    public sealed class UserRepository : IRepository
    {
        #region Constructors

        public UserRepository(IConnection connection)
        {
            _connection = connection;
        }

        #endregion



        #region Main Logic

        public void Set<T>(T data)
        {
            string query = String.Format("INSERT INTO Users(login, password, email, roleid) VALUES('{0}', '{1}', '{2}', (SELECT Id From Roles WHERE Title = '{3}')) ON CONFLICT DO NOTHING;",
                (data as UserModel).Login, (data as UserModel).Password, (data as UserModel).Email, (data as UserModel).Role);
            using (var command = new NpgsqlCommand(query, (_connection as DataBaseConnection).Connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public ICollection<T> Get<T>()
        {
            ICollection<T> users = new List<T>();
            string allFromUsersQuery = "SELECT Users.Id, Users.Login, Users.Password, Users.Email, Roles.Title FROM Users INNER JOIN Roles On Users.RoleId = Roles.Id;";
            using (var command = new NpgsqlCommand(allFromUsersQuery, (_connection as DataBaseConnection).Connection))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var user = new UserModel(reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt32(0), reader.GetString(4));
                        (users as List<UserModel>).Add(user);
                    }
                    return users;
                }
            }
        }

        public IModel GetById(int id)
        {
            string getUserQuery = String.Format("SELECT Users.Id, Users.Login, Users.Password, Users.Email, Roles.Title FROM Users INNER JOIN Roles On Users.RoleId = Roles.Id WHERE Users.Id = {0};", id);
            using (var command = new NpgsqlCommand(getUserQuery, (_connection as DataBaseConnection).Connection))
            {
                using (NpgsqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var user = new UserModel(reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetInt32(0), reader.GetString(4));
                        return user;
                    }
                    else throw new DataBaseNullSelectException(String.Format("В таблице Users не существует записи с Id = {0}", id));
                }
            }
        }

        public void Update<T>(int id, T data)
        {
            string query = String.Format("UPDATE Users SET Login = '{0}', password = '{1}', email = '{2}', roleid = (SELECT Id FROM Roles WHERE Title = '{3}') WHERE Id = {4};",
                (data as UserModel).Login, (data as UserModel).Password, (data as UserModel).Email, (data as UserModel).Role, id);
            using (var command = new NpgsqlCommand(query, (_connection as DataBaseConnection).Connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void Remove<T>(int id)
        {
            string query = String.Format("DELETE FROM Users WHERE Id = {0};", id);
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
