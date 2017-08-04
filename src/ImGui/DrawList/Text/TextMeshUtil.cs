using System;
using System.Collections.Generic;
using ImGui.Common.Primitive;

namespace ImGui
{
    class TextMeshUtil
    {
        static readonly Dictionary<int, TextMesh> TextMeshCache = new Dictionary<int, TextMesh>();
        static readonly Dictionary<int, ITextContext> TextContextCache = new Dictionary<int, ITextContext>();

        static int GetTextId(string text, Size size, GUIStyle style, GUIState state)
        {
            int hash = 17;
            hash = hash * 23 + text.GetHashCode();
            hash = hash * 23 + size.GetHashCode();
            hash = hash * 23 + style.GetHashCode();
            hash = hash * 23 + state.GetHashCode();
            return hash;
        }

        /// <summary>
        /// build the text context against the size and style
        /// </summary>
        internal static TextMesh GetTextMesh(string text, Size size, GUIStyle style, GUIState state)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (style == null) throw new ArgumentNullException(nameof(style));

            //TODO re-think text mesh caching method and when to rebuild and remove unused text mesh

            int textMeshId = GetTextId(text, size, style, state);

            TextMesh mesh;
            if (TextMeshCache.TryGetValue(textMeshId, out mesh))
            {
                return mesh;
            }
            else
            {
                // create a text mesh
                ITextContext textContext = GetTextContext(text, size, style, state);
                mesh = new TextMesh();
                mesh.Build(Point.Zero, style, textContext);
                TextMeshCache.Add(textMeshId, mesh);
            }

            return mesh;
        }

        internal static ITextContext GetTextContext(string text, Size size, GUIStyle style, GUIState state)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (style == null) throw new ArgumentNullException(nameof(style));

            int textMeshId = GetTextId(text, size, style, state);

            ITextContext textContext;
            if (TextContextCache.TryGetValue(textMeshId, out textContext))
            {
                return textContext;
            }
            else
            {
                // create a TextContent for the text
                var fontFamily = style.Get<string>(GUIStyleName.FontFamily, state);
                var fontSize = style.Get<double>(GUIStyleName.FontSize, state);
                var fontStretch = (FontStretch)style.Get<int>(GUIStyleName.FontStretch, state);
                var fontStyle = (FontStyle)style.Get<int>(GUIStyleName.FontStyle, state);
                var fontWeight = (FontWeight)style.Get<int>(GUIStyleName.FontWeight, state);
                var textAlignment = (TextAlignment)style.Get<int>(GUIStyleName.TextAlignment, state);
                textContext = Application.platformContext.CreateTextContext(
                    text,
                    fontFamily, (int)fontSize, fontStretch, fontStyle, fontWeight,
                    (int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height),
                    textAlignment);
                textContext.Build(Point.Zero, Color.Clear, null);
                TextContextCache.Add(textMeshId, textContext);
            }
            return textContext;
        }
    }
}