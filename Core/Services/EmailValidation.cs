using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAUEP.Core.Interfaces;

namespace SAUEP.Core.Services
{
    public sealed class EmailValidation : IValidation
    {
        public bool Validate<T>(T data)
        {
            return true;
        }
    }
}
