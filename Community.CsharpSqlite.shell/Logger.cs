using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Community
{
	public static class Log
	{
		static int indent = 0;

		static string indentString = String.Empty;

		public static void WriteLine (String str)
		{
			Console.Write (indentString);
			Console.WriteLine (str);
		}

		public static void WriteLine (object obj)
		{
			Console.WriteLine (obj.ToString ());
		}

		public static void Indent ()
		{
			indent++;
			indentString = new String ('\t', indent);
		}

		public static void Unindent ()
		{
			indent--;
			indentString = new String ('\t', indent);
		}

		internal static void WriteHeader (string p)
		{
			Console.ForegroundColor = ConsoleColor.Cyan;
			WriteLine (p);
			Console.ForegroundColor = ConsoleColor.White;
		}

        internal static void TRACE(string x, params object [] ap)
        {
            CsharpSqlite.Sqlite3.TRACE(x,ap);

        }
    }
}
