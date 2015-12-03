using System;
using System.Diagnostics;
using TinyIoC;
using Context = Cairo.Context;

//BUG The text will slightly move up when there are only chinese characters.
namespace IMGUI
{
    internal class TextBox : Control
    {
        private Point caretTopPoint;
        private Point caretBottomPoint;

        public ITextFormat Format { get; private set; }
        public ITextLayout Layout { get; private set; }
        public string Text { get; set; }

        public Point CaretPosition { get; set; }
        public bool Selecting { get; set; }

        public uint CaretIndex { get; set; }

        public uint SelectIndex { get; set; }

        public byte[] StringBytes
        {
            get { return System.Text.Encoding.UTF8.GetBytes(Text); }
        }

        public TextBox(string name, BaseForm form, string text, Rect rect)
            : base(name, form)
        {
            Rect = rect;
            Text = text;

            var font = Skin.current.TextBox[State].Font;
            Format = Application.IocContainer.Resolve<ITextFormat>(
                new NamedParameterOverloads
                    {
                        {"fontFamilyName", font.FontFamily},
                        {"fontWeight", font.FontWeight},
                        {"fontStyle", font.FontStyle},
                        {"fontStretch", font.FontStretch},
                        {"fontSize", (float) font.Size}
                    });
            var textStyle = Skin.current.TextBox[State].TextStyle;
            Format.Alignment = textStyle.TextAlignment;
            Layout = Application.IocContainer.Resolve<ITextLayout>(
                new NamedParameterOverloads
                    {
                        {"text", Text},
                        {"textFormat", Format},
                        {"maxWidth", (int)Rect.Width},
                        {"maxHeight", (int)Rect.Height}
                    });
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

        public override void OnUpdate()
        {
            bool insideRect = Rect.Contains(Input.Mouse.GetMousePos(Form));

            var oldState = State;
            if(State == "Active")
            {
                if(insideRect)
                {
                    this.Form.Cursor = Cursor.Text;
                }
                else
                {
                    this.Form.Cursor = Cursor.Default;
                }

                //Mouse left button clicked inside
                if(insideRect && Input.Mouse.LeftButtonClicked)
                {
                    var style = Skin.current.TextBox[State];
                    var contentRect = Utility.GetContentRect(Rect, style);
                    var offsetOfTextRect = contentRect.TopLeft;
                    uint caretIndex;
                    bool isInside;
                    caretIndex = Layout.XyToIndex((float)(Input.Mouse.GetMousePos(Form).X - offsetOfTextRect.X),
                        (float)(Input.Mouse.GetMousePos(Form).Y - offsetOfTextRect.Y), out isInside);
                    if (!isInside && caretIndex == Text.Length - 1)
                    {
                        ++caretIndex;
                    }
                    SelectIndex = CaretIndex = caretIndex;
                }
                else
                {
                    if(insideRect && Input.Mouse.MouseDraging) //Mouse left button draging inside
                    {
                        var style = Skin.current.TextBox[State];
                        var contentRect = Utility.GetContentRect(Rect, style);
                        var offsetOfTextRect = contentRect.TopLeft;
                        uint caretIndex;
                        bool isInside;
                        caretIndex = Layout.XyToIndex(
                            (float)(Input.Mouse.GetMousePos(Form).X - offsetOfTextRect.X),
                            (float)(Input.Mouse.GetMousePos(Form).Y - offsetOfTextRect.Y), out isInside);
                        if (!isInside && caretIndex == Text.Length-1)
                        {
                            ++caretIndex;
                        }
                        CaretIndex = caretIndex;
                    }
                    else
                    {
                        if(Input.Keyboard.KeyPressed(Key.Home))
                        {
                            CaretIndex = SelectIndex = 0;
                        }
                        if(Input.Keyboard.KeyPressed(Key.End))
                        {
                            CaretIndex = SelectIndex = (uint)Text.Length;
                        }
                        if(Input.Keyboard.KeyPressed(Key.Left, true))
                        {
                            if(CaretIndex > 0)
                            {
                                CaretIndex -= 1;
                            }
                            if(!Input.Keyboard.KeyDown(Key.LeftShift))
                            {
                                SelectIndex = CaretIndex;
                            }
                        }
                        else if(Input.Keyboard.KeyPressed(Key.Right, true))
                        {
                            if(CaretIndex < Text.Length)
                            {
                                CaretIndex += 1;
                            }
                            if(!Input.Keyboard.KeyDown(Key.LeftShift))
                            {
                                SelectIndex = CaretIndex;
                            }
                        }

                        if(Input.Mouse.LeftButtonClicked && !insideRect)
                        {
                            State = "Normal";
                        }
                        if(Input.Keyboard.KeyDown(Key.LeftShift) && !Selecting)
                        {
                            Selecting = true;
                            SelectIndex = CaretIndex;
                        }
                        if(!Input.Keyboard.KeyDown(Key.LeftShift) && Selecting)
                        {
                            Selecting = false;
                        }
                    }
                }

                {
                    string textBeforeCaret;

                    //Input characters
                    if(Application.ImeBuffer.Count != 0)
                    {
                        var inputText = new string(Application.ImeBuffer.ToArray());
                        if(CaretIndex != SelectIndex) //Replace selected text with inputText
                        {
                            //TODO check whether convert int and uint back and forth is appropriate
                            var minIndex = (int)Math.Min(CaretIndex, SelectIndex);
                            var maxIndex = (int)Math.Max(CaretIndex, SelectIndex);

                            Text = Text.Substring(0, minIndex) + inputText + Text.Substring(maxIndex);
                            CaretIndex = SelectIndex = (uint)minIndex;
                            textBeforeCaret = Text.Substring(0, minIndex) + inputText;
                        }
                        else //Insert inputText into caret position
                        {
                            if(Text.Length == 0)
                            {
                                Text = inputText;
                                textBeforeCaret = Text;
                            }
                            else
                            {
                                Text = Text.Substring(0, (int)CaretIndex) + inputText + Text.Substring((int)CaretIndex);
                                textBeforeCaret = Text.Substring(0, (int)CaretIndex) + inputText;
                            }
                        }
                        SelectIndex = CaretIndex = (uint)textBeforeCaret.Length;
                        Application.ImeBuffer.Clear();
                    }
                    //Backspace, delete one character before the caret
                    else if(Input.Keyboard.KeyPressed(Key.Back, true))
                    {
                        if(CaretIndex != SelectIndex)
                        {
                            var minIndex = (int)Math.Min(CaretIndex, SelectIndex);
                            var maxIndex = (int)Math.Max(CaretIndex, SelectIndex);

                            Text = Text.Substring(0, minIndex) + Text.Substring(maxIndex);
                            CaretIndex = SelectIndex = (uint)minIndex;
                            textBeforeCaret = Text.Substring(0, minIndex);
                            SelectIndex = CaretIndex = (uint)textBeforeCaret.Length;
                        }
                        else if(CaretIndex > 0)
                        {
                            var newText = Text.Remove((int)(CaretIndex - 1), 1);
                            if(CaretIndex == 0)
                            {
                                textBeforeCaret = "";
                            }
                            else
                            {
                                textBeforeCaret = Text.Substring(0, (int)(CaretIndex - 1));
                            }
                            SelectIndex = CaretIndex = (uint)textBeforeCaret.Length;
                            Text = newText;
                        }
                    }
                    //Delete, delete one character after the caret
                    else if(Input.Keyboard.KeyPressed(Key.Delete, true))
                    {
                        if(CaretIndex != SelectIndex)
                        {
                            var minIndex = (int)Math.Min(CaretIndex, SelectIndex);
                            var maxIndex = (int)Math.Max(CaretIndex, SelectIndex);

                            Text = Text.Substring(0, minIndex) + Text.Substring(maxIndex);
                            CaretIndex = SelectIndex = (uint)minIndex;
                            textBeforeCaret = Text.Substring(0, minIndex);
                            SelectIndex = CaretIndex = (uint)textBeforeCaret.Length;
                        }
                        else if(CaretIndex < Text.Length)
                        {
                            Text = Text.Remove((int)CaretIndex, 1);
                        }
                    }

                }

                //Update layout text
                Layout.Text = Text;
                NeedRepaint = true;
            }
            else
            {
                bool active = Input.Mouse.LeftButtonState == InputState.Down && insideRect;
                bool hover = Input.Mouse.LeftButtonState == InputState.Up && insideRect;
                if(active)
                    State = "Active";
                else if(hover)
                    State = "Hover";
                else
                    State = "Normal";
                if (State != oldState)
                {
                    NeedRepaint = true;
                }
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
                Layout.IndexToXY(CaretIndex, false, out pointX, out pointY, out caretHeight);
                caretTopPoint = new Point(pointX, pointY);
                caretBottomPoint = new Point(pointX, pointY + caretHeight);
                caretTopPoint.Offset(offsetOfTextRect.X, offsetOfTextRect.Y);
                caretBottomPoint.Offset(offsetOfTextRect.X, offsetOfTextRect.Y);

                //TODO Clean up this alpha mess
                var caretAlpha = (byte) (Utility.Millis%1060/1060.0f*255);
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
                g.DrawText(textRect, Layout, style.Font, style.TextStyle);

                //Draw selection rect
                if(SelectIndex != CaretIndex)
                {
                    float selectPointX, selectPointY, dummyHeight;
                    Layout.IndexToXY(SelectIndex, false, out selectPointX, out selectPointY, out dummyHeight);
                    var selectionRect = new Rect(
                        new Point(pointX, pointY),
                        new Point(selectPointX, selectPointY + caretHeight));
                    selectionRect.Offset(offsetOfTextRect.X, offsetOfTextRect.Y);
                    g.FillRectangle(selectionRect,
                        CairoEx.ColorArgb(100, 100, 100, 100));
                }

                //Draw caret
                g.DrawLine(caretTopPoint, caretBottomPoint, 1.0f, CairoEx.ColorArgb(caretAlpha, 0, 0, 0));
            }
            else
            {
                g.DrawBoxModel(Rect, new Content(Layout), style);
            }
        }

        public override void Dispose()
        {
            Layout.Dispose();
            Format.Dispose();
        }

        public override void OnClear(Context g)
        {
            g.FillRectangle(Rect, CairoEx.ColorWhite);
        }

    }
}
