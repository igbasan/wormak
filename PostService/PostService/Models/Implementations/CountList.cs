using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostService.Models.Implementations
{
    public class CountList<T> : List<T>
    {
        public long TotalCount { get; set; }
    }
}
