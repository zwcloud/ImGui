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
            Image(rect, filePath, GUISkin.Instance[GUIControlName.Image]);
        }

        public static void Image(Rect rect, string filePath, GUIStyle style)
        {
            DoImage(rect, filePath, style);
        }

        public static void Image(Rect rect, ITexture texture)
        {
            Image(rect, texture, GUISkin.Instance[GUIControlName.Image]);
        }

        public static void Image(Rect rect, ITexture texture, GUIStyle style)
        {
            DoImage(rect, texture, style);
        }

        internal static void DoImage(Rect rect, string filePath, GUIStyle style)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            Window window = g.CurrentWindow;
            DrawList d = window.DrawList;

            var texture = TextureUtil.GetTexture(filePath);

            d.DrawBoxModel(rect, texture, style);
        }

        internal static void DoImage(Rect rect, ITexture texture, GUIStyle style)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            Window window = g.CurrentWindow;
            DrawList d = window.DrawList;

            d.DrawBoxModel(rect, texture, style);
        }
    }

    public partial class GUILayout
    {
        /// <summary>
        /// Create an auto-layout image.
        /// </summary>
        /// <param name="filePath">file path of the image to display. The path should be relative to current dir or absolute.</param>
        /// <param name="id">the unique id of this control</param>
        /// <param name="options">layout options that specify layouting properties. See also <see cref="GUILayout.Width"/>, <see cref="GUILayout.Height"/>, <see cref="GUILayout.ExpandWidth"/>, <see cref="GUILayout.ExpandHeight"/>, <see cref="GUILayout.StretchWidth"/>, <see cref="GUILayout.StretchHeight"/></param>
        public static void Image(string filePath, params LayoutOption[] options)
        {
            Image(filePath, GUISkin.Instance[GUIControlName.Image], options);
        }

        public static void Image(string filePath, GUIStyle style, params LayoutOption[] options)
        {
            DoImage(filePath, style, options);//var texture = TextureUtil.GetTexture(filePath);
        }

        private static void DoImage(string filePath, GUIStyle style, params LayoutOption[] options)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            var id = window.GetID(filePath);
            Size size = style.CalcSize(filePath, GUIState.Normal, options);
            var rect = window.GetRect(id, size, style,
                new[] { GUILayout.Width(size.Width), GUILayout.Height(size.Height) });
            GUI.Image(rect, filePath, style);
        }

        internal static void Image(ITexture texture, params LayoutOption[] options)
        {
            Image(texture, GUISkin.Instance[GUIControlName.Image], options);
        }

        internal static void Image(ITexture texture, GUIStyle style, params LayoutOption[] options)
        {
            DoImage(texture, style, options);
        }

        private static void DoImage(ITexture texture, GUIStyle style, params LayoutOption[] options)
        {
            Window window = GetCurrentWindow();
            var id = window.GetID(texture);
            Size size = style.CalcSize(texture, GUIState.Normal, options);
            var rect = window.GetRect(id, size, style,
                new[] { GUILayout.Width(size.Width), GUILayout.Height(size.Height) });
            GUI.Image(rect, texture, style);
        }
    }
}
