using System.Diagnostics;

namespace ImGui
{
    internal class DebugLogger : ILogger
    {
        public void Clear()
        {
            //dummy, too hard to implement in Visual Studio
        }

        public void Msg(string format, params object[] args)
        {
            Debug.WriteLine(format, args);
        }

        public void Warning(string format, params object[] args)
        {
            Debug.WriteLine(format, args);
        }

        public void Error(string format, params object[] args)
        {
            Debug.WriteLine(format, args);
        }
    }
}
