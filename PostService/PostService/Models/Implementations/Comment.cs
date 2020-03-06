using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostService.Models.Implementations
{
    public class Comment : ProfileBased
    {
        public string Message { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
