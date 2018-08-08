using System;

namespace ImGui
{
    static class Log
    {
        internal static ILogger logger;

        public static bool Enabled { get; set; } = false;

        public static bool LogStatus { get; set; } = true;

        public static void Init(ILogger logger)
        {
            Log.logger = logger;
        }

        public static void Msg(string format, params string[] args)
        {
            if (!Enabled) return;
            if (logger == null) throw new InvalidOperationException("The logger hasn't been initialized.");
            logger.Msg(format, args);
        }
        public static void Warning(string format, params string[] args)
        {
            if (!Enabled) return;
            if (logger == null) throw new InvalidOperationException("The logger hasn't been initialized.");
            logger.Warning(format, args);
        }
        public static void Error(string format, params string[] args)
        {
            if (!Enabled) return;
            if (logger == null) throw new InvalidOperationException("The logger hasn't been initialized.");
            logger.Error(format, args);
        }

    }
}
