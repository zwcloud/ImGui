using System;
using System.Collections;
using System.Diagnostics;
using System.Net;
using Cairo;

//BUG Input can pass through surface

namespace IMGUI
{
    public class GUI
    {
        private Context g;
        private Context gTop;
        public GUI(Context context, Context contextTop)
        {
            g = context;
            gTop = contextTop;
        }

        public bool Button(Rect rect, string text, string name)
        {
            return IMGUI.Button.DoControl(g, rect, text, name);
        }

        public void Label(Rect rect, string text, string name)
        {
            IMGUI.Label.DoControl(g, rect, text, name);
        }

        public bool Toggle(Rect rect, string text, bool value, string name)
        {
            return IMGUI.Toggle.DoControl(g, rect, text, value, name);
        }

        public int CombolBox(Rect rect, string[] text, int selectedIndex, string name)
        {
            return ComboBox.DoControl(g, gTop, rect, text, selectedIndex, name);
        }

        public void Image(Rect rect, Texture image, string name)
        {
            IMGUI.Image.DoControl(g, rect, image, name);
        }

        public bool Radio(Rect rect, string text, string groupName, bool value, string name)
        {
            return IMGUI.Radio.DoControl(g, rect, text, groupName, value, name);
        }

        public string TextBox(Rect rect, string text, string name)
        {
            return IMGUI.TextBox.DoControl(g, rect, text, name);
        }
    }
}
