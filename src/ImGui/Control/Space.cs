using ImGui.Common.Primitive;

namespace ImGui
{
    public partial class GUILayout
    {
        /// <summary>
        /// Create a fixed size space.
        /// </summary>
        public static void Space(string str_id, double size)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            int id = window.GetID(str_id);

            // rect
            var layout = window.StackLayout;
            layout.GetRect(id, window.RenderTree.CurrentContainer.IsVertical ? new Size(0, size) : new Size(size, 0));
        }

        /// <summary>
        /// Create a flexible space.
        /// </summary>
        public static void FlexibleSpace(string str_id, int stretchFactor = 1)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            // style apply
            var style = GUIStyle.Basic;
            var layout = window.StackLayout;
            style.PushStretchFactor(window.RenderTree.CurrentContainer.IsVertical, stretchFactor);

            // rect
            int id = window.GetID(str_id);
            window.GetRect(id);

            // style restore
            style.PopStyle();
        }
    }
}
