using Ivony.Logs;

namespace ImGui
{
    class DefaultLogger : ILogger
    {
        private readonly ConsoleLogger logger = new ConsoleLogger();

        public void Clear()
        {
            logger.Clear();
        }

        public void Msg(string format, params object[] args)
        {
            logger.LogInfo(format, args);
        }

        public void Warning(string format, params object[] args)
        {
            logger.LogWarning(format, args);
        }

        public void Error(string format, params object[] args)
        {
            logger.LogError(format, args);
        }
    }
}
