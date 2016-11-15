using System;
using System.Collections.Generic;

namespace ImGui
{
    public sealed class Content : IDisposable
    {
        public static Content None = new Content();

        /// <summary>
        /// text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// text context
        /// </summary>
        public ITextContext TextContext { get; set; }

        /// <summary>
        /// image
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


        /// <summary>
        /// Get size of this content
        /// </summary>
        public Size GetSize(Style style, LayoutOption[] options)
        {
            var width = -1d;
            var height = -1d;
            foreach (var option in options)
            {
                if (option.type == LayoutOption.Type.fixedWidth)
                {
                    width = (double)option.value;
                }
                else if (option.type == LayoutOption.Type.fixedHeight)
                {
                    height = (double)option.value;
                }
            }

            if (Text != null)
            {
                if (width < 0 && height < 0) // auto-sized text
                {
                    var actualSize = style.GetTextActualSize(this.Text);
                    width = actualSize.Width;
                    height = actualSize.Height;
                }
                else
                {
                    if (width < 0) // width-auto-sized text
                    {
                        var actualSize = style.GetTextActualSize(this.Text);
                        width = actualSize.Width;
                    }
                    if (height < 0) // height-auto-sized text
                    {
                        var actualSize = style.GetTextActualSize(this.Text);
                        height = actualSize.Height;
                    }
                }
            }

            if (width < 0 && height < 0)
            {
                throw new NotImplementedException();
            }
            if (width < 0)
            {
                width = 0;
            }
            if (height < 0)
            {
                height = 0;
            }

            return new Size(Math.Ceiling(width), Math.Ceiling(height));
        }

        /// <summary>
        /// build the text context against the size and style
        /// </summary>
        internal void Build(Size size, Style style)
        {
            if (this.Text == null) throw new InvalidOperationException();
            if (style == null) throw new ArgumentNullException();

            var font = style.Font;
            var textStyle = style.TextStyle;

            if (this.TextContext != null)
            {
                this.TextContext.Dispose();
                this.TextContext = null;
            }

            this.TextContext = Application._map.CreateTextContext(
                this.Text,
                font.FontFamily, font.Size, font.FontStretch, font.FontStyle, font.FontWeight,
                (int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height),
                textStyle.TextAlignment);
        }

        internal static Content Cached(string t, string id)
        {
            Content content;
            if(!chachedContentMap.TryGetValue(id, out content))
            {
                content = new Content(t);
                chachedContentMap.Add(id, content);
            }
            chachedContentMap[id].Text = t;
            return content;
        }

        private static readonly Dictionary<string, Content> chachedContentMap = new Dictionary<string, Content>(256);

        #region IDisposable Members

        public void Dispose()
        {
            if (Image != null)
            {
                //Image.Dispose();
            }
            if (TextContext != null)
            {
                TextContext.Dispose();
            }
        }

        #endregion
    }
}