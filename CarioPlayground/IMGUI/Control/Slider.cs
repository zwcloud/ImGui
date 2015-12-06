//#define INSPECT_STATE
using System.Diagnostics;
using Cairo;

//TODO complete slider
namespace IMGUI
{
    internal class Slider : Control
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

        private Point leftPoint
        {
            get { return new Point(Rect.X + 10, Rect.Y + Rect.Height/2); }
        }

        private Point rightPoint
        {
            get { return new Point(Rect.Right - 10, Rect.Y + Rect.Height/2); }
        }

        public Slider(string name, BaseForm form, Rect rect, float value, float leftValue, float rightValue)
            : base(name, form)
        {
            Rect = rect;
            Value = value;
            MinValue = leftValue;
            MaxValue = rightValue;
            stateMachine = new StateMachine(SliderState.Normal, states);
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
#if INSPECT_STATE
            var A = stateMachine.CurrentState;
#endif
            //Execute state commands
            if (!Rect.Contains(Utility.ScreenToClient(Input.Mouse.LastMousePos, Form)) && Rect.Contains(Utility.ScreenToClient(Input.Mouse.MousePos, Form)))
            {
                stateMachine.MoveNext(SliderCommand.MoveIn);
            }
            if (Rect.Contains(Utility.ScreenToClient(Input.Mouse.LastMousePos, Form)) && !Rect.Contains(Utility.ScreenToClient(Input.Mouse.MousePos, Form)))
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
            var mousePos = Utility.ScreenToClient(Input.Mouse.LastMousePos, Form);
            if (active)
            {
                State = "Active";
                var minX = leftPoint.X;
                var maxX = rightPoint.X;
                var currentPointX = MathEx.Clamp(mousePos.X, minX, maxX);
                Value = (float)(MinValue + (currentPointX - minX) / (maxX - minX) * (MaxValue - MinValue));
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
            var h = Rect.Height;
            var a = 0.2*h;
            var b = 0.3*h;

            var minX = leftPoint.X;
            var maxX = rightPoint.X;
            var currentPoint = leftPoint + new Vector((Value - MinValue)/(MaxValue - MinValue)*(maxX - minX), 0);

            var topArcCenter = currentPoint + new Vector(0, b);
            var bottomArcCenter = currentPoint + new Vector(0, -b);
            var bottomStartPoint = bottomArcCenter + new Vector(-a, 0);

            g.DrawLine(leftPoint, currentPoint, 1.0f, (Color)Skin.current.Slider["Normal"].ExtraStyles["Line:Used"]);
            g.DrawLine(currentPoint, rightPoint, 1.0f, (Color)Skin.current.Slider["Normal"].ExtraStyles["Line:Unused"]);

            g.NewPath();
            g.Arc(topArcCenter.X, topArcCenter.Y, a, 0, System.Math.PI);
            g.LineTo(bottomStartPoint.ToPointD());
            g.Arc(bottomArcCenter.X, bottomArcCenter.Y, a, System.Math.PI, System.Math.PI * 2);
            g.ClosePath();
            g.SetSourceColor(CairoEx.ColorDarkBlue);
            g.Fill();
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
