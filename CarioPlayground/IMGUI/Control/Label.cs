using System.Diagnostics;
using Cairo;

namespace IMGUI
{
    internal class Label : Control
    {
        internal Label(string name)
        {
            Name = name;
            State = "Normal";

            Controls[Name] = this;
        }

        //TODO Control-less DoControl overload (without name parameter)
        internal static void DoControl(Context g, Rect rect, string text, string name)
        {
            #region Get control reference
            Label label;
            if(!Controls.ContainsKey(name))
            {
                label = new Label(name);
            }
            else
            {
                label = Controls[name] as Label;
            }

            Debug.Assert(label != null);
            #endregion

            bool active = Input.Mouse.LeftButtonState == InputState.Down && rect.Contains(Input.Mouse.MousePos);
            bool hover = Input.Mouse.LeftButtonState == InputState.Up && rect.Contains(Input.Mouse.MousePos);
            if(active)
                label.State = "Active";
            else if(hover)
                label.State = "Hover";
            else
                label.State = "Normal";

            g.DrawBoxModel(rect, new Content(text), Skin.current.Label[label.State]);
        }

    }
}