using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostService.Models.Implementations
{
    public class Like: ProfileBased
    {
        public DateTime DateLiked { get; set; }
    }
}
