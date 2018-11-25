using ImGui.Common.Primitive;
using ImGui.Rendering;

namespace ImGui.GraphicsImplementation
{
    internal static class BuiltinPrimitiveRendererExtension
    {
        public static bool DrawNode(this BuiltinPrimitiveRenderer primitiveRenderer, Node node, Rect rootClipRect, MeshList meshList)
        {
            if (!node.ActiveInTree)
            {
                return false;
            }

            var clipRect = node.GetClipRect(rootClipRect);

            if (node.IsClipped(clipRect))
            {
                return false;
            }

            node.Draw(primitiveRenderer, meshList);
            return true;
        }

    }
}