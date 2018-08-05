using System.Collections.Generic;
using ImGui.Common;
using ImGui.Common.Primitive;
using ImGui.Input;

namespace ImGui
{
    public partial class GUI
    {
        /// <summary>
        /// Create a polyon-button.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="points"><see cref="Point"/> list of the polygon.</param>
        /// <param name="textRect">the rect that occupied by the text</param>
        /// <param name="text">text to display on the button</param>
        /// <returns>true when the users clicks the button.</returns>
        public static bool PolygonButton(Rect rect, IReadOnlyList<Point> points, Rect textRect, string text)
        {
            return DoPolygonButton(rect, points, textRect, text);
        }

        internal static bool DoPolygonButton(Rect rect, IReadOnlyList<Point> points, Rect textRect, string text)
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            Window window = g.WindowManager.CurrentWindow;
            DrawList d = window.DrawList;
            int id = window.GetID(text);
            var mousePos = Mouse.Instance.Position;

            var clicked = false;
            var hovered = MathEx.IsPointInPolygon(mousePos, points, new Vector(rect.X, rect.Y));
            textRect.Offset(rect.X, rect.Y);

            //control logic
            g.KeepAliveID(id);
            if (hovered)
            {
                g.SetHoverID(id);

                if (Mouse.Instance.LeftButtonPressed)//start track
                {
                    g.SetActiveID(id);
                }

                if (Mouse.Instance.LeftButtonReleased)//end track
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
                if (g.ActiveId == id && Mouse.Instance.LeftButtonState == KeyState.Down)
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

                d.DrawBoxModel(textRect, text, style, state);
            }

            return clicked;
        }
        
    }

    public partial class GUILayout
    {
        /// <summary>
        /// Create an auto-layout polyon-button.
        /// </summary>
        /// <param name="points"><see cref="Point"/> list of the polygon.</param>
        /// <param name="textRect">the rect that occupied by the text</param>
        /// <param name="text">text to display on the button</param>
        /// <param name="options">layout options that specify layouting properties. See also <see cref="GUILayout.Width"/>, <see cref="GUILayout.Height"/>, <see cref="GUILayout.ExpandWidth"/>, <see cref="GUILayout.ExpandHeight"/>, <see cref="GUILayout.StretchWidth"/>, <see cref="GUILayout.StretchHeight"/></param>
        /// <returns>true when the users clicks the button.</returns>
        public static bool PolygonButton(IReadOnlyList<Point> points, Rect textRect, string text, params LayoutOption[] options)
        {
            return PolygonButton(points, textRect, text, GUISkin.Instance[GUIControlName.Button], options);
        }

        internal static bool PolygonButton(IReadOnlyList<Point> points, Rect textRect, string text, GUIStyle style, params LayoutOption[] options)
        {
            return DoPolygonButton(points, textRect, text, style, options);
        }

        private static bool DoPolygonButton(IReadOnlyList<Point> points, Rect textRect, string text, GUIStyle style, params LayoutOption[] options)
        {
            Window window = GetCurrentWindow();
            var id = window.GetID(text);
            var rect = new Rect();
            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i];
                rect.Union(point);
            }
            rect = window.GetRect(id);
            return GUI.PolygonButton(rect, points, textRect, text);
        }
    }
}