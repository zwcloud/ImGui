using ImGui.Common.Primitive;
using ImGui.Rendering;

namespace ImGui.GraphicsAbstraction
{
    internal interface IPrimitiveRenderer
    {
        void DrawPath(PathPrimitive primitive, Vector offset);

        void DrawText(TextPrimitive primitive, Rect rect, StyleRuleSet style);

        void DrawImage(ImagePrimitive primitive, Rect rect, StyleRuleSet style);

        void DrawBoxModel(Rect rect, StyleRuleSet style);

        void DrawBoxModel(TextPrimitive textPrimitive, Rect rect, StyleRuleSet style);

        void DrawBoxModel(ImagePrimitive imagePrimitive, Rect rect, StyleRuleSet style);
    }
}