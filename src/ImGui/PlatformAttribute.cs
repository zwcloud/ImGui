using System;

namespace ImGui
{
    class PlatformAttribute : Attribute
    {
        public Platform Value { get; private set; }
        public PlatformAttribute(Platform platform)
        {
            this.Value = platform;
        }
    }
}
