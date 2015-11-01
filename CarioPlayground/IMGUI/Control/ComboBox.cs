using System.Diagnostics;
using Cairo;
using System.Collections.Generic;
using TinyIoC;

//TODO use stand-alone window to show the items
//BUG Hover state persists when move from mainRect to outside.
//BUG Abnormal representation when drag from mainRect to outside.

namespace IMGUI
{
    internal class ComboBox : Control
    {
        public int textCount { get; private set; }
        public string[] Texts { get; private set; }
        public BorderlessForm ItemsContainer { get; private set; }
        public Rect Rect { get; private set; }

        public ITextFormat Format { get; private set; }
        public ITextLayout Layout { get; private set; }

        public int SelectedIndex { get; private set; }//TODO consider remove this property

        private int HoverIndex { get; set; }

        private int ActiveIndex { get; set; }

        internal ComboBox(string name, BaseForm form, string[] texts, Rect rect)
            : base(name, form)
        {
            Rect = rect;
            Texts = texts;

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
                        {"text", Texts[0]},
                        {"textFormat", Format},
                        {"maxWidth", (int)Rect.Width},
                        {"maxHeight", (int)Rect.Height}
                    });
        }

        internal static int DoControl(Context g, Context gTop, BaseForm form, Rect rect, string[] texts, int selectedIndex, string name)
        {
            if (!form.Controls.ContainsKey(name))
            {
                var comboBox = new ComboBox(name, form, texts, rect);
                comboBox.SelectedIndex = selectedIndex;
                comboBox.OnUpdate();
                comboBox.OnRender(g);
            }
            var control = form.Controls[name] as ComboBox;
            Debug.Assert(control != null);

            return control.SelectedIndex;
        }

        #region Overrides of Control

        public override void OnUpdate()
        {
            var oldState = State;
            textCount = Texts.Length;
            Rect extendRect = new Rect(
                Rect.BottomLeft + new Vector(1, 1),
                Rect.BottomRight + textCount * (new Vector(Rect.Height, Rect.Height)));

            Debug.Assert(!Rect.IntersectsWith(extendRect));

            bool inMainRect = Rect.Contains(Input.Mouse.MousePos);
            bool inExtendRect = extendRect.Contains(Input.Mouse.MousePos);

            if (!inMainRect && !inExtendRect)
            {
                if (State == "Hover")
                {
                    State = "Normal";
                }
                else if (State == "Active")
                {
                    if (Input.Mouse.LeftButtonClicked)
                    {
                        State = "Normal";
                    }
                }
            }
            else if (inMainRect)
            {
                if (State == "Normal")
                {
                    State = "Hover";
                }
                if (State == "Hover")
                {
                    if (Input.Mouse.LeftButtonClicked)
                    {
                        State = "Active";
                    }
                }
                else if (State == "Active")
                {
                    if (Input.Mouse.LeftButtonClicked)
                    {
                        State = "Hover";
                    }
                }
            }
            else
            {
                if (State == "Active")
                {
                    if (ItemsContainer == null)
                    {
                        ItemsContainer = new ComboxBoxItemsForm(extendRect, Texts);
                        Application.Forms.Add(ItemsContainer);
                    }
                    ItemsContainer.Show();
#if f
                    for (int i = 0; i < textCount; i++)
                    {
                        var itemRect = Rect;
                        itemRect.Y += (i + 1) * Rect.Height;
                        bool inItemRect = itemRect.Contains(Input.Mouse.MousePos);
                        if (inItemRect)
                        {
                            if (Input.Mouse.LeftButtonState == InputState.Up)
                            {
                                HoverIndex = i;
                            }
                            else if (Input.Mouse.LeftButtonState == InputState.Down)
                            {
                                ActiveIndex = i;
                            }

                            if (Input.Mouse.LeftButtonClicked)
                            {
                                SelectedIndex = i;
                                State = "Normal";
                            }
                            break;
                        }
                    }
#endif

                }
            }

            if(State != "Active")
            {
                if(ItemsContainer!=null)
                {
                    ItemsContainer.Hide();
                }
            }

            if (State != oldState)
            {
                NeedRepaint = true;
            }
        }

        public override void OnRender(Context g)
        {
            g.DrawBoxModel(Rect, new Content(Layout), Skin.current.ComboBox[State]);
            g.LineWidth = 1;
            var trianglePoints = new Point[3];
            trianglePoints[0].X = Rect.TopRight.X - 5;
            trianglePoints[0].Y = Rect.TopRight.Y + 0.2 * Rect.Height;
            trianglePoints[1].X = trianglePoints[0].X - 0.6 * Rect.Height;
            trianglePoints[1].Y = trianglePoints[0].Y;
            trianglePoints[2].X = trianglePoints[0].X - 0.3 * Rect.Height;
            trianglePoints[2].Y = trianglePoints[0].Y + 0.6 * Rect.Height;
            g.StrokePolygon(trianglePoints, CairoEx.ColorBlack);
        }

        public override void Dispose()
        {
            Layout.Dispose();
            Format.Dispose();
        }

        #endregion
    }

    internal class ComboxBoxItemsForm : BorderlessForm
    {
        private Rect Rect { get; set; }
        private List<string> TextList;

        public ComboxBoxItemsForm(Rect rect, string[] text)
            : base((int)rect.Width, (int)rect.Height)
        {
            Rect = rect;
            TextList = new List<string>(text);
        }

        protected override void OnGUI(GUI gui)
        {
            gui.BeginVertical(Rect);
            for (int i = 0; i < TextList.Count; i++)
            {
                var itemRect = Rect;
                itemRect.Y += (i + 1) * Rect.Height;
                gui.Button(new Rect(Rect.Width, itemRect.Height), TextList[i], this.Name + "item" + i);
            }
            gui.EndVertical();
        }
    }
}
