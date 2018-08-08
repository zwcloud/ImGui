using System;
using System.Runtime.InteropServices;

namespace ImGui
{
    public class ConsoleLogger : ILogger
    {
        public void Clear()
        {
            Console.Clear();
        }

        public void Msg(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }

        public void Warning(string format, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(format, args);
            Console.ResetColor();
        }

        public void Error(string format, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(format, args);
            Console.ResetColor();
        }
    }
}