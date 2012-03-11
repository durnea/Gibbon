using System;

namespace Gibbon.Logging
{
    public static class Console
    {
        public static void Write(string format, params object[] items)
        {
            System.Console.Write(format, items);
        }

        public static void WriteLine(string format, params object[] items)
        {
            System.Console.WriteLine(format, items);
        }

        public static void Log(string format, params object[] items)
        {
            System.Console.WriteLine("[" + DateTime.UtcNow + "] " + format, items);
        }

        public static void Info(string format, params object[] items)
        {
            System.Console.WriteLine("Info[" + DateTime.UtcNow + "] " + format, items);
        }

        public static void Warn(string format, params object[] items)
        {
            System.Console.WriteLine("Warn[" + DateTime.UtcNow + "] : " + format, items);
        }

        public static void Error(string format, params object[] items)
        {
            System.Console.WriteLine("Error[" + DateTime.UtcNow + "] : " + format, items);
        }
    }
}
