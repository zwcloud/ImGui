using ImGui.Common.Primitive;

namespace ImGui.Rendering
{
    internal class MeshBuffer
    {
        public Mesh ShapeMesh { get; } = new Mesh();

        public TextMesh TextMesh { get; } = new TextMesh();

        public Mesh ImageMesh { get; } = new Mesh();

        public void Clear()
        {
            ShapeMesh.Clear();
            TextMesh.Clear();
            ImageMesh.Clear();
        }

        public void Init()
        {
            ShapeMesh.CommandBuffer.Add(DrawCommand.Default);
            TextMesh.Commands.Add(DrawCommand.Default);
            ImageMesh.CommandBuffer.Add(DrawCommand.Default);
        }

        public void Build(MeshList meshList)
        {
            foreach (var mesh in meshList.ShapeMeshes)
            {
                if (mesh.Visible)
                {
                    ShapeMesh.Append(mesh);
                }
            }
            foreach (var mesh in meshList.ImageMeshes)
            {
                if (mesh.Visible)
                {
                    ImageMesh.Append(mesh);
                }
            }
            foreach (var textMesh in meshList.TextMeshes)
            {
                if (textMesh.Visible)
                {
                    TextMesh.Append(textMesh, Vector.Zero);
                }
            }
        }
    }
}