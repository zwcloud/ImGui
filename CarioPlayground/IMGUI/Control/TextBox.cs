using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Layout = Pango.Layout;
using Context = Cairo.Context;

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
        internal int CaretIndex { get; set; }
        internal int SelectIndex { get; set; }
        internal bool Selecting { get; set; }


        internal TextBox(string name, Context g)
            : base(name)
        {
            CaretIndex = 0;
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

            bool insideRect = rect.Contains(Input.MousePos);

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
                if(insideRect && Input.LeftButtonClicked)
                {
                    var style = Skin.current.TextBox[textBox.State];
                    var contentRect = Utility.GetContentRect(rect, style);
                    var offsetOfTextRect = contentRect.TopLeft;
                    int caretIndex, trailing;
                    textBox.Layout.XyToIndex(
                        Pango.Units.FromDouble(Input.MousePos.X - offsetOfTextRect.X),
                        Pango.Units.FromDouble(Input.MousePos.Y - offsetOfTextRect.Y),
                        out caretIndex, out trailing);
                    textBox.SelectIndex = textBox.CaretIndex = caretIndex + trailing;
                }
                else
                {
                    if(insideRect && Input.MouseDraging)//Mouse left button draging inside
                    {
                        var style = Skin.current.TextBox[textBox.State];
                        var contentRect = Utility.GetContentRect(rect, style);
                        var offsetOfTextRect = contentRect.TopLeft;
                        int caretIndex, trailing;
                        textBox.Layout.XyToIndex(
                            Pango.Units.FromDouble(Input.MousePos.X - offsetOfTextRect.X),
                            Pango.Units.FromDouble(Input.MousePos.Y - offsetOfTextRect.Y),
                            out caretIndex, out trailing);
                        textBox.CaretIndex = caretIndex + trailing;
                    }
                    else
                    {
                        byte[] stringBytes = System.Text.Encoding.UTF8.GetBytes(textBox.Text);
                        int charCountBeforeCaret = System.Text.Encoding.UTF8.GetCharCount(stringBytes, 0, textBox.CaretIndex);
                        if(Input.KeyPressed(Key.Left))
                        {
                            if(textBox.CaretIndex > 0)
                            {
                                textBox.CaretIndex -= System.Text.Encoding.UTF8.GetByteCount(textBox.Text.Substring(charCountBeforeCaret - 1, 1));
                            }
                            if(!Input.KeyDown(Key.LeftShift))
                            {
                                textBox.SelectIndex = textBox.CaretIndex;
                            }
                        }
                        else if(Input.KeyPressed(Key.Right))
                        {
                            if(textBox.CaretIndex < System.Text.Encoding.UTF8.GetByteCount(textBox.Text))
                            {
                                textBox.CaretIndex += System.Text.Encoding.UTF8.GetByteCount(textBox.Text.Substring(charCountBeforeCaret, 1));
                            }
                            if(!Input.KeyDown(Key.LeftShift))
                            {
                                textBox.SelectIndex = textBox.CaretIndex;
                            }
                        }

                        if(Input.LeftButtonClicked && !insideRect)
                        {
                            textBox.State = "Normal";
                        }
                        if(Input.KeyDown(Key.LeftShift) && !textBox.Selecting)
                        {
                            textBox.Selecting = true;
                            textBox.SelectIndex = textBox.CaretIndex;
                        }
                        if(!Input.KeyDown(Key.LeftShift) && textBox.Selecting)
                        {
                            textBox.Selecting = false;
                        }
                    }
                }

                //TODO more properly approach these UTF8 and Unicode affairs
                {
                    string textBeforeCaret;
                    byte[] stringBytes = System.Text.Encoding.UTF8.GetBytes(textBox.Text);
                    int charCountBeforeCaret = System.Text.Encoding.UTF8.GetCharCount(stringBytes, 0, textBox.CaretIndex);

                    //Input characters
                    if(Application.ImeBuffer.Count != 0)
                    {
                        var inputText = new string(Application.ImeBuffer.ToArray());
                        if(textBox.CaretIndex != textBox.SelectIndex)//Replace selected text with inputText
                        {
                            var minByteIndex = Math.Min(textBox.CaretIndex, textBox.SelectIndex);
                            var maxByteIndex = Math.Max(textBox.CaretIndex, textBox.SelectIndex);
                            var minIndex = System.Text.Encoding.UTF8.GetCharCount(stringBytes, 0, minByteIndex);
                            var maxIndex = System.Text.Encoding.UTF8.GetCharCount(stringBytes, 0, maxByteIndex);

                            textBox.Text = textBox.Text.Substring(0, minIndex) + inputText + textBox.Text.Substring(maxIndex);
                            textBox.CaretIndex = textBox.SelectIndex = minIndex;
                            textBeforeCaret = textBox.Text.Substring(0, minIndex) + inputText;
                        }
                        else//Insert inputText into caret position
                        {
                            textBox.Text = textBox.Text.Substring(0, charCountBeforeCaret) + inputText + textBox.Text.Substring(charCountBeforeCaret);
                            textBeforeCaret = textBox.Text.Substring(0, charCountBeforeCaret) + inputText;
                        }
                        textBox.SelectIndex = textBox.CaretIndex = System.Text.Encoding.UTF8.GetByteCount(textBeforeCaret);
                        Application.ImeBuffer.Clear();
                    }
                    //Backspace, delete one character before the caret
                    else if(Input.KeyPressed(Key.Back))
                    {
                        if(textBox.CaretIndex != textBox.SelectIndex)
                        {
                            var minByteIndex = Math.Min(textBox.CaretIndex, textBox.SelectIndex);
                            var maxByteIndex = Math.Max(textBox.CaretIndex, textBox.SelectIndex);
                            var minIndex = System.Text.Encoding.UTF8.GetCharCount(stringBytes, 0, minByteIndex);
                            var maxIndex = System.Text.Encoding.UTF8.GetCharCount(stringBytes, 0, maxByteIndex);

                            textBox.Text = textBox.Text.Substring(0, minIndex) + textBox.Text.Substring(maxIndex);
                            textBox.CaretIndex = textBox.SelectIndex = minIndex;
                            textBeforeCaret = textBox.Text.Substring(0, minIndex);
                            textBox.SelectIndex = textBox.CaretIndex = System.Text.Encoding.UTF8.GetByteCount(textBeforeCaret);
                        }
                        else if(textBox.CaretIndex > 0)
                        {
                            textBox.Text = textBox.Text.Remove(charCountBeforeCaret - 1, 1);
                            textBeforeCaret = textBox.Text.Substring(0, charCountBeforeCaret-1);
                            textBox.SelectIndex = textBox.CaretIndex = System.Text.Encoding.UTF8.GetByteCount(textBeforeCaret);
                        }
                    }
                    //Delete, delete one character after the caret
                    else if(Input.KeyPressed(Key.Delete))
                    {
                        if(textBox.CaretIndex != textBox.SelectIndex)
                        {
                            var minByteIndex = Math.Min(textBox.CaretIndex, textBox.SelectIndex);
                            var maxByteIndex = Math.Max(textBox.CaretIndex, textBox.SelectIndex);
                            var minIndex = System.Text.Encoding.UTF8.GetCharCount(stringBytes, 0, minByteIndex);
                            var maxIndex = System.Text.Encoding.UTF8.GetCharCount(stringBytes, 0, maxByteIndex);

                            textBox.Text = textBox.Text.Substring(0, minIndex) + textBox.Text.Substring(maxIndex);
                            textBox.CaretIndex = textBox.SelectIndex = minIndex;
                            textBeforeCaret = textBox.Text.Substring(0, minIndex);
                            textBox.SelectIndex = textBox.CaretIndex = System.Text.Encoding.UTF8.GetByteCount(textBeforeCaret);
                        }
                        else if(textBox.CaretIndex < System.Text.Encoding.UTF8.GetByteCount(textBox.Text))
                        {
                            textBox.Text = textBox.Text.Remove(charCountBeforeCaret, 1);
                        }
                    }

                    //Update pango layout
                    textBox.Layout.SetText(textBox.Text);
                    Pango.CairoHelper.UpdateLayout(g, textBox.Layout);
                }
            }
            else
            {
                bool active = Input.LeftButtonState == InputState.Down && insideRect;
                bool hover = Input.LeftButtonState == InputState.Up && insideRect;
                if(active)
                {
                    textBox.State = "Active";
                }
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
                    var contentRect = Utility.GetContentRect(rect, style);
                    var offsetOfTextRect = contentRect.TopLeft;
                    //Draw text
                    g.DrawBoxModel(rect, new Content(textBox.Layout), style);

                    //TODO weak pos from Pango not used (check if it is really useless)

                    //Draw selection rect
                    if(textBox.SelectIndex != int.MaxValue)
                    {
                        //TODO combine rightStrongRect with strongCursorPosFromPango
                        Pango.Rectangle rightStrongRect, rightWeakRect;
                        textBox.Layout.GetCursorPos(textBox.SelectIndex, out rightStrongRect, out rightWeakRect);

                        Pango.Rectangle leftStrongRect, leftWeakRect;
                        textBox.Layout.GetCursorPos(textBox.CaretIndex, out leftStrongRect, out leftWeakRect);

                        var selectionRect = new Rect(
                            new Point(Pango.Units.ToPixels(leftStrongRect.X), Pango.Units.ToPixels(leftStrongRect.Y)),
                            new Point(Pango.Units.ToPixels(rightStrongRect.X), Pango.Units.ToPixels(rightStrongRect.Y + rightStrongRect.Height))
                            );
                        selectionRect.Offset(offsetOfTextRect.X, offsetOfTextRect.Y);
                        g.FillRectangle(selectionRect,
                            CairoEx.ColorArgb(100, 100, 100, 100));
                    }

                    //Draw caret
                    Pango.Rectangle strongCursorPosFromPango, weakCursorPosFromPango;
                    textBox.Layout.GetCursorPos(textBox.CaretIndex,
                        out strongCursorPosFromPango, out weakCursorPosFromPango);
                    textBox.caretTopPoint = new Point(Pango.Units.ToPixels(strongCursorPosFromPango.X), Pango.Units.ToPixels(strongCursorPosFromPango.Y));
                    textBox.caretBottomPoint = new Point(Pango.Units.ToPixels(strongCursorPosFromPango.X), Pango.Units.ToPixels(strongCursorPosFromPango.Y + strongCursorPosFromPango.Height));
                    textBox.caretTopPoint.Offset(offsetOfTextRect.X, offsetOfTextRect.Y);
                    textBox.caretBottomPoint.Offset(offsetOfTextRect.X, offsetOfTextRect.Y);
                    //TODO Clean up this alpha mess
                    var caretAlpha = (byte)(Utility.Millis % 1060 / 1060.0f * 255);
                    caretAlpha = (byte)(caretAlpha < 100 ? 0 : 255);
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
