using Cairo;

namespace ImGui
{
    public partial class GUI
    {
        public delegate bool WindowFunction(GUI gui);

        private readonly Context g;
        private readonly BaseForm form;
        
        public GUI(Context context, BaseForm form)
        {
            this.g = context;
            this.form = form;
        }

        public void Space(Rect rect)
        {
            DoLayout(rect);
        }

        //TODO auto-size button and other controls
        public bool Button(Rect rect, string text, string name)
        {
            rect = DoLayout(rect);
            return ImGui.Button.DoControl(form, rect, text, name);
        }

        public void Label(Rect rect, string text, string name)
        {
            rect = DoLayout(rect);
            ImGui.Label.DoControl(g, form, rect, text, name);
        }

        public bool Toggle(Rect rect, string text, bool value, string name)
        {
            rect = DoLayout(rect);
            return ImGui.Toggle.DoControl(form, rect, text, value, name);
        }

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

        public bool HoverButton(Rect rect, string text, string name)
        {
            rect = DoLayout(rect);
            return ImGui.HoverButton.DoControl(form, rect, text, name);
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
        #endregion

        #region control methods

        public Rect GetControlRect(string name)
        {
            Control control;
            if(form.Controls.TryGetValue(name, out control))
            {
                return control.Rect;
            }
            throw new System.InvalidOperationException(string.Format("Can not find control <{0}>", name));
        }

        #endregion
    }

}
