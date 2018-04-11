namespace ImGui.Rendering
{
    internal static class MeshPool
    {
        public static ObjectPool<Mesh> ShapeMeshPool = new ObjectPool<Mesh>(256);
        public static ObjectPool<Mesh> ImageMeshPool = new ObjectPool<Mesh>(256);
        public static ObjectPool<TextMesh> TextMeshPool = new ObjectPool<TextMesh>(256);
    }
}