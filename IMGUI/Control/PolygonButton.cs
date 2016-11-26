using System;

namespace ImGui
{
    internal class PolygonButton
    {
        private static bool IsPointInPolygon(Point p, Point[] polygon)
        {
            double minX = polygon[0].X;
            double maxX = polygon[0].X;
            double minY = polygon[0].Y;
            double maxY = polygon[0].Y;
            for (int i = 1; i < polygon.Length; i++)
            {
                Point q = polygon[i];
                minX = Math.Min(q.X, minX);
                maxX = Math.Max(q.X, maxX);
                minY = Math.Min(q.Y, minY);
                maxY = Math.Max(q.Y, maxY);
            }

            if (p.X < minX || p.X > maxX || p.Y < minY || p.Y > maxY)
            {
                return false;
            }

            // http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
            bool inside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if ((polygon[i].Y > p.Y) != (polygon[j].Y > p.Y) &&
                     p.X < (polygon[j].X - polygon[i].X) * (p.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X)
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        private static bool IsPointInPolygon(Point p, Point[] polygon, Vector offset)
        {
            var polygon0 = polygon[0] + offset;
            double minX = polygon0.X;
            double maxX = polygon0.X;
            double minY = polygon0.Y;
            double maxY = polygon0.Y;
            for (int i = 1; i < polygon.Length; i++)
            {
                Point q = polygon[i] + offset;
                minX = Math.Min(q.X, minX);
                maxX = Math.Max(q.X, maxX);
                minY = Math.Min(q.Y, minY);
                maxY = Math.Max(q.Y, maxY);
            }

            if (p.X < minX || p.X > maxX || p.Y < minY || p.Y > maxY)
            {
                return false;
            }

            // http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
            bool inside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                var point = polygon[i] + offset;
                var point1 = polygon[j] + offset;
                if ((point.Y > p.Y) != (point1.Y > p.Y) &&
                     p.X < (point1.X - point.X) * (p.Y - point.Y) / (point1.Y - point.Y) + point.X)
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        //http://stackoverflow.com/a/33852627/3427520
        private static Point centroidForPoly(Point[] verts)
        {
            var sum = 0.0;
            Point vsum = Point.Zero;

            var numVerts = verts.Length;
            for (int i = 0; i<numVerts; i++){
                Point v1 = verts[i];
                Point v2 = verts[(i + 1) % numVerts];
                var cross = v1.X*v2.Y - v1.Y*v2.X;
                sum += cross;
                vsum = new Point(((v1.X + v2.X) * cross) + vsum.X, ((v1.Y + v2.Y) * cross) + vsum.Y);
            }

            var z = 1.0 / (3.0 * sum);
            return new Point(vsum.X * z, vsum.Y * z);
        }

        internal static bool DoControl(Rect rect, Point[] points, Rect textRect, Content content, string id)
        {
            var clicked = false;
            var hovered = IsPointInPolygon(Form.current.GetMousePos(), points, new Vector(rect.X, rect.Y));
            textRect.Offset(rect.X, rect.Y);

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

                if (Input.Mouse.LeftButtonReleased)//end track
                {
                    clicked = true;
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
                var style = Skin.current.PolygonButton[state];
                
                g.PathClear();
                foreach (var point in points)
                {
                    g.PathMoveTo(point + new Vector(rect.X, rect.Y));
                }
                g.PathFill(style.FillColor);

                g.PathClear();
                foreach (var point in points)
                {
                    g.PathMoveTo(point + new Vector(rect.X, rect.Y));
                }
                g.PathStroke(style.LineColor, true, 2);

                GUIPrimitive.DrawBoxModel(textRect, content, style);
            }

            return clicked;
        }
        
    }
}