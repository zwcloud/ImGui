using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cairo;

//TODO complete slider
namespace IMGUI
{
    internal class Slider : Control
    {
        internal int Value { get; set; }

        internal Slider(string name)
        {
            Name = name;
            State = "Normal";

            Controls[Name] = this;
        }

        //TODO Control-less DoControl overload (without name parameter)
        internal static int DoControl(Context g, Rect rect, int value, int leftValue, int rightValue, string name)
        {
            #region Get control reference
            Slider slider;
            if(!Controls.ContainsKey(name))
            {
                slider = new Slider(name);
                slider.Value = value;
            }
            else
            {
                slider = Controls[name] as Slider;
            }

            Debug.Assert(slider != null);
            #endregion

            #region Set control data
            {
            }
            #endregion

            #region Logic
            bool active = Input.Mouse.LeftButtonState == InputState.Down && rect.Contains(Input.Mouse.MousePos);
            bool hover = Input.Mouse.LeftButtonState == InputState.Up && rect.Contains(Input.Mouse.MousePos);
            if(active)
                slider.State = "Active";
            else if(hover)
                slider.State = "Hover";
            else
                slider.State = "Normal";
            #endregion

            //TODO non-standrad style
            //TODO Consider use SVG to draw readonly shape
            #if  f
            if(slider.State == "Normal")
            {
                g.DrawLine(new Point(rect.X, rect.Y + rect.Height / 2), new Point(rect.Right, rect.Y + rect.Height / 2), 1.0f, Skin.current.Slider["Line:Normal"].color);
            }
            g.DrawBoxModel(rect, new Content(text), Skin.current.Slider[slider.State]);

            slider.Value = 0;
            return clicked;
            #endif
            return 1;
        }

        #region Overrides of Control

        public override void OnUpdate()
        {
            throw new NotImplementedException();
        }

        public override void OnRender(Context g)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
