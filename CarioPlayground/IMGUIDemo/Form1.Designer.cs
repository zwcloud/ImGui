//#define ShowButton
//#define ShowToggle
//#define ShowRadio
//#define ShowImage
#define ShowComboxBox

using System.Diagnostics;
using IMGUI;

namespace IMGUIDemo
{
    partial class Form1
    {
        #region paramters
        private bool _opened;

        private readonly string[] comboBoxItems = new[] { "item0", "item1", "item2", "item3" };

        private Texture myImage = new Texture(@"W:\VS2013\IMGUI\IMGUIDemo\empowered-by-gnu.png");

        private bool radio0Selected = false;
        private bool radio1Selected = false;
        private bool radio2Selected = false;

        private int selectedindex;

        #endregion

        protected override void OnGUI(GUI gui)
        {
            int i = 0;

            gui.Label(new Rect(0, 0, this.Size.Width, 40), "IMGUI Demo project", "CaptionLabel");

            int firstY = 80;
#if ShowButton
            i = 0; //Debug.WriteLine("at ({0},{1})", 20, 20 + i * 20);
            if (gui.Button(new Rect(20, firstY + i * 20, 100, 20), "button " + i + "!", "Button" + i))
            {
                Debug.WriteLine("button {0} clicked!", i);
            }

            ++i; //Debug.WriteLine("at ({0},{1})", 20, 20 + i * 20);
            if (gui.Button(new Rect(20, firstY + i * 20, 100, 20), "button " + i + "!", "Button" + i))
            {
                Debug.WriteLine("button {0} clicked!", i);
            }
#endif

#if ShowToggle
            ++i; //Debug.WriteLine("at ({0},{1})", 20, 20 + i * 20);
            var oldValueOfTaggle = _opened;
            _opened = gui.Toggle(new Rect(20, firstY + i * 20, 100, 20), "Opened?", _opened, "Toggle0");
            if(_opened ^ oldValueOfTaggle)
                Debug.WriteLine("Toggle 0 {0}", _opened ? "on!" : "off!", null);
#endif

#if ShowRadio
            ++i; //Debug.WriteLine("at ({0},{1})", 20, 20 + i * 20);
            var oldValueOfradio0 = radio0Selected;
            radio0Selected = gui.Radio(new Rect(20, firstY + i * 20, 100, 20), "RadioItem0", "G0", radio0Selected, "Radio0");
            if(radio0Selected && radio0Selected != oldValueOfradio0)
                Debug.WriteLine("Radio0 selected");

            ++i; //Debug.WriteLine("at ({0},{1})", 20, 20 + i * 20);
            var oldValueOfradio1 = radio1Selected;
            radio1Selected = gui.Radio(new Rect(20, firstY + i * 20, 100, 20), "RadioItem1", "G0", radio1Selected, "Radio1");
            if(radio1Selected && radio1Selected != oldValueOfradio1)
                Debug.WriteLine("Radio1 selected");

            ++i; //Debug.WriteLine("at ({0},{1})", 20, 20 + i * 20);
            var oldValueOfradio2 = radio2Selected;
            radio2Selected = gui.Radio(new Rect(20, firstY + i * 20, 100, 20), "RadioItem2", "G0", radio2Selected, "Radio2");
            if(radio2Selected && radio2Selected != oldValueOfradio2)
                Debug.WriteLine("Radio2 selected");
#endif

#if ShowImage
            gui.Image(new Rect(130, firstY, 240, 200), myImage, "MyImage");
#endif


#if ShowComboxBox
            var oldValueOfComboBox = selectedindex;
            selectedindex = gui.CombolBox(new Rect(new Point(20, 108), new Point(120, 128)), comboBoxItems, selectedindex,
                "MyCombo");
            if(selectedindex != oldValueOfComboBox)
                Debug.WriteLine("ComboBox item changed to {0}:{1}", selectedindex, comboBoxItems[selectedindex]);
#endif

        }
    }

}
