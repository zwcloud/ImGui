using ImGui.Common.Primitive;
using ImGui.Rendering;

namespace ImGui.GraphicsImplementation
{
    internal static class BuiltinPrimitiveRendererExtension
    {
        public static bool Draw(this BuiltinGeometryRenderer geometryRenderer, Visual visual, Rect rootClipRect, MeshList meshList)
        {
            if (!visual.ActiveInTree)
            {
                return false;
            }

            if (visual is Node)
            {
                var clipRect = visual.GetClipRect(rootClipRect);
                if (visual.IsClipped(clipRect))
                {
                    return false;
                }
            }

            visual.Draw(geometryRenderer, meshList);
            return true;
        }

    }
}