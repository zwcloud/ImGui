using ImGui.Common.Primitive;
using ImGui.Rendering;

namespace ImGui.GraphicsAbstraction
{
    internal interface IPrimitiveRenderer
    {
        void Stroke(Primitive primitive, Brush brush, StrokeStyle strokeStyle);

        void Fill(Primitive primitive, Brush brush);

        void DrawText(TextPrimitive primitive, string fontFamily, double fontSize, Color fontColor,
            FontStyle fontStyle, FontWeight fontWeight);

        void DrawImage(ImagePrimitive primitive, Brush brush);
    }
}