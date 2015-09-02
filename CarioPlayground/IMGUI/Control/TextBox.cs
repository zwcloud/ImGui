using System;
using System.Diagnostics;
using Layout = Pango.Layout;
using Context = Cairo.Context;

//BUG The text will slightly move up when there are only chinese characters.
//TODO Check if Pango.Layout is automatically released
namespace IMGUI
{
    internal class TextBox : Control
    {
        private Point caretTopPoint;
        private Point caretBottomPoint;

        internal string Text { get; set; }
        internal Layout Layout { get; set; }
        internal Point CaretPosition { get; set; }
        internal int CaretByteIndex { get; set; }
        internal int SelectByteIndex { get; set; }
        internal bool Selecting { get; set; }

        internal int CaretIndex
        {
            get
            {
                if(StringBytes.Length==0)
                {
                    return 0;
                }
                return System.Text.Encoding.UTF8.GetCharCount(StringBytes, 0,
                    CaretByteIndex);
            }
        }

        internal int SelectIndex
        {
            get
            {
                if(StringBytes.Length == 0)
                {
                    return 0;
                }
                return System.Text.Encoding.UTF8.GetCharCount(StringBytes, 0,
                    SelectByteIndex);
            }
        }

        internal byte[] StringBytes
        {
            get { return System.Text.Encoding.UTF8.GetBytes(Text); }
        }

        internal TextBox(string name, Context g)
            : base(name)
        {
            CaretByteIndex = 0;
            //Unique layout for this TextBox
            Layout = Pango.CairoHelper.CreateLayout(g);
            Controls[Name] = this;
        }

        internal static string DoControl(Context g, Rect rect, string text, string name)
        {
            #region Get control reference
            TextBox textBox;
            if(!Controls.ContainsKey(name))
            {
                textBox = new TextBox(name, g);
                textBox.Text = text;
            }
            else
            {
                textBox = Controls[name] as TextBox;
            }

            Debug.Assert(textBox != null);
            #endregion

            #region Set control data
            {
                var style = Skin.current.TextBox[textBox.State];
                var contentRect = Utility.GetContentRect(rect, style);
                textBox.Layout.SetText(textBox.Text);
                textBox.Layout.FontDescription = style.Font.Description;
                Pango.CairoHelper.UpdateLayout(g, textBox.Layout);
                textBox.Layout.Alignment = style.TextStyle.TextAlign;
                textBox.Layout.Width = (int)(contentRect.Width * Pango.Scale.PangoScale);
            }
            #endregion

            #region Logic

            bool insideRect = rect.Contains(Input.Mouse.MousePos);

            if(textBox.State == "Active")
            {
                if(insideRect)
                {
                    var activeForm = (Form)System.Windows.Forms.Form.ActiveForm;
                    if(activeForm != null)
                        activeForm.Cursor = Cursor.Text;
                }
                else
                {
                    var activeForm = (Form)System.Windows.Forms.Form.ActiveForm;
                    if(activeForm != null)
                        activeForm.Cursor = Cursor.Default;
                }

                //Mouse left button clicked inside
                if(insideRect && Input.Mouse.LeftButtonClicked)
                {
                    var style = Skin.current.TextBox[textBox.State];
                    var contentRect = Utility.GetContentRect(rect, style);
                    var offsetOfTextRect = contentRect.TopLeft;
                    int caretIndex, trailing;
                    textBox.Layout.XyToIndex(
                        Pango.Units.FromDouble(Input.Mouse.MousePos.X - offsetOfTextRect.X),
                        Pango.Units.FromDouble(Input.Mouse.MousePos.Y - offsetOfTextRect.Y),
                        out caretIndex, out trailing);
                    textBox.SelectByteIndex = textBox.CaretByteIndex = caretIndex + trailing;
                }
                else
                {
                    if(insideRect && Input.Mouse.MouseDraging)//Mouse left button draging inside
                    {
                        var style = Skin.current.TextBox[textBox.State];
                        var contentRect = Utility.GetContentRect(rect, style);
                        var offsetOfTextRect = contentRect.TopLeft;
                        int caretIndex, trailing;
                        textBox.Layout.XyToIndex(
                            Pango.Units.FromDouble(Input.Mouse.MousePos.X - offsetOfTextRect.X),
                            Pango.Units.FromDouble(Input.Mouse.MousePos.Y - offsetOfTextRect.Y),
                            out caretIndex, out trailing);
                        textBox.CaretByteIndex = caretIndex + trailing;
                    }
                    else
                    {
                        if(Input.Keyboard.KeyPressed(Key.Home))
                        {
                            textBox.CaretByteIndex = textBox.SelectByteIndex = 0;
                        }
                        if (Input.Keyboard.KeyPressed(Key.End))
                        {
                            textBox.CaretByteIndex = textBox.SelectByteIndex = textBox.StringBytes.Length;
                        }
                        if(Input.Keyboard.KeyPressed(Key.Left, true))
                        {
                            if(textBox.CaretByteIndex > 0)
                            {
                                textBox.CaretByteIndex -= System.Text.Encoding.UTF8.GetByteCount(textBox.Text.Substring(textBox.CaretIndex - 1, 1));
                            }
                            if(!Input.Keyboard.KeyDown(Key.LeftShift))
                            {
                                textBox.SelectByteIndex = textBox.CaretByteIndex;
                            }
                        }
                        else if (Input.Keyboard.KeyPressed(Key.Right, true))
                        {
                            if(textBox.CaretByteIndex < System.Text.Encoding.UTF8.GetByteCount(textBox.Text))
                            {
                                textBox.CaretByteIndex += System.Text.Encoding.UTF8.GetByteCount(textBox.Text.Substring(textBox.CaretIndex, 1));
                            }
                            if(!Input.Keyboard.KeyDown(Key.LeftShift))
                            {
                                textBox.SelectByteIndex = textBox.CaretByteIndex;
                            }
                        }

                        if(Input.Mouse.LeftButtonClicked && !insideRect)
                        {
                            textBox.State = "Normal";
                        }
                        if(Input.Keyboard.KeyDown(Key.LeftShift) && !textBox.Selecting)
                        {
                            textBox.Selecting = true;
                            textBox.SelectByteIndex = textBox.CaretByteIndex;
                        }
                        if(!Input.Keyboard.KeyDown(Key.LeftShift) && textBox.Selecting)
                        {
                            textBox.Selecting = false;
                        }
                    }
                }

                //TODO more properly approach these UTF8 and Unicode affairs
                {
                    string textBeforeCaret;
                    byte[] stringBytes = System.Text.Encoding.UTF8.GetBytes(textBox.Text);

                    //Input characters
                    if(Application.ImeBuffer.Count != 0)
                    {
                        var inputText = new string(Application.ImeBuffer.ToArray());
                        if(textBox.CaretByteIndex != textBox.SelectByteIndex)//Replace selected text with inputText
                        {
                            var minByteIndex = Math.Min(textBox.CaretByteIndex, textBox.SelectByteIndex);
                            var maxByteIndex = Math.Max(textBox.CaretByteIndex, textBox.SelectByteIndex);
                            var minIndex = System.Text.Encoding.UTF8.GetCharCount(textBox.StringBytes, 0, minByteIndex);
                            var maxIndex = System.Text.Encoding.UTF8.GetCharCount(textBox.StringBytes, 0, maxByteIndex);

                            textBox.Text = textBox.Text.Substring(0, minIndex) + inputText + textBox.Text.Substring(maxIndex);
                            textBox.CaretByteIndex = textBox.SelectByteIndex = minIndex;
                            textBeforeCaret = textBox.Text.Substring(0, minIndex) + inputText;
                        }
                        else//Insert inputText into caret position
                        {
                            textBox.Text = textBox.Text.Substring(0, textBox.CaretIndex) + inputText + textBox.Text.Substring(textBox.CaretIndex);
                            textBeforeCaret = textBox.Text.Substring(0, textBox.CaretIndex) + inputText;
                        }
                        textBox.SelectByteIndex = textBox.CaretByteIndex = System.Text.Encoding.UTF8.GetByteCount(textBeforeCaret);
                        Application.ImeBuffer.Clear();
                    }
                    //Backspace, delete one character before the caret
                    else if(Input.Keyboard.KeyPressed(Key.Back, true))
                    {
                        if(textBox.CaretByteIndex != textBox.SelectByteIndex)
                        {
                            var minByteIndex = Math.Min(textBox.CaretByteIndex, textBox.SelectByteIndex);
                            var maxByteIndex = Math.Max(textBox.CaretByteIndex, textBox.SelectByteIndex);
                            var minIndex = System.Text.Encoding.UTF8.GetCharCount(textBox.StringBytes, 0, minByteIndex);
                            var maxIndex = System.Text.Encoding.UTF8.GetCharCount(textBox.StringBytes, 0, maxByteIndex);

                            textBox.Text = textBox.Text.Substring(0, minIndex) + textBox.Text.Substring(maxIndex);
                            textBox.CaretByteIndex = textBox.SelectByteIndex = minIndex;
                            textBeforeCaret = textBox.Text.Substring(0, minIndex);
                            textBox.SelectByteIndex = textBox.CaretByteIndex = System.Text.Encoding.UTF8.GetByteCount(textBeforeCaret);
                        }
                        else if(textBox.CaretByteIndex > 0)
                        {
                            var newText = textBox.Text.Remove(textBox.CaretIndex - 1, 1);
                            if(textBox.CaretIndex==0)
                            {
                                textBeforeCaret = "";
                            }
                            else
                            {
                                textBeforeCaret = textBox.Text.Substring(0, textBox.CaretIndex - 1);
                            }
                            textBox.SelectByteIndex = textBox.CaretByteIndex = System.Text.Encoding.UTF8.GetByteCount(textBeforeCaret);
                            textBox.Text = newText;
                        }
                    }
                    //Delete, delete one character after the caret
                    else if (Input.Keyboard.KeyPressed(Key.Delete, true))
                    {
                        if(textBox.CaretByteIndex != textBox.SelectByteIndex)
                        {
                            var minByteIndex = Math.Min(textBox.CaretByteIndex, textBox.SelectByteIndex);
                            var maxByteIndex = Math.Max(textBox.CaretByteIndex, textBox.SelectByteIndex);
                            var minIndex = System.Text.Encoding.UTF8.GetCharCount(textBox.StringBytes, 0, minByteIndex);
                            var maxIndex = System.Text.Encoding.UTF8.GetCharCount(textBox.StringBytes, 0, maxByteIndex);

                            textBox.Text = textBox.Text.Substring(0, minIndex) + textBox.Text.Substring(maxIndex);
                            textBox.CaretByteIndex = textBox.SelectByteIndex = minIndex;
                            textBeforeCaret = textBox.Text.Substring(0, minIndex);
                            textBox.SelectByteIndex = textBox.CaretByteIndex = System.Text.Encoding.UTF8.GetByteCount(textBeforeCaret);
                        }
                        else if(textBox.CaretByteIndex < System.Text.Encoding.UTF8.GetByteCount(textBox.Text))
                        {
                            textBox.Text = textBox.Text.Remove(textBox.CaretIndex, 1);
                        }
                    }

                    //Update pango layout
                    textBox.Layout.SetText(textBox.Text);
                    Pango.CairoHelper.UpdateLayout(g, textBox.Layout);
                }
            }
            else
            {
                bool active = Input.Mouse.LeftButtonState == InputState.Down && insideRect;
                bool hover = Input.Mouse.LeftButtonState == InputState.Up && insideRect;
                if(active)
                    textBox.State = "Active";
                else if(hover)
                    textBox.State = "Hover";
                else
                    textBox.State = "Normal";
            }
            #endregion

            #region Draw
            {
                var style = Skin.current.TextBox[textBox.State];
                if(textBox.State == "Active")
                {
                    //Calculate positions and sizes
                    var contentRect = Utility.GetContentRect(rect, style);
                    var offsetOfTextRect = contentRect.TopLeft;
                    Pango.Rectangle strongCursorPosFromPango, weakCursorPosFromPango;
                    textBox.Layout.GetCursorPos(textBox.CaretByteIndex, out strongCursorPosFromPango, out weakCursorPosFromPango);
                    textBox.caretTopPoint = new Point(Pango.Units.ToPixels(strongCursorPosFromPango.X), Pango.Units.ToPixels(strongCursorPosFromPango.Y));
                    textBox.caretBottomPoint = new Point(Pango.Units.ToPixels(strongCursorPosFromPango.X), Pango.Units.ToPixels(strongCursorPosFromPango.Y + strongCursorPosFromPango.Height));
                    textBox.caretTopPoint.Offset(offsetOfTextRect.X, offsetOfTextRect.Y);
                    textBox.caretBottomPoint.Offset(offsetOfTextRect.X, offsetOfTextRect.Y);

                    //TODO Clean up this alpha mess
                    var caretAlpha = (byte)(Utility.Millis % 1060 / 1060.0f * 255);
                    caretAlpha = (byte)(caretAlpha < 100 ? 0 : 255);

                    //Check if the caret is outside the rect. If so, move the text rect so the caret is shown.
                    var textRect = contentRect;
                    var caretX = textBox.caretTopPoint.X;
                    if(caretX < textRect.X || caretX > textRect.Right)
                    {
                        var offsetX = -(caretX - textRect.Width - rect.X);
                        textRect.Offset(offsetX, 0);
                        textBox.caretTopPoint.Offset(offsetX, 0);
                        textBox.caretBottomPoint.Offset(offsetX, 0);
                    }

                    //Draw the box
                    g.DrawBoxModel(rect, null, style);

                    //Clip the text
                    g.MoveTo(rect.TopLeft);
                    g.LineTo(rect.TopRight);
                    g.LineTo(rect.BottomRight);
                    g.LineTo(rect.BottomLeft);
                    g.LineTo(rect.TopLeft);
                    g.ClosePath();
                    g.Clip();

                    //Draw text
                    g.DrawText(textRect, textBox.Layout, style.Font);

                    //TODO weak pos from Pango not used (check if it is really useless)

                    //Draw selection rect
                    if(textBox.SelectByteIndex != int.MaxValue)
                    {
                        Pango.Rectangle rightStrongRect, rightWeakRect;
                        textBox.Layout.GetCursorPos(textBox.SelectByteIndex, out rightStrongRect, out rightWeakRect);
                        var selectionRect = new Rect(
                            new Point(Pango.Units.ToPixels(strongCursorPosFromPango.X), Pango.Units.ToPixels(strongCursorPosFromPango.Y)),
                            new Point(Pango.Units.ToPixels(rightStrongRect.X), Pango.Units.ToPixels(rightStrongRect.Y + rightStrongRect.Height))
                            );
                        selectionRect.Offset(offsetOfTextRect.X, offsetOfTextRect.Y);
                        g.FillRectangle(selectionRect,
                            CairoEx.ColorArgb(100, 100, 100, 100));
                    }

                    //Draw caret
                    g.DrawLine(textBox.caretTopPoint, textBox.caretBottomPoint, 1.0f, CairoEx.ColorArgb(caretAlpha, 0, 0, 0));
                }
                else
                {
                    g.DrawBoxModel(rect, new Content(textBox.Layout), style);
                }
            }
            #endregion

            return textBox.Text;
        }
    }
}
