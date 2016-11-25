using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ImGui;

namespace DefaultTemplate
{
    partial class Form1
    {
        private bool toggleValue = false;
        
        private Radio radio = new Radio(new[]{"RadioItem0##A", "RadioItem1##B", "RadioItem2##C", "RadioItem4##D"});
        private int activeRadioIndex = -1;

        private double sliderValue = 10;

        protected override void OnGUI()
        {
            if (GUILayout.Button("Test Button"))
            {
                Debug.WriteLine("Test Button clicked.");
            }
            GUILayout.HoverButton("A hover button");
            toggleValue = GUILayout.Toggle("Toggle0", toggleValue);
            activeRadioIndex = radio.DoGUI();
            sliderValue = GUILayout.Slider(new Size(200, 30), sliderValue, 0, 30, "SliderA");
        }

    }
}
