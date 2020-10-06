using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAUEP.TCPServer.Interfaces
{
    interface IWriter
    {
        void Write<T>(T data);
    }
}
