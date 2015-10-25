using Cairo;

namespace IMGUI
{
    public class GUI
    {
        private Context g;

        public GUI(Context context, BaseForm form)
        {
            this.g = context;
            this.form = form;
        }

        private BaseForm form;
        private BaseForm Form { get { return form; } }

        public bool Button(Rect rect, string text, string name)
        {
            return IMGUI.Button.DoControl(g, Form, rect, text, name);
        }

        public void Label(Rect rect, string text, string name)
        {
            IMGUI.Label.DoControl(g, Form, rect, text, name);
        }

        public bool Toggle(Rect rect, string text, bool value, string name)
        {
            return IMGUI.Toggle.DoControl(g, Form, rect, text, value, name);
        }

        public int CombolBox(Rect rect, string[] text, int selectedIndex, string name)
        {
            return ComboBox.DoControl(g, null /*FIXME ComboBox subitems should be shown in a new form*/, Form, rect, text,
                selectedIndex, name);
        }

        public void Image(Rect rect, Texture image, string name)
        {
            IMGUI.Image.DoControl(g, Form, rect, image, name);
        }

        public bool Radio(Rect rect, string text, string groupName, bool value, string name)
        {
            return IMGUI.Radio.DoControl(g, Form, rect, text, groupName, value, name);
        }

        public string TextBox(Rect rect, string text, string name)
        {
            return IMGUI.TextBox.DoControl(g, Form, rect, text, name);
        }

        public bool PolygonButton(Point[] points, string text, string name)
        {
            return IMGUI.PolygonButton.DoControl(g, Form, points, text, name);
        }

    }
}
