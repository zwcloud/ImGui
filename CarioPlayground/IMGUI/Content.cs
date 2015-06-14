namespace IMGUI
{
    public sealed class Content
    {
        private string _text = string.Empty;
#if false
        private Texture _image;
#endif
        private string _tooltip = string.Empty;
        public static Content None = new Content(string.Empty);
        private static Content s_text = new Content();
#if false
        private static Content Image = new Content();
        private static Content TextImage = new Content();
#endif
        public string Text
        {
            get
            {
                return this._text;
            }
            set
            {
                this._text = value;
            }
        }
#if false
        public Texture Image
        {
            get
            {
                return this._image;
            }
            set
            {
                this._image = value;
            }
        }
#endif
        public Content()
        {
        }
        public Content(string text)
        {
            this._text = text;
        }
#if false
        public Content(Texture image)
        {
            this._image = image;
        }
#endif
    }
}