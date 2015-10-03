using System.Windows.Forms;
using IMGUI;
using Form = IMGUI.Form;

namespace IMGUIDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            ClientSize = new System.Drawing.Size(800, 600);
            StartPosition = FormStartPosition.CenterScreen;

            Skin.current.Radio["Normal"].OutlineWidth = Length.OnePixel;
        }
    }


}

