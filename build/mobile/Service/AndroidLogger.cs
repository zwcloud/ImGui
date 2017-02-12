using System;

namespace ImGui
{
    //[Platform(Platform.Android)]
    class AndroidLogger : ILogger
    {
        public void Clear()
        {

        }

        public void Msg(string format, params object[] args)
        {
            Android.Util.Log.Info("ImGui", format, args);
        }
        public void Warning(string format, params object[] args)
        {
            Android.Util.Log.Warn("ImGui", format, args);
        }
        public void Error(string format, params object[] args)
        {
            Android.Util.Log.Error("ImGui", format, args);
        }
    }
}