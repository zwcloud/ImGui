using System;
using System.Collections.Generic;

namespace ImGui
{
    internal class PolygonButton
    {
        internal static bool DoControl(Rect rect, IReadOnlyList<Point> points, Rect textRect, Content content, string id)
        {
            var clicked = false;
            var hovered = MathEx.IsPointInPolygon(Form.current.GetMousePos(), points, new Vector(rect.X, rect.Y));
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