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
    public sealed class DeviceGroupModelsSorter : ISorter
    {
        #region Main Logic

        public ICollection<T> Sort<T>(ICollection<T> collection, string key, bool direction = true)
        {
            switch (key)
            {
                case "Id":
                    if (direction)
                    {
                        ICollection<T> sortedCollection = new List<T>();
                        (sortedCollection as List<DeviceGroupModel>).AddRange((collection as List<DeviceGroupModel>).OrderBy(k => k.Id));
                        return sortedCollection;
                    }
                    else
                    {
                        ICollection<T> sortedCollection = new List<T>();
                        (sortedCollection as List<DeviceGroupModel>).AddRange((collection as List<DeviceGroupModel>).OrderByDescending(k => k.Id));
                        return sortedCollection;
                    }
                case "Title":
                    if (direction)
                    {
                        ICollection<T> sortedCollection = new List<T>();
                        (sortedCollection as List<DeviceGroupModel>).AddRange((collection as List<DeviceGroupModel>).OrderBy(k => k.Title));
                        return sortedCollection;
                    }
                    else
                    {
                        ICollection<T> sortedCollection = new List<T>();
                        (sortedCollection as List<DeviceGroupModel>).AddRange((collection as List<DeviceGroupModel>).OrderByDescending(k => k.Title));
                        return sortedCollection;
                    }
                default: throw new BadKeyException("Такого ключа сортировки не существует: " + key);
            }

        }

        #endregion
    }
}
