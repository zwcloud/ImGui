using System;
using System.Collections.Generic;
using System.Text;
using ImGui.Common.Primitive;

namespace ImGui
{
    public partial class GUILayout
    {
        /// <summary>
        /// Put a fixed-size space inside a layout group.
        /// </summary>
        public static void Space(string str_id, double size)
        {
            Window window = GetCurrentWindow();
            var layout = window.StackLayout;

            int id = window.GetID(str_id);
            //TODO apply GUISkin.Instance[GUIControlName.Space]
            window.GetRect(id, layout.TopGroup.IsVertical? new Size(0,size): new Size(size,0));
        }

        /// <summary>
        /// Put a expanded space inside a layout group.
        /// </summary>
        public static void FlexibleSpace(string str_id, int stretchFactor = 1)
        {
            Window window = GetCurrentWindow();
            var layout = window.StackLayout;

            int id = window.GetID(str_id);

            var style = GUISkin.Instance[GUIControlName.FlexibleSpace];
            var oldHorizontalStretchFactor = style.HorizontalStretchFactor;
            var oldVerticalStretchFactor = style.VerticalStretchFactor;

            var horizontalStretchFactor = layout.TopGroup.IsVertical ? -1 : stretchFactor;
            var verticalStretchFactor = layout.TopGroup.IsVertical ? stretchFactor : -1;
            style.HorizontalStretchFactor = horizontalStretchFactor;
            style.VerticalStretchFactor = verticalStretchFactor;
            Rect rect = window.GetRect(id, Size.Zero);

            style.HorizontalStretchFactor = oldHorizontalStretchFactor;
            style.VerticalStretchFactor = oldVerticalStretchFactor;
        }
    }
}
