using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ImGui
{
    internal sealed partial class Content : IDisposable
    {
        public static Content None = new Content();

        Rect rect;
        GUIStyle style;

        internal TextMesh TextMesh;

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
        public ITexture Image { get; set; }

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
            this.TextMesh = new TextMesh();
            this.TextContext = null;
        }

        public Content(ITexture image)
        {
            this.Image = image;
            this.TextContext = null;
            this.Text = null;
        }

        public Content(ITextContext textContext)
        {
            this.Image = null;
            this.Text = textContext.Text;
            this.TextMesh = new TextMesh();
            this.TextContext = textContext;
        }

        public bool Dirty { get; set; }

        /// <summary>
        /// build the text context against the size and style
        /// </summary>
        internal void BuildText(Rect rect, GUIStyle style, GUIState state)
        {
            // check 
            if (this.Text == null) throw new InvalidOperationException();
            if (style == null) throw new ArgumentNullException();

            //check if the content need to be rebuilt
            // 1. rect size changed
            // 2. style changed that make the text different looking
            // 3. the content is dirty
            bool rebuiltNeeded = Dirty || this.rect != rect// TODO If rect size isn't changed, there is no need to rebuild the text mesh from glyphs. Offset the mesh is enough.
                || GUIStyle.IsRebuildTextContextRequired(this.style, style);
            if (!rebuiltNeeded)
            {
                Debug.Assert(TextMesh != null);
                return;
            }

            // clear dirty
            Dirty = false;

            // (re)create a TextContent for the text
            var fontFamily = style.Get<string>(GUIStyleName.FontFamily, state);
            var fontSize = style.Get<double>(GUIStyleName.FontSize, state);
            var fontStretch = (FontStretch)style.Get<int>(GUIStyleName.FontStretch, state);
            var fontStyle = (FontStyle)style.Get<int>(GUIStyleName.FontStyle, state);
            var fontWeight = (FontWeight)style.Get<int>(GUIStyleName.FontWeight, state);
            var textAlignment = (TextAlignment)style.Get<int>(GUIStyleName.TextAlignment, state);

            if (this.TextContext != null)
            {
                this.TextContext.Dispose();
                this.style = style;
                this.rect = rect;
            }

            this.TextContext = Application.platformContext.CreateTextContext(
                this.Text,
                fontFamily, (int)fontSize, fontStretch, fontStyle, fontWeight,
                (int)Math.Ceiling(rect.Size.Width), (int)Math.Ceiling(rect.Size.Height),
                textAlignment);

            // create a text mesh
            this.TextMesh.Clear();
            this.TextMesh.Build(rect.TopLeft, style, this.TextContext);
        }

        // TODO This is a temp hack and not a real cache!!

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