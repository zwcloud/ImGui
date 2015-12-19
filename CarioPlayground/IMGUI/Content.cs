using TextLayout = ImGui.ITextLayout;

namespace ImGui
{
    public sealed class Content
    {
        private string tooltip = string.Empty;

        /// <summary>
        /// Simple text(Single line and short--three or two word)
        /// Not used! TODO
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Layouted text
        /// </summary>
        public ITextLayout Layout { get; set; }

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

        public Content(ITextLayout layout)
        {
            Text = null;
            Image = null;
            Layout = layout;
        }

        /// <summary>
        /// Get rect of the content that it may occupy
        /// </summary>
        /// <returns>rect of the content</returns>
        public Rect GetRect()
        {
            if(Layout!= null)
            {
                return Layout.Rect;
            }

            //Others' are not implemented
            throw new System.NotImplementedException();
        }
    }
}