using System;

namespace ImGui
{
    static class Log
    {
        private static ILogger logger;

        public static bool LogStatus { get; set; } = false;

        public static void Init(ILogger logger)
        {
            Log.logger = logger;
        }

        public static void Msg(string format, params string[] args)
        {
            if (logger == null) throw new InvalidOperationException("The logger hasn't been initialized.");
            if (!logger.Enabled) return;
            logger.Msg(format, args);
        }
        public static void Warning(string format, params string[] args)
        {
            if (logger == null) throw new InvalidOperationException("The logger hasn't been initialized.");
            if (!logger.Enabled) return;
            logger.Warning(format, args);
        }
        public static void Error(string format, params string[] args)
        {
            if (logger == null) throw new InvalidOperationException("The logger hasn't been initialized.");
            if (!logger.Enabled) return;
            logger.Error(format, args);
        }

    }
}
