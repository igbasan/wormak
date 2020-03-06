using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterUserService.Utilities
{
    public class HexToDoubleTool
    {
        public static double ConvertToDouble(string hexadecimalNumber)
        {
            double result = 0;
            if (string.IsNullOrWhiteSpace(hexadecimalNumber)) return result;

            foreach (char item in hexadecimalNumber)
            {
                result *= 16;
                result += Convert.ToInt32(item.ToString(), 16);
            }
            return result;
        }
    }
}
