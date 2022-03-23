namespace ImGui.Development
{
    public class GUIDebug
    {
        public static void SetWindowPosition(string windowName, Point position)
        {
            var possibleWindow = Application.ImGuiContext.WindowManager.Windows.Find(window => window.Name == windowName);
            if (possibleWindow != null)
            {
                possibleWindow.Position = position;
            }
        }
    }
}