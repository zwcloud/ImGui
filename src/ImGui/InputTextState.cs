using System;
using System.Threading.Tasks;
using ImGui.Common.Primitive;

namespace ImGui
{
    internal class InputTextState
    {
        #region State machine define
        private readonly string[] states = new string[11 * 3]
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

        private readonly Action<InputTextContext>[] callBacks = new Action<InputTextContext>[11]
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

        public StateMachineEx stateMachine;
        public InputTextContext inputTextContext;

        private static void MoveCaretCallBack(InputTextContext textBox)
        {
            var rect = textBox.Rect;
            var style = textBox.Style;
            ITextContext textContext = TextMeshUtil.GetTextContext(textBox.Text, rect.Size, style, GUIState.Normal);

            var contentRect = Utility.GetContentRect(rect, style);
            var mousePos = Input.Mouse.Position;
            var offsetOfTextRect = contentRect.TopLeft;
            uint caretIndex;
            bool isInside;
            caretIndex = textContext.XyToIndex(
                (float)(mousePos.X - offsetOfTextRect.X),
                (float)(mousePos.Y - offsetOfTextRect.Y), out isInside);
            textBox.SelectIndex = textBox.CaretIndex = caretIndex;
        }

        private static void MoveCaretKeyboardCallBack(InputTextContext textBox)
        {
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
                if (!Input.Keyboard.KeyDown(Key.LeftShift))
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
                if (!Input.Keyboard.KeyDown(Key.LeftShift))
                {
                    textBox.SelectIndex = textBox.CaretIndex;
                }
            }
        }

        private static void DoSelectCallBack(InputTextContext textBox)
        {
            var rect = textBox.Rect;
            var style = textBox.Style;
            var text = textBox.Text;
            var textContext = TextMeshUtil.GetTextContext(text, rect.Size, style, GUIState.Normal);

            var contentRect = Utility.GetContentRect(rect, style);
            var mousePos = Input.Mouse.Position;
            var offsetOfTextRect = contentRect.TopLeft;
            uint caretIndex;
            bool isInside;
            caretIndex = textContext.XyToIndex(
                (float)(mousePos.X - offsetOfTextRect.X),
                (float)(mousePos.Y - offsetOfTextRect.Y), out isInside);
            textBox.CaretIndex = caretIndex;
        }

        private static Task<string> keyboardTask;
        private static void DoEditCallBack(InputTextContext textBox)
        {
            if(CurrentOS.IsDesktopPlatform)
            {
                var CaretIndex = textBox.CaretIndex;
                var SelectIndex = textBox.SelectIndex;

                string textBeforeCaret;
                //Input characters
                if (Input.ImeBuffer.Count != 0)
                {
                    var inputText = new string(Input.ImeBuffer.ToArray());
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
                    Input.ImeBuffer.Clear();
                }
                //Backspace, delete one character before the caret
                else if (Input.Keyboard.KeyPressed(Key.Back, true))
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
            else
            {
                if (keyboardTask == null)
                {
                    keyboardTask = Keyboard.Show(textBox.Text);
                }
                else
                {
                    if (keyboardTask.IsCompleted)
                    {
                        var inputText = keyboardTask.Result;
                        textBox.Text = inputText;
                        keyboardTask = null;

                        // deactive the textbox we are editing
                        var g = Form.current.uiContext;
                        g.SetActiveID(0);
                    }
                }
            }
        }

        public InputTextState()
        {
            stateMachine = new StateMachineEx(TextBoxState.Normal, states, callBacks);
            inputTextContext = new InputTextContext();
        }
    }

    internal class InputTextContext
    {
        private uint caretIndex;
        private uint selectIndex;

        public int Id { get; set; }

        public Rect Rect { get; set; }

        public string Text { get; set; }

        public GUIStyle Style { get; set; }

        public Point CaretPosition { get; set; }

        public bool Selecting { get; set; }

        public uint CaretIndex
        {
            get { return caretIndex; }
            set
            {
                if (value != caretIndex)
                {
                    HoldAlpha = true;
                    caretIndex = value;
                }
            }
        }

        public uint SelectIndex
        {
            get { return selectIndex; }
            set
            {
                if (value != selectIndex)
                {
                    selectIndex = value;
                }
            }
        }

        public void MoveCaret(uint index)
        {
            this.CaretIndex = this.SelectIndex = index;
        }

        public byte CaretAlpha
        {
            get
            {
                if (HoldAlpha && holdingTime < 100)
                {
                    holdingTime += Application.DeltaTime;
                    return 255;
                }

                HoldAlpha = false;
                holdingTime = 0;
                var result = (byte)(Application.Time % 1060 / 1060.0f * 255);
                result = (byte)(result < 100 ? 0 : 255);
                return result;
            }
        }

        private bool HoldAlpha { get; set; }
        private double holdingTime;
    }
}