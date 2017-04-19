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
            DrawList d = form.DrawList;
            Window window = g.CurrentWindow;
            int id = window.GetID(str_id);
            var mousePos = form.GetMousePos();

            var clicked = false;
            var hovered = MathEx.IsPointInPolygon(mousePos, points, new Vector(rect.X, rect.Y));
            textRect.Offset(rect.X, rect.Y);

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

                GUIPrimitive.DrawBoxModel(textRect, content, style, state);
            }

            return clicked;
        }
        
    }
}