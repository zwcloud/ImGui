using System;
using System.Collections.Generic;
using System.Text;

namespace ImGui
{
    static class Flags
    {
        public static bool HaveFlag(this ButtonFlags value, ButtonFlags flag)
        {
            return (value & flag) != 0;
        }

        public static bool HaveFlag(this WindowFlags value, WindowFlags flag)
        {
            return (value & flag) != 0;
        }
    }
}
