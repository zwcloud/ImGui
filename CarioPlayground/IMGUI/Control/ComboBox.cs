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

        public ITextFormat Format { get; private set; }
        public ITextLayout Layout { get; private set; }

        public int SelectedIndex { get; private set; }//TODO consider remove this property
        
        internal ComboBox(string name, BaseForm form, string[] texts, Rect rect)
            : base(name, form)
        {
            Rect = rect;
            Texts = texts;
            Text = texts[0];
            SelectedIndex = 0;

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


            var screenRect = Utility.GetScreenRect(Rect, this.Form);
            ItemsContainer = new ComboxBoxItemsForm(
                screenRect,
                Texts, i =>
                {
                    SelectedIndex = i;
                    this.State = "Normal";
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
            Text = Texts[SelectedIndex];
            Layout.Text = Text;

            var oldState = State;
            textCount = Texts.Length;
            Rect extendRect = new Rect(
                Rect.BottomLeft + new Vector(1, 1),
                Rect.BottomRight + textCount * (new Vector(Rect.Height, Rect.Height)));

            Debug.Assert(!Rect.IntersectsWith(extendRect));

            bool inMainRect = Rect.Contains(Input.Mouse.GetMousePos(Form));
            switch (State)
            {
                case "Normal":
                    if(inMainRect && Input.Mouse.LeftButtonState == InputState.Up)
                    {
                        State = "Hover";
                    }
                    break;
                case "Hover":
                    if (Input.Mouse.LeftButtonClicked)
                    {
                        var screenRect = Utility.GetScreenRect(Rect, this.Form);
                        ItemsContainer.Position = screenRect.TopLeft;
                        Application.Forms.Add(ItemsContainer);
                        ItemsContainer.Show();
                        State = "Active";
                    }
                    else if (!inMainRect)
                    {
                        State = "Normal";
                    }
                    break;
                case "Active":
                    break;
            }

            if (State != oldState)
            {
                NeedRepaint = true;
                Debug.WriteLine("{0} => {1}", oldState, State);
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

    internal sealed class ComboxBoxItemsForm : BorderlessForm
    {
        private Rect Rect { get; set; }
        private List<string> TextList;
        private System.Action<int> CallBack { get; set; }

        public ComboxBoxItemsForm(Rect rect, string[] text, System.Action<int> callBack)
            : base((int)rect.Width, (int)rect.Height * text.Length)
        {
            Position = rect.TopLeft;
            rect.X = rect.Y = 0;
            Rect = rect;
            TextList = new List<string>(text);
            CallBack = callBack;
        }

        protected override void OnGUI(GUI gui)
        {
            gui.BeginVertical(Rect);
            for (int i = 0; i < TextList.Count; i++)
            {
                var itemRect = Rect;
                itemRect.Y += (i + 1) * Rect.Height;
                if(gui.Button(new Rect(Rect.Width, itemRect.Height), TextList[i], this.Name + "item" + i))
                {
                    Debug.WriteLine("Clicked");
                    if(CallBack!=null)
                    {
                        CallBack(i);
                    }
                    this.Hide();
                    Application.Forms.Remove(this);
                }
            }
            gui.EndVertical();
        }
    }
}
