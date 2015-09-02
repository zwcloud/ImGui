using System;
using System.Diagnostics;
using Cairo;

namespace IMGUI
{
    internal class PolygonButton : Control
    {
        internal PolygonButton(string name)
        {
            Name = name;
            State = "Normal";

            Controls[Name] = this;
        }

        public static bool IsPointInPolygon(Point p, Point[] polygon)
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

        internal static bool DoControl(Context g, Point[] points, string text, string name)
        {
            #region Get control reference
            PolygonButton polygonButton;
            if(!Controls.ContainsKey(name))
            {
                polygonButton = new PolygonButton(name);
            }
            else
            {
                polygonButton = Controls[name] as PolygonButton;
            }

            Debug.Assert(polygonButton != null);
            #endregion

            #region Logic

            bool isHit = IsPointInPolygon(Input.Mouse.MousePos, points);
            bool active = Input.Mouse.LeftButtonState == InputState.Down && isHit;
            bool hover = Input.Mouse.LeftButtonState == InputState.Up && isHit;
            if(active)
                polygonButton.State = "Active";
            else if(hover)
                polygonButton.State = "Hover";
            else
                polygonButton.State = "Normal";
            #endregion

            var style = Skin.current.PolygonButton[polygonButton.State];
            g.StrokePolygon(points, style.LineColor);
            g.FillPolygon(points, style.FillColor);

            //TODO draw text at the center of this polygon
            //g.DrawBoxModel(rect, new Content(text), Skin.current.Button[button.State]);

            bool clicked = Input.Mouse.LeftButtonClicked && isHit;
            return clicked;
        }
    }
}