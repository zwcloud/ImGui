namespace ImGui
{
    internal class Slider
    {
        internal static double DoControl(Rect rect, double value, double minValue, double maxValue, string id)
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
                    var leftPoint = new Point(rect.X + 10, rect.Y + rect.Height / 2);
                    var rightPoint = new Point(rect.Right - 10, rect.Y + rect.Height / 2);
                    var minX = leftPoint.X;
                    var maxX = rightPoint.X;
                    var mousePos = Form.current.GetMousePos();
                    var currentPointX = MathEx.Clamp(mousePos.X, minX, maxX);
                    value = minValue + (currentPointX - minX) / (maxX - minX) * (maxValue - minValue);
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

                var h = rect.Height;
                var a = 0.2f * h;
                var b = 0.3f * h;
                var leftPoint = new Point(rect.X + 10, rect.Y + rect.Height / 2);
                var rightPoint = new Point(rect.Right - 10, rect.Y + rect.Height / 2);

                var minX = leftPoint.X;
                var maxX = rightPoint.X;
                var currentPoint = leftPoint + new Vector((value - minValue) / (maxValue - minValue) * (maxX - minX), 0);

                var topArcCenter = currentPoint + new Vector(0, b);
                var bottomArcCenter = currentPoint + new Vector(0, -b);
                var bottomStartPoint = bottomArcCenter + new Vector(-a, 0);

                g.PathMoveTo(leftPoint);
                g.PathLineTo(currentPoint);
                g.PathStroke((Color)style.ExtraStyles["Line:Used"], false, 2);

                g.PathMoveTo(currentPoint);
                g.PathLineTo(rightPoint);
                g.PathStroke((Color)style.ExtraStyles["Line:Unused"], false, 2);
                
                g.PathArcToFast(topArcCenter, a, 0, 6);
                g.PathLineTo(bottomStartPoint);
                g.PathArcToFast(bottomArcCenter, a, 6, 12);
                g.PathClose();
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
