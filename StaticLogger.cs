using System;
using System.Text;

namespace Deploy
{
    public static class StaticLogger
    {
        public static string Log => _sb.ToString();
        private static StringBuilder _sb = new StringBuilder();
        private static object _lock = new object();

        public static void WriteLine(string message)
        {
            lock(_lock)
            {
                Console.WriteLine(message);
               _sb.AppendLine(Cut(message));
            }
        }

        public static void WriteErrorLine(string message)
        {
            lock(_lock)
            {
                Console.Error.WriteLine(message);
                _sb.AppendLine(Cut(message));
            }
        }

        private static string Cut(string message)
        {
            return message?.Length < 500 ? message : message?.Substring(0, 500);
        }
    }
}