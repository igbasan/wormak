using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostService.Utility
{
    public class HexToDoubleTool
    {
        public static double ConvertToDouble(DateTime datetime)
        {
            double result = double.Parse(datetime.ToString("yyyyMMddHHmmss"));
            return result;
        }
    }
}
