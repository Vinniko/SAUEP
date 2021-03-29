using System.Collections.Generic;
using System.Linq;
using SAUEP.Core.Interfaces;
using SAUEP.WPF.Models;
using SAUEP.Core.Exceptions;

namespace SAUEP.WPF.Services
{
    public sealed class DeviceModelViewingFilter : IFilter
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
                case "Title":
                    (filterCollection as List<DeviceModelViewing>).AddRange(from model in list as List<DeviceModelViewing>
                                                                                 where model.Title.Contains(filter)
                                                                                 select model);
                    return filterCollection;
                case "Serial":
                    (filterCollection as List<DeviceModelViewing>).AddRange(from model in list as List<DeviceModelViewing>
                                                                                 where model.Serial.Contains(filter)
                                                                                 select model);
                    return filterCollection;
                default: throw new BadKeyException("Такого ключа фильтрации не существует: " + key);
            }
        }

        public ICollection<T> Filtering<T>(ICollection<T> list, string key, double upFilter, double lowFilter = 0)
        {
            ICollection<T> filterCollection = new List<T>();
            switch (key)
            {
                case "Power":
                    (filterCollection as List<DeviceModelViewing>).AddRange(from model in list as List<DeviceModelViewing>
                                                                    where model.Power > lowFilter
                                                                    where model.Power < upFilter
                                                                    select model);
                    return filterCollection;
                case "PowerPercent":
                    (filterCollection as List<DeviceModelViewing>).AddRange(from model in list as List<DeviceModelViewing>
                                                                                 where model.PowerPercent > lowFilter
                                                                                 where model.PowerPercent < upFilter
                                                                                 select model);
                    return filterCollection;
                case "ElectricityConsumption":
                    (filterCollection as List<DeviceModelViewing>).AddRange(from model in list as List<DeviceModelViewing>
                                                                                 where model.ElectricityConsumption > lowFilter
                                                                                 where model.ElectricityConsumption < upFilter
                                                                                 select model);
                    return filterCollection;
                default:
                    throw new BadKeyException("Такого ключа фильтрации не существует: " + key);
            }  
        }
        public ICollection<T> Filtering<T>(ICollection<T> list, string key, int filter)
        {
            ICollection<T> filterCollection = new List<T>();
            if (key.Equals("Status"))
            {
                if (filter == 0)
                {
                    return filterCollection;
                }
                else if (filter == 1)
                {
                    (filterCollection as List<DeviceModelViewing>).AddRange(from model in list as List<DeviceModelViewing>
                                                                            where model.Status == true
                                                                            select model);
                    return filterCollection;
                }
                else if (filter == 2)
                {
                    (filterCollection as List<DeviceModelViewing>).AddRange(from model in list as List<DeviceModelViewing>
                                                                            where model.Status == false
                                                                            select model);
                    return filterCollection;
                }
                else
                {
                    return filterCollection;
                }
            }
            else throw new BadKeyException("Такого ключа фильтрации не существует: " + key);
        }

        #endregion
    }
}
