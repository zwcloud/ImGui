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
            var style = GUIStyle.Basic;
            style.PushBorder(1.0);//+4

            // rect
            rect = window.GetRect(rect);

            // render
            var texture = TextureUtil.GetTexture(filePath);
            DrawList d = window.DrawList;
            style.PushBorderColor(Color.Black);//+4
            d.DrawBoxModel(rect, texture, style);

            style.PopStyle(4+4);
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

            // style
            var style = GUIStyle.Basic;
            style.PushBorder(1.0);

            // rect
            rect = window.GetRect(rect);

            // render
            DrawList d = window.DrawList;
            style.PushBorderColor(Color.Black);//+4
            d.DrawBoxModel(rect, texture, style);

            style.PopStyle(4+4);
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

            // style
            var style = GUIStyle.Basic;
            style.PushBorder(1.0);

            // rect
            var texture = TextureUtil.GetTexture(filePath);
            Size size = style.CalcSize(texture, GUIState.Normal);
            var rect = window.GetRect(id);

            // render
            DrawList d = window.DrawList;
            style.PushBorderColor(Color.Black);//+4
            d.DrawBoxModel(rect, texture, style);

            style.PopStyle(4 + 4);
        }

        public static void Image(ITexture texture)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            var id = window.GetID(texture);

            // style
            var style = GUIStyle.Basic;
            style.PushBorder(1.0);//+4

            // rect
            Size size = style.CalcSize(texture, GUIState.Normal);
            var rect = window.GetRect(id);

            // render
            DrawList d = window.DrawList;
            style.PushBorderColor(Color.Black);//+4
            d.DrawBoxModel(rect, texture, style);

            style.PopStyle(4+4);
        }
    }
}
