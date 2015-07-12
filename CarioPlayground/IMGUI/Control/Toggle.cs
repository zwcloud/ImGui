using System.Diagnostics;
using Cairo;

namespace IMGUI
{
    internal class Toggle : Control
    {
        internal Toggle(string name)
        {
            Name = name;
            State = "Normal";

            Controls[Name] = this;
        }

        //TODO Control-less DoControl overload (without name parameter)
        internal static bool DoControl(Context g, Rect rect, string text, bool value, string name)
        {
            #region Get control reference
            Toggle toggle;
            if(!Controls.ContainsKey(name))
            {
                toggle = new Toggle(name);
            }
            else
            {
                toggle = Controls[name] as Toggle;
            }

            Debug.Assert(toggle != null);
            #endregion

            bool active = Input.LeftButtonState == InputState.Down && rect.Contains(Input.MousePos);
            bool hover = Input.LeftButtonState == InputState.Up && rect.Contains(Input.MousePos);
            if(active)
                toggle.State = "Active";
            else if(hover)
                toggle.State = "Hover";
            else
                toggle.State = "Normal";

            bool changed = Input.LeftButtonClicked && rect.Contains(Input.MousePos);
            bool on = changed ? !value : value;

            var toggleBoxRect = new Rect(rect.TopLeft, new Size(20, 20));
            g.DrawBoxModel(toggleBoxRect,
                new Content(Texture._presets[on ? "Toggle.On" : "Toggle.Off"]),
                Skin._current.Toggle[toggle.State]);

            var toggleTextRect = new Rect(toggleBoxRect.TopRight, rect.BottomRight);
            g.DrawBoxModel(toggleTextRect,
                new Content(text),
                Skin._current.Toggle[toggle.State]);

            return on;
        }
    }
}