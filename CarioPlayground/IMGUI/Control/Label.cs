using System.Diagnostics;
using Cairo;

namespace ImGui
{
    internal class Label : Control
    {
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

        public override void OnUpdate()
        {
            TextContext.Text = Text;

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
            g.DrawBoxModel(Rect, new Content(TextContext), Skin.current.Label[State]);
            this.RenderRects.Add(Rect);
        }

        public override void Dispose()
        {
            TextContext.Dispose();
        }

        public override void OnClear(Context g)
        {
            g.FillRectangle(Rect, CairoEx.ColorWhite);
            this.RenderRects.Add(Rect);
        }

        internal Label(string name, BaseForm form, string text, Rect rect) : base(name, form)
        {
            Rect = rect;
            Text = text;

            var style = Skin.current.Label[State];
            var font = style.Font;
            var textStyle = style.TextStyle;
            var contentRect = Utility.GetContentRect(Rect, style);
            TextContext = Application._map.CreateTextContext(
                Text, font.FontFamily, font.Size,
                font.FontStretch, font.FontStyle, font.FontWeight,
                (int)contentRect.Width, (int)contentRect.Height,
                textStyle.TextAlignment);
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