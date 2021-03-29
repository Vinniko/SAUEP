using System.Collections.Generic;
using System.Linq;
using SAUEP.Core.Interfaces;
using SAUEP.WPF.Models;
using SAUEP.Core.Exceptions;

namespace SAUEP.WPF.Services
{
    public sealed class DeviceGroupModelViewingFilter : IFilter
    {
        #region Main Logic

        public ICollection<T> Filtering<T>(ICollection<T> list)
        {
            return list;
        }

        public ICollection<T> Filtering<T>(ICollection<T> list, string key, string filter)
        {
            ICollection<T> filterCollection = new List<T>();
            if (key.Equals("Title"))
            {
                (filterCollection as List<DeviceGroupModelViewing>).AddRange(from model in list as List<DeviceGroupModelViewing>
                                                                             where model.Title.Contains(filter)
                                                                             select model);
                return filterCollection;
            }
            else throw new BadKeyException("Такого ключа фильтрации не существует: " + key);

        }

        public ICollection<T> Filtering<T>(ICollection<T> list, string key, double upFilter, double lowFilter = 0)
        {
            ICollection<T> filterCollection = new List<T>();
            switch (key)
            {
                case "Power":
                    (filterCollection as List<DeviceGroupModelViewing>).AddRange(from model in list as List<DeviceGroupModelViewing>
                                                                    where model.Power > lowFilter
                                                                    where model.Power < upFilter
                                                                    select model);
                    return filterCollection;
                case "PowerPercent":
                    (filterCollection as List<DeviceGroupModelViewing>).AddRange(from model in list as List<DeviceGroupModelViewing>
                                                                                 where model.PowerPercent > lowFilter
                                                                                 where model.PowerPercent < upFilter
                                                                                 select model);
                    return filterCollection;
                case "ElectricityConsumption":
                    (filterCollection as List<DeviceGroupModelViewing>).AddRange(from model in list as List<DeviceGroupModelViewing>
                                                                                 where model.ElectricityConsumption > lowFilter
                                                                                 where model.ElectricityConsumption < upFilter
                                                                                 select model);
                    return filterCollection;
                default:
                    throw new BadKeyException("Такого ключа фильтрации не существует: " + key);
            }  
        }

        #endregion
    }
}
