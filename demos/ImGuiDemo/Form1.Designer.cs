#define ShowButton
//#define ShowToggle
//#define ShowRadio
//#define ShowSlider
//#define ShowImage
//#define ShowComboxBox

using System.Diagnostics;
using ImGui;

namespace Calculator
{
    partial class Form1
    {
        #region paramters

        private bool hoverButton0Actived = false;

        private bool _opened1, _opened2;

        private readonly string[] comboBoxItems = new[] { "item0", "item1", "item2", "item3" };
        
        //private Texture myImage = new Texture("gnu_hornedword.png");

        private bool radio0Selected = false;
        private bool radio1Selected = false;
        private bool radio2Selected = false;

        private int selectedindex1, selectedindex2;
        private float valueOfSlider0, valueOfSlider1;

        #endregion

        protected override void OnGUI()
        {
#if ShowButton
            if (GUILayout.Button("a button", "Button0"))
            {
                Debug.WriteLine("Button0 clicked!");
            }

            if (GUILayout.Button("another button ", "Button1"))
            {
                Debug.WriteLine("Button1 clicked!");
            }

            //Y += 20; //Debug.WriteLine("at ({0},{1})", 20, offsetY);
            //{
            //    var oldValue = hoverButton0Actived;
            //    hoverButton0Actived = GUI.HoverButton(new Rect(20, Y, 100, 20), "a hover-button", "HoverButton0");
            //    if (hoverButton0Actived ^ oldValue)
            //    {
            //        Debug.WriteLine("HoverButton0 becomes {0}", hoverButton0Actived ? "active" : "deactive", null);
            //    }
            //}
#endif

#if ShowToggle
            i += 20; //Debug.WriteLine("at ({0},{1})", 20, firstY + i);
            var oldValueOfTaggle1 = _opened1;
            _opened1 = gui.Toggle(new Rect(20, firstY + i, 100, 20), "Opened?", _opened1, "Toggle0");
            if(_opened1 ^ oldValueOfTaggle1)
            {
                Debug.WriteLine("Toggle 0 {0}", _opened1 ? "on!" : "off!", null);
            }
            if(_opened1)
            {

#if ShowImage
                gui.Image(new Rect(130, firstY, 240, 200), myImage, "MyImage");
#endif
            }
            i += 20; //Debug.WriteLine("at ({0},{1})", 20, firstY + i);
            var oldValueOfTaggle2 = _opened2;
            _opened2 = gui.ToggleButton(new Rect(20, firstY + i, 100, 20), "Opened?", _opened1, "ToggleButton0");
            if (_opened2 ^ oldValueOfTaggle2)
                Debug.WriteLine("ToggleButton 0 {0}", _opened2 ? "on!" : "off!", null);
#endif

#if ShowRadio
            i += 20; //Debug.WriteLine("at ({0},{1})", 20, firstY + i);
            var oldValueOfradio0 = radio0Selected;
            radio0Selected = gui.Radio(new Rect(20, firstY + i, 100, 20), "RadioItem0", "G0", radio0Selected, "Radio0");
            if(radio0Selected && radio0Selected != oldValueOfradio0)
                Debug.WriteLine("Radio0 selected");

            i += 20; //Debug.WriteLine("at ({0},{1})", 20, firstY + i);
            var oldValueOfradio1 = radio1Selected;
            radio1Selected = gui.Radio(new Rect(20, firstY + i, 100, 20), "RadioItem1", "G0", radio1Selected, "Radio1");
            if(radio1Selected && radio1Selected != oldValueOfradio1)
                Debug.WriteLine("Radio1 selected");

            i += 20; //Debug.WriteLine("at ({0},{1})", 20, firstY + i);
            var oldValueOfradio2 = radio2Selected;
            radio2Selected = gui.Radio(new Rect(20, firstY + i, 100, 20), "RadioItem2", "G0", radio2Selected, "Radio2");
            if(radio2Selected && radio2Selected != oldValueOfradio2)
                Debug.WriteLine("Radio2 selected");
#endif

#if ShowSlider
            i += 20; //Debug.WriteLine("at ({0},{1})", 20, firstY + i);
            var oldValueOfSlider0 = valueOfSlider0;
            valueOfSlider0 = gui.Slider(new Rect(20, firstY + i, 100, 20), "Slider0", 0f, -100f, 100f, "Slider0");
            if (valueOfSlider0 != oldValueOfSlider0)
                Debug.WriteLine("Value of slider0 changed to {0}", valueOfSlider0);

            i += 20; //Debug.WriteLine("at ({0},{1})", 20, firstY + i);
            var oldValueOfSlider1 = valueOfSlider1;
            valueOfSlider1 = gui.SliderV(new Rect(20, firstY + i, 20, 100), "Slider1", 0f, -100f, 100f, "Slider1");
            if (valueOfSlider1 != oldValueOfSlider1)
                Debug.WriteLine("Value of slider1 changed to {0}", valueOfSlider1);
#endif

#if ShowComboxBox
            i += 100; //Debug.WriteLine("at ({0},{1})", 20, firstY + i);
            var oldValueOfComboBox1 = selectedindex1;
            selectedindex1 = gui.CombolBox(new Rect(20, firstY + i, 100, 20), comboBoxItems, selectedindex1,
                "Combo1");
            if(selectedindex1 != oldValueOfComboBox1)
                Debug.WriteLine("ComboBox item changed to {0}:{1}", selectedindex1, comboBoxItems[selectedindex1]);

            i += 20; //Debug.WriteLine("at ({0},{1})", 20, firstY + i);
            var oldValueOfComboBox2 = selectedindex2;
            selectedindex2 = gui.CombolBox(new Rect(20, firstY + i, 100, 20), comboBoxItems, selectedindex2,
                "Combo2");
            if (selectedindex2 != oldValueOfComboBox2)
                Debug.WriteLine("ComboBox item changed to {0}:{1}", selectedindex2, comboBoxItems[selectedindex2]);
#endif
        }
    }
}
