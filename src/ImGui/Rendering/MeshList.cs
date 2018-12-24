using System;
using System.Collections.Generic;

namespace ImGui.Rendering
{
    internal class MeshList
    {
        public LinkedList<Mesh> ShapeMeshes { get; } = new LinkedList<Mesh>();

        public LinkedList<TextMesh> TextMeshes { get; } = new LinkedList<TextMesh>();

        public LinkedList<Mesh> ImageMeshes { get; } = new LinkedList<Mesh>();

        public void AddOrUpdateShapeMesh(Mesh shapeMesh)
        {
            if (this.ShapeMeshes.Contains(shapeMesh))
            {
                return;
            }

            this.ShapeMeshes.AddLast(shapeMesh);
        }

        public void AddOrUpdateTextMesh(TextMesh textMesh)
        {
            if (this.TextMeshes.Contains(textMesh))
            {
                return;
            }

            this.TextMeshes.AddLast(textMesh);
        }

        public void AddOrUpdateImageMesh(Mesh imageMesh)
        {
            if (imageMesh == null)
            {
                throw new ArgumentNullException(nameof(imageMesh));
            }

            if (this.ImageMeshes.Contains(imageMesh))
            {
                return;
            }

            this.ImageMeshes.AddLast(imageMesh);
        }
    }
}