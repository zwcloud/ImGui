namespace ImGui
{
    internal class Slider
    {
        internal static double DoControl(Rect rect, double value, double minValue, double maxValue, bool isHorizontal, string id)
        {
            var hovered = rect.Contains(Form.current.GetMousePos());
            
            //control logic
            var uiState = Form.current.uiState;
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
                    var mousePos = Form.current.GetMousePos();
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
                    uiState.SetActiveId(GUIState.None);
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
            if (Event.current.type == EventType.Repaint)
            {
                var g = Form.current.DrawList;
                var style = Skin.current.Slider[state];

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

                    g.PathMoveTo(leftPoint);
                    g.PathLineTo(currentPoint);
                    g.PathStroke((Color) style.ExtraStyles["Line:Used"], false, 2);

                    g.PathMoveTo(currentPoint);
                    g.PathLineTo(rightPoint);
                    g.PathStroke((Color) style.ExtraStyles["Line:Unused"], false, 2);

                    g.PathArcToFast(topArcCenter, a, 0, 6);
                    g.PathLineTo(bottomStartPoint);
                    g.PathArcToFast(bottomArcCenter, a, 6, 12);
                    g.PathClose();
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

                    g.PathMoveTo(upPoint);
                    g.PathLineTo(currentPoint);
                    g.PathStroke((Color)style.ExtraStyles["Line:Used"], false, 2);

                    g.PathMoveTo(currentPoint);
                    g.PathLineTo(bottomPoint);
                    g.PathStroke((Color)style.ExtraStyles["Line:Unused"], false, 2);

                    g.PathArcToFast(leftArcCenter, a, 3, 9);
                    g.PathLineTo(rightStartPoint);
                    g.PathArcToFast(rightArcCenter, a, 9, 12);
                    g.PathArcToFast(rightArcCenter, a, 0, 3);
                    g.PathClose();
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
                g.PathFill(fillColor);
            }

            return value;
        }
        
    }
}
