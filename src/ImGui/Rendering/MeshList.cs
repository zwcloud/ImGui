using System.Collections.Generic;

namespace ImGui.Rendering
{
    internal static class MeshList
    {
        public static List<Mesh> ShapeMeshes { get; } = new List<Mesh>();

        public static List<TextMesh> TextMeshes { get; } = new List<TextMesh>();

        public static List<Mesh> ImageMeshes { get; } = new List<Mesh>();
    }
}