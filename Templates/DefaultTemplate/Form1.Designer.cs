using System.Collections.Generic;
using System.Diagnostics;
using ImGui;

namespace DefaultTemplate
{
    partial class Form1
    {
        private bool toggleValue = false;
        
        private Radio radio = new Radio(new[]{"RadioItem0##A", "RadioItem1##B", "RadioItem2##C", "RadioItem4##D"});
        private int activeRadioIndex = -1;

        private double sliderValue = 10;
        private double vSliderValue = 10;

        private bool toggleValue1 = false;

        private Point[] points =
        {
            new Point(0, 20),
            new Point(10, 30),
            new Point(20, 30),
            new Point(30, 20),
            new Point(30, 10),
            new Point(20, 0),
            new Point(10, 0),
            new Point(0, 10),
            new Point(0, 20),
        };

        private ITexture image0;

        protected void InitializeComponent()
        {
            image0 = GUI.CreateTexture("D:\\1.png");
        }
        
        public override void Dispose()
        {
            image0.Dispose();
        }

        protected override void OnGUI()
        {
            //if (GUILayout.Button("Test Button"))
            //{
            //    Debug.WriteLine("Test Button clicked.");
            //}
            //GUILayout.HoverButton("A hover button");
            //toggleValue = GUILayout.Toggle("Toggle0", toggleValue);
            //activeRadioIndex = radio.DoGUI();
            //sliderValue = GUILayout.Slider(new Size(200, 30), sliderValue, 0, 30, "Horizontal Slider");
            //vSliderValue = GUILayout.VSlider(new Size(30, 200), vSliderValue, 0, 30, "Vertical Slider");
            //toggleValue1 = GUILayout.ToggleButton("Toggle button", toggleValue1);
            //GUILayout.PolygonButton(points,
            //    new Rect(
            //        new Point((points[0].X + points[1].X)/2, (points[0].Y + points[1].Y)/2),
            //        new Point((points[4].X + points[5].X)/2, (points[4].Y + points[5].Y)/2)
            //    ),
            //    "P##PolygonButton0");

            GUILayout.Image("D:\\1.png");
            GUILayout.Image("D:\\hello.png");
            GUILayout.Image("D:\\by.png");
        }

    }
}
