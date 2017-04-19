namespace ImGui
{
    internal class Slider
    {
        internal static double DoControl(Rect rect, double value, double minValue, double maxValue, bool isHorizontal, string str_id)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            DrawList d = form.DrawList;
            Window window = g.CurrentWindow;
            int id = window.GetID(str_id);

            var mousePos = form.GetMousePos();
            var hovered = rect.Contains(mousePos);
            
            //control logic
            var uiState = Form.current.uiContext;
            uiState.KeepAliveId(id);
            if (hovered)
            {
                uiState.SetHoverId(id);

                if (Input.Mouse.LeftButtonPressed)//start track
                {
                    uiState.SetActiveId(id);
                }
            }
            if (uiState.ActiveId == id)
            {
                if (Input.Mouse.LeftButtonState == InputState.Down)
                {
                    if (isHorizontal)
                    {
                        var leftPoint = new Point(rect.X + 10, rect.Y + rect.Height/2);
                        var rightPoint = new Point(rect.Right - 10, rect.Y + rect.Height/2);
                        var minX = leftPoint.X;
                        var maxX = rightPoint.X;
                        var currentPointX = MathEx.Clamp(mousePos.X, minX, maxX);
                        value = minValue + (currentPointX - minX)/(maxX - minX)*(maxValue - minValue);
                    }
                    else
                    {
                        var upPoint= new Point(rect.X + rect.Width / 2, rect.Y + 10);
                        var bottomPoint = new Point(rect.X + rect.Width / 2, rect.Bottom - 10);
                        var minY = upPoint.Y;
                        var maxY = bottomPoint.Y;
                        var currentPointY = MathEx.Clamp(mousePos.Y, minY, maxY);
                        value = (float)(minValue + (currentPointY - minY) / (maxY - minY) * (maxValue - minValue));
                    }
                }
                else//end track
                {
                    uiState.SetActiveId(GUIContext.None);
                }
            }

            // ui representation
            var state = GUI.Normal;
            if (hovered)
            {
                state = GUI.Hover;
            }
            if (uiState.ActiveId == id && Input.Mouse.LeftButtonState == InputState.Down)
            {
                state = GUI.Active;
            }

            // ui painting
            {
                var style = GUISkin.Instance[GUIControlName.Slider];
                var colorForLineUsed = style.Get<Color>(GUIStyleName.Slider_LineUsed);
                var colorForLineUnused = style.Get<Color>(GUIStyleName.Slider_LineUnused);

                if (isHorizontal)
                {
                    var h = rect.Height;
                    var a = 0.2f*h;
                    var b = 0.3f*h;
                    var leftPoint = new Point(rect.X + 10, rect.Y + rect.Height/2);
                    var rightPoint = new Point(rect.Right - 10, rect.Y + rect.Height/2);

                    var minX = leftPoint.X;
                    var maxX = rightPoint.X;
                    var currentPoint = leftPoint + new Vector((value - minValue)/(maxValue - minValue)*(maxX - minX), 0);

                    var topArcCenter = currentPoint + new Vector(0, b);
                    var bottomArcCenter = currentPoint + new Vector(0, -b);
                    var bottomStartPoint = bottomArcCenter + new Vector(-a, 0);

                    d.PathMoveTo(leftPoint);
                    d.PathLineTo(currentPoint);
                    d.PathStroke(colorForLineUsed, false, 2);

                    d.PathMoveTo(currentPoint);
                    d.PathLineTo(rightPoint);
                    d.PathStroke(colorForLineUnused, false, 2);

                    d.PathArcToFast(topArcCenter, a, 0, 6);
                    d.PathLineTo(bottomStartPoint);
                    d.PathArcToFast(bottomArcCenter, a, 6, 12);
                    d.PathClose();
                }
                else
                {
                    var h = rect.Width;
                    var a = 0.2 * h;
                    var b = 0.3 * h;
                    var upPoint = new Point(rect.X + rect.Width / 2, rect.Y + 10);
                    var bottomPoint = new Point(rect.X + rect.Width / 2, rect.Bottom - 10);

                    var minY = upPoint.Y;
                    var maxY = bottomPoint.Y;
                    var currentPoint = upPoint + new Vector(0, (value - minValue) / (maxValue - minValue) * (maxY - minY));

                    var leftArcCenter = currentPoint + new Vector(-b, 0);
                    var rightArcCenter = currentPoint + new Vector(b, 0);
                    var rightStartPoint = rightArcCenter + new Vector(0, -a);

                    d.PathMoveTo(upPoint);
                    d.PathLineTo(currentPoint);
                    d.PathStroke(colorForLineUsed, false, 2);

                    d.PathMoveTo(currentPoint);
                    d.PathLineTo(bottomPoint);
                    d.PathStroke(colorForLineUnused, false, 2);

                    d.PathArcToFast(leftArcCenter, a, 3, 9);
                    d.PathLineTo(rightStartPoint);
                    d.PathArcToFast(rightArcCenter, a, 9, 12);
                    d.PathArcToFast(rightArcCenter, a, 0, 3);
                    d.PathClose();
                }

                var fillColor = Color.Rgb(204, 204, 204);
                if (state == GUI.Normal)
                {
                    fillColor = Color.Rgb(0, 120, 215);
                }
                else if (state == GUI.Hover)
                {
                    fillColor = Color.Rgb(23, 23, 23);
                }
                d.PathFill(fillColor);

                //GUIPrimitive.DrawBoxModel(rect, null, GUISkin.Instance[GUIControlName.Slider]);
            }

            return value;
        }
        
    }
}
