using System;

namespace ImGui
{
    internal class Toggle
    {
        internal static bool DoControl(Rect rect, bool value, string id)
        {
            var uiState = Form.current.uiState;
            if (rect.Contains(Form.current.GetMousePos()))
            {
                uiState.hotitem = id;
                if (uiState.activeitem == GUIState.None && Input.Mouse.LeftButtonState == InputState.Down)
                {
                    uiState.activeitem = id;
                }
            }

            var state = GUI.Normal;
            if (uiState.hotitem == id)
            {
                if (uiState.activeitem == id)
                {
                    state = GUI.Active;
                }
                else
                {
                    state = GUI.Hover;
                }
            }

            var result = value;
            if (Input.Mouse.LeftButtonState == InputState.Up &&
                uiState.hotitem == id && uiState.activeitem == id)
            {
                result = !value;
            }

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