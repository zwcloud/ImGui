using ImGui.OSAbstraction.Graphics;
using System.Collections.Generic;

namespace ImGui
{
    /// <summary>
    /// The interface for GUI with manual positioning.
    /// </summary>
    public partial class GUI
    {
        internal static GUIContext GetCurrentContext()
        {
            return Form.current.uiContext;
        }

        internal static Window GetCurrentWindow()
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            Window window = g.WindowManager.CurrentWindow;
            window.Accessed = true;
            return window;
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

        public static void SetSkin(Dictionary<GUIControlName, IReadOnlyList<StyleModifier>> rules)
        {
            GUISkin skin = new GUISkin(rules);
            GUISkin.Current = skin;
        }

        public static void SetDefaultSkin()
        {
            GUISkin.Current = GUISkin.Instance;
        }
    }

}
