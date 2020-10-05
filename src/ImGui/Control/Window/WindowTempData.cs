using System.Collections.Generic;

namespace ImGui
{
    internal class WindowTempData
    {
        public Dictionary<int, int> StackSizeMap = new Dictionary<int, int>(16);

        public void Clear()
        {
            StackSizeMap.Clear();
        }
    }
}
