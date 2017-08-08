using System;
using System.Collections.Generic;
using System.Text;

namespace ImGui
{
    static class Log
    {
        private static ILogger logger;

        public static void Init(ILogger logger)
        {
            Log.logger = logger;
        }

        public static void Msg(string format, params string[] args)
        {
            if (logger == null) throw new InvalidOperationException("The logger hasn't been initialized.");
            logger.Msg(format, args);
        }
        public static void Warning(string format, params string[] args)
        {
            if (logger == null) throw new InvalidOperationException("The logger hasn't been initialized.");
            logger.Warning(format, args);
        }
        public static void Error(string format, params string[] args)
        {
            if (logger == null) throw new InvalidOperationException("The logger hasn't been initialized.");
            logger.Error(format, args);
        }

    }
}
