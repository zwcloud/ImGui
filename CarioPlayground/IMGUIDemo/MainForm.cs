using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Cairo;
using IMGUI;
using Win32;
using WinFormCario;
using Point = IMGUI.Point;

namespace IMGUIDemo
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            InitIMGUI();
        }
    }
}
