using ImGui.Common.Primitive;
using ImGui.Rendering;

namespace ImGui.GraphicsAbstraction
{
    internal interface IPrimitiveRenderer
    {
        void DrawPath(PathPrimitive primitive);

        void DrawText(TextPrimitive primitive, Rect rect, string fontFamily, double fontSize, Color fontColor,
            FontStyle fontStyle, FontWeight fontWeight);

        void DrawImage(ImagePrimitive primitive, Color tintColor);
    }
}