using System.Diagnostics;
using Cairo;
using TinyIoC;

namespace IMGUI
{
    internal class Toggle : Control
    {
        public ITextFormat Format { get; private set; }
        public ITextLayout Layout { get; private set; }

        internal Toggle(string name, string text, int width, int height)
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
        internal static bool DoControl(Context g, Rect rect, string text, bool value, string name)
        {
            #region Get control reference
            Toggle toggle;
            if(!Controls.ContainsKey(name))
            {
                toggle = new Toggle(name, text, (int)rect.Width, (int)rect.Height);
                Debug.Assert(toggle != null);
            }
            else
            {
                toggle = Controls[name] as Toggle;
                Debug.Assert(toggle != null);

                #region Set control data
                var style = Skin.current.Button[toggle.State];
                toggle.Format.Alignment = style.TextStyle.TextAlignment;
                toggle.Layout.MaxWidth = (int)rect.Width;
                toggle.Layout.MaxHeight = (int)rect.Height;
                toggle.Layout.Text = text;
                #endregion
            }

            #endregion

            bool active = Input.Mouse.LeftButtonState == InputState.Down && rect.Contains(Input.Mouse.MousePos);
            bool hover = Input.Mouse.LeftButtonState == InputState.Up && rect.Contains(Input.Mouse.MousePos);
            if(active)
                toggle.State = "Active";
            else if(hover)
                toggle.State = "Hover";
            else
                toggle.State = "Normal";

            bool changed = Input.Mouse.LeftButtonClicked && rect.Contains(Input.Mouse.MousePos);
            bool on = changed ? !value : value;

            var toggleBoxRect = new Rect(rect.TopLeft, new Size(20, 20));
            g.DrawBoxModel(toggleBoxRect,
                new Content(Texture._presets[on ? "Toggle.On" : "Toggle.Off"]),
                Skin.current.Toggle[toggle.State]);

            var toggleTextRect = new Rect(toggleBoxRect.TopRight, rect.BottomRight);
            g.DrawBoxModel(toggleTextRect,
                new Content(text),
                Skin.current.Toggle[toggle.State]);

            return on;
        }
    }
}