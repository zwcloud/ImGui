using System.Diagnostics;
using Cairo;
using System.Collections.Generic;

//BUG Hover state persists when move from mainRect to outside.
//BUG Abnormal representation when drag from mainRect to outside.

namespace ImGui
{
    internal class ComboBox : Control
    {
        #region State machine constants
        static class ComboBoxState
        {
            public const string Normal = "Normal";
            public const string Hovered = "Hovered";
            public const string Active = "Active";
            public const string ShowingItems = "ShowingItems";
        }

        static class ComboBoxCommand
        {
            public const string MoveIn = "MoveIn";
            public const string MoveOut = "MoveOut";
            public const string MousePress = "MouseDown";
            public const string ShowItems = "ShowItems";
            public const string SelectItem = "SelectItem";
        }

        static readonly string[] states =
        {
            ComboBoxState.Normal, ComboBoxCommand.MoveIn, ComboBoxState.Hovered,
            ComboBoxState.Hovered, ComboBoxCommand.MoveOut, ComboBoxState.Normal,
            ComboBoxState.Hovered, ComboBoxCommand.MousePress, ComboBoxState.Active,
            ComboBoxState.Active, ComboBoxCommand.ShowItems, ComboBoxState.ShowingItems,
            ComboBoxState.ShowingItems, ComboBoxCommand.SelectItem, ComboBoxState.Normal,
        };
        #endregion

        private readonly StateMachine stateMachine;

        public string[] Texts { get; private set; }
        public Form ItemsContainer { get; private set; }

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

        public ITextContext TextContext { get; private set; }

        public int SelectedIndex { get; private set; }
        
        internal ComboBox(string name, Form form, string[] texts, Rect rect)
            : base(name, form)
        {
            Rect = rect;
            Texts = texts;
            Text = texts[0];
            SelectedIndex = 0;
            stateMachine = new StateMachine(ComboBoxState.Normal, states);

            var style = Skin.current.ComboBox[State];
            var font = style.Font;
            var textStyle = style.TextStyle;
            var contentRect = Utility.GetContentRect(Rect, style);

            TextContext = Application._map.CreateTextContext(
                Text, font.FontFamily, font.Size,
                font.FontStretch, font.FontStyle, font.FontWeight,
                (int)contentRect.Width, (int)contentRect.Height,
                textStyle.TextAlignment);

            var screenPos = this.Form.ClientToScreen(Rect.TopLeft);
            var screenRect = new Rect(screenPos, Rect.Size);
            ItemsContainer = new ComboxBoxItemsForm(
                screenRect,
                Texts, i =>
                {
                    SelectedIndex = i;
                    this.stateMachine.MoveNext(ComboBoxCommand.SelectItem);
                });
        }

        internal static int DoControl(Form form, Rect rect, string[] texts, int selectedIndex, string name)
        {
            if (!form.Controls.ContainsKey(name))
            {
                var comboBox = new ComboBox(name, form, texts, rect);
                comboBox.SelectedIndex = selectedIndex;
            }
            var control = form.Controls[name] as ComboBox;
            Debug.Assert(control != null);
            control.Active = true;

            return control.SelectedIndex;
        }

        #region Overrides of Control

        public override void OnUpdate()
        {
            Text = Texts[SelectedIndex];
            TextContext.Text = Text;

            var oldState = State;
            bool active = stateMachine.CurrentState == ComboBoxState.Active;
            bool hover = stateMachine.CurrentState == ComboBoxState.Hovered;
            if (active)
            {
                State = "Active";
            }
            else if (hover)
            {
                State = "Hover";
            }
            else
            {
                State = "Normal";
            }

            if (State != oldState)
            {
                NeedRepaint = true;
            }

            //Execute state commands
            var containMousePosition = Rect.Contains(Form.current.ScreenToClient(Mouse.Instance.MousePos));
            if (!Rect.Contains(Form.current.ScreenToClient(Mouse.Instance.LastMousePos)) && containMousePosition)
            {
                stateMachine.MoveNext(ComboBoxCommand.MoveIn);
            }
            if (Rect.Contains(Form.current.ScreenToClient(Mouse.Instance.LastMousePos)) && !containMousePosition)
            {
                stateMachine.MoveNext(ComboBoxCommand.MoveOut);
            }
            if (Mouse.Instance.stateMachine.CurrentState == Mouse.Instance.MouseState.Pressed && containMousePosition && Form.Focused)
            {
                if (stateMachine.MoveNext(ComboBoxCommand.MousePress))
                {
                    Mouse.Instance.stateMachine.MoveNext(Mouse.Instance.MouseCommand.Fetch);
                }
            }
            if(stateMachine.CurrentState == ComboBoxState.Active)//instant transition of state
            {
                ItemsContainer.Position = Form.current.ClientToScreen(Rect.BottomLeft);
                Application.Forms.Add(ItemsContainer);
                ItemsContainer.Show();
                stateMachine.MoveNext(ComboBoxCommand.ShowItems);
            }

        }

        public override void OnRender(Context g)
        {
            var style = Skin.current.ComboBox[State];
            g.DrawBoxModel(Rect, new Content(TextContext), style);
            g.LineWidth = 1;
            var trianglePoints = new Point[3];
            trianglePoints[0].X = Rect.TopRight.X - 5;
            trianglePoints[0].Y = Rect.TopRight.Y + 0.2 * Rect.Height;
            trianglePoints[1].X = trianglePoints[0].X - 0.6 * Rect.Height;
            trianglePoints[1].Y = trianglePoints[0].Y;
            trianglePoints[2].X = trianglePoints[0].X - 0.3 * Rect.Height;
            trianglePoints[2].Y = trianglePoints[0].Y + 0.6 * Rect.Height;
            g.StrokePolygon(trianglePoints, style.LineColor);

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


        #endregion
    }

    internal sealed class ComboxBoxItemsForm : Form
    {
        private Rect Rect { get; set; }
        private readonly List<string> TextList;
        private System.Action<int> CallBack { get; set; }

        public ComboxBoxItemsForm(Rect rect, string[] texts, System.Action<int> callBack)
            : base(rect)
        {
            Position = rect.TopLeft;
            rect.X = rect.Y = 0;
            Rect = rect;
            TextList = new List<string>(texts);
            CallBack = callBack;
        }

        protected override void OnGUI()
        {
            GUILayout.BeginVertical();
            for (int i = 0; i < TextList.Count; i++)
            {
                var itemRect = Rect;
                itemRect.Y += (i + 1) * Rect.Height;
                if(GUI.Button(new Rect(Rect.Width, itemRect.Height), TextList[i], this.GetHashCode() + "item" + i))
                {
                    if(CallBack!=null)
                    {
                        CallBack(i);
                    }
                    this.Hide();
                    Application.Forms.Remove(this);
                }
            }
            GUILayout.EndVertical();
        }
    }
}
