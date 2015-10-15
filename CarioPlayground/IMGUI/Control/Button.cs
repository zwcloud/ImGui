using System;
using System.Diagnostics;
using Cairo;
using SFML.Window;
using TinyIoC;

namespace IMGUI
{
    internal class Button : Control
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

        public override void OnUpdate()
        {
            var style = Skin.current.Button[State];
            Layout.MaxWidth = (int)Rect.Width;
            Layout.MaxHeight = (int)Rect.Height;
            Layout.Text = Text;

            var oldState = State;
            bool active = Input.Mouse.LeftButtonState == InputState.Down && Rect.Contains(Input.Mouse.MousePos);
            bool hover = Input.Mouse.LeftButtonState == InputState.Up && Rect.Contains(Input.Mouse.MousePos);
            if (active)
            {
                State = "Active";
                //ToolTip.Instance.Hide();
            }
            else if (hover)
            {
                State = "Hover";
                //ToolTip.Instance.Show(Text.Substring(0, 5));
            }
            else
            {
                State = "Normal";
                //ToolTip.Instance.Hide();
            }

            if(State != oldState)
            {
                NeedRepaint = true;
            }

            bool clicked = Input.Mouse.LeftButtonClicked && Rect.Contains(Input.Mouse.MousePos);
            Result = clicked;
        }

        public override void OnRender(Context g)
        {
            g.DrawBoxModel(Rect, new Content(Layout), Skin.current.Button[State]);
        }

        public override void Dispose()
        {
            Layout.Dispose();
            Format.Dispose();
        }

        internal Button(string name, BasicForm form, string text, Rect rect) :base(name, form)
        {
            Rect = rect;
            Text = text;

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
            var textStyle = Skin.current.Button[State].TextStyle;
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

        //TODO Control-less DoControl overload (without name parameter)
        internal static bool DoControl(Context g, BasicForm form, Rect rect, string text, string name)
        {
            //The control hasn't been created, create it.
            if (!form.Controls.ContainsKey(name))
            {
                var button = new Button(name, form, text, rect);
                button.OnUpdate();
                button.OnRender(g);
            }

            var control = form.Controls[name] as Button;
            Debug.Assert(control != null);

            //Check if the control need to be relayout

            return control.Result;
        }
    }
}