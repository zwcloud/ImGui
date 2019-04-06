using ImGui.Common.Primitive;

namespace ImGui.Rendering
{
    /// <summary>
    /// Base class for all primitive
    /// </summary>
    internal abstract class Primitive
    {
        /// <summary>
        /// Offset relative to the position of the <see cref="Node"/>.
        /// </summary>
        public Vector Offset { get; set; }

        public abstract void UpdateContent();
    }
}
