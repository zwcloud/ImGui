using System.Collections.Generic;
using System.Diagnostics;
using Cairo;

namespace ImGui
{
    internal class Radio : Control
    {
        #region State machine define
        static class RadioState
        {
            public const string Normal = "Normal";
            public const string Hover = "Hover";
            public const string Active = "Active";
        }

        static class RadioCommand
        {
            public const string MoveIn = "MoveIn";
            public const string MoveOut = "MoveOut";
            public const string MousePress = "MousePress";
            public const string MouseRelease = "MouseRelease";
        }

        static readonly string[] states =
        {
            RadioState.Normal, RadioCommand.MoveIn, RadioState.Hover,
            RadioState.Hover, RadioCommand.MoveOut, RadioState.Normal,
            RadioState.Hover, RadioCommand.MousePress, RadioState.Active,
            RadioState.Active, RadioCommand.MoveOut, RadioState.Normal,
            RadioState.Active, RadioCommand.MouseRelease, RadioState.Hover
        };
        #endregion

        public static Dictionary<string, HashSet<string>> Groups;

        static Radio()
        {
            Groups = new Dictionary<string, HashSet<string>>();
        }

        private readonly StateMachine stateMachine;
        private string text;

        public ITextContext TextContext { get; private set; }
        public string Text
        {
            get { return text; }
            private set
            {
                if (Text == value)
                {
                    return;
                }

                text = value;
                NeedRepaint = true;
            }
        }
        protected string groupName;
        protected bool actived;

        public string GroupName
        {
            get { return groupName; }
            private set
            {
                if(groupName!=null)
                {
                    if(value == groupName) return;
#if DEBUG
                    Debug.Assert(!Groups.ContainsKey(groupName),
                        "Older group is not recorded.");
                    var removed = Groups[groupName].Remove(Name);
                    Debug.Assert(removed, "This radio is not in the Group <{0}>.", groupName);
#else
                    Groups[_groupName].Remove(Name);
#endif
                }
                if(!Groups.ContainsKey(value))
                    Groups[value] = new HashSet<string>();
                Groups[value].Add(Name);
                groupName = value;
            }
        }

        public bool Actived
        {
            get { return actived; }
            set
            {
                if(value == actived)
                {
                    return;
                }

                actived = value;
                NeedRepaint = true;
            }
        }


        public Radio(string name, Form form, string text, Rect rect, string groupName)
            : base(name, form)
        {
            Rect = rect;
            Text = text;
            GroupName = groupName;
            stateMachine = new StateMachine(RadioState.Normal, states);

            var style = Skin.current.Button[State];
            var font = style.Font;
            var textStyle = style.TextStyle;
            var contentRect = Utility.GetContentRect(Rect, style);

            TextContext = Application._map.CreateTextContext(
                Text, font.FontFamily, font.Size,
                font.FontStretch, font.FontStyle, font.FontWeight,
                (int)contentRect.Width, (int)contentRect.Height,
                textStyle.TextAlignment);
        }

        public static bool DoControl(Form form, Rect rect, string text, string groupName, bool value, string name)
        {
            if (!form.Controls.ContainsKey(name))
            {
                var radio = new Radio(name, form, text, rect, groupName);
            }

            var control = form.Controls[name] as Radio;
            Debug.Assert(control != null);
            control.Active = true;

            return control.Actived;
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
                stateMachine.MoveNext(RadioCommand.MoveIn);
            }
            if (Rect.Contains(Utility.ScreenToClient(Input.Mouse.LastMousePos, Form)) && !Rect.Contains(Utility.ScreenToClient(Input.Mouse.MousePos, Form)))
            {
                stateMachine.MoveNext(RadioCommand.MoveOut);
            }
            if (Input.Mouse.stateMachine.CurrentState == Input.Mouse.MouseState.Pressed)
            {
                if (stateMachine.MoveNext(RadioCommand.MousePress))
                {
                    Input.Mouse.stateMachine.MoveNext(Input.Mouse.MouseCommand.Fetch);
                }
            }
            if (Input.Mouse.stateMachine.CurrentState == Input.Mouse.MouseState.Released)
            {
                if (stateMachine.MoveNext(RadioCommand.MouseRelease))
                {
                    Input.Mouse.stateMachine.MoveNext(Input.Mouse.MouseCommand.Fetch);
                }
            }
#if INSPECT_STATE
            var B = stateMachine.CurrentState;
            Debug.WriteLineIf(A != B, string.Format("Button{0} {1}=>{2}", Name, A, B));
#endif

            var oldState = State;
            bool active = stateMachine.CurrentState == RadioState.Active;
            bool hover = stateMachine.CurrentState == RadioState.Hover;
            if (active)
            {
                State = "Active";
            }
            else if (hover)
            {
                State = "Hover";
            }
            else
            {
                State = "Normal";
            }
            if (oldState == "Active" && State == "Hover")
            {
                var group = Groups[groupName];
                foreach (var radioName in group)
                {
                    var radio = Form.Controls[radioName];
                    ((Radio)radio).Actived = false;
                }
                Actived = true;
            }
            if (State != oldState)
            {
                NeedRepaint = true;
            }
        }

        public override void OnRender(Context g)
        {
            var style = Skin.current.Radio[State];
            var radioBoxRect = new Rect(Rect.X, Rect.Y, new Size(Rect.Height, Rect.Height));
            var radioBoxCenter = radioBoxRect.Center;
            var tmp = (float) (radioBoxRect.Width - 1)/12;
            var pointRadius = tmp*3;
            var circleRadius = tmp*5;
            g.FillRectangle(radioBoxRect, style.BackgroundStyle.Color);
            if (Actived)
            {
                g.FillCircle(radioBoxCenter, pointRadius, CairoEx.ColorBlack);
                g.StrokeCircle(radioBoxCenter, circleRadius,
                    (Color)style.ExtraStyles["CircleColor.Selected"]);
            }
            else
            {
                g.StrokeCircle(radioBoxCenter, circleRadius, CairoEx.ColorBlack);
            }
            var radioTextRect = new Rect(radioBoxRect.TopRight, Rect.BottomRight);
            g.DrawBoxModel(radioTextRect, new Content(TextContext), Skin.current.Radio[State]);
            this.RenderRects.Add(Rect);
        }

        public override void Dispose()
        {
            TextContext.Dispose();
        }
        
        public override void OnClear(Context g)
        {
            g.FillRectangle(Rect, CairoEx.ColorWhite); ;
            this.RenderRects.Add(Rect);
        }

        #endregion
    }
}