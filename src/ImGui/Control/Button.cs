using System;
using System.Collections.Generic;

namespace ImGui
{
    internal class Button
    {
        public static bool DoControl(Rect rect, Content content, string id)
        {
            Form form = Form.current;
            var mousePos = form.GetMousePos();
            GUIContext g = form.uiContext;

            if (Utility.CurrentOS.IsAndroid)
            {
                var clicked = false;
                var inside = rect.Contains(Input.Mouse.MousePos);

                //control logic
                g.KeepAliveId(id);
                if (inside && Input.Mouse.LeftButtonPressed)//start track
                {
                    g.SetActiveId(id);
                }

                if (g.ActiveId == id && Input.Mouse.LeftButtonReleased)//end track
                {
                    clicked = true;
                    g.SetActiveId(GUIContext.None);
                }

                // ui representation
                var state = GUI.Normal;
                if (g.ActiveId == id && Input.Mouse.LeftButtonState == InputState.Down)
                {
                    state = GUI.Active;
                }

                // ui painting
                GUIPrimitive.DrawBoxModel(rect, content, GUISkin.Instance[GUIControlName.Button], state);
                return clicked;
            }
            else
            {
                var clicked = false;
                var hovered = rect.Contains(mousePos);

                //control logic
                g.KeepAliveId(id);
                if (hovered)
                {
                    g.SetHoverId(id);

                    if (Input.Mouse.LeftButtonPressed)//start track
                    {
                        g.SetActiveId(id);
                    }

                    if (Input.Mouse.LeftButtonReleased)//end track
                    {
                        clicked = true;
                        g.SetActiveId(GUIContext.None);
                    }
                }

                // ui representation
                var state = GUI.Normal;
                if (hovered)
                {
                    state = GUI.Hover;
                    if (g.ActiveId == id && Input.Mouse.LeftButtonState == InputState.Down)
                    {
                        state = GUI.Active;
                    }
                }

                GUIPrimitive.DrawBoxModel(rect, content, GUISkin.Instance[GUIControlName.Button], state);
                return clicked;
            }
        }
    }
}