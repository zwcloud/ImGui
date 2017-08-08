using ImGui.OSAbstraction.Graphics;

namespace ImGui
{
    /// <summary>
    /// The interface for GUI with manual positioning.
    /// </summary>
    public partial class GUI
    {
        private static Window GetCurrentWindow()
        {
            return Utility.GetCurrentWindow();
        }

        #region Constant

        public const GUIState Normal = GUIState.Normal;
        public const GUIState Hover = GUIState.Hover;
        public const GUIState Active = GUIState.Active;

        #endregion

        #region Helper

        public static ITexture CreateTexture(string filePath)
        {
            var texture = Application.PlatformContext.CreateTexture();
            texture.LoadImage(filePath);
            return texture;
        }

        #endregion
    }

}
