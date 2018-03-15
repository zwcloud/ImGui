using System.Collections.Generic;
using ImGui.Rendering;

namespace ImGui.GraphicsAbstraction
{
    internal interface IPrimitiveRenderer
    {
        void Stroke(Primitive primitive, Brush brush, StrokeStyle strokeStyle);

        void Fill(Primitive primitive, Brush brush);
    }
}