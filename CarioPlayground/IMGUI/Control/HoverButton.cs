//#define INSPECT_STATE
using Cairo;
using System.Diagnostics;

namespace ImGui
{
    class HoverButton : SimpleControl
    {
        #region State machine define
        static class ButtonState
        {
            public const string Normal = "Normal";
            public const string Active = "Active";
        }

        static class ButtonCommand
        {
            public const string MoveIn = "MoveIn";
            public const string MoveOut = "MoveOut";
        }

        static readonly string[] states =
        {
            ButtonState.Normal, ButtonCommand.MoveIn, ButtonState.Active,
            ButtonState.Active, ButtonCommand.MoveOut, ButtonState.Normal,
        };
        #endregion

        private readonly StateMachine stateMachine;
        private readonly string name;
        private string text;
        private Rect rect;
        private Content content;
        private Style style;

        private bool textChanged = false;

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
                textChanged = true;
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
#if INSPECT_STATE
            var B = stateMachine.CurrentState;
            Debug.WriteLineIf(A != B, string.Format("HoverButton{0} {1}=>{2}", Name, A, B));
#endif

            var oldState = State;
            bool active = stateMachine.CurrentState == ButtonState.Active;
            if (active)
            {
                State = "Active";
            }
            else
            {
                State = "Normal";
            }

            if (State != oldState)
            {
                NeedRepaint = true;
            }
            Result = active;
        }

        public HoverButton(string name, Form form, string text, Rect rect)
        {
            this.name = name;
            this.stateMachine = new StateMachine(ButtonState.Normal, states);
            this.State = ButtonState.Normal;
            this.NeedRepaint = true;
            this.Form = form;
            this.rect = rect;
            this.style = Skin.current.Button[State];
            var textContext = Content.BuildTextContext(text, rect, style);
            this.content = new Content(textContext);
            form.SimpleControls[name] = this;
        }

        public static bool DoControl(Form form, Rect rect, string text, string name)
        {
            //Create
            if (!form.SimpleControls.ContainsKey(name))
            {
                var hoverButton = new HoverButton(name, form, text, rect);
            }

            //Update
            var control = form.SimpleControls[name] as HoverButton;
            Debug.Assert(control != null);
            control.Text = text;
            control.Update();

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
            set { rect = value; }
        }

        public override Content Content
        {
            get
            {
                if(textChanged)
                {
                    textChanged = false;
                    content.TextContext.Text = text;
                }
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