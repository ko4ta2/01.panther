using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public static class StringExtentions
    {
        public static bool In(this string str, params string[] strings)
        {
            return strings.Contains(str);
        }
    }
}
