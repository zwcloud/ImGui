namespace ImGui
{
    public partial class GUILayout
    {
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
