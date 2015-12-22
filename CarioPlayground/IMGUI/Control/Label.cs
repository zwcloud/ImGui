using System.Diagnostics;
using Cairo;

namespace ImGui
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


        public override void OnUpdate()
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

        public override void OnRender(Context g)
        {
            g.DrawBoxModel(Rect, new Content(Layout), Skin.current.Label[State]);
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

        internal Label(string name, BaseForm form, string text, Rect rect) : base(name, form)
        {
            Rect = rect;
            Text = text;

            var style = Skin.current.Label[State];
            var font = style.Font;
            Format = Application._map.CreateTextFormat(
                font.FontFamily,
                font.FontWeight,
                font.FontStyle,
                font.FontStretch,
                font.Size);

            var textStyle = style.TextStyle;
            var contentRect = Utility.GetContentRect(Rect, style);
            Format.Alignment = textStyle.TextAlignment;
            Layout = Application._map.CreateTextLayout(
                Text, Format,
                (int)contentRect.Width,
                (int)contentRect.Height);
        }

        internal static void DoControl(Context g, BaseForm form, Rect rect, string text, string name)
        {
            if (!form.Controls.ContainsKey(name))
            {
                var label = new Label(name, form, text, rect);
                label.OnUpdate();
                label.OnRender(g);
            }

            var control = form.Controls[name] as Label;
            Debug.Assert(control != null);
            control.Active = true;

            //Update text
            control.Text = text;
        }

    }
}