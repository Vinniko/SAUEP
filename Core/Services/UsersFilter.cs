using System.Collections.Generic;
using System.Linq;
using SAUEP.Core.Interfaces;
using SAUEP.Core.Models;
using SAUEP.Core.Exceptions;

namespace SAUEP.Core.Services
{
    public sealed class UsersFilter : IFilter
    {
        #region Main Logic

        public ICollection<T> Filtering<T>(ICollection<T> list)
        {
            return list;
        }

        public ICollection<T> Filtering<T>(ICollection<T> list, string key, string filter)
        {
            ICollection<T> filterCollection = new List<T>();
            switch (key)
            {
                case "Login":
                    (filterCollection as List<UserModel>).AddRange(from model in list as List<UserModel>
                                                                            where model.Login.Contains(filter)
                                                                            select model);
                    return filterCollection;
                case "Email":
                    (filterCollection as List<UserModel>).AddRange(from model in list as List<UserModel>
                                                                            where model.Email.Contains(filter)
                                                                            select model);
                    return filterCollection;
                case "Role":
                    (filterCollection as List<UserModel>).AddRange(from model in list as List<UserModel>
                                                                   where model.Role.Contains(filter)
                                                                   select model);
                    return filterCollection;
                default: throw new BadKeyException("Такого ключа фильтрации не существует: " + key);
            }
        }

        #endregion
    }
}
