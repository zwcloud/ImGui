using System;
using ImGui.Common.Primitive;
using ImGui.OSAbstraction.Graphics;
#if false
namespace ImGui.Rendering
{
    internal enum ContentType
    {
        None,
        Text,
        Texture
    }

    internal class BoxPrimitive : Primitive
    {
        private Rect borderBox;
        private Rect paddingBox;
        private Rect contentBox;
        private ContentType contentType;
        private string contentText;
        private ITexture contentTexture;

        public BoxPrimitive(Rect rect, GUIStyle style, GUIState state = GUIState.Normal)
        {
            this.borderBox = rect;
            this.paddingBox = Rect.Inflate(rect,
                -style.Get<int>(GUIStyleName.BorderTop, state),
                -style.Get<int>(GUIStyleName.BorderRight, state),
                -style.Get<int>(GUIStyleName.BorderBottom, state),
                -style.Get<int>(GUIStyleName.BorderLeft, state)
            );
            this.contentType = ContentType.None;
        }

        public BoxPrimitive(Rect rect, string text, GUIStyle style, GUIState state = GUIState.Normal) : this(rect, style, state)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }
            this.contentBox = new Rect(rect.Location, style.CalcSize(text, state));
            this.contentText = text;
            this.contentType = ContentType.Text;
        }

        public BoxPrimitive(Rect rect, ITexture texture, GUIStyle style, GUIState state = GUIState.Normal) : this(rect, style, state)
        {
            if (texture == null)
            {
                throw new ArgumentNullException(nameof(texture));
            }
            this.contentBox = new Rect(rect.Location, style.CalcSize(texture, state));
            this.contentTexture = texture;
            this.contentType = ContentType.Texture;
        }
    }
}
#endif