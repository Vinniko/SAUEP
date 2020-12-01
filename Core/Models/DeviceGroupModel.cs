using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAUEP.Core.Interfaces;

namespace SAUEP.Core.Models
{
    public sealed class DeviceGroupModel : IModel
    {
        #region Constructors

        public DeviceGroupModel(string title, int id = 0)
        {
            Id = id;
            Title = title;
        }

        #endregion



        #region Fields

        public int Id { get; set; }
        public string Title { get; set; }

        #endregion
    }
}
