using ImGui.OSAbstraction.Graphics;

namespace ImGui
{
    internal struct DrawCommand
    {
        /// <summary>
        /// Number of indices (multiple of 3) to be rendered as triangles.
        /// Vertices are stored in <see cref="Mesh.VertexBuffer"/>, indices in <see cref="Mesh.IndexBuffer"/>.
        /// </summary>
        /// <remarks>Added when calling <see cref="ImGui.Mesh.PrimReserve"/></remarks>
        public int ElemCount { get; set; }

        /// <summary>
        /// Clipping rectangle
        /// </summary>
        public Rect ClipRect { get; set; }

        /// <summary>
        /// texture data
        /// </summary>
        public ITexture TextureData { get; set; }

        /// <summary>
        /// The default big and empty draw command without texture
        /// </summary>
        public static DrawCommand Default = new DrawCommand {ClipRect = Rect.Big, ElemCount = 0, TextureData = null};
    }
}
