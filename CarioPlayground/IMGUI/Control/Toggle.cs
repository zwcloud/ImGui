using System;
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

        internal Toggle(string name, string displayText, Rect rect)
        {
            //Check paramter
            if (rect.Width < rect.Height)
            {
                throw new ArgumentException(
                    string.Format(
                        "Width of the toggle must bigger than height. The current width is {0} and height is {1}",
                        rect.Width, rect.Height));
            }

            Name = name;
            State = "Normal";
            Application.MainForm.Controls[Name] = this;

            Rect = rect;
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
                        {"maxWidth", (int)Rect.Width},
                        {"maxHeight", (int)Rect.Height}
                    });
        }

        internal static bool DoControl(Context g, Rect rect, string displayText, bool value, string name)
        {
            if (!Application.MainForm.Controls.ContainsKey(name))
            {
                var toggle = new Toggle(name, displayText, rect);
                Debug.Assert(toggle != null);
                toggle.OnUpdate();
                toggle.OnRender(g);
            }

            var control = Application.MainForm.Controls[name] as Toggle;
            Debug.Assert(control != null);

            //The control need to be relayout
            //TODO

            return control.Result;
        }

        #region Overrides of Control

        public override void OnUpdate()
        {
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
            var toggleBoxRect = new Rect(Rect.X, Rect.Y, new Size(Rect.Size.Height, Rect.Size.Height));
            g.DrawImage(toggleBoxRect, Texture._presets[Result ? "Toggle.On" : "Toggle.Off"]);

            var toggleTextRect = new Rect(toggleBoxRect.TopRight, Rect.BottomRight);
            g.DrawBoxModel(toggleTextRect,
                new Content(Layout),
                Skin.current.Toggle[State]);
        }

        public override void Dispose()
        {
            Layout.Dispose();
            Format.Dispose();
        }

        #endregion
    }
}