using System;

namespace ImGui
{
    internal class Toggle
    {
        internal static bool DoControl(Rect rect, bool value, string id)
        {
            var result = value;
            var hovered = rect.Contains(Form.current.GetMousePos());

            //control logic
            var uiState = Form.current.uiState;
            uiState.KeepAliveId(id);
            if (hovered)
            {
                uiState.SetHoverId(id);

                if (Input.Mouse.LeftButtonPressed)
                {
                    uiState.ActiveId = id;
                }

                if (uiState.ActiveId == id && Input.Mouse.LeftButtonReleased)
                {
                    result = !value;
                    uiState.SetActiveId(GUIState.None);
                }
            }

            // ui representation
            var state = GUI.Normal;
            if (hovered)
            {
                state = GUI.Hover;
                if (uiState.ActiveId == id && Input.Mouse.LeftButtonState == InputState.Down)
                {
                    state = GUI.Active;
                }
            }

            // ui painting
            if (Event.current.type == EventType.Repaint)
            {
                var g = Form.current.DrawList;
                var style = Skin.current.Toggle[state];
                if (result)
                {
                    if (state == GUI.Normal)
                    {
                        //□
                        g.AddRectFilled(rect.TopLeft, rect.BottomRight, Color.Rgb(0, 120, 215));
                        //√
                        var d = rect.Height;
                        g.PathMoveTo(new Point(0.125f * d + rect.X, 0.50f * d + rect.Y));
                        g.PathLineTo(new Point(0.333f * d + rect.X, 0.75f * d + rect.Y));
                        g.PathLineTo(new Point(0.875f * d + rect.X, 0.25f * d + rect.Y));
                        g.PathStroke(Color.White, false, 2);
                    }
                    else if (state == GUI.Hover)
                    {
                        //□
                        g.AddRectFilled(rect.TopLeft, rect.BottomRight, Color.Rgb(0, 120, 215));
                        //□
                        g.AddRect(rect.TopLeft, rect.BottomRight, Color.Black, 0, 0, 2);
                        //√
                        var d = rect.Height;
                        g.PathMoveTo(new Point(0.125f * d + rect.X, 0.50f * d + rect.Y));
                        g.PathLineTo(new Point(0.333f * d + rect.X, 0.75f * d + rect.Y));
                        g.PathLineTo(new Point(0.875f * d + rect.X, 0.25f * d + rect.Y));
                        g.PathStroke(Color.White, false, 2);
                    }
                    else
                    {
                        //□
                        g.AddRectFilled(rect.TopLeft, rect.BottomRight, Color.Rgb(51, 51, 51));
                        //√
                        var d = rect.Height;
                        g.PathMoveTo(new Point(0.125f * d + rect.X, 0.50f * d + rect.Y));
                        g.PathLineTo(new Point(0.333f * d + rect.X, 0.75f * d + rect.Y));
                        g.PathLineTo(new Point(0.875f * d + rect.X, 0.25f * d + rect.Y));
                        g.PathStroke(Color.Rgb(102, 102, 102), false, 2);
                    }
                }
                else
                {
                    //□
                    if (state == GUI.Normal)
                    {
                        g.AddRect(rect.TopLeft, rect.BottomRight, Color.Rgb(102, 102, 102), 0, 0, 2);
                    }
                    else if (state == GUI.Hover)
                    {
                        g.AddRect(rect.TopLeft, rect.BottomRight, Color.Black, 0, 0, 2);
                    }
                    else
                    {
                        g.AddRectFilled(rect.TopLeft, rect.BottomRight, Color.Rgb(102, 102, 102));
                    }
                }
            }

            return result;
        }

    }
}