//#define INSPECT_STATE
using System.Diagnostics;

namespace ImGui
{
    /// <summary>
    /// Button
    /// </summary>
    /// <remarks>
    /// The button is a simple control, which only contains a text as its content.
    /// It handles the click event to respond when the user clicks a Button.
    /// </remarks>
    internal class Button : SimpleControl
    {
        #region State machine define
        static class ButtonState
        {
            public const string Normal = "Normal";
            public const string Hover = "Hover";
            public const string Active = "Active";
        }

        static class ButtonCommand
        {
            public const string MoveIn = "MoveIn";
            public const string MoveOut = "MoveOut";
            public const string MousePress = "MousePress";
            public const string MouseRelease = "MouseRelease";
        }

        static readonly string[] states =
        {
            ButtonState.Normal, ButtonCommand.MoveIn, ButtonState.Hover,
            ButtonState.Hover, ButtonCommand.MoveOut, ButtonState.Normal,
            ButtonState.Hover, ButtonCommand.MousePress, ButtonState.Active,
            ButtonState.Active, ButtonCommand.MoveOut, ButtonState.Normal,
            ButtonState.Active, ButtonCommand.MouseRelease, ButtonState.Hover
        };
        #endregion

        private readonly StateMachine stateMachine;
        private readonly string name;

        private Rect rect;
        private Content content;
        private Style style;

        public string Text
        {
            get { return Content.Text; }
            private set
            {
                if (Content.Text == value)
                {
                    return;
                }

                Content.Text = value;
                Content.Build(Skin.current.Button[State]);
                NeedRepaint = true;
            }
        }
        public bool Result { get; private set; }

        public void Update()
        {
#if INSPECT_STATE
            var A = stateMachine.CurrentState;
#endif
            //Execute state commands
            if (!Rect.Contains(Utility.ScreenToClient(Input.Mouse.LastMousePos, Form)) && Rect.Contains(Utility.ScreenToClient(Input.Mouse.MousePos, Form)))
            {
                stateMachine.MoveNext(ButtonCommand.MoveIn);
            }
            if (Rect.Contains(Utility.ScreenToClient(Input.Mouse.LastMousePos, Form)) && !Rect.Contains(Utility.ScreenToClient(Input.Mouse.MousePos, Form)))
            {
                stateMachine.MoveNext(ButtonCommand.MoveOut);
            }
            if (Input.Mouse.stateMachine.CurrentState == Input.Mouse.MouseState.Pressed)
            {
                if(stateMachine.MoveNext(ButtonCommand.MousePress))
                {
                    Input.Mouse.stateMachine.MoveNext(Input.Mouse.MouseCommand.Fetch);
                }
            }
            if (Input.Mouse.stateMachine.CurrentState == Input.Mouse.MouseState.Released)
            {
                if(stateMachine.MoveNext(ButtonCommand.MouseRelease))
                {
                    Input.Mouse.stateMachine.MoveNext(Input.Mouse.MouseCommand.Fetch);
                }
            }
#if INSPECT_STATE
            var B = stateMachine.CurrentState;
            Debug.WriteLineIf(A != B, string.Format("Button{0} {1}=>{2}", Name, A, B));
#endif

            var oldState = State;
            bool active = stateMachine.CurrentState == ButtonState.Active;
            bool hover = stateMachine.CurrentState == ButtonState.Hover;
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

            if (State != oldState)
            {
                NeedRepaint = true;
                Event.current.type = EventType.Repaint;
            }
            bool clicked = oldState == "Active" && State == "Hover";
            Result = clicked;
        }

        public Button(string name, Form form, Content content)
        {
            this.name = name;
            this.stateMachine = new StateMachine(ButtonState.Normal, states);
            this.State = ButtonState.Normal;
            this.NeedRepaint = true;
            this.Form = form;

            this.style = Skin.current.Button[State];
            this.content = content;

            form.SimpleControls[name] = this;
        }

        public static bool DoControl(Rect rect, Content content, string name)
        {
            var form = Form.current;
            //Create
            if (!form.SimpleControls.ContainsKey(name))
            {
                var button = new Button(name, form, content);
            }

            //Update
            var control = form.SimpleControls[name] as Button;
            Debug.Assert(control != null);
            if (Event.current.type == EventType.Repaint)
            {
                control.Rect = rect;
                control.content = content;
            }
            control.Update();

            //Active
            form.renderBoxMap[name]= control;

            //Result
            return control.Result;
        }

        public override string Name
        {
            get { return name; }
        }

        #region Overrides of SimpleControl

        public override Rect Rect
        {
            get { return rect; }
            set
            {
                //TODO need re-layout
                rect = value;
            }
        }

        public override Content Content
        {
            get
            {
                return content;
            }
        }

        public override Style Style
        {
            get
            {
                this.style = Skin.current.Button[State];
                return this.style;
            }
        }

        #endregion
    }
}