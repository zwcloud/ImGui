namespace ImGui
{
    public partial class GUI
    {
        public delegate bool WindowFunction();

        #region Button

        public static bool Button(Rect rect, string text, string id)
        {
            return DoButton(rect, Content.Cached(text, id), id);
        }

        public static bool Button(Rect rect, Content content, string id)
        {
            return DoButton(rect, content, id);
        }

        private static bool DoButton(Rect rect, Content content, string id)
        {
            return ImGui.Button.DoControl(rect, content, id);
        }

        #endregion

        #region Label

        public static void Label(Rect rect, string text, string id)
        {
            ImGui.Label.DoControl(rect, Content.Cached(text, id), id);
        }

        public static void Label(Rect rect, Content content, string id)
        {
            ImGui.Label.DoControl(rect, content, id);
        }

        #endregion

        #region Box

        public static void Box(Rect rect, Content content, string id)
        {
            DoBox(rect, content, id);
        }

        private static void DoBox(Rect rect, Content content, string id)
        {
            ImGui.Box.DoControl(rect, content, id);
        }

        #endregion

        #region Toggle

        public static bool Toggle(Rect rect, bool value, string id)
        {
            return DoToggle(rect, value, id);
        }

        private static bool DoToggle(Rect rect, bool value, string id)
        {
            return ImGui.Toggle.DoControl(rect, value, id);
        }

        #endregion

        #region HoverButton

        public static bool HoverButton(Rect rect, string text, string id)
        {
            return DoHoverButton(rect, Content.Cached(text, id), id);
        }

        public static bool HoverButton(Rect rect, Content content, string id)
        {
            return DoHoverButton(rect, content, id);
        }

        private static bool DoHoverButton(Rect rect, Content content, string id)
        {
            return ImGui.HoverButton.DoControl(rect, content, id);
        }

        #endregion

        #region Slider

        public static double Slider(Rect rect, double value, double minValue, double maxValue, string id)
        {
            return ImGui.Slider.DoControl(rect, value, minValue, maxValue, id);
        }

        #endregion

        /*

        public int CombolBox(Rect rect, string[] text, int selectedIndex, string name)
        {
            rect = DoLayout(rect);
            return ComboBox.DoControl(form, rect, text, selectedIndex, name);
        }

        public void Image(Rect rect, Texture image, string name)
        {
            rect = DoLayout(rect);
            ImGui.Image.DoControl(form, rect, image, name);
        }

        public bool Radio(Rect rect, string text, string groupName, bool value, string name)
        {
            rect = DoLayout(rect);
            return ImGui.Radio.DoControl(form, rect, text, groupName, value, name);
        }

        public float Slider(Rect rect, string text, float value, float leftValue, float rightValue, string name)
        {
            rect = DoLayout(rect);
            return ImGui.Slider.DoControl(form, rect, value, leftValue, rightValue, name);
        }

        public float SliderV(Rect rect, string text, float value, float leftValue, float rightValue, string name)
        {
            rect = DoLayout(rect);
            return ImGui.SliderV.DoControl(form, rect, value, leftValue, rightValue, name);
        }

        public string TextBox(Rect rect, string text, string name)
        {
            rect = DoLayout(rect);
            return ImGui.TextBox.DoControl(form, rect, text, name);
        }

        public bool PolygonButton(Point[] points, string text, string name)
        {
            return ImGui.PolygonButton.DoControl(form, points, text, name);
        }

        public bool ToggleButton(Rect rect, string text, bool value, string name)
        {
            rect = DoLayout(rect);
            return ImGui.ToggleButton.DoControl(form, rect, text, value, name);
        }


        public bool RadioButton(Rect rect, string text, string groupName, bool value, string name)
        {
            rect = DoLayout(rect);
            return ImGui.RadioButton.DoControl(form, rect, text, groupName, value, name);
        }

        public void TitleBar(Rect rect, Texture iconTexture, string caption, string name)
        {
            ImGui.TitleBar.DoControl(this, rect, iconTexture, caption, form, name);
        }

        public void Window(Rect rect, WindowFunction func, string name)
        {
            ImGui.Window.DoControl(form, rect, func, name);
        }

        public bool MenuItem(Rect rect, string text, string name)
        {
            rect = DoLayout(rect);
            return ImGui.MenuItem.DoControl(rect, text, name);
        }

        #region group methods

        public void BeginClipArea(Rect rect)
        {
            g.Rectangle(rect.TopLeft.ToPointD(), rect.Width, rect.Height);
            g.Clip();
        }

        public void EndClipArea()
        {
            g.ResetClip();
        }
        #endregion

        #region layout methods
        public void BeginH()
        {
            BeginGroup(LayoutMode.Horizontal);
        }

        public void EndH()
        {
            EndGroup(LayoutMode.Horizontal);
        }

        public void BeginV()
        {
            BeginGroup(LayoutMode.Vertical);
        }

        public void EndV()
        {
            EndGroup(LayoutMode.Vertical);
        }

        public void BeginScrollView(Rect occupiedRect, Point scrollPosition, Rect viewRect)
        {

        }

        public void EndScrollView()
        {
        }

        #endregion
        */

        #region control methods

        public Rect GetControlRect(string id)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Constant

        public const string Normal = "Normal";
        public const string Hover = "Hover";
        public const string Active = "Active";

        #endregion
    }

}
