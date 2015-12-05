using System;
using System.Diagnostics;
using Cairo;
using TinyIoC;

namespace IMGUI
{
    internal class Toggle : Control
    {
        private string text;
        private StateMachine stateMachine;

        internal Toggle(string name, BaseForm form, bool value, string displayText, Rect rect)
            : base(name, form)
        {
            Rect = rect;
            Text = displayText;
            Result = value;

            stateMachine = new StateMachine(ToggleState.Normal, states);

            var font = Skin.current.Toggle[State].Font;
            Format = Application.IocContainer.Resolve<ITextFormat>(
                new NamedParameterOverloads
                {
                    {"fontFamilyName", font.FontFamily},
                    {"fontWeight", font.FontWeight},
                    {"fontStyle", font.FontStyle},
                    {"fontStretch", font.FontStretch},
                    {"fontSize", (float) font.Size}
                });
            var textStyle = Skin.current.Toggle[State].TextStyle;
            Format.Alignment = textStyle.TextAlignment;
            Layout = Application.IocContainer.Resolve<ITextLayout>(
                new NamedParameterOverloads
                {
                    {"text", Text},
                    {"textFormat", Format},
                    {"maxWidth", (int) Rect.Width},
                    {"maxHeight", (int) Rect.Height}
                });
        }

        public ITextFormat Format { get; private set; }
        public ITextLayout Layout { get; private set; }

        public string Text
        {
            get { return text; }
            private set
            {
                if(Text == value)
                {
                    return;
                }

                text = value;
                NeedRepaint = true;
            }
        }

        public bool Result { get; private set; }

        internal static bool DoControl(BaseForm form, Rect rect, string displayText, bool value, string name)
        {
            if(!form.Controls.ContainsKey(name))
            {
                var toggle = new Toggle(name, form, value, displayText, rect);
                Debug.Assert(toggle != null);
            }

            var control = form.Controls[name] as Toggle;
            Debug.Assert(control != null);
            control.Active = true;

            return control.Result;
        }

        #region State machine define

        private static class ToggleState
        {
            public const string Normal = "Normal";
            public const string Hover = "Hover";
            public const string Active = "Active";
        }

        private static class ToggleCommand
        {
            public const string MoveIn = "MoveIn";
            public const string MoveOut = "MoveOut";
            public const string MousePress = "MousePress";
            public const string MouseRelease = "MouseRelease";
        }

        private static readonly string[] states =
        {
            ToggleState.Normal, ToggleCommand.MoveIn, ToggleState.Hover,
            ToggleState.Hover, ToggleCommand.MoveOut, ToggleState.Normal,
            ToggleState.Hover, ToggleCommand.MousePress, ToggleState.Active,
            ToggleState.Active, ToggleCommand.MouseRelease, ToggleState.Hover
        };

        #endregion

        #region Overrides of Control

        public override void OnUpdate()
        {
            //Execute state commands
            if (!Rect.Contains(Utility.ScreenToClient(Input.Mouse.LastMousePos, Form)) && Rect.Contains(Utility.ScreenToClient(Input.Mouse.MousePos, Form)))
            {
                stateMachine.MoveNext(ToggleCommand.MoveIn);
            }
            if (Rect.Contains(Utility.ScreenToClient(Input.Mouse.LastMousePos, Form)) && !Rect.Contains(Utility.ScreenToClient(Input.Mouse.MousePos, Form)))
            {
                stateMachine.MoveNext(ToggleCommand.MoveOut);
            }
            if (Input.Mouse.stateMachine.CurrentState == Input.Mouse.MouseState.Pressed)
            {
                if(stateMachine.MoveNext(ToggleCommand.MousePress))
                {
                    Input.Mouse.stateMachine.MoveNext(Input.Mouse.MouseCommand.Fetch);
                }
            }
            if (Input.Mouse.stateMachine.CurrentState == Input.Mouse.MouseState.Released)
            {
                if(stateMachine.MoveNext(ToggleCommand.MouseRelease))
                {
                    Input.Mouse.stateMachine.MoveNext(Input.Mouse.MouseCommand.Fetch);
                }
            }

            var oldState = State;
            var active = stateMachine.CurrentState == ToggleState.Active;
            var hover = stateMachine.CurrentState == ToggleState.Hover;
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
            if(oldState != State)
            {
                NeedRepaint = true;
            }

            if (oldState == "Hover" && State == "Active")
                Result = !Result;
        }

        public override void OnRender(Context g)
        {
            var style = Skin.current.Toggle[State];
            var toggleBoxRect = new Rect(Rect.X, Rect.Y, new Size(Rect.Size.Height, Rect.Size.Height));
            if(Result)
            {
                g.FillRectangle(toggleBoxRect, (Color)style.ExtraStyles["FillColor"]);
                var d = toggleBoxRect.Height;
                Point[] tickPoints = new[]
                {
                    new Point(0.125f*d, 0.5f*d),
                    new Point(0.333f*d,0.75f*d),
                    new Point(0.875f*d,0.25f*d),
                };
                tickPoints[0].Offset(toggleBoxRect.X, toggleBoxRect.Y);
                tickPoints[1].Offset(toggleBoxRect.X, toggleBoxRect.Y);
                tickPoints[2].Offset(toggleBoxRect.X, toggleBoxRect.Y);
                g.Save();
                g.LineWidth = 2;
                g.StrokeLineStrip(tickPoints, (Color)style.ExtraStyles["TickColor"]);
                g.Restore();
            }
            else
            {
                g.FillRectangle(toggleBoxRect, CairoEx.ColorWhite);
                g.StrokeRectangle(toggleBoxRect, CairoEx.ColorBlack);
            }

            var toggleTextRect = new Rect(toggleBoxRect.TopRight, Rect.BottomRight);
            g.DrawBoxModel(toggleTextRect, new Content(Layout), style);
        }

        public override void Dispose()
        {
            Layout.Dispose();
            Format.Dispose();
        }

        public override void OnClear(Context g)
        {
            g.FillRectangle(Rect, CairoEx.ColorWhite);
        }

        #endregion
    }
}