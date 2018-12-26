using ImGui.Common.Primitive;
using ImGui.Rendering;

namespace ImGui.GraphicsImplementation
{
    internal partial class BuiltinPrimitiveRenderer
    {
        public void DrawPathPrimitive(Mesh shapeMesh, PathPrimitive pathPrimitive, Vector offset)
        {
            this.SetShapeMesh(shapeMesh);
            this.DrawPath(pathPrimitive, offset);
            this.SetShapeMesh(null);
        }

        public void DrawImagePrimitive(Mesh imageMesh,
            ImagePrimitive imagePrimitive, Rect rect, StyleRuleSet style, Vector offset)
        {
            this.SetImageMesh(imageMesh);
            this.DrawImage(imagePrimitive, Rect.Offset(rect, offset), style);
            this.SetImageMesh(null);
        }

        public void DrawSlicedImagePrimitive(Mesh imageMesh,
            ImagePrimitive imagePrimitive, Rect rect, StyleRuleSet style, Vector offset)
        {
            this.SetImageMesh(imageMesh);
            this.DrawSlicedImage(imagePrimitive, Rect.Offset(rect, offset), style);
            this.SetImageMesh(null);
        }

        public void DrawTextPrimitive(TextMesh textMesh,
            TextPrimitive textPrimitive, Rect rect, StyleRuleSet style, Vector offset)
        {
            this.SetTextMesh(textMesh);
            this.DrawText(textPrimitive, Rect.Offset(rect, offset), style);
            this.SetTextMesh(null);
        }
    }
}