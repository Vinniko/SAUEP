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
    public sealed class DeviceModelViewingSorter : ISorter
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
                        (sortedCollection as List<DeviceModelViewing>).AddRange((collection as List<DeviceModelViewing>).OrderBy(k => k.Title));
                        return sortedCollection;
                    }
                    else
                    {
                        ICollection<T> sortedCollection = new List<T>();
                        (sortedCollection as List<DeviceModelViewing>).AddRange((collection as List<DeviceModelViewing>).OrderByDescending(k => k.Title));
                        return sortedCollection;
                    }
                case "Serial":
                    if (direction)
                    {
                        ICollection<T> sortedCollection = new List<T>();
                        (sortedCollection as List<DeviceModelViewing>).AddRange((collection as List<DeviceModelViewing>).OrderBy(k => k.Serial));
                        return sortedCollection;
                    }
                    else
                    {
                        ICollection<T> sortedCollection = new List<T>();
                        (sortedCollection as List<DeviceModelViewing>).AddRange((collection as List<DeviceModelViewing>).OrderByDescending(k => k.Serial));
                        return sortedCollection;
                    }
                case "Power":
                    if (direction)
                    {
                        ICollection<T> sortedCollection = new List<T>();
                        (sortedCollection as List<DeviceModelViewing>).AddRange((collection as List<DeviceModelViewing>).OrderBy(k => k.Power));
                        return sortedCollection;
                    }
                    else
                    {
                        ICollection<T> sortedCollection = new List<T>();
                        (sortedCollection as List<DeviceModelViewing>).AddRange((collection as List<DeviceModelViewing>).OrderByDescending(k => k.Power));
                        return sortedCollection;
                    }
                case "PowerPercent":
                    if (direction)
                    {
                        ICollection<T> sortedCollection = new List<T>();
                        (sortedCollection as List<DeviceModelViewing>).AddRange((collection as List<DeviceModelViewing>).OrderBy(k => k.PowerPercent));
                        return sortedCollection;
                    }
                    else
                    {
                        ICollection<T> sortedCollection = new List<T>();
                        (sortedCollection as List<DeviceModelViewing>).AddRange((collection as List<DeviceModelViewing>).OrderByDescending(k => k.PowerPercent));
                        return sortedCollection;
                    }
                case "ElectricityConsumption":
                    if (direction)
                    {
                        ICollection<T> sortedCollection = new List<T>();
                        (sortedCollection as List<DeviceModelViewing>).AddRange((collection as List<DeviceModelViewing>).OrderBy(k => k.ElectricityConsumption));
                        return sortedCollection;
                    }
                    else
                    {
                        ICollection<T> sortedCollection = new List<T>();
                        (sortedCollection as List<DeviceModelViewing>).AddRange((collection as List<DeviceModelViewing>).OrderByDescending(k => k.ElectricityConsumption));
                        return sortedCollection;
                    }
                case "Status":
                    if (direction)
                    {
                        ICollection<T> sortedCollection = new List<T>();
                        (sortedCollection as List<DeviceModelViewing>).AddRange((collection as List<DeviceModelViewing>).OrderBy(k => k.Status));
                        return sortedCollection;
                    }
                    else
                    {
                        ICollection<T> sortedCollection = new List<T>();
                        (sortedCollection as List<DeviceModelViewing>).AddRange((collection as List<DeviceModelViewing>).OrderByDescending(k => k.Status));
                        return sortedCollection;
                    }
                default: throw new BadKeyException("Такого ключа сортировки не существует: " + key);
            }

        }

        #endregion
    }
}
