using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAUEP.Core.Exceptions
{
    public sealed class EmptyStreamException : Exception
    {
        public EmptyStreamException(string message)
            : base(message)
        { }
    }
}
