using System.Collections.Generic;
using ImGui.Common.Primitive;
using ImGui.GraphicsAbstraction;
using ImGui.Rendering;

namespace ImGui.GraphicsImplementation
{
    internal class CairoPrimitiveRenderer : IPrimitiveRenderer
    {
        public void Stroke(Primitive primitive, Brush brush, StrokeStyle strokeStyle)
        {
            throw new System.NotImplementedException();
        }

        public void Fill(Primitive primitive, Brush brush)
        {
            throw new System.NotImplementedException();
        }

        public void DrawText(TextPrimitive primitive, string fontFamily, double fontSize, Color fontColor,
            FontStyle fontStyle, FontWeight fontWeight)
        {
            throw new System.NotImplementedException();
        }

        public void DrawImage(ImagePrimitive primitive, Brush brush)
        {
            throw new System.NotImplementedException();
        }
    }
}