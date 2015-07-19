using System.Diagnostics;
using Cairo;

namespace IMGUI
{
    internal class TextBox : Control
    {
        internal string Text { get; set; }

        internal TextBox(string name)
            : base(name)
        {
            
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

            #endregion

            #region Draw

            #endregion

            return textBox.Text;
        }
    }
}
