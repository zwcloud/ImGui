using System;

namespace ImGui
{
    public class GUILayout
    {
        #region container

        public static void BeginHorizontal(params LayoutOption[] options)
        {
            GUILayout.BeginHorizontal(Content.None, Style.Default, options);
        }

        public static void BeginHorizontal(Style style, params LayoutOption[] options)
        {
            GUILayout.BeginHorizontal(Content.None, style, options);
        }

        public static void BeginHorizontal(Content content, Style style, params LayoutOption[] options)
        {
            LayoutGroup layoutGroup = LayoutUtility.BeginLayoutGroup(isVertical: false, style: style, options: options);
            if (style != Style.Default || content != Content.None)
            {
                GUI.Box(layoutGroup.rect, content, "box_" + layoutGroup.GetHashCode());
            }
        }

        public static void EndHorizontal()
        {
            LayoutUtility.EndLayoutGroup();
        }

        public static void BeginVertical(params LayoutOption[] options)
        {
            GUILayout.BeginVertical(Content.None, Style.Default, options);
        }

        public static void BeginVertical(Style style, params LayoutOption[] options)
        {
            GUILayout.BeginVertical(Content.None, style, options);
        }

        public static void BeginVertical(Content content, Style style, params LayoutOption[] options)
        {
            LayoutGroup layoutGroup = LayoutUtility.BeginLayoutGroup(isVertical: true, style:style, options: options);
            if (style != Style.Default)
            {
                GUI.Box(layoutGroup.rect, content, "box_" + layoutGroup.GetHashCode());
            }
        }

        public static void EndVertical()
        {
            LayoutUtility.EndLayoutGroup();
        }

        #endregion

        #region options

        public static LayoutOption Width(double width)
        {
            return new LayoutOption(LayoutOption.Type.fixedWidth, width);
        }

        public static LayoutOption Height(double height)
        {
            return new LayoutOption(LayoutOption.Type.fixedHeight, height);
        }

        public static LayoutOption ExpandWidth(bool expand)
        {
            return new LayoutOption(LayoutOption.Type.stretchWidth, (!expand) ? 0 : 1);
        }

        public static LayoutOption ExpandHeight(bool expand)
        {
            return new LayoutOption(LayoutOption.Type.stretchHeight, (!expand) ? 0 : 1);
        }

        public static LayoutOption StretchWidth(int factor)
        {
            if(factor <= 0) throw new ArgumentOutOfRangeException("factor", "The stretch factor must be positive.");
            return new LayoutOption(LayoutOption.Type.stretchWidth, factor);
        }

        public static LayoutOption StretchHeight(int factor)
        {
            if (factor <= 0) throw new ArgumentOutOfRangeException("factor", "The stretch factor must be positive.");
            return new LayoutOption(LayoutOption.Type.stretchHeight, factor);
        }

        #endregion

        #region simple control

        #region Space

        public static void Space(double pixels)
        {
            LayoutUtility.GetRect(Content.None, Skin.current.Space,
                LayoutUtility.current.topGroup.isVertical
                    ? new[] {GUILayout.Height(pixels)}
                    : new[] {GUILayout.Width(pixels)});
        }

        public static void FlexibleSpace()
        {
            LayoutUtility.GetRect(Size.Zero, Skin.current.Space,
                LayoutUtility.current.topGroup.isVertical
                    ? new[] { GUILayout.StretchHeight(1) }
                    : new[] { GUILayout.StretchWidth(1) });
        }

        #endregion

        #region Button

        public static bool Button(string textWithPossibleId, params LayoutOption[] options)
        {
            string text, id;
            Utility.GetId(textWithPossibleId, out text, out id);
            return Button(text, Skin.current.Button["Normal"], id, options);
        }

        public static bool Button(string text, string name, params LayoutOption[] options)
        {
            return Button(text, Skin.current.Button["Normal"], name, options);
        }

        public static bool Button(Content content, params LayoutOption[] options)
        {
            string text, id;
            Utility.GetId(content.Text, out text, out id);
            return Button(content, Skin.current.Button["Normal"], id, options);
        }

        public static bool Button(Content content, string name, params LayoutOption[] options)
        {
            return Button(content, Skin.current.Button["Normal"], name, options);
        }

        public static bool Button(string text, Style style, string name, params LayoutOption[] options)
        {
            return DoButton(Content.Cached(text, name), style, name, options);
        }

        public static bool Button(Content content, Style style, string name, params LayoutOption[] options)
        {
            return DoButton(content, style, name);
        }

        private static bool DoButton(Content content, Style style, string name, params LayoutOption[] options)
        {
            var rect = LayoutUtility.GetRect(content, style, options);
            return GUI.Button(rect, content, name);
        }

        #endregion

        #region Label

        public static void Label(string textWithPossibleId, params LayoutOption[] options)
        {
            string text, id;
            Utility.GetId(textWithPossibleId, out text, out id);
            Label(text, Skin.current.Label["Normal"], id, options);
        }

        public static void Label(string text, string name, params LayoutOption[] options)
        {
            Label(text, Skin.current.Label["Normal"], name, options);
        }

        public static void Label(Content content, params LayoutOption[] options)
        {
            string text, id;
            Utility.GetId(content.Text, out text, out id);
            Label(content, Skin.current.Label["Normal"], id, options);
        }

        public static void Label(Content content, string name, params LayoutOption[] options)
        {
            Label(content, Skin.current.Label["Normal"], name, options);
        }

        public static void Label(string text, Style style, string name, params LayoutOption[] options)
        {
            DoLabel(Content.Cached(text, name), style, name, options);
        }

        public static void Label(Content content, Style style, string name, params LayoutOption[] options)
        {
            DoLabel(content, style, name, options);
        }

        private static void DoLabel(Content content, Style style, string name, params LayoutOption[] options)
        {
            var rect = LayoutUtility.GetRect(content, style, options);
            GUI.Label(rect, content, name);
        }

        #endregion

        #region Toggle

        #region Toggle with label

        public static bool Toggle(string textWithPossibleId, bool value, params LayoutOption[] options)
        {
            string text, id;
            Utility.GetId(textWithPossibleId, out text, out id);
            return DoToggle(Content.Cached(text, id), value, Skin.current.Toggle["Normal"], id, options);
        }

        public static bool Toggle(string text, bool value, string name, params LayoutOption[] options)
        {
            return DoToggle(Content.Cached(text, name), value, Skin.current.Toggle["Normal"], name, options);
        }

        public static bool Toggle(string text, bool value, Style style, string name, params LayoutOption[] options)
        {
            return DoToggle(Content.Cached(text, name), value, style, name, options);
        }

        public static bool Toggle(Content content, bool value, Style style, params LayoutOption[] options)
        {
            string text, id;
            Utility.GetId(content.Text, out text, out id);
            return DoToggle(content, value, style, id, options);
        }

        public static bool Toggle(Content content, bool value, Style style, string name, params LayoutOption[] options)
        {
            return DoToggle(content, value, style, name, options);
        }

        private static bool DoToggle(Content content, bool value, Style style, string name, params LayoutOption[] options)
        {
            GUILayout.BeginHorizontal();
            var result = GUI.Toggle(LayoutUtility.GetRect(new Size(16, 16), style, options), value, name);
            GUILayout.Label(content, name);
            GUILayout.EndHorizontal();
            return result;
        }

        #endregion

        #region Toggle without label

        public static bool Toggle(bool value, string name, params LayoutOption[] options)//TODO How to auto-gen id?
        {
            return DoToggle(value, Skin.current.Toggle["Normal"], name, options);
        }

        public static bool Toggle(bool value, Style style, string id, params LayoutOption[] options)
        {
            return DoToggle(value, style, id, options);
        }

        private static bool DoToggle(bool value, Style style, string id, params LayoutOption[] options)
        {
            return GUI.Toggle(LayoutUtility.GetRect(new Size(16, 16), style, options), value, id);
        }

        #endregion

        #endregion

        #region HoverButton

        public static bool HoverButton(string textWithPossibleId, params LayoutOption[] options)
        {
            string text, id;
            Utility.GetId(textWithPossibleId, out text, out id);
            return HoverButton(text, Skin.current.Button["Normal"], id, options);
        }

        public static bool HoverButton(string text, string name, params LayoutOption[] options)
        {
            return HoverButton(text, Skin.current.Button["Normal"], name, options);
        }

        public static bool HoverButton(Content content, params LayoutOption[] options)
        {
            string text, id;
            Utility.GetId(content.Text, out text, out id);
            return HoverButton(content, Skin.current.Button["Normal"], id, options);
        }

        public static bool HoverButton(Content content, string name, params LayoutOption[] options)
        {
            return HoverButton(content, Skin.current.Button["Normal"], name, options);
        }

        public static bool HoverButton(string text, Style style, string name, params LayoutOption[] options)
        {
            return DoHoverButton(Content.Cached(text, name), style, name, options);
        }

        public static bool HoverButton(Content content, Style style, string name, params LayoutOption[] options)
        {
            return DoHoverButton(content, style, name);
        }

        private static bool DoHoverButton(Content content, Style style, string name, params LayoutOption[] options)
        {
            var rect = LayoutUtility.GetRect(content, style, options);
            return GUI.HoverButton(rect, content, name);
        }

        #endregion

        #region Slider

        public static double Slider(Size size, double value, double minValue, double maxValue, string id, params LayoutOption[] options)//TODO How to auto-gen name?
        {
            return DoSlider(size, value, minValue, maxValue, Skin.current.Slider["Normal"], true, id, options);
        }

        public static double Slider(Size size, double value, double minValue, double maxValue, Style style, string id, params LayoutOption[] options)//TODO How to auto-gen name?
        {
            return DoSlider(size, value, minValue, maxValue, style, true, id, options);
        }

        public static double VSlider(Size size, double value, double minValue, double maxValue, string id, params LayoutOption[] options)//TODO How to auto-gen name?
        {
            return DoSlider(size, value, minValue, maxValue, Skin.current.Slider["Normal"], false, id, options);
        }

        public static double VSlider(Size size, double value, double minValue, double maxValue, Style style, string id, params LayoutOption[] options)//TODO How to auto-gen name?
        {
            return DoSlider(size, value, minValue, maxValue, style, false, id, options);
        }

        private static double DoSlider(Size size, double value, double minValue, double maxValue, Style style, bool isHorizontal, string id, params LayoutOption[] options)
        {
            return GUI.Slider(LayoutUtility.GetRect(size, style, options), value, minValue, maxValue, isHorizontal, id);
        }


        #endregion

        #region ToggleButton

        public static bool ToggleButton(string textWithPossibleId, bool value, params LayoutOption[] options)
        {
            string text, id;
            Utility.GetId(textWithPossibleId, out text, out id);
            return DoToggleButton(Content.Cached(text, id), value, Skin.current.Button["Normal"], id, options);
        }

        public static bool ToggleButton(string text, bool value, string name, params LayoutOption[] options)
        {
            return DoToggleButton(Content.Cached(text, name), value, Skin.current.Button["Normal"], name, options);
        }

        public static bool ToggleButton(string text, bool value, Style style, string name, params LayoutOption[] options)
        {
            return DoToggleButton(Content.Cached(text, name), value, style, name, options);
        }

        public static bool ToggleButton(Content content, bool value, Style style, params LayoutOption[] options)
        {
            string text, id;
            Utility.GetId(content.Text, out text, out id);
            return DoToggleButton(content, value, style, id, options);
        }

        public static bool ToggleButton(Content content, bool value, Style style, string name, params LayoutOption[] options)
        {
            return DoToggleButton(content, value, style, name, options);
        }

        private static bool DoToggleButton(Content content, bool value, Style style, string name, params LayoutOption[] options)
        {
            var rect = LayoutUtility.GetRect(content, style, options);
            var result = GUI.ToggleButton(rect, content, value, name);
            return result;
        }

        #endregion

        #region PolygonButton

        public static bool PolygonButton(Point[] points, Rect textRect, string textWithPossibleId, params LayoutOption[] options)
        {
            string text, id;
            Utility.GetId(textWithPossibleId, out text, out id);
            return PolygonButton(points, textRect, text, Skin.current.Button["Normal"], id, options);
        }

        public static bool PolygonButton(Point[] points, Rect textRect, string text, string name, params LayoutOption[] options)
        {
            return PolygonButton(points, textRect, text, Skin.current.Button["Normal"], name, options);
        }

        public static bool PolygonButton(Point[] points, Rect textRect, Content content, params LayoutOption[] options)
        {
            string text, id;
            Utility.GetId(content.Text, out text, out id);
            return PolygonButton(points, textRect, content, Skin.current.Button["Normal"], id, options);
        }

        public static bool PolygonButton(Point[] points, Rect textRect, Content content, string name, params LayoutOption[] options)
        {
            return PolygonButton(points, textRect, content, Skin.current.Button["Normal"], name, options);
        }

        public static bool PolygonButton(Point[] points, Rect textRect, string text, Style style, string name, params LayoutOption[] options)
        {
            return DoPolygonButton(points, textRect, Content.Cached(text, name), style, name, options);
        }

        public static bool PolygonButton(Point[] points, Rect textRect, Content content, Style style, string name, params LayoutOption[] options)
        {
            return DoPolygonButton(points, textRect, content, style, name);
        }

        private static bool DoPolygonButton(Point[] points, Rect textRect, Content content, Style style, string id, params LayoutOption[] options)
        {
            var rect = new Rect();
            for (int i = 0; i < points.Length; i++)
            {
                var point = points[i];
                rect.Union(point);
            }
            rect = LayoutUtility.GetRect(rect.Size, style, options);
            return GUI.PolygonButton(rect, points, textRect, content, id);
        }


        #endregion

        #region Image

        public static void Image(string imageFilePathWithPossibleId, params LayoutOption[] options)
        {
            string imageFilePath, id;
            Utility.GetId(imageFilePathWithPossibleId, out imageFilePath, out id);
            Image(Content.CachedTexture(imageFilePath, id), Skin.current.Image["Normal"], id, options);
        }

        public static void Image(ITexture texture, string id, params LayoutOption[] options)
        {
            Image(Content.Cached(texture, id), Skin.current.Image["Normal"], id, options);
        }

        public static void Image(Content content, string id, params LayoutOption[] options)
        {
            Image(content, Skin.current.Image["Normal"], id, options);
        }

        public static void Image(Content content, Style style, string id, params LayoutOption[] options)
        {
            DoImage(content, style, id, options);
        }

        private static void DoImage(Content content, Style style, string id, params LayoutOption[] options)
        {
            var rect = LayoutUtility.GetRect(content, style, options);
            GUI.Image(rect, content, id);
        }

        #endregion


        #endregion

        #region helpers


        #endregion
    }
}