using Layout = IMGUI.ITextLayout;

namespace IMGUI
{
    public sealed class Content
    {
        private string tooltip = string.Empty;

        /// <summary>
        /// Simple text(Single line and short--three or two word)
        /// Not used!
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Layouted text
        /// </summary>
        public Layout Layout { get; set; }

        /// <summary>
        /// Image
        /// </summary>
        public Texture Image { get; set; }

        //TODO add a Shape(SVG) Content

        private Content()
        {
            Text = string.Empty;
            Image = null;
            Layout = null;
        }

        public Content(string text)
        {
            Text = text;
            Image = null;
            Layout = null;
        }

        public Content(Texture image)
        {
            Text = null;
            Image = image;
            Layout = null;
        }

        public Content(Layout layout)
        {
            Text = null;
            Image = null;
            Layout = layout;
        }
    }
}