using System.Collections.Generic;
using ImGui.Rendering;

namespace ImGui.GraphicsAbstraction
{
    internal interface IPrimitiveRenderer
    {
        void Draw(List<Primitive> primitives);
    }
}