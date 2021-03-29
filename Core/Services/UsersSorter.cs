using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAUEP.Core.Interfaces;
using SAUEP.Core.Models;
using SAUEP.Core.Exceptions;

namespace SAUEP.Core.Services
{
    public sealed class UsersSorter : ISorter
    {
        #region Main Logic

        public ICollection<T> Sort<T>(ICollection<T> collection, string key, bool direction = true)
        {
            switch (key)
            {
                case "Login":
                    if (direction)
                    {
                        ICollection<T> sortedCollection = new List<T>();
                        (sortedCollection as List<UserModel>).AddRange((collection as List<UserModel>).OrderBy(k => k.Login));
                        return sortedCollection;
                    }
                    else
                    {
                        ICollection<T> sortedCollection = new List<T>();
                        (sortedCollection as List<UserModel>).AddRange((collection as List<UserModel>).OrderByDescending(k => k.Login));
                        return sortedCollection;
                    }
                case "Email":
                    if (direction)
                    {
                        ICollection<T> sortedCollection = new List<T>();
                        (sortedCollection as List<UserModel>).AddRange((collection as List<UserModel>).OrderBy(k => k.Email));
                        return sortedCollection;
                    }
                    else
                    {
                        ICollection<T> sortedCollection = new List<T>();
                        (sortedCollection as List<UserModel>).AddRange((collection as List<UserModel>).OrderByDescending(k => k.Email));
                        return sortedCollection;
                    }
                case "Role":
                    if (direction)
                    {
                        ICollection<T> sortedCollection = new List<T>();
                        (sortedCollection as List<UserModel>).AddRange((collection as List<UserModel>).OrderBy(k => k.Role));
                        return sortedCollection;
                    }
                    else
                    {
                        ICollection<T> sortedCollection = new List<T>();
                        (sortedCollection as List<UserModel>).AddRange((collection as List<UserModel>).OrderByDescending(k => k.Role));
                        return sortedCollection;
                    }
                default: throw new BadKeyException("Такого ключа сортировки не существует: " + key);
            }

        }

        #endregion
    }
}
