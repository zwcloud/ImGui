using System;
using System.Diagnostics;
using System.Windows.Forms;
using Cairo;

namespace IMGUI
{
    internal class TextBox : Control
    {
        private int selectIndex;
        internal string Text { get; set; }
        internal Point CaretPosition { get; set; }
        internal int CaretIndex { get; set; }
        internal int CachedCaretIndex { get; set; }
        internal bool Selecting { get; set; }

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

        internal TextBox(string name)
            : base(name)
        {
            CaretIndex = 0;
            Controls[Name] = this;
        }

        internal static string DoControl(Context g, Rect rect, string text, string name)
        {
            #region Get control reference
            TextBox textBox;
            if(!Controls.ContainsKey(name))
            {
                textBox = new TextBox(name);
            }
            else
            {
                textBox = Controls[name] as TextBox;
            }

            Debug.Assert(textBox != null);
            #endregion

            #region Set control data
            textBox.Text = text;
            #endregion

            #region Logic

            bool insideRect = rect.Contains(Input.MousePos);

            if(textBox.State == "Active")
            {
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
            var style = Skin.current.TextBox[textBox.State];
            if(textBox.State == "Active")
            {
                g.DrawBoxModel(rect, new Content(text, textBox.CaretIndex, textBox.SelectIndex), style);
            }
            else
            {
                g.DrawBoxModel(rect, new Content(text), style);
            }
            #endregion

            return textBox.Text;
        }
    }
}
