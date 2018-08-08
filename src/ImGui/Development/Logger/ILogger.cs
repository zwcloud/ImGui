namespace ImGui
{
    internal interface ILogger
    {
        void Clear();
        void Msg(string format, params object[] args);
        void Warning(string format, params object[] args);
        void Error(string format, params object[] args);
    }
}
