using ImGui.Common.Primitive;
using ImGui.OSAbstraction.Graphics;

namespace ImGui
{
    public partial class GUI
    {
        /// <summary>
        /// Create an image.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="filePath">file path of the image to display. The path should be relative to current dir or absolute.</param>
        public static void Image(Rect rect, string filePath)
        {
            DoImage(rect, filePath);
        }

        public static void Image(Rect rect, ITexture texture)
        {
            DoImage(rect, texture);
        }

        internal static void DoImage(Rect rect, string filePath)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            var s = g.StyleStack;
            var modifiers = GUISkin.Instance[GUIControlName.Image];
            s.PushRange(modifiers);

            var style = s.Style;
            var texture = TextureUtil.GetTexture(filePath);
            DrawList d = window.DrawList;
            d.DrawBoxModel(rect, texture, style);

            s.PopStyle(modifiers.Length);
        }

        internal static void DoImage(Rect rect, ITexture texture)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            var s = g.StyleStack;
            var modifiers = GUISkin.Instance[GUIControlName.Image];
            s.PushRange(modifiers);

            DrawList d = window.DrawList;
            var style = s.Style;
            d.DrawBoxModel(rect, texture, style);

            s.PopStyle(modifiers.Length);
        }
    }

    public partial class GUILayout
    {
        /// <summary>
        /// Create an auto-layout image.
        /// </summary>
        /// <param name="filePath">file path of the image to display. The path should be relative to current dir or absolute.</param>
        public static void Image(string filePath)
        {
            DoImage(filePath);
        }

        public static void Image(ITexture texture)
        {
            DoImage(texture);
        }

        private static void DoImage(string filePath)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            var s = g.StyleStack;
            var modifiers = GUISkin.Instance[GUIControlName.Image];
            s.PushRange(modifiers);

            var id = window.GetID(filePath);
            var texture = TextureUtil.GetTexture(filePath);
            var style = s.Style;
            Size size = style.CalcSize(texture, GUIState.Normal);
            var rect = window.GetRect(id, size);

            DrawList d = window.DrawList;
            d.DrawBoxModel(rect, texture, style);

            s.PopStyle(modifiers.Length);
        }

        private static void DoImage(ITexture texture)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            var s = g.StyleStack;
            var modifiers = GUISkin.Instance[GUIControlName.Image];
            s.PushRange(modifiers);

            var id = window.GetID(texture);
            var style = s.Style;
            Size size = style.CalcSize(texture, GUIState.Normal);
            var rect = window.GetRect(id, size);

            DrawList d = window.DrawList;
            d.DrawBoxModel(rect, texture, style);

            s.PopStyle(modifiers.Length);
        }
    }

    partial class GUISkin
    {
        void InitImageStyles()
        {
            var imageStyles = new StyleModifier[]
            {
                 new StyleModifier(GUIStyleName.BorderTop, StyleType.@double, 1.0),
                 new StyleModifier(GUIStyleName.BorderRight, StyleType.@double, 1.0),
                 new StyleModifier(GUIStyleName.BorderBottom, StyleType.@double, 1.0),
                 new StyleModifier(GUIStyleName.BorderLeft, StyleType.@double, 1.0),
                 new StyleModifier(GUIStyleName.BorderTopColor, StyleType.Color, Color.Black),
                 new StyleModifier(GUIStyleName.BorderRightColor, StyleType.Color,Color.Black),
                 new StyleModifier(GUIStyleName.BorderBottomColor, StyleType.Color,Color.Black),
                 new StyleModifier(GUIStyleName.BorderLeftColor, StyleType.Color,Color.Black),
            };
            this.styles.Add(GUIControlName.Image, imageStyles);
        }
    }
}
