using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterUserService.Models.Exceptions
{
    public class InterUserException : Exception
    {
        public InterUserException(string message): base(message)
        {
        }
    }
}
