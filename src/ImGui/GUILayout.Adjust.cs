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
            var window = GetCurrentWindow();
            var layout = window.StackLayout;
            layout.PushWidth(width);
        }

        public static void PopWidth()
        {
            var window = GetCurrentWindow();
            var layout = window.StackLayout;
            layout.PopWidth();
        }

        public static void PushHeight((double, double) height)
        {
            var window = GetCurrentWindow();
            var layout = window.StackLayout;
            layout.PushHeight(height);
        }

        public static void PopHeight()
        {
            var window = GetCurrentWindow();
            var layout = window.StackLayout;
            layout.PopHeight();
        }

        #endregion

        public static void PushHStretchFactor(int factor)
        {
            var window = GetCurrentWindow();
            var layout = window.StackLayout;
            layout.PushStretchFactor(false, factor);
        }

        public static void PopHStretchFactor()
        {
            var window = GetCurrentWindow();
            var layout = window.StackLayout;
            layout.PopStretchFactor(false);
        }

        public static void PushVStretchFactor(int factor)
        {
            var window = GetCurrentWindow();
            var layout = window.StackLayout;
            layout.PushStretchFactor(true, factor);
        }

        public static void PopVStretchFactor()
        {
            var window = GetCurrentWindow();
            var layout = window.StackLayout;
            layout.PopStretchFactor(true);
        }
    }
}
