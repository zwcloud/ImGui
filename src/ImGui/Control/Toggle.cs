using System;

namespace ImGui
{
    internal class Toggle
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="value"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// |←16→|
        /// +----+--
        /// | √  |16
        /// +----+--
        internal static bool DoControl(Rect rect, bool value, string id)
        {
            var result = value;
            var hovered = rect.Contains(Form.current.GetMousePos());

            //control logic
            var uiState = Form.current.uiContext;
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
                    uiState.SetActiveId(GUIContext.None);
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
                var style = GUISkin.Instance[GUIControlName.Toggle];
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="label"></param>
        /// <param name="value"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// |←16→|
        /// |    |---------------+
        /// |    |               |
        /// +----+               |
        /// | √  | label         |
        /// +----+               |
        ///      |               |
        ///      +---------------+
        internal static bool DoControl(Rect rect, Content label, bool value, string id)
        {
            var result = value;
            var hovered = rect.Contains(Form.current.GetMousePos());

            //control logic
            var uiState = Form.current.uiContext;
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
                    uiState.SetActiveId(GUIContext.None);
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
                var boxRect = new Rect(rect.X, rect.Y + (rect.Height - 16) / 2, 16, 16);
                var textRect = new Rect(rect.X + 16, rect.Y, rect.Width - 16, rect.Height);

                // box
                var g = Form.current.DrawList;
                var style = GUISkin.Instance[GUIControlName.Toggle];
                if (result)
                {
                    if (state == GUI.Normal)
                    {
                        //□
                        g.AddRectFilled(boxRect.TopLeft, boxRect.BottomRight, Color.Rgb(0, 120, 215));
                        //√
                        var d = boxRect.Height;
                        g.PathMoveTo(new Point(0.125f * d + boxRect.X, 0.50f * d + boxRect.Y));
                        g.PathLineTo(new Point(0.333f * d + boxRect.X, 0.75f * d + boxRect.Y));
                        g.PathLineTo(new Point(0.875f * d + boxRect.X, 0.25f * d + boxRect.Y));
                        g.PathStroke(Color.White, false, 2);
                    }
                    else if (state == GUI.Hover)
                    {
                        //□
                        g.AddRectFilled(boxRect.TopLeft, boxRect.BottomRight, Color.Rgb(0, 120, 215));
                        //□
                        g.AddRect(boxRect.TopLeft, boxRect.BottomRight, Color.Black, 0, 0, 2);
                        //√
                        var d = boxRect.Height;
                        g.PathMoveTo(new Point(0.125f * d + boxRect.X, 0.50f * d + boxRect.Y));
                        g.PathLineTo(new Point(0.333f * d + boxRect.X, 0.75f * d + boxRect.Y));
                        g.PathLineTo(new Point(0.875f * d + boxRect.X, 0.25f * d + boxRect.Y));
                        g.PathStroke(Color.White, false, 2);
                    }
                    else
                    {
                        //□
                        g.AddRectFilled(boxRect.TopLeft, boxRect.BottomRight, Color.Rgb(51, 51, 51));
                        //√
                        var d = boxRect.Height;
                        g.PathMoveTo(new Point(0.125f * d + boxRect.X, 0.50f * d + boxRect.Y));
                        g.PathLineTo(new Point(0.333f * d + boxRect.X, 0.75f * d + boxRect.Y));
                        g.PathLineTo(new Point(0.875f * d + boxRect.X, 0.25f * d + boxRect.Y));
                        g.PathStroke(Color.Rgb(102, 102, 102), false, 2);
                    }
                }
                else
                {
                    //□
                    if (state == GUI.Normal)
                    {
                        g.AddRect(boxRect.TopLeft, boxRect.BottomRight, Color.Rgb(102, 102, 102), 0, 0, 2);
                    }
                    else if (state == GUI.Hover)
                    {
                        g.AddRect(boxRect.TopLeft, boxRect.BottomRight, Color.Black, 0, 0, 2);
                    }
                    else
                    {
                        g.AddRectFilled(boxRect.TopLeft, boxRect.BottomRight, Color.Rgb(102, 102, 102));
                    }
                }
                // label
                GUIPrimitive.DrawBoxModel(textRect, label, GUISkin.Instance[GUIControlName.Label]);
            }

            return result;
        }
    }
}