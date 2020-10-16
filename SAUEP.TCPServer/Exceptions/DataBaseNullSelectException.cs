using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAUEP.TCPServer.Exceptions
{
    public sealed class DataBaseNullSelectException : Exception
    {
        public DataBaseNullSelectException(string message)
        : base(message)
        { }
    }
}
