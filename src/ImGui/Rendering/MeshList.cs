using System.Collections.Generic;

namespace ImGui.Rendering
{
    internal class MeshList
    {
        public LinkedList<Mesh> ShapeMeshes { get; } = new LinkedList<Mesh>();

        public LinkedList<TextMesh> TextMeshes { get; } = new LinkedList<TextMesh>();

        public LinkedList<Mesh> ImageMeshes { get; } = new LinkedList<Mesh>();
    }
}