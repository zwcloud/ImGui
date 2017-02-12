using System;
using System.Collections.Generic;
using System.Text;

namespace ImGui
{
    [Flags]
    enum Platform
    {
        Windows = 0,
        Mac     = 1,
        Linux   = 2,
        Android = 4,
        iOS     = 8,
    }

    class PlatformAttribute : Attribute
    {
        public Platform Value { get; private set; }
        public PlatformAttribute(Platform platform)
        {
            this.Value = platform;
        }
    }
}
