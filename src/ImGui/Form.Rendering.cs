using System;
using ImGui.Rendering;

namespace ImGui
{
    partial class Form
    {
        /// <summary>
        /// First draw context to be rendered.
        /// </summary>
        public DrawingContext BackgroundDrawingContext { get; private set; }

        /// <summary>
        /// Last draw context to be rendered. This is where we the render most debug overlays.
        /// </summary>
        public DrawingContext ForegroundDrawingContext { get; private set; }

        private Node backgroundNode = new Node(nameof(backgroundNode));

        private Node foregroundNode = new Node(nameof(foregroundNode));

        private readonly GraphicsImplementation.BuiltinGeometryRenderer backforegroudGeometryRenderer
            = new GraphicsImplementation.BuiltinGeometryRenderer();

        private MeshList backforegroudMeshList { get; set; } = new MeshList();

        private RenderContext backforegroudRenderContext;

        void InitializeBackForegroundRenderContext()
        {
            backforegroudRenderContext = new RenderContext(backforegroudGeometryRenderer, backforegroudMeshList);
        }
    }
}
