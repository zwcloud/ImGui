namespace ImGui
{
    public class GUILayout
    {
        public static void BeginHorizontal()
        {
            GUILayout.BeginHorizontal(Content.None, Style.None);
        }

        public static void BeginHorizontal(Style style)
        {
            GUILayout.BeginHorizontal(Content.None, style);
        }

        public static void BeginHorizontal(Content content, Style style)
        {
            LayoutGroup layoutGroup = LayoutUtility.BeginLayoutGroup(style);
            layoutGroup.isVertical = false;
            if (style != Style.None || content != Content.None)
            {
                GUI.Box(layoutGroup.rect, content, "box_" + layoutGroup.GetHashCode());
            }
        }

        public static void EndHorizontal()
        {
            LayoutUtility.EndLayoutGroup();
        }

        public static void BeginVertical()
        {
            GUILayout.BeginVertical(Content.None, Style.None);
        }

        public static void BeginVertical(Style style)
        {
            GUILayout.BeginVertical(Content.None, style);
        }

        public static void BeginVertical(Content content, Style style)
        {
            LayoutGroup layoutGroup = LayoutUtility.BeginLayoutGroup(style);
            layoutGroup.isVertical = true;
            if (style != Style.None)
            {
                GUI.Box(layoutGroup.rect, content, "box_" + layoutGroup.GetHashCode());
            }
        }

        public static void EndVertical()
        {
            LayoutUtility.EndLayoutGroup();
        }

        public static bool Button(string text, string name)
        {
            return Button(text, Skin.current.Button["Normal"], name);
        }

        public static bool Button(Content content, string name)
        {
            return Button(content, Skin.current.Button["Normal"], name);
        }

        public static bool Button(string text, Style style, string name)
        {
            return DoButton(Content.Cached(text, name), style, name);
        }

        public static bool Button(Content content, Style style, string name)
        {
            return DoButton(content, style, name);
        }

        private static bool DoButton(Content content, Style style, string name)
        {
            var rect = LayoutUtility.GetRect(content, style);
            return GUI.Button(rect, content, name);
        }
    }
}