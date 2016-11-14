//#define INSPECT_STATE
using System.Diagnostics;
using Cairo;

namespace ImGui
{
    internal class SliderV : Control
    {
        #region State machine define
        static class SliderState
        {
            public const string Normal = "Normal";
            public const string Hover = "Hover";
            public const string Active = "Active";
            public const string ActiveOut = "ActiveOut";
        }

        static class SliderCommand
        {
            public const string MoveIn = "MoveIn";
            public const string MoveOut = "MoveOut";
            public const string MousePress = "MousePress";
            public const string MouseRelease = "MouseRelease";
        }

        static readonly string[] states =
        {
            SliderState.Normal, SliderCommand.MoveIn, SliderState.Hover,
            SliderState.Hover, SliderCommand.MoveOut, SliderState.Normal,
            SliderState.Hover, SliderCommand.MousePress, SliderState.Active,
            SliderState.Active, SliderCommand.MouseRelease, SliderState.Hover,
            SliderState.Active, SliderCommand.MoveOut, SliderState.ActiveOut,
            SliderState.ActiveOut, SliderCommand.MoveIn, SliderState.Active,
            SliderState.ActiveOut, SliderCommand.MouseRelease, SliderState.Normal,
        };
        #endregion

        private readonly StateMachine stateMachine;

        public float Value { get; private set; }
        public float MinValue { get; private set; }
        public float MaxValue { get; private set; }

        private Point upPoint
        {
            get { return new Point(Rect.X + Rect.Width / 2, Rect.Y + 10); }
        }

        private Point bottomPoint
        {
            get { return new Point(Rect.X + Rect.Width / 2, Rect.Bottom - 10); }
        }

        public SliderV(string name, Form form, Rect rect, float value, float upValue, float bottomValue)
            : base(name, form)
        {
            Rect = rect;
            Value = value;
            MinValue = upValue;
            MaxValue = bottomValue;
            stateMachine = new StateMachine(SliderState.Normal, states);
        }

        internal static float DoControl(Form form, Rect rect, float value, float upValue, float bottomValue, string name)
        {
            SliderV slider;
            if (!form.Controls.ContainsKey(name))
            {
                slider = new SliderV(name, form, rect, value, upValue, bottomValue);
            }
            else
            {
                slider = form.Controls[name] as SliderV;
            }
            Debug.Assert(slider != null);
            slider.Active = true;

            return slider.Value;
        }

        #region Overrides of Control

        public override void OnUpdate()
        {
#if INSPECT_STATE
            var A = stateMachine.CurrentState;
#endif
            //Execute state commands
            if (!Rect.Contains(Form.current.ScreenToClient(Input.Mouse.LastMousePos)) && Rect.Contains(Form.current.ScreenToClient(Input.Mouse.MousePos)))
            {
                stateMachine.MoveNext(SliderCommand.MoveIn);
            }
            if (Rect.Contains(Form.current.ScreenToClient(Input.Mouse.LastMousePos)) && !Rect.Contains(Form.current.ScreenToClient(Input.Mouse.MousePos)))
            {
                stateMachine.MoveNext(SliderCommand.MoveOut);
            }
            if (Input.Mouse.stateMachine.CurrentState == Input.Mouse.MouseState.Pressed)
            {
                if (stateMachine.MoveNext(SliderCommand.MousePress))
                {
                    Input.Mouse.stateMachine.MoveNext(Input.Mouse.MouseCommand.Fetch);
                }
            }
            if (Input.Mouse.stateMachine.CurrentState == Input.Mouse.MouseState.Released)
            {
                if (stateMachine.MoveNext(SliderCommand.MouseRelease))
                {
                    Input.Mouse.stateMachine.MoveNext(Input.Mouse.MouseCommand.Fetch);
                }
            }
#if INSPECT_STATE
            var B = stateMachine.CurrentState;
            Debug.WriteLineIf(A != B, string.Format("Button{0} {1}=>{2}", Name, A, B));
#endif

            var oldValue = Value;
            var oldState = State;
            bool active = stateMachine.CurrentState == SliderState.Active;
            bool hover = stateMachine.CurrentState == SliderState.Hover;
            var mousePos = Form.current.ScreenToClient(Input.Mouse.LastMousePos);
            if (active)
            {
                State = "Active";
                var minY = upPoint.Y;
                var maxY = bottomPoint.Y;
                var currentPointX = MathEx.Clamp(mousePos.Y, minY, maxY);
                Value = (float)(MinValue + (currentPointX - minY) / (maxY - minY) * (MaxValue - MinValue));
            }
            else if (hover)
            {
                State = "Hover";
            }
            else
            {
                State = "Normal";
            }
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (State != oldState || Value != oldValue)
            {
                //Debug.WriteLine("Repaint");
                NeedRepaint = true;
            }
        }

        public override void OnRender(Context g)
        {
            OnClear(g);
            var h = Rect.Width;
            var a = 0.2*h;
            var b = 0.3*h;

            var minY = upPoint.Y;
            var maxY = bottomPoint.Y;
            var currentPoint = upPoint + new Vector(0, (Value - MinValue) / (MaxValue - MinValue) * (maxY - minY));

            var leftArcCenter = currentPoint + new Vector(-b, 0);
            var rightArcCenter = currentPoint + new Vector(b, 0);
            var rightStartPoint = rightArcCenter + new Vector(0, -a);

            g.DrawLine(upPoint, currentPoint, 1.0f, (Color)Skin.current.Slider["Normal"].ExtraStyles["Line:Used"]);
            g.DrawLine(currentPoint, bottomPoint, 1.0f, (Color)Skin.current.Slider["Normal"].ExtraStyles["Line:Unused"]);

            g.NewPath();
            g.Arc(leftArcCenter.X, leftArcCenter.Y, a, System.Math.PI/2, System.Math.PI*3/2);
            g.LineTo(rightStartPoint.ToPointD());
            g.Arc(rightArcCenter.X, rightArcCenter.Y, a, System.Math.PI*3/2, System.Math.PI/2);
            g.ClosePath();
            g.SetSourceColor(CairoEx.ColorDarkBlue);
            g.Fill();

            this.RenderRects.Add(Rect);
        }

        public override void Dispose()
        {
        }

        public override void OnClear(Context g)
        {
            g.FillRectangle(Rect, CairoEx.ColorWhite);
            this.RenderRects.Add(Rect);
        }

        #endregion
    }
}
