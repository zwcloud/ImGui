using Cairo;
using TinyIoC;

namespace IMGUI
{
    public partial class GUI
    {
        public delegate void WindowFunction(GUI gui);

        private readonly Context g;
        private readonly BaseForm form;
        
        public GUI(Context context, BaseForm form)
        {
            this.g = context;
            this.form = form;
        }

        //TODO auto-size button and other controls
        public bool Button(Rect rect, string text, string name)
        {
            rect = DoLayout(rect);
            return IMGUI.Button.DoControl(g, form, rect, text, name);
        }

        public void Label(Rect rect, string text, string name)
        {
            rect = DoLayout(rect);
            IMGUI.Label.DoControl(g, form, rect, text, name);
        }

        public bool Toggle(Rect rect, string text, bool value, string name)
        {
            rect = DoLayout(rect);
            return IMGUI.Toggle.DoControl(g, form, rect, text, value, name);
        }

        public int CombolBox(Rect rect, string[] text, int selectedIndex, string name)
        {
            rect = DoLayout(rect);
            return ComboBox.DoControl(g, null /*FIXME ComboBox subitems should be shown in a new form*/, form, rect, text,
                selectedIndex, name);
        }

        public void Image(Rect rect, Texture image, string name)
        {
            rect = DoLayout(rect);
            IMGUI.Image.DoControl(g, form, rect, image, name);
        }

        public bool Radio(Rect rect, string text, string groupName, bool value, string name)
        {
            rect = DoLayout(rect);
            return IMGUI.Radio.DoControl(g, form, rect, text, groupName, value, name);
        }

        public string TextBox(Rect rect, string text, string name)
        {
            rect = DoLayout(rect);
            return IMGUI.TextBox.DoControl(g, form, rect, text, name);
        }

        public bool PolygonButton(Point[] points, string text, string name)
        {
            return IMGUI.PolygonButton.DoControl(g, form, points, text, name);
        }

        public bool ToggleButton(Rect rect, string text, bool value, string name)
        {
            rect = DoLayout(rect);
            return IMGUI.ToggleButton.DoControl(g, form, rect, text, value, name);
        }

        public bool HoverButton(Rect rect, string text, string name)
        {
            rect = DoLayout(rect);
            return IMGUI.HoverButton.DoControl(g, form, rect, text, name);
        }

        public bool RadioButton(Rect rect, string text, string groupName, bool value, string name)
        {
            rect = DoLayout(rect);
            return IMGUI.RadioButton.DoControl(g, form, rect, text, groupName, value, name);
        }

        public void Window(Rect rect, WindowFunction func, string name)
        {
            IMGUI.Window.DoControl(g, form, rect, func, name);
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
                if(control is IRect)
                {
                    return ((IRect) control).Rect;
                }
                throw new System.InvalidOperationException(
                    string.Format("Can not get the rect of control <{0}> because it is not a rectangle control.",
                        name));
            }
            throw new System.InvalidOperationException(string.Format("Can not find control <{0}>", name));
        }

        #endregion
    }

}
