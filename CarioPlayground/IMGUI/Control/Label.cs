namespace ImGui
{
    internal class Label : SimpleControl
    {
        private string text;
        private readonly string name;
        private Rect rect;
        private Style style;
        private Content content;
        private bool textChanged;

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

        public void Update()
        {
            var oldState = State;
            bool active = Input.Mouse.LeftButtonState == InputState.Down && Rect.Contains(Input.Mouse.GetMousePos(Form));
            bool hover = Input.Mouse.LeftButtonState == InputState.Up && Rect.Contains(Input.Mouse.GetMousePos(Form));
            if(active)
                State = "Active";
            else if(hover)
                State = "Hover";
            else
                State = "Normal";
            if(State != oldState)
            {
                NeedRepaint = true;
            }
        }

        internal Label(string name, Form form, string text, Rect rect)
        {
            this.name = name;
            this.NeedRepaint = true;
            this.State = "Normal";
            this.Form = form;
            this.rect = rect;
            this.style = Skin.current.Label[State];
            var textContext = Content.BuildTextContext(text, rect, style);
            this.content = new Content(textContext);
            form.SimpleControls[name] = this;
        }

        internal static void DoControl(Form form, Rect rect, string text, string name)
        {
            if (!form.Controls.ContainsKey(name))
            {
                var label = new Label(name, form, text, rect);
                label.Update();
            }

            var control = form.SimpleControls[name] as Label;
            System.Diagnostics.Debug.Assert(control != null);

            //Update text
            control.Text = text;
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
                if (textChanged)
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
                this.style = Skin.current.Label[State];
                return this.style;
            }
        }

        #endregion
    }
}