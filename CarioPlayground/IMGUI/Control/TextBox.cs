//#define INSPECT_STATE
using System;
using System.Diagnostics;
using Context = Cairo.Context;
using Key = SFML.Window.Keyboard.Key;

namespace ImGui
{
    internal class TextBox : Control
    {
        #region State machine define
        private static class TextBoxState
        {
            public const string Normal = "Normal";
            public const string Hover = "Hover";
            public const string Active = "Active";
            public const string ActiveSelecting = "ActiveSelecting";
        }

        private static class TextBoxCommand
        {
            public const string MoveIn = "MoveIn";
            public const string MoveOut = "MoveOut";
            public const string EnterEdit = "EnterEdit";
            public const string ExitEditIn = "ExitEditIn";
            public const string ExitEditOut = "ExitEditOut";
            public const string BeginSelect = "BeginSelect";
            public const string EndSelect = "EndSelect";
            
            public const string MoveCaret = "MoveCaret";
            public const string MoveCaretKeyboard = "MoveCaretKeyboard";
            public const string DoSelect = "DoSelect";
            public const string DoEdit = "DoEdit";
        }

        private static readonly string[] states = new string[11*3]
        {
            TextBoxState.Normal,            TextBoxCommand.MoveIn,      TextBoxState.Hover,
            TextBoxState.Hover,             TextBoxCommand.MoveOut,     TextBoxState.Normal,
            TextBoxState.Hover,             TextBoxCommand.EnterEdit,   TextBoxState.Active,
            TextBoxState.Active,            TextBoxCommand.ExitEditIn,  TextBoxState.Hover,
            TextBoxState.Active,            TextBoxCommand.ExitEditOut, TextBoxState.Normal,
            TextBoxState.Active,            TextBoxCommand.BeginSelect, TextBoxState.ActiveSelecting,
            TextBoxState.ActiveSelecting,   TextBoxCommand.EndSelect,   TextBoxState.Active,
            
            TextBoxState.Active,            TextBoxCommand.MoveCaret,   TextBoxState.Active,
            TextBoxState.Active,            TextBoxCommand.MoveCaretKeyboard,   TextBoxState.Active,
            TextBoxState.Active,            TextBoxCommand.DoEdit,   TextBoxState.Active,
            TextBoxState.ActiveSelecting,   TextBoxCommand.DoSelect,    TextBoxState.ActiveSelecting,

        };

        private static readonly Action<Control>[] callBacks = new Action<Control>[11]
        {
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            MoveCaretCallBack,
            MoveCaretKeyboardCallBack,
            DoEditCallBack,
            DoSelectCallBack,
        };
        #endregion

        private readonly StateMachineEx stateMachine;

        private Point caretTopPoint;
        private Point caretBottomPoint;
        private uint caretIndex;
        private uint selectIndex;
        private string text;

        public ITextContext TextContext { get; private set; }

        public string Text
        {
            get { return text; }
            set
            {
                if(value != text)
                {
                    text = value;
                    //Update layout text
                    TextContext.Text = value;
                    NeedRepaint = true;
                }
            }
        }

        public Point CaretPosition { get; set; }
        public bool Selecting { get; set; }

        public uint CaretIndex
        {
            get { return caretIndex; }
            set
            {
                if (value != caretIndex)
                {
                    caretIndex = value;
                    NeedRepaint = true;
                }
            }
        }

        public uint SelectIndex
        {
            get { return selectIndex; }
            set
            {
                if(value!=selectIndex)
                {
                    selectIndex = value;
                    NeedRepaint = true;
                }
            }
        }

        public byte[] StringBytes
        {
            get { return System.Text.Encoding.UTF8.GetBytes(Text); }
        }

        public TextBox(string name, BaseForm form, string text, Rect rect)
            : base(name, form)
        {
            stateMachine = new StateMachineEx(TextBoxState.Normal, states, callBacks);
            Rect = rect;

            var style = Skin.current.TextBox[State];
            var font = style.Font;
            var textStyle = style.TextStyle;
            var contentRect = Utility.GetContentRect(Rect, style);

            TextContext = Application._map.CreateTextContext(
                text, font.FontFamily, font.Size,
                font.FontStretch, font.FontStyle, font.FontWeight,
                (int)contentRect.Width, (int)contentRect.Height,
                textStyle.TextAlignment);
            Text = text;
        }

        internal static string DoControl(BaseForm form, Rect rect, string text, string name)
        {
            if (!form.Controls.ContainsKey(name))
            {
                var textBox = new TextBox(name, form, text, rect);
            }
            var control = form.Controls[name] as TextBox;
            Debug.Assert(control != null);
            control.Active = true;

            return control.Text;
        }

        private static void MoveCaretCallBack(Control textBoxControl)
        {
            TextBox textBox = (TextBox)textBoxControl;

            var State = textBox.State;
            var Rect = textBox.Rect;
            var TextContext = textBox.TextContext;
            var Form = textBox.Form;
            var Text = textBox.Text;

            var style = Skin.current.TextBox[State];
            var contentRect = Utility.GetContentRect(Rect, style);
            var offsetOfTextRect = contentRect.TopLeft;
            uint caretIndex;
            bool isInside;
            caretIndex = TextContext.XyToIndex(
                (float)(Input.Mouse.GetMousePos(Form).X - offsetOfTextRect.X),
                (float)(Input.Mouse.GetMousePos(Form).Y - offsetOfTextRect.Y), out isInside);
            if (!isInside && caretIndex == Text.Length - 1)
            {
                ++caretIndex;
            }
            textBox.SelectIndex = textBox.CaretIndex = caretIndex;
        }

        private static void MoveCaretKeyboardCallBack(Control textBoxControl)
        {
            TextBox textBox = (TextBox)textBoxControl;

            if (Input.Keyboard.KeyDown(Key.Home))
            {
                textBox.SelectIndex = textBox.CaretIndex = 0;
            }
            if (Input.Keyboard.KeyDown(Key.End))
            {
                textBox.SelectIndex = textBox.CaretIndex = (uint)textBox.Text.Length;
            }
            if (Input.Keyboard.KeyPressed(Key.Left, true))
            {
                if (textBox.CaretIndex > 0)
                {
                    textBox.CaretIndex -= 1;
                }
                if (!Input.Keyboard.KeyDown(Key.LShift))
                {
                    textBox.SelectIndex = textBox.CaretIndex;
                }
            }
            else if (Input.Keyboard.KeyPressed(Key.Right, true))
            {
                if (textBox.CaretIndex < textBox.Text.Length)
                {
                    textBox.CaretIndex += 1;
                }
                if (!Input.Keyboard.KeyDown(Key.LShift))
                {
                    textBox.SelectIndex = textBox.CaretIndex;
                }
            }
        }

        private static void DoSelectCallBack(Control textBoxControl)
        {
            TextBox textBox = (TextBox)textBoxControl;

            var State = textBox.State;
            var Rect = textBox.Rect;
            var TextContext = textBox.TextContext;
            var Form = textBox.Form;
            var Text = textBox.Text;

            var style = Skin.current.TextBox[State];
            var contentRect = Utility.GetContentRect(Rect, style);
            var offsetOfTextRect = contentRect.TopLeft;
            uint caretIndex;
            bool isInside;
            caretIndex = TextContext.XyToIndex(
                (float)(Input.Mouse.GetMousePos(Form).X - offsetOfTextRect.X),
                (float)(Input.Mouse.GetMousePos(Form).Y - offsetOfTextRect.Y), out isInside);
            if (!isInside && caretIndex == Text.Length - 1)
            {
                ++caretIndex;
            }
            textBox.CaretIndex = caretIndex;
        }

        private static void DoEditCallBack(Control textBoxControl)
        {
            TextBox textBox = (TextBox)textBoxControl;

            var CaretIndex = textBox.CaretIndex;
            var SelectIndex = textBox.SelectIndex;

            string textBeforeCaret;
            //Input characters
            if (Application.ImeBuffer.Count != 0)
            {
                var inputText = new string(Application.ImeBuffer.ToArray());
                if (CaretIndex != SelectIndex) //Replace selected text with inputText
                {
                    //TODO check whether convert int and uint back and forth is appropriate
                    var minIndex = (int)Math.Min(CaretIndex, SelectIndex);
                    var maxIndex = (int)Math.Max(CaretIndex, SelectIndex);

                    textBox.Text = textBox.Text.Substring(0, minIndex) + inputText + textBox.Text.Substring(maxIndex);
                    textBox.MoveCaret((uint)minIndex);
                    textBeforeCaret = textBox.Text.Substring(0, minIndex) + inputText;
                }
                else //Insert inputText into caret position
                {
                    if (textBox.Text.Length == 0)
                    {
                        textBox.Text = inputText;
                        textBeforeCaret = textBox.Text;
                    }
                    else
                    {
                        textBox.Text = textBox.Text.Substring(0, (int)CaretIndex) + inputText + textBox.Text.Substring((int)CaretIndex);
                        textBeforeCaret = textBox.Text.Substring(0, (int)CaretIndex) + inputText;
                    }
                }
                textBox.MoveCaret((uint)textBeforeCaret.Length);
                Application.ImeBuffer.Clear();
            }
            //Backspace, delete one character before the caret
            else if (Input.Keyboard.KeyPressed(Key.BackSpace, true))
            {
                if (CaretIndex != SelectIndex)
                {
                    var minIndex = (int)Math.Min(CaretIndex, SelectIndex);
                    var maxIndex = (int)Math.Max(CaretIndex, SelectIndex);

                    textBox.Text = textBox.Text.Substring(0, minIndex) + textBox.Text.Substring(maxIndex);
                    textBox.MoveCaret((uint)minIndex);
                    textBeforeCaret = textBox.Text.Substring(0, minIndex);
                    textBox.MoveCaret((uint)textBeforeCaret.Length);
                }
                else if (CaretIndex > 0)
                {
                    var newText = textBox.Text.Remove((int)(CaretIndex - 1), 1);
                    if (CaretIndex == 0)
                    {
                        textBeforeCaret = "";
                    }
                    else
                    {
                        textBeforeCaret = textBox.Text.Substring(0, (int)(CaretIndex - 1));
                    }
                    textBox.MoveCaret((uint)textBeforeCaret.Length);
                    textBox.Text = newText;
                }
            }
            //Delete, delete one character after the caret
            else if (Input.Keyboard.KeyPressed(Key.Delete, true))
            {
                if (CaretIndex != SelectIndex)
                {
                    var minIndex = (int)Math.Min(CaretIndex, SelectIndex);
                    var maxIndex = (int)Math.Max(CaretIndex, SelectIndex);

                    textBox.Text = textBox.Text.Substring(0, minIndex) + textBox.Text.Substring(maxIndex);
                    textBox.MoveCaret((uint)minIndex);
                    textBeforeCaret = textBox.Text.Substring(0, minIndex);
                    textBox.MoveCaret((uint)textBeforeCaret.Length);
                }
                else if (CaretIndex < textBox.Text.Length)
                {
                    textBox.Text = textBox.Text.Remove((int)CaretIndex, 1);
                }
            }
        }

        public void MoveCaret(uint index)
        {
            this.CaretIndex = this.SelectIndex = index;
        }

        public override void OnUpdate()
        {

#if INSPECT_STATE
            var A = stateMachine.CurrentState;
#endif
            bool insideRectLast = Rect.Contains(Utility.ScreenToClient(Input.Mouse.LastMousePos, Form));
            bool insideRectCurrent = Rect.Contains(Utility.ScreenToClient(Input.Mouse.MousePos, Form));
            bool insideRect = insideRectCurrent;

            //Execute state commands
            if (!insideRectLast && insideRectCurrent)
            {
                stateMachine.MoveNext(TextBoxCommand.MoveIn, this);
            }
            if (insideRectLast && !insideRectCurrent)
            {
                stateMachine.MoveNext(TextBoxCommand.MoveOut, this);
            }
            if (insideRectCurrent)
            {
                if(Input.Mouse.stateMachine.CurrentState == Input.Mouse.MouseState.Pressed)
                {
                    stateMachine.MoveNext(TextBoxCommand.EnterEdit, this);
                    stateMachine.MoveNext(TextBoxCommand.MoveCaret, this);
                }
                else
                {
                    if (Input.Mouse.LeftButtonState == InputState.Down && stateMachine.CurrentState != TextBoxState.ActiveSelecting)
                    {
                        stateMachine.MoveNext(TextBoxCommand.MoveCaret, this);
                        stateMachine.MoveNext(TextBoxCommand.BeginSelect, this);
                    }
                    if (Input.Mouse.LeftButtonState == InputState.Up)
                    {
                        stateMachine.MoveNext(TextBoxCommand.EndSelect, this);
                    }
                }
            }
            else
            {
                if(Input.Mouse.stateMachine.CurrentState == Input.Mouse.MouseState.Pressed)
                {
                    stateMachine.MoveNext(TextBoxCommand.ExitEditOut, this);
                }
            }

#if INSPECT_STATE
            var B = stateMachine.CurrentState;
            Debug.WriteLineIf(A != B,
                string.Format("TextBox {0} {1}=>{2} CaretIndex: {3}, SelectIndex: {4}",
                    Name, A, B, CaretIndex, SelectIndex));
#endif

            this.Form.Cursor = insideRect ? Cursor.Text : Cursor.Default;
            if (stateMachine.CurrentState == TextBoxState.Active)
            {
                stateMachine.MoveNext(TextBoxCommand.DoEdit, this);
            }
            if (stateMachine.CurrentState == TextBoxState.ActiveSelecting)
            {
                stateMachine.MoveNext(TextBoxCommand.DoSelect, this);
            }
            stateMachine.MoveNext(TextBoxCommand.MoveCaretKeyboard, this);

            var oldState = State;
            bool active = stateMachine.CurrentState.StartsWith("Active");
            bool hover = stateMachine.CurrentState == TextBoxState.Hover;
            if (active)
            {
                State = "Active";
                NeedRepaint = true;//TODO Render tree
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
        }

        public override void OnRender(Context g)
        {
            var style = Skin.current.TextBox[State];
            if(State == "Active")
            {
                //Calculate positions and sizes
                var contentRect = Utility.GetContentRect(Rect, style);
                var offsetOfTextRect = contentRect.TopLeft;
                float pointX, pointY;
                float caretHeight;
                TextContext.IndexToXY(CaretIndex, false, out pointX, out pointY, out caretHeight);
                caretTopPoint = new Point(pointX, pointY);
                caretBottomPoint = new Point(pointX, pointY + caretHeight);
                caretTopPoint.Offset(offsetOfTextRect.X, offsetOfTextRect.Y);
                caretBottomPoint.Offset(offsetOfTextRect.X, offsetOfTextRect.Y);

                var caretAlpha = (byte) (Application.Time%1060/1060.0f*255);
                caretAlpha = (byte) (caretAlpha < 100 ? 0 : 255);

                //FIXME: This is not working! Check if the caret is outside the rect. If so, move the text rect so the caret is shown.
                var textRect = contentRect;
                var caretX = caretTopPoint.X;
                if(caretX < textRect.X || caretX > textRect.Right)
                {
                    var offsetX = -(caretX - textRect.Width - Rect.X);
                    textRect.Offset(offsetX, 0);
                    caretTopPoint.Offset(offsetX, 0);
                    caretBottomPoint.Offset(offsetX, 0);
                }

                //Draw the box
                g.DrawBoxModel(Rect, null, style);

                //Clip the text
                g.MoveTo(Rect.TopLeft.ToPointD());
                g.LineTo(Rect.TopRight.ToPointD());
                g.LineTo(Rect.BottomRight.ToPointD());
                g.LineTo(Rect.BottomLeft.ToPointD());
                g.LineTo(Rect.TopLeft.ToPointD());
                g.ClosePath();
                g.Clip();
                g.ResetClip(); 

                //Draw text
                g.DrawText(textRect, TextContext, style.Font, style.TextStyle);

                //Draw selection rect
                if(SelectIndex != CaretIndex)
                {
                    float selectPointX, selectPointY, dummyHeight;
                    TextContext.IndexToXY(SelectIndex, false, out selectPointX, out selectPointY, out dummyHeight);
                    var selectionRect = new Rect(
                        new Point(pointX, pointY),
                        new Point(selectPointX, selectPointY + caretHeight));
                    selectionRect.Offset(offsetOfTextRect.X, offsetOfTextRect.Y);
                    g.FillRectangle(selectionRect,
                        CairoEx.ColorArgb(100, 100, 100, 100));
                }

                //Draw caret
                g.DrawLine(caretTopPoint, caretBottomPoint, 2.0f, CairoEx.ColorArgb(caretAlpha, 0, 0, 0));
            }
            else
            {
                g.DrawBoxModel(Rect, new Content(TextContext), style);
            }
            this.RenderRects.Add(Rect);//TODO construct a more specific render section
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

    }
}
