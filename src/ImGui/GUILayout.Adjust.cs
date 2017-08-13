namespace ImGui
{
    public partial class GUILayout
    {
        #region fixed width/height (same min/max width/height)

        public static void PushWidth(double width) => PushWidth((width, width));

        public static void PushHeight(double height) => PushHeight((height, height));

        //pop methods are shared
        #endregion

        #region min/max width

        public static void PushWidth((double, double) width)
        {
            var context = GetCurrentContext();
            var styleStack = context.StyleStack;
            styleStack.PushWidth(width);
        }

        public static void PopWidth()
        {
            var context = GetCurrentContext();
            var styleStack = context.StyleStack;
            styleStack.PopWidth();
        }

        public static void PushHeight((double, double) height)
        {
            var context = GetCurrentContext();
            var styleStack = context.StyleStack;
            styleStack.PushHeight(height);
        }

        public static void PopHeight()
        {
            var context = GetCurrentContext();
            var styleStack = context.StyleStack;
            styleStack.PopHeight();
        }

        #endregion

        #region stretch factor

        public static void PushHStretchFactor(int factor)
        {
            var context = GetCurrentContext();
            var styleStack = context.StyleStack;
            styleStack.PushStretchFactor(false, factor);
        }

        public static void PopHStretchFactor()
        {
            var context = GetCurrentContext();
            var styleStack = context.StyleStack;
            styleStack.PopStretchFactor(false);
        }

        public static void PushVStretchFactor(int factor)
        {
            var context = GetCurrentContext();
            var styleStack = context.StyleStack;
            styleStack.PushStretchFactor(true, factor);
        }

        public static void PopVStretchFactor()
        {
            var context = GetCurrentContext();
            var styleStack = context.StyleStack;
            styleStack.PopStretchFactor(true);
        }

        #endregion

        #region cell spacing
        //TODO
        #endregion

        #region alignment
        //TODO
        #endregion

        #region box model

        public static void PushBorder((double, double, double, double) border)
        {
            var context = GetCurrentContext();
            var styleStack = context.StyleStack;
            styleStack.PushBorder(border);
        }

        public static void PopBorder()
        {
            var context = GetCurrentContext();
            var styleStack = context.StyleStack;
            styleStack.PopBorder();
        }

        public static void PushPadding((double, double, double, double) padding)
        {
            var context = GetCurrentContext();
            var styleStack = context.StyleStack;
            styleStack.PushPadding(padding);
        }

        public static void PopPadding()
        {
            var context = GetCurrentContext();
            var styleStack = context.StyleStack;
            styleStack.PopPadding();
        }

        #endregion
    }
}
