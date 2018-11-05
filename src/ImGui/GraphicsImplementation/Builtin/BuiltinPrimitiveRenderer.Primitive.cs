using ImGui.Common.Primitive;
using ImGui.Rendering;

namespace ImGui.GraphicsImplementation
{
    internal partial class BuiltinPrimitiveRenderer
    {
        public void DrawPathPrimitive(Mesh shapeMesh, PathPrimitive pathPrimitive, Vector offset)
        {
            shapeMesh.Clear();
            shapeMesh.CommandBuffer.Add(DrawCommand.Default);

            //draw
            this.SetShapeMesh(shapeMesh);
            this.DrawPath(pathPrimitive, offset);
            this.SetShapeMesh(null);
        }

        public void DrawImagePrimitive(Vector offset, Mesh imageMesh,
            ImagePrimitive imagePrimitive, Rect rect, StyleRuleSet style)
        {
            imageMesh.Clear();

            this.SetImageMesh(imageMesh);
            this.DrawImage(imagePrimitive, Rect.Offset(rect, offset), style);
            this.SetImageMesh(null);
        }

        public void DrawTextPrimitive(Vector offset, TextMesh textMesh,
            TextPrimitive textPrimitive, Rect rect, StyleRuleSet style)
        {
            textMesh.Clear();

            //draw
            this.SetTextMesh(textMesh);
            this.DrawText(textPrimitive, Rect.Offset(rect, offset), style);
            this.SetTextMesh(null);
        }
    }
}