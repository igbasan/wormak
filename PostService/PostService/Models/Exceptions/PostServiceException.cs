using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostService.Models.Exceptions
{
    public class PostServiceException : Exception
    {
        public PostServiceException(string message) : base(message)
        {
        }
    }
}
