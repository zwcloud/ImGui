using System;
using System.Collections.Generic;

namespace ImGui
{
    /// <summary>
    /// The interface for GUI with manual positioning.
    /// </summary>
    public partial class GUI
    {
        #region Constant

        public const GUIState Normal = GUIState.Normal;
        public const GUIState Hover = GUIState.Hover;
        public const GUIState Active = GUIState.Active;

        #endregion

        #region Helper

        public static ITexture CreateTexture(string filePath)
        {
            var texture = Application.platformContext.CreateTexture();
            texture.LoadImage(filePath);
            return texture;
        }

        #endregion
    }

}
