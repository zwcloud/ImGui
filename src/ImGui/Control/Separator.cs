using ImGui.Common.Primitive;

namespace ImGui
{
    public partial class GUILayout
    {
        /// <summary>
        /// Create an auto-layout horizontal separating line.
        /// </summary>
        public static void Separator(string str_id)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            int id = window.GetID(str_id);

            // style
            var s = g.StyleStack;
            var style = g.StyleStack.Style;
            s.PushStretchFactor(false, 1);//+1

            // rect
            var rect = window.GetRect(id, new Size(0, 1));

            // render
            var d = window.DrawList;
            var offset = new Vector(rect.Width / 2, 0);
            var start = rect.Center - offset;
            var end = rect.Center + offset;
            d.AddLine(start, end, style.StrokeColor);

            s.PopStyle();//-1
        }
    }
}
