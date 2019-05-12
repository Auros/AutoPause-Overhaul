using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AutoPause
{
    static class Log
    {
        private static string loggerName = "AutoPause";

        public static void Info(object message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[" + loggerName + " - Info] " + message);
            Console.ResetColor();
        }

        public static void Warning(object message)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("[" + loggerName + " - Warning] " + message);
            Console.ResetColor();
        }

        public static void Error(object message)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("[" + loggerName + " - Error] " + message);
            Console.ResetColor();
        }

        public static void Exception(object message)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("[" + loggerName + " - Exception] " + message);
            Console.ResetColor();
        }

        public static void Stats(object message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[" + loggerName + " - Statistics] " + message);
            Console.ResetColor();
        }

    }
}