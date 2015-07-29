using System;
using System.Diagnostics;
using System.Windows.Forms;
using Layout = Pango.Layout;
using Context = Cairo.Context;

//TODO Check if Pango.Layout is automatically released
namespace IMGUI
{
    internal class TextBox : Control
    {
        private int selectIndex;
        internal string Text { get; set; }
        internal Point CaretPosition { get; set; }
        internal int CaretIndex { get; set; }
        internal bool Selecting { get; set; }

        internal Layout Layout { get; set; }

        internal int SelectIndex
        {
            get { return selectIndex; }
            set
            {
                if(value < 0)
                {
                    selectIndex = 0;
                }
                else if(value>Text.Length)
                {
                    selectIndex = Text.Length;
                }
                else
                {
                    selectIndex = value;
                }
            }
        }

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
                textBox.Text = text;
                textBox.Layout.SetText(text);
                textBox.Layout.FontDescription = style.Font.Description;
                Pango.CairoHelper.UpdateLayout(g, textBox.Layout);
                textBox.Layout.Alignment = style.TextStyle.TextAlign;
                textBox.Layout.Width = (int)(contentRect.Width * Pango.Scale.PangoScale);
            }
            #endregion

            #region Logic

            bool insideRect = rect.Contains(Input.MousePos);

            bool usingMouse = true;
            if(textBox.State == "Active")
            {
                if(insideRect
                    && (Input.LeftButtonClicked || Input.LeftButtonState == InputState.Down))//Using mouse
                {
                    usingMouse = true;
                }
                else//Using keyboard
                {
                    usingMouse = false;
                    if(Input.KeyPressed(Key.Left))
                    {
                        if(textBox.CaretIndex > 0)
                        {
                            --textBox.CaretIndex;
                        }
                        if(!Input.KeyDown(Key.LeftShift))
                        {
                            textBox.SelectIndex = textBox.CaretIndex;
                        }
                    }
                    else if(Input.KeyPressed(Key.Right))
                    {
                        if(textBox.CaretIndex < text.Length)
                        {
                            ++textBox.CaretIndex;
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

                    //Draw selection rect
                    if(textBox.SelectIndex != int.MaxValue)
                    {
                        Pango.Rectangle rightStrongRect, rightRect;
                        textBox.Layout.GetCursorPos(textBox.SelectIndex, out rightStrongRect, out rightRect);

                        Pango.Rectangle leftStrongRect, leftRect;
                        textBox.Layout.GetCursorPos(textBox.CaretIndex, out leftStrongRect, out leftRect);

                        var selectionRect = new Rect(
                            new Point(Pango.Units.ToPixels(leftRect.X), Pango.Units.ToPixels(leftRect.Y)),
                            new Point(Pango.Units.ToPixels(rightRect.X), Pango.Units.ToPixels(rightRect.Y + rightRect.Height))
                            );
                        selectionRect.Offset(offsetOfTextRect.X, offsetOfTextRect.Y);
                        g.FillRectangle(selectionRect,
                            CairoEx.ColorArgb(100, 100, 100, 100));
                    }

                    //Draw caret
                    Pango.Rectangle strongCursorPosFromPango, weakCursorPosFromPango;
                    textBox.Layout.GetCursorPos(textBox.CaretIndex,
                        out strongCursorPosFromPango, out weakCursorPosFromPango);
                    var caretTopPoint = new Point(Pango.Units.ToPixels(strongCursorPosFromPango.X), Pango.Units.ToPixels(strongCursorPosFromPango.Y));
                    var caretBottomPoint = new Point(Pango.Units.ToPixels(strongCursorPosFromPango.X), Pango.Units.ToPixels(strongCursorPosFromPango.Y + strongCursorPosFromPango.Height));
                    caretTopPoint.Offset(offsetOfTextRect.X, offsetOfTextRect.Y);
                    caretBottomPoint.Offset(offsetOfTextRect.X, offsetOfTextRect.Y);
                    //TODO Clean up this alpha mess
                    var caretAlpha = (byte)(Utility.Millis % 1060 / 1060.0f * 255);
                    caretAlpha = (byte)(caretAlpha < 100 ? 0 : 255);
                    g.DrawLine(caretTopPoint, caretBottomPoint, 1.0f, CairoEx.ColorArgb(caretAlpha, 0, 0, 0));
                }
                else
                {
                    g.DrawBoxModel(rect, new Content(text), style);
                }
            }
            #endregion

            return textBox.Text;
        }
    }
}
