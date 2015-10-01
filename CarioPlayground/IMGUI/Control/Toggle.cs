using System.Diagnostics;
using Cairo;
using TinyIoC;

namespace IMGUI
{
    internal class Toggle : Control
    {
        public ITextFormat Format { get; private set; }
        public ITextLayout Layout { get; private set; }

        private string text;
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
        public Rect Rect { get; private set; }
        public bool Result { get; private set; }

        internal Toggle(string name, string displayText, int width, int height)
        {
            Name = name;
            State = "Normal";
            Controls[Name] = this;

            Text = displayText;

            var font = Skin.current.Button[State].Font;
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
                        {"maxWidth", width},
                        {"maxHeight", height}
                    });
        }

        internal static bool DoControl(Context g, Rect rect, string displayText, bool value, string name)
        {
            if(!Controls.ContainsKey(name))
            {
                var toggle = new Toggle(name, displayText, (int)rect.Width, (int)rect.Height);
                Debug.Assert(toggle != null);
                toggle.Result = value;
            }

            var control = Controls[name] as Toggle;
            Debug.Assert(control != null);
            control.Rect = rect;
            control.Text = displayText;
            control.Layout.Text = control.Text;

            //The control need to be relayout
            //TODO

            return control.Result;
        }

        #region Overrides of Control

        public override void OnUpdate()
        {
            var style = Skin.current.Button[State];
            Format.Alignment = style.TextStyle.TextAlignment;
            Layout.MaxWidth = (int)Rect.Width;
            Layout.MaxHeight = (int)Rect.Height;
            Layout.Text = Text;

            var oldState = State;
            bool active = Input.Mouse.LeftButtonState == InputState.Down && Rect.Contains(Input.Mouse.MousePos);
            bool hover = Input.Mouse.LeftButtonState == InputState.Up && Rect.Contains(Input.Mouse.MousePos);
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

            bool changed = Input.Mouse.LeftButtonClicked && Rect.Contains(Input.Mouse.MousePos);
            Result = changed ? !Result : Result;
        }

        public override void OnRender(Context g)
        {
            var toggleBoxRect = new Rect(Rect.TopLeft, new Size(20, 20));
            g.DrawBoxModel(toggleBoxRect,
                new Content(Texture._presets[Result ? "Toggle.On" : "Toggle.Off"]),
                Skin.current.Toggle[State]);

            var toggleTextRect = new Rect(toggleBoxRect.TopRight, Rect.BottomRight);
            g.DrawBoxModel(toggleTextRect,
                new Content(Layout),
                Skin.current.Toggle[State]);
        }

        #endregion
    }
}