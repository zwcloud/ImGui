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

        public Rect Rect { get; private set; }
        public int Result { get; private set; }

        internal Slider(string name, BaseForm form, Rect rect, int value, int leftValue, int rightValue)
            : base(name, form)
        {
            Rect = rect;
            Value = value;
        }

        //TODO Control-less DoControl overload (without name parameter)
        internal static int DoControl(Context g, BaseForm form, Rect rect, int value, int leftValue, int rightValue, string name)
        {
            Slider slider;
            if (!form.Controls.ContainsKey(name))
            {
                slider = new Slider(name, form, rect, value, leftValue, rightValue);
            }
            else
            {
                slider = form.Controls[name] as Slider;
            }
            Debug.Assert(slider != null);

            return slider.Value;
        }

        #region Overrides of Control

        public override void OnUpdate()
        {
            bool active = Input.Mouse.LeftButtonState == InputState.Down && Rect.Contains(Input.Mouse.GetMousePos(Form));
            bool hover = Input.Mouse.LeftButtonState == InputState.Up && Rect.Contains(Input.Mouse.GetMousePos(Form));
            if (active)
                State = "Active";
            else if (hover)
                State = "Hover";
            else
                State = "Normal";
        }

        public override void OnRender(Context g)
        {
            //TODO Consider use SVG to draw readonly shape
            if (State == "Normal")
            {
                g.DrawLine(new Point(Rect.X, Rect.Y + Rect.Height/2), new Point(Rect.Right, Rect.Y + Rect.Height/2),
                    1.0f, Skin.current.Slider["Line:Normal"].LineColor);
            }
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
