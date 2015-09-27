using System.Diagnostics;
using Cairo;
using TinyIoC;

namespace IMGUI
{
    internal class Label : Control
    {
        public ITextFormat Format { get; private set; }
        public ITextLayout Layout { get; private set; }

        internal Label(string name, string text, int width, int height)
        {
            Name = name;
            State = "Normal";

            Controls[Name] = this;

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
        }

        //TODO Control-less DoControl overload (without name parameter)
        internal static void DoControl(Context g, Rect rect, string text, string name)
        {
            #region Get control reference
            Label label;
            if(!Controls.ContainsKey(name))
            {
                label = new Label(name, text, (int)rect.Width, (int)rect.Height);
            }
            else
            {
                label = Controls[name] as Label;
                Debug.Assert(label != null);

                #region Set control data
                var style = Skin.current.Button[label.State];
                label.Format.Alignment = style.TextStyle.TextAlignment;
                label.Layout.MaxWidth = (int)rect.Width;
                label.Layout.MaxHeight = (int)rect.Height;
                label.Layout.Text = text;
                #endregion
            }

            Debug.Assert(label != null);
            #endregion

            bool active = Input.Mouse.LeftButtonState == InputState.Down && rect.Contains(Input.Mouse.MousePos);
            bool hover = Input.Mouse.LeftButtonState == InputState.Up && rect.Contains(Input.Mouse.MousePos);
            if(active)
                label.State = "Active";
            else if(hover)
                label.State = "Hover";
            else
                label.State = "Normal";

            g.DrawBoxModel(rect, new Content(label.Layout), Skin.current.Label[label.State]);
        }

    }
}