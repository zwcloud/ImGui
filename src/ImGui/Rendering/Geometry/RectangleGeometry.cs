using ImGui.Common.Primitive;

namespace ImGui.Rendering
{
    internal class RectangleGeometry : PathGeometry
    {
        public RectangleGeometry(Rect rect)
        {
            this.Rect = rect;
        }

        public Rect Rect { get; set; }

        public override void UpdateContent()
        {
            throw new System.NotImplementedException();
        }
    }
}