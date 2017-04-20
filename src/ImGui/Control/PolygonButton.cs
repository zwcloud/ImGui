using System;
using System.Collections.Generic;

namespace ImGui
{
    internal class PolygonButton
    {
        internal static bool DoControl(Rect rect, IReadOnlyList<Point> points, Rect textRect, Content content, string str_id)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            Window window = g.CurrentWindow;
            DrawList d = window.DrawList;
            int id = window.GetID(str_id);
            var mousePos = Input.Mouse.MousePos;

            var clicked = false;
            var hovered = MathEx.IsPointInPolygon(mousePos, points, new Vector(rect.X, rect.Y));
            textRect.Offset(rect.X, rect.Y);

            //control logic
            g.KeepAliveID(id);
            if (hovered)
            {
                g.SetHoverID(id);

                if (Input.Mouse.LeftButtonPressed)//start track
                {
                    g.SetActiveID(id);
                }

                if (Input.Mouse.LeftButtonReleased)//end track
                {
                    clicked = true;
                    g.SetActiveID(GUIContext.None);
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

            // ui painting
            {
                var style = GUISkin.Instance[GUIControlName.PolygonButton];
                
                d.PathClear();
                foreach (var point in points)
                {
                    d.PathMoveTo(point + new Vector(rect.X, rect.Y));
                }
                d.PathFill(style.FillColor);

                d.PathClear();
                foreach (var point in points)
                {
                    d.PathMoveTo(point + new Vector(rect.X, rect.Y));
                }
                d.PathStroke(style.LineColor, true, 2);

                d.DrawBoxModel(textRect, content, style, state);
            }

            return clicked;
        }
        
    }
}