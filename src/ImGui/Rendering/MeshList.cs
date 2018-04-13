using System.Collections.Generic;

namespace ImGui.Rendering
{
    internal static class MeshList
    {
        public static LinkedList<Mesh> ShapeMeshes { get; } = new LinkedList<Mesh>();

        public static LinkedList<TextMesh> TextMeshes { get; } = new LinkedList<TextMesh>();

        public static LinkedList<Mesh> ImageMeshes { get; } = new LinkedList<Mesh>();
    }
}