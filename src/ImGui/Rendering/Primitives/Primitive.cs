using ImGui.Common.Primitive;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImGui.Rendering
{
    /// <summary>
    /// Base class for all primitive
    /// </summary>
    internal class Primitive
    {
        /// <summary>
        /// Offset relative to the position of the <see cref="Node"/>.
        /// </summary>
        public Vector Offset { get; set; }
    }
}
