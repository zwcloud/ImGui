using ImGui.Common.Primitive;

namespace ImGui
{
    public partial class GUI
    {
        /// <summary>
        /// Create a label.
        /// </summary>
        /// <param name="rect">position and size</param>
        /// <param name="text">text to display</param>
        public static void Label(Rect rect, string text)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            // style apply
            var style = GUIStyle.Basic;

            // rect
            rect = window.GetRect(rect);

            // render
            DrawList d = window.DrawList;
            d.DrawBoxModel(rect, text, style);
        }
    }

    public partial class GUILayout
    {
        /// <summary>
        /// Create an auto-layout label.
        /// </summary>
        /// <param name="text">text to display</param>
        public static void Label(string text)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            int id = window.GetID(text);

            // style
            var style = GUIStyle.Basic;

            // rect
            Size contentSize = style.CalcSize(text, GUIState.Normal);
            Rect rect = window.GetRect(id, contentSize);

            // rendering
            DrawList d = window.DrawList;
            d.DrawBoxModel(rect, text, style);
        }

        /// <summary>
        /// Create a colored auto-layout label.
        /// </summary>
        /// <param name="color">text color</param>
        /// <param name="text">text</param>
        public static void Label(Color color, string text)
        {
            PushFontColor(color);
            Label(text);
            PopStyleVar();
        }

        /// <summary>
        /// Create a auto-layout and disabled label.
        /// </summary>
        /// <param name="text">text</param>
        public static void LabelDisabled(string text)
        {
            Label(Color.TextDisabled, text);
        }

        public static void Label(string format, object arg0)
        {
            Label(string.Format(format, arg0));
        }

        public static void Label(string format, object arg0, object arg1)
        {
            Label(string.Format(format, arg0, arg1));
        }

        public static void Label(string format, object arg0, object arg1, object arg2)
        {
            Label(string.Format(format, arg0, arg1, arg2));
        }

        public static void Label(string format, params object[] args)
        {
            Label(string.Format(format, args));
        }

        public static void Text(string text) => Label(text);

        public static void Text(string format, params object[] args) => Label(format, args);

        #region Bullets


        /// <summary>
        /// Create a bullet.
        /// </summary>
        public static void Bullet(string str_id)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            GUIContext g = GetCurrentContext();
            int id = window.GetID(str_id);

            // style
            var style = GUIStyle.Basic;

            // rect
            var lineHeight = style.GetLineHeight();
            Rect rect = window.GetRect(id, new Size(lineHeight, lineHeight));

            // Render
            var d = window.DrawList;
            var bulletPosition = rect.TopLeft + new Vector(0, lineHeight * 0.5f);
            d.RenderBullet(bulletPosition, lineHeight, style.FontColor);
        }

        /// <summary>
        /// Create a label with a little bullet.
        /// </summary>
        public static void BulletText(string text)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            GUIContext g = GetCurrentContext();
            int id = window.GetID(text);

            // style
            var style = GUIStyle.Basic;

            // rect
            Size contentSize = style.CalcSize(text, GUIState.Normal);
            var lineHeight = style.GetLineHeight();
            Rect rect = window.GetRect(id, contentSize);

            // Render
            var d = window.DrawList;
            var bulletPosition = rect.TopLeft + new Vector(0, lineHeight * 0.5f);
            d.RenderBullet(bulletPosition, lineHeight, style.FontColor);
            rect.Offset(lineHeight, 0);
            d.AddText(rect, text, style, GUIState.Normal);
        }

        public static void BulletText(string format, params object[] args)
        {
            BulletText(string.Format(format, args));
        }
    #endregion
    }

    internal static partial class DrawListExtension
    {
        public static void RenderBullet(this DrawList drawList, Point pos, double lineHeight, Color color)
        {
            drawList.AddCircleFilled(pos, (float)lineHeight * 0.20f, color, 8);
        }
    }

}
