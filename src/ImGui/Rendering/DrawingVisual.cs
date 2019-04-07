using System.Diagnostics;
using ImGui.Common.Primitive;
using ImGui.GraphicsAbstraction;

namespace ImGui.Rendering
{
    internal class DrawingVisual : Visual
    {
        public DrawingVisual(int id) : base(id)
        {
        }

        public DrawingVisual(string name) : base(name)
        {
        }

        public DrawingVisual(int id, string name) : base(id, name)
        {
        }

        public override Rect GetClipRect(Rect rootClipRect)
        {
            return rootClipRect;
        }

        internal override void Draw(IGeometryRenderer renderer, MeshList meshList)
        {
            var r = renderer as GraphicsImplementation.BuiltinGeometryRenderer;
            Debug.Assert(r != null);
            r.DrawPrimitive(this.Geometry, this.Rect, this.RuleSet, meshList);
        }
    }
}