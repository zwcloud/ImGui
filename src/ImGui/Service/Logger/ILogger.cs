using System;
using System.Collections.Generic;
using System.Text;

namespace ImGui
{
    interface ILogger
    {
        void Clear();
        void Msg(string format, params object[] args);
        void Warning(string format, params object[] args);
        void Error(string format, params object[] args);
    }
}
