using ImGui.Common.Primitive;
using ImGui.OSAbstraction.Graphics;

namespace ImGui
{
    public partial class GUI
    {
        /// <summary>
        /// Create an image.
        /// </summary>
        /// <param name="rect">position and size</param>
        /// <param name="filePath">file path of the image. The path should be relative to current dir or absolute.</param>
        public static void Image(Rect rect, string filePath)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            // style apply
            var s = g.StyleStack;
            var style = s.Style;
            var modifiers = GUISkin.Instance[GUIControlName.Image];
            s.PushRange(modifiers);

            // rect
            rect = window.GetRect(rect);

            // render
            var texture = TextureUtil.GetTexture(filePath);
            DrawList d = window.DrawList;
            d.DrawBoxModel(rect, texture, style);

            // style restore
            s.PopStyle(modifiers.Length);
        }

        /// <summary>
        /// Create an auto-layout image.
        /// </summary>
        /// <param name="rect">position and size</param>
        /// <param name="texture">texture, call<see cref="CreateTexture"/>to load it manually.</param>
        public static void Image(Rect rect, ITexture texture)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            // style apply
            var s = g.StyleStack;
            var style = s.Style;
            var modifiers = GUISkin.Instance[GUIControlName.Image];
            s.PushRange(modifiers);

            // rect
            rect = window.GetRect(rect);

            // render
            DrawList d = window.DrawList;
            d.DrawBoxModel(rect, texture, style);

            // style restore
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
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            var id = window.GetID(filePath);

            // style apply
            var s = g.StyleStack;
            var style = s.Style;
            var modifiers = GUISkin.Instance[GUIControlName.Image];
            s.PushRange(modifiers);

            // rect
            var texture = TextureUtil.GetTexture(filePath);
            Size size = style.CalcSize(texture, GUIState.Normal);
            var rect = window.GetRect(id, size);

            // render
            DrawList d = window.DrawList;
            d.DrawBoxModel(rect, texture, style);

            // style restore
            s.PopStyle(modifiers.Length);
        }

        public static void Image(ITexture texture)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            var id = window.GetID(texture);

            // style apply
            var s = g.StyleStack;
            var modifiers = GUISkin.Instance[GUIControlName.Image];
            s.PushRange(modifiers);

            // rect
            var style = s.Style;
            Size size = style.CalcSize(texture, GUIState.Normal);
            var rect = window.GetRect(id, size);

            // render
            DrawList d = window.DrawList;
            d.DrawBoxModel(rect, texture, style);

            // style restore
            s.PopStyle(modifiers.Length);
        }
    }

    partial class GUISkin
    {
        void InitImageStyles()
        {
            var imageStyles = new []
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
