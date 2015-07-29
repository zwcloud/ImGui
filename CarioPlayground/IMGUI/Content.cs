using Layout = Pango.Layout;
namespace IMGUI
{
    public sealed class Content
    {
#if false
        private Texture _image;
#endif
        private string _tooltip = string.Empty;
        public static Content None = new Content(string.Empty);
        private static Content s_text = new Content();

        /// <summary>
        /// Simple text(Single line and short--three or two word)
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Layouted text
        /// </summary>
        public Layout Layout { get; set; }

        public Texture Image { get; set; }

        //TODO add a Shape(SVG) Content

        public Content()
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