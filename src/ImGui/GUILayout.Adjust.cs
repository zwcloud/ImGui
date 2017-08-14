namespace ImGui
{
    public partial class GUILayout
    {
        /// <summary>
        /// Pop 1 or more style modifiers from stack.
        /// </summary>
        /// <param name="number">number of style modifiers</param>
        public static void PopStyleVar(int number = 1)
        {
            var context = GetCurrentContext();
            var styleStack = context.StyleStack;
            styleStack.PopStyle(number);
        }

        #region fixed width/height (same min/max width/height)
        public static void PushFixedWidth(double width) => PushMinMaxWidth((width, width));

        public static void PushFixedHeight(double height) => PushMinMaxHeight((height, height));
        #endregion

        #region min/max width
        public static void PushMinMaxWidth((double, double) width)
        {
            var context = GetCurrentContext();
            var styleStack = context.StyleStack;
            styleStack.PushWidth(width);
        }
        public static void PopMinMaxWidth() => PopStyleVar(2);

        public static void PushMinMaxHeight((double, double) height)
        {
            var context = GetCurrentContext();
            var styleStack = context.StyleStack;
            styleStack.PushHeight(height);
        }
        public static void PopMinMaxHeight() => PopStyleVar(2);
        #endregion

        #region stretch factor
        public static void PushHStretchFactor(int factor)
        {
            var context = GetCurrentContext();
            var styleStack = context.StyleStack;
            styleStack.PushStretchFactor(false, factor);
        }

        public static void PushVStretchFactor(int factor)
        {
            var context = GetCurrentContext();
            var styleStack = context.StyleStack;
            styleStack.PushStretchFactor(true, factor);
        }
        #endregion

        #region cell spacing
        public static void PushHCellSpacing(double spacing)
        {
            var context = GetCurrentContext();
            var styleStack = context.StyleStack;
            styleStack.PushCellSpacing(false, spacing);
        }

        public static void PushVCellSpacing(double spacing)
        {
            var context = GetCurrentContext();
            var styleStack = context.StyleStack;
            styleStack.PushCellSpacing(true, spacing);
        }
        #endregion

        #region alignment
        public static void PushHAlign(Alignment alignment)
        {
            var context = GetCurrentContext();
            var styleStack = context.StyleStack;
            styleStack.PushAlignment(false, alignment);
        }
        public static void PushVAlign(Alignment alignment)
        {
            var context = GetCurrentContext();
            var styleStack = context.StyleStack;
            styleStack.PushAlignment(true, alignment);
        }
        #endregion

        #region box model
        public static void PushBorder((double, double, double, double) border)
        {
            var context = GetCurrentContext();
            var styleStack = context.StyleStack;
            styleStack.PushBorder(border);
        }

        public static void PushPadding((double, double, double, double) padding)
        {
            var context = GetCurrentContext();
            var styleStack = context.StyleStack;
            styleStack.PushPadding(padding);
        }
        #endregion
    }
}
