using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAUEP.Core.Interfaces;
using SAUEP.WPF.Models;
using SAUEP.Core.Exceptions;

namespace SAUEP.WPF.Services
{
    public sealed class DeviceGroupModelViewingSorter : ISorter
    {
        #region Main Logic

        public ICollection<T> Sort<T>(ICollection<T> collection, string key, bool direction = true)
        {
            switch (key)
            {
                case "Title":
                    if (direction)
                    {
                        ICollection<T> sortedCollection = new List<T>();
                        (sortedCollection as List<DeviceGroupModelViewing>).AddRange((collection as List<DeviceGroupModelViewing>).OrderBy(k => k.Title));
                        return sortedCollection;
                    }
                    else
                    {
                        ICollection<T> sortedCollection = new List<T>();
                        (sortedCollection as List<DeviceGroupModelViewing>).AddRange((collection as List<DeviceGroupModelViewing>).OrderByDescending(k => k.Title));
                        return sortedCollection;
                    }
                case "Power":
                    if (direction)
                    {
                        ICollection<T> sortedCollection = new List<T>();
                        (sortedCollection as List<DeviceGroupModelViewing>).AddRange((collection as List<DeviceGroupModelViewing>).OrderBy(k => k.Power));
                        return sortedCollection;
                    }
                    else
                    {
                        ICollection<T> sortedCollection = new List<T>();
                        (sortedCollection as List<DeviceGroupModelViewing>).AddRange((collection as List<DeviceGroupModelViewing>).OrderByDescending(k => k.Power));
                        return sortedCollection;
                    }
                case "PowerPercent":
                    if (direction)
                    {
                        ICollection<T> sortedCollection = new List<T>();
                        (sortedCollection as List<DeviceGroupModelViewing>).AddRange((collection as List<DeviceGroupModelViewing>).OrderBy(k => k.PowerPercent));
                        return sortedCollection;
                    }
                    else
                    {
                        ICollection<T> sortedCollection = new List<T>();
                        (sortedCollection as List<DeviceGroupModelViewing>).AddRange((collection as List<DeviceGroupModelViewing>).OrderByDescending(k => k.PowerPercent));
                        return sortedCollection;
                    }
                case "ElectricityConsumption":
                    if (direction)
                    {
                        ICollection<T> sortedCollection = new List<T>();
                        (sortedCollection as List<DeviceGroupModelViewing>).AddRange((collection as List<DeviceGroupModelViewing>).OrderBy(k => k.ElectricityConsumption));
                        return sortedCollection;
                    }
                    else
                    {
                        ICollection<T> sortedCollection = new List<T>();
                        (sortedCollection as List<DeviceGroupModelViewing>).AddRange((collection as List<DeviceGroupModelViewing>).OrderByDescending(k => k.ElectricityConsumption));
                        return sortedCollection;
                    }
                default: throw new BadKeyException("Такого ключа сортировки не существует: " + key);
            }

        }

        #endregion
    }
}
