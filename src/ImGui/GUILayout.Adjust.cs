using ImGui.Common.Primitive;

namespace ImGui
{
    public partial class GUILayout
    {
        /// <summary>
        /// Push 1 style color to stack.
        /// </summary>
        /// <param name="name">style name, <see cref="GUIStyleName"/></param>
        /// <param name="color">color</param>
        /// <param name="state">which state will this style be apply to</param>
        public static void PushStyleColor(GUIStyleName name, Color color, GUIState state = GUIState.Normal)
        {
            var style = GUIStyle.Basic;
            style.Push(new StyleModifier(name, StyleType.Color, color, state));
        }

        /// <summary>
        /// Pop 1 or more style modifiers from stack.
        /// </summary>
        /// <param name="number">number of style modifiers</param>
        public static void PopStyleVar(int number = 1)
        {
            var style = GUIStyle.Basic;
            style.PopStyle(number);
        }


        #region fixed width/height (same min/max width/height)
        public static void PushFixedWidth(double width) => PushMinMaxWidth((width, width));

        public static void PushFixedHeight(double height) => PushMinMaxHeight((height, height));
        #endregion

        #region min/max width
        public static void PushMinMaxWidth((double, double) width)
        {
            var style = GUIStyle.Basic;
            style.PushWidth(width);
        }
        public static void PopMinMaxWidth() => PopStyleVar(2);

        public static void PushMinMaxHeight((double, double) height)
        {
            var style = GUIStyle.Basic;
            style.PushHeight(height);
        }
        public static void PopMinMaxHeight() => PopStyleVar(2);
        #endregion

        #region stretch factor
        public static void PushHStretchFactor(int factor)
        {
            var style = GUIStyle.Basic;
            style.PushStretchFactor(false, factor);
        }

        public static void PushVStretchFactor(int factor)
        {
            var style = GUIStyle.Basic;
            style.PushStretchFactor(true, factor);
        }
        #endregion

        #region cell spacing
        public static void PushHCellSpacing(double spacing)
        {
            var style = GUIStyle.Basic;
            style.PushCellSpacing(false, spacing);
        }

        public static void PushVCellSpacing(double spacing)
        {
            var style = GUIStyle.Basic;
            style.PushCellSpacing(true, spacing);
        }
        #endregion

        #region alignment
        public static void PushHAlign(Alignment alignment)
        {
            var style = GUIStyle.Basic;
            style.PushAlignment(false, alignment);
        }
        public static void PushVAlign(Alignment alignment)
        {
            var style = GUIStyle.Basic;
            style.PushAlignment(true, alignment);
        }
        #endregion

        #region box model
        public static void PushBorder((double, double, double, double) border)
        {
            var style = GUIStyle.Basic;
            style.PushBorder(border);
        }

        public static void PushPadding((double, double, double, double) padding)
        {
            var style = GUIStyle.Basic;
            style.PushPadding(padding);
        }
        #endregion

        #region color
        public static void PushBgColor(Color color)
        {
            var style = GUIStyle.Basic;
            style.PushBgColor(color);
        }
        #endregion

        #region font
        public static void PushFontColor(Color color)
        {
            var style = GUIStyle.Basic;
            style.PushFontColor(color);
        }

        public static void PushFontSize(double fontSize)
        {
            var style = GUIStyle.Basic;
            style.PushFontSize(fontSize);
        }

        public static void PushFontFamily(string fontFamily)
        {
            var style = GUIStyle.Basic;
            style.PushFontFamily(fontFamily);
        }
        #endregion
    }
}
