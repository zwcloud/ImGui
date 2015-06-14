using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IMGUI
{
    public class GUI
    {
        Cairo.Context g;
        public GUI(Cairo.Context context)
        {
            g = context;
        }

        public bool Button(Rect rect, string text)
        {
            g.DrawBoxModel(rect, new Content(text), Skin._current.Button, Skin._current.Font);
            return Input.LeftButtonClicked && rect.Contains(Input.MousePos);
        }

        public void Label(Rect rect, string text)
        {
            g.DrawText(rect,text,Skin._current.Font);
        }
    }
}
