using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProfileService.Models.Exceptions
{
    public class ProfileServiceException : Exception
    {
        public ProfileServiceException(string message) : base(message)
        {
        }
    }
}
