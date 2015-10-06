using System.Collections.Generic;
using System.Diagnostics;
using Cairo;
using TinyIoC;
using System;
using System.Runtime.Caching;

namespace IMGUI
{
    internal class Radio : Control
    {
        public static Dictionary<string, HashSet<string>> Groups;

        static Radio()
        {
            Groups = new Dictionary<string, HashSet<string>>();
        }

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
        private string groupName;
        private bool actived;

        public string GroupName
        {
            get { return groupName; }
            private set
            {
                if(groupName!=null)
                {
                    if(value == groupName) return;
#if DEBUG
                    Debug.Assert(!Groups.ContainsKey(groupName),
                        "Older group is not recorded.");
                    var removed = Groups[groupName].Remove(Name);
                    Debug.Assert(removed, "This radio is not in the Group <{0}>.", groupName);
#else
                    Groups[_groupName].Remove(Name);
#endif
                }
                if(!Groups.ContainsKey(value))
                    Groups[value] = new HashSet<string>();
                Groups[value].Add(Name);
                groupName = value;
            }
        }

        public bool Actived
        {
            get { return actived; }
            set
            {
                if(value == actived)
                {
                    return;
                }

                actived = value;
                NeedRepaint = true;
            }
        }


        public Radio(string name, string text, int width, int height, string groupName)
        {
            Name = name;
            State = "Normal";
            Controls[Name] = this;

            Text = text;

            GroupName = groupName;
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
            var style = Skin.current.Radio[State];
            Format.Alignment = style.TextStyle.TextAlignment;
            Layout = Application.IocContainer.Resolve<ITextLayout>(
                new NamedParameterOverloads
                    {
                        {"text", Text},
                        {"textFormat", Format},
                        {"maxWidth", width},
                        {"maxHeight", height}
                    });
        }

        public static bool DoControl(Context g, Rect rect, string text, string groupName, bool value, string name)
        {
            if(!Controls.ContainsKey(name))
            {
                var radio = new Radio(name, text, (int)rect.Width, (int)rect.Height, groupName);
                radio.Rect = rect;
                radio.OnUpdate();
                radio.OnRender(g);
            }

            var control = Controls[name] as Radio;
            Debug.Assert(control != null);

            return control.Actived;
        }

        #region Overrides of Control

        public override void OnUpdate()
        {
            var oldState = State;
            if (Rect.Contains(Input.Mouse.MousePos))
            {
                if (Input.Mouse.LeftButtonState == InputState.Up)
                {
                    State = "Hover";
                }
                else if (Input.Mouse.LeftButtonState == InputState.Down)
                {
                    State = "Active";
                }

                if (Input.Mouse.LeftButtonClicked)
                {
                    Actived = true;
                }
            }
            else
            {
                State = "Normal";
            }
            if (Actived)
            {
                var group = Groups[groupName];
                foreach (var radioName in group)
                {
                    var radio = Controls[radioName];
#if DEBUG
                    Debug.Assert(radio is Radio);
#endif
                    ((Radio)radio).Actived = false;
                }
                Actived = true;
            }
            if (State != oldState)
            {
                NeedRepaint = true;
            }
        }

        public override void OnRender(Context g)
        {
            var style = Skin.current.Radio[State];
            var radioBoxRect = new Rect(Rect.X, Rect.Y, new Size(Rect.Height, Rect.Height));
            var radioBoxCenter = radioBoxRect.Center;
            var pointRadius = (float)(radioBoxRect.Width-1) / 4;
            var circleRadius = (float)(radioBoxRect.Width - 1) / 2;

            g.FillRectangle(radioBoxRect, style.BackgroundStyle.Color);
            g.StrokeCircle(radioBoxCenter.ToPointD(), circleRadius, CairoEx.ColorBlack);
            if (!Actived)
            {
                if (State == "Hover")
                    g.FillCircle(radioBoxCenter.ToPointD(), pointRadius, CairoEx.ColorRgb(46, 167, 224));
                else if (State == "Active")
                    g.FillCircle(radioBoxCenter.ToPointD(), pointRadius, CairoEx.ColorBlack);
            }
            else
            {
                if (State == "Normal")
                    g.FillCircle(radioBoxCenter.ToPointD(), pointRadius, CairoEx.ColorBlack);
                else if (State == "Hover")
                {
                    g.FillCircle(radioBoxCenter.ToPointD(), pointRadius, CairoEx.ColorRgb(46, 167, 224));
                    g.StrokeCircle(radioBoxCenter.ToPointD(), pointRadius, CairoEx.ColorBlack);
                }
                else if (State == "Active")
                    g.StrokeCircle(radioBoxCenter.ToPointD(), pointRadius, CairoEx.ColorBlack);
            }

            var radioTextRect = new Rect(radioBoxRect.TopRight, Rect.BottomRight);
            g.DrawBoxModel(radioTextRect, new Content(Layout), Skin.current.Radio[State]);
        }

        public override void Dispose()
        {
            Layout.Dispose();
            Format.Dispose();
        }

        #endregion
    }
}