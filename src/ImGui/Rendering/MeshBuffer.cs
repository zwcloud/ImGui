namespace ImGui.Rendering
{
    internal static class MeshBuffer
    {
        public static Mesh ShapeMesh { get; } = new Mesh();

        public static TextMesh TextMesh { get; } = new TextMesh();

        public static Mesh ImageMesh { get; } = new Mesh();

        public static void Clear()
        {
            ShapeMesh.Clear();
            TextMesh.Clear();
            ImageMesh.Clear();
        }

        public static void Init()
        {
            ShapeMesh.CommandBuffer.Add(DrawCommand.Default);
        }
    }
}