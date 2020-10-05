using System.Collections.Generic;

namespace ImGui
{
    internal class WindowTempData
    {
        public Dictionary<int, int> StackSizeMap = new Dictionary<int, int>(16);

        public int LastItemId = -1;
        public GUIState LastItemState { get; set; }

        public void Clear()
        {
            StackSizeMap.Clear();
            LastItemId = -1;
        }
    }
}
