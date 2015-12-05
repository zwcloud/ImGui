using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cairo;

//TODO complete slider
namespace IMGUI
{
    internal class Slider : Control
    {
        public float Value { get; private set; }
        public float MinValue { get; private set; }
        public float MaxValue { get; private set; }

        public Slider(string name, BaseForm form, Rect rect, float value, float leftValue, float rightValue)
            : base(name, form)
        {
            Rect = rect;
            Value = value;
            MinValue = leftValue;
            MaxValue = rightValue;
        }

        internal static float DoControl(BaseForm form, Rect rect, float value, float leftValue, float rightValue, string name)
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
            slider.Active = true;

            return slider.Value;
        }

        #region Overrides of Control

        public override void OnUpdate()
        {
            bool active = Input.Mouse.LeftButtonState == InputState.Down && Rect.Contains(Input.Mouse.GetMousePos(Form));
            bool hover = Input.Mouse.LeftButtonState == InputState.Up && Rect.Contains(Input.Mouse.GetMousePos(Form));
            if(active)
            {
                State = "Active";
            }
            else if(hover)
            {
                State = "Hover";
            }
            else
            {
                State = "Normal";
            }
        }

        public override void OnRender(Context g)
        {
            var leftPoint = new Point(Rect.X, Rect.Y + Rect.Height/2);
            var rightPoint = new Point(Rect.Right, Rect.Y + Rect.Height/2);
            var currentPoint = leftPoint + new Vector((Value - MinValue)/(MaxValue - MinValue)*Rect.Width, 0);
            if (State == "Normal")
            {
                g.FillCircle(currentPoint, 5.0f, CairoEx.ColorDarkBlue);
                g.DrawLine(new Point(Rect.X, Rect.Y + Rect.Height/2), new Point(Rect.Right, Rect.Y + Rect.Height/2),
                    1.0f, Skin.current.Slider["Line:Normal"].LineColor);
            }
        }

        public override void Dispose()
        {
        }

        public override void OnClear(Context g)
        {
            g.FillRectangle(Rect, CairoEx.ColorWhite);
        }

        #endregion
    }
}
