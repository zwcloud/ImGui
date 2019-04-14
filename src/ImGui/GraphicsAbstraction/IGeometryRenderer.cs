using ImGui.Rendering;

namespace ImGui.GraphicsAbstraction
{
    internal interface IGeometryRenderer
    {
        void DrawPath(PathGeometry geometry, Vector offset);

        void DrawText(TextGeometry geometry, Rect rect, StyleRuleSet style);

        void DrawImage(ImageGeometry geometry, Rect rect, StyleRuleSet style);

        void DrawBoxModel(Rect rect, StyleRuleSet style);

        void DrawBoxModel(TextGeometry textGeometry, Rect rect, StyleRuleSet style);

        void DrawBoxModel(ImageGeometry imageGeometry, Rect rect, StyleRuleSet style);
    }
}