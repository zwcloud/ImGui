using System.Collections.Generic;
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
    }
}