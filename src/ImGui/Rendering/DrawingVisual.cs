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
    }
}