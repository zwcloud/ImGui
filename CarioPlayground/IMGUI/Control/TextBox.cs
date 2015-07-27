using System.Diagnostics;
using System.Threading;
using System.Windows.Forms.VisualStyles;
using Cairo;

namespace IMGUI
{
    internal class TextBox : Control
    {
        internal string Text { get; set; }
        internal float Scroll { get; set; }
        internal Point CaretPosition { get; set; }
        internal int CaretIndex { get; set; }

        internal TextBox(string name)
            : base(name)
        {
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

            if(Input.LeftButtonState == InputState.Down && insideRect)
            {
                textBox.State = "Active";
            }
            if(textBox.State == "Active")
            {
                if(Input.LeftButtonClicked && !insideRect)
                {
                    textBox.State = "Normal";
                }

                if(Input.KeyPressed(Key.Left))
                {
                    if(textBox.CaretIndex > 0)
                    {
                        --textBox.CaretIndex;
                    }
                }
                else if(Input.KeyPressed(Key.Right))
                {
                    if(textBox.CaretIndex < text.Length)
                    {
                        ++textBox.CaretIndex;
                    }
                }
                textBox.Scroll = 0;
            }
            else
            {
                bool hover = Input.LeftButtonState == InputState.Up && rect.Contains(Input.MousePos);
                if(hover)
                    textBox.State = "Hover";
                else
                    textBox.State = "Normal";
            }
            #endregion

            #region Draw
            var style = Skin.current.TextBox[textBox.State];
            if(textBox.State == "Active")
            {
                g.DrawBoxModel(rect, new Content(text, textBox.CaretIndex), style);
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
