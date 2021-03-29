using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using SAUEP.Core.Models;

namespace SAUEP.WPF.Models
{
    public sealed class DeviceModelViewing 
    {
        #region Constructors

        public DeviceModelViewing()
        {

        }
        public DeviceModelViewing(DeviceModel deviceModel)
        {
            Serial = deviceModel.Serial;
            Title = deviceModel.Title;
            Status = deviceModel.Status;
        }

        #endregion



        #region Fields

        public string Serial { get; set; }
        public string Title { get; set; }
        public double Power { get; set; }
        public double PowerPercent { get; set; }
        public double ElectricityConsumption { get; set; }
        public bool Status { get; set; }

        #endregion
    }
}
