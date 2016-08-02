using System;
using System.Collections.Generic;

namespace ImGui
{
    public sealed class Content : IDisposable
    {
        public static Content None = new Content();

        /// <summary>
        /// built text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// built text
        /// </summary>
        public ITextContext TextContext { get; set; }

        /// <summary>
        /// Image
        /// </summary>
        public Texture Image { get; set; }

        private Content()
        {
            this.Image = null;
            this.TextContext = null;
            this.Text = null;
        }

        public Content(string text)
        {
            this.Image = null;
            this.Text = text;
            this.TextContext = null;
        }

        public Content(Texture image)
        {
            this.Image = image;
            this.TextContext = null;
            this.Text = null;
        }

        public Content(ITextContext textContext)
        {
            this.Image = null;
            this.Text = textContext.Text;
            this.TextContext = textContext;
        }

        internal void Build(Style style)
        {
            if (this.Text == null) throw new InvalidOperationException();
            if (style == null) throw new ArgumentNullException();
            this.TextContext = BuildTextContext(this.Text, style);
        }

        /// <summary>
        /// Get rect of the content that it may occupy
        /// </summary>
        /// <returns>rect of the content</returns>
        public Size GetSize(Style style)
        {
            if (Text != null)
            {
                Build(style);
            }
            if (TextContext != null)
            {
                var width = Math.Ceiling(TextContext.Rect.Width);
                var height = Math.Ceiling(TextContext.Rect.Height);
                return new Size(width, height);
            }

            //Others' are not implemented
            throw new System.NotImplementedException();
        }

        //dummy
        public static ITextContext BuildTextContext(string text, Rect rect, Style style)
        {
            return null;
        }

        public static ITextContext BuildTextContext(string text, Style style)
        {
            var font = style.Font;
            var textStyle = style.TextStyle;

            //get actual rect of the text
            var measureContext = Application._map.CreateTextContext(
                text,
                font.FontFamily, font.Size, font.FontStretch, font.FontStyle, font.FontWeight,
                4096, 4096,
                textStyle.TextAlignment);
            var measureRect = measureContext.Rect;
            measureContext.Dispose();

            // build text context against the rect and style
            var textContext = Application._map.CreateTextContext(
                text,
                font.FontFamily, font.Size, font.FontStretch, font.FontStyle, font.FontWeight,
                (int)Math.Ceiling(measureRect.Width), (int)Math.Ceiling(measureRect.Height),
                textStyle.TextAlignment);
            return textContext;
        }

        internal static Content Cached(string t, string id)
        {
            Content content;
            if(!chachedContentMap.TryGetValue(id, out content))
            {
                content = new Content(t);
                chachedContentMap.Add(id, content);
            }
            return content;
        }

        private static readonly Dictionary<string, Content> chachedContentMap = new Dictionary<string, Content>(256);

        #region IDisposable Members

        public void Dispose()
        {
            if (Image != null)
            {
                Image.Dispose();
            }
            if (TextContext != null)
            {
                TextContext.Dispose();
            }
        }

        #endregion
    }
}