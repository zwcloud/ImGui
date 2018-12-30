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

        public void Clear()
        {
            foreach (var shapeMesh in this.ShapeMeshes)
            {
                MeshPool.ShapeMeshPool.Put(shapeMesh);
            }

            foreach (var imageMesh in this.ImageMeshes)
            {
                MeshPool.ImageMeshPool.Put(imageMesh);
            }

            foreach (var textMesh in this.TextMeshes)
            {
                MeshPool.TextMeshPool.Put(textMesh);
            }

            this.ShapeMeshes.Clear();
            this.ImageMeshes.Clear();
            this.TextMeshes.Clear();
        }
    }
}