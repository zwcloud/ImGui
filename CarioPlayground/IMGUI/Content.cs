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

        public string Text { get; set; }
        public int CaretIndex { get; set; }

        public Texture Image { get; set; }

        //TODO add a Shape(SVG) Content

        public Content()
        {
            Text = string.Empty;
            Image = null;
        }

        public Content(string text)
        {
            Text = text;
            Image = null;
        }

        public Content(Texture image)
        {
            Text = null;
            Image = image;
        }

        public Content(string text, int caretIndex, int selectIndex0 = 0, int selectIndex1=0)
        {
            Text = text;
            CaretIndex = caretIndex;
        }
    }
}