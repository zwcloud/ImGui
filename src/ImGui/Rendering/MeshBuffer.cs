namespace ImGui.Rendering
{
    internal class MeshBuffer
    {
        public Mesh ShapeMesh { get; } = new Mesh();

        public TextMesh TextMesh { get; } = new TextMesh();

        public Mesh ImageMesh { get; } = new Mesh();

        public void Clear()
        {
            this.ShapeMesh.Clear();
            this.TextMesh.Clear();
            this.ImageMesh.Clear();
        }

        public void Init()
        {
            this.ShapeMesh.CommandBuffer.Add(DrawCommand.Default);
            this.TextMesh.Commands.Add(DrawCommand.Default);
            this.ImageMesh.CommandBuffer.Add(DrawCommand.Default);
        }

        public void Build(MeshList meshList)
        {
            foreach (var mesh in meshList.ShapeMeshes)
            {
                this.ShapeMesh.Append(mesh);
            }
            foreach (var mesh in meshList.ImageMeshes)
            {
                this.ImageMesh.Append(mesh);
            }
            foreach (var textMesh in meshList.TextMeshes)
            {
                this.TextMesh.Append(textMesh, Vector.Zero);
            }
        }

        internal string OwnerName;
    }
}