using System.Diagnostics;
using Cairo;
using TinyIoC;

namespace IMGUI
{
    internal class Button : Control
    {
        public ITextFormat Format { get; private set; }
        public ITextLayout Layout { get; private set; }

        internal Button(string name, string text, float width, float height)
        {
            Name = name;
            State = "Normal";

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
            Layout = Application.IocContainer.Resolve<ITextLayout>(
                new NamedParameterOverloads
                    {
                        {"text", text},
                        {"textFormat", Format},
                        {"maxWidth", width},
                        {"maxHeight", height}
                    });

            Controls[Name] = this;
        }

        //TODO Control-less DoControl overload (without name parameter)
        internal static bool DoControl(Context g, Rect rect, string text, string name)
        {
            #region Get control reference
            Button button;
            if(!Controls.ContainsKey(name))
            {
                button = new Button(name, text, (float)rect.Width, (float)rect.Height);
            }
            else
            {
                button = Controls[name] as Button;
            }

            Debug.Assert(button != null);
            #endregion

            #region Logic
            bool active = Input.Mouse.LeftButtonState == InputState.Down && rect.Contains(Input.Mouse.MousePos);
            bool hover = Input.Mouse.LeftButtonState == InputState.Up && rect.Contains(Input.Mouse.MousePos);
            if(active)
                button.State = "Active";
            else if(hover)
                button.State = "Hover";
            else
                button.State = "Normal";
            #endregion

            g.DrawBoxModel(rect, new Content(button.Layout), Skin.current.Button[button.State]);

            bool clicked = Input.Mouse.LeftButtonClicked && rect.Contains(Input.Mouse.MousePos);
            return clicked;
        }
    }
}