using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAUEP.Core.Interfaces;

namespace SAUEP.Core.Models
{
    public class ConnectionModel : IModel
    {
        public string ConnectionUrl { get; set; }
    }
}
