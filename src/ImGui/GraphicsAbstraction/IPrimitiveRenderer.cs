using ImGui.Common.Primitive;
using ImGui.Rendering;

namespace ImGui.GraphicsAbstraction
{
    internal interface IPrimitiveRenderer
    {
        void DrawPath(PathPrimitive primitive);

        void DrawText(TextPrimitive primitive, Rect rect, GUIStyle style);

        void DrawImage(ImagePrimitive primitive, Rect rect, GUIStyle style);
    }
}