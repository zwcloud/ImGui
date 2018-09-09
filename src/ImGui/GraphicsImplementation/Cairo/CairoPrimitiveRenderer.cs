using System.Collections.Generic;
using ImGui.Common.Primitive;
using ImGui.GraphicsAbstraction;
using ImGui.Rendering;

namespace ImGui.GraphicsImplementation
{
    internal class CairoPrimitiveRenderer : IPrimitiveRenderer
    {
        public void DrawPath(PathPrimitive primitive)
        {
            throw new System.NotImplementedException();
        }

        public void DrawText(TextPrimitive primitive, Rect rect, string fontFamily, double fontSize, Color fontColor,
            FontStyle fontStyle, FontWeight fontWeight)
        {
            throw new System.NotImplementedException();
        }

        public void DrawImage(ImagePrimitive primitive, Color tintColor)
        {
            throw new System.NotImplementedException();
        }
    }
}