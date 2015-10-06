using System.Diagnostics;
using Cairo;
using TinyIoC;

namespace IMGUI
{
    internal class Label : Control
    {
        private string text;
        public ITextFormat Format { get; private set; }
        public ITextLayout Layout { get; private set; }

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

        public override void OnUpdate()
        {
            var style = Skin.current.Button[State];
            Format.Alignment = style.TextStyle.TextAlignment;
            Layout.MaxWidth = (int) Rect.Width;
            Layout.MaxHeight = (int)Rect.Height;
            Layout.Text = Text;

            var oldState = State;
            bool active = Input.Mouse.LeftButtonState == InputState.Down && Rect.Contains(Input.Mouse.MousePos);
            bool hover = Input.Mouse.LeftButtonState == InputState.Up && Rect.Contains(Input.Mouse.MousePos);
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

        public override void OnRender(Context g)
        {
            g.DrawBoxModel(Rect, new Content(Layout), Skin.current.Label[State]);
        }

        public override void Dispose()
        {
            Layout.Dispose();
            Format.Dispose();
        }

        internal Label(string name, string text, int width, int height)
        {
            Name = name;
            State = "Normal";
            Controls[Name] = this;

            Text = text;

            var font = Skin.current.Label[State].Font;
            Format = Application.IocContainer.Resolve<ITextFormat>(
                new NamedParameterOverloads
                    {
                        {"fontFamilyName", font.FontFamily},
                        {"fontWeight", font.FontWeight},
                        {"fontStyle", font.FontStyle},
                        {"fontStretch", font.FontStretch},
                        {"fontSize", (float) font.Size}
                    });
            var textStyle = Skin.current.Label[State].TextStyle;
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

        //TODO Control-less DoControl overload (without name parameter)
        internal static void DoControl(Context g, Rect rect, string text, string name)
        {
            if(!Controls.ContainsKey(name))
            {
                var label = new Label(name, text, (int)rect.Width, (int)rect.Height);
                label.Rect = rect;
                label.Text = text;
                label.OnUpdate();
                label.OnRender(g);
            }
            
            var control = Controls[name] as Label;
            Debug.Assert(control != null);

            //The control need to be relayout
            //TODO
        }

    }
}