using Community.CsharpSqlite.Ast;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Community.CsharpSqlite.Utils
{
    public static class StringUtils
    {
        public static string pad(this String str, int length)
        {
            var left = (length - str.Length) / 2;
            var right = length - left - str.Length;
            String s = new String(' ', left) + str + new String(' ', right);
            return s;
        }

        
    }
}
