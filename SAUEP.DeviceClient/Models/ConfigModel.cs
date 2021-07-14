using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAUEP.DeviceClient.Interfaces;

namespace SAUEP.DeviceClient.Models
{
    class ConfigModel : IModel
    {
        #region Fields

        public string Ip { get; set; }
        public int Port { get; set; }

        #endregion
    }
}
