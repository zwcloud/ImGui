using System.Diagnostics;
using Cairo;

namespace IMGUI
{
    internal class Button : Control
    {
        internal Button(string name)
        {
            Name = name;
            State = "Normal";

            Controls[Name] = this;
        }

        //TODO Control-less DoControl overload (without name parameter)
        internal static bool DoControl(Context g, Rect rect, string text, string name)
        {
            #region Get control reference
            Button button;
            if(!Controls.ContainsKey(name))
            {
                button = new Button(name);
            }
            else
            {
                button = Controls[name] as Button;
            }

            Debug.Assert(button != null);
            #endregion

            #region Logic
            bool active = Input.Mouse.LeftButtonState == InputState.Down && rect.Contains(Input.Mouse.MousePos);
            bool hover = Input.Mouse.LeftButtonState == InputState.Up && rect.Contains(Input.Mouse.MousePos);
            if(active)
                button.State = "Active";
            else if(hover)
                button.State = "Hover";
            else
                button.State = "Normal";
            #endregion

            g.DrawBoxModel(rect, new Content(text), Skin.current.Button[button.State]);

            bool clicked = Input.Mouse.LeftButtonClicked && rect.Contains(Input.Mouse.MousePos);
            return clicked;
        }
    }
}