using ImGui.Common.Primitive;

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

        public static void Build()
        {
            foreach (var mesh in MeshList.ShapeMeshes)
            {
                ShapeMesh.Append(mesh);
            }
            foreach (var mesh in MeshList.ImageMeshes)
            {
                ImageMesh.Append(mesh);
            }
            foreach (var textMesh in MeshList.TextMeshes)
            {
                TextMesh.Append(textMesh, Vector.Zero);
            }
        }
    }
}