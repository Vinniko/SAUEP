using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace SAUEP.WPF.Models
{
    public sealed class DeviceGroupModelViewing 
    {
        #region Constructors

        public DeviceGroupModelViewing()
        {

        }

        #endregion



        #region Fields

        public string Title { get; set; }
        public double Power { get; set; }
        public double PowerPercent { get; set; }
        public double ElectricityConsumption { get; set; }

        #endregion
    }
}
