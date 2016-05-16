namespace ImGui
{
    internal static class TitleBar
    {
        public static void DoControl(GUI gui, Rect rect, Texture iconTexture, string caption, Form form, string name)
        {
            if(rect.Height > rect.Width)
            {
                throw new System.InvalidOperationException();
            }
            var rectIcon = new Rect(rect.Height, rect.Height);
            var rectCloseButton = new Rect(rect.Height, rect.Height);
            var rectCaption = new Rect(rect.Width - rectIcon.Width - rectCloseButton.Width, rect.Height);
            gui.BeginH();
            {
                gui.Image(rectIcon, iconTexture ?? Texture._presets["DefaultAppIcon"], name + "_Icon");
                gui.Label(rectCaption, caption, name + "_Caption");
                if(gui.Button(rectCloseButton, "×", name + "_CloseButton"))
                {
                    Application.Quit();
                }
            }
            gui.EndH();
        }
    }
}