using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterUserService.Models.Implemetations
{
    public class CountList<T> : List<T>
    {
        public long TotalCount { get; set; }
        public double AverageRating { get; set; }
    }

}
