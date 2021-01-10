using ImGui.Development;
using ImGui.OSAbstraction.Graphics;
using ImGui.Rendering;

namespace ImGui
{
    public partial class Form
    {
        /// <summary>
        /// First draw context to be rendered.
        /// </summary>
        internal DrawingContext BackgroundDrawingContext { get; private set; }

        /// <summary>
        /// Last draw context to be rendered. This is where we the render most debug overlays.
        /// </summary>
        internal DrawingContext ForegroundDrawingContext { get; private set; }

        private Node backgroundNode = new Node("#" + nameof(backgroundNode));

        private Node foregroundNode = new Node("#" + nameof(foregroundNode));

        private readonly GraphicsImplementation.BuiltinGeometryRenderer backgroundGeometryRenderer
            = new GraphicsImplementation.BuiltinGeometryRenderer();
        private readonly GraphicsImplementation.BuiltinGeometryRenderer foregroundGeometryRenderer
            = new GraphicsImplementation.BuiltinGeometryRenderer();
        internal MeshList backgroundMeshList { get; set; } = new MeshList();
        private MeshBuffer backgroundMeshBuffer { get; set; } = new MeshBuffer();

        internal MeshList foregroundMeshList { get; set; } = new MeshList();
        private MeshBuffer foregroundMeshBuffer { get; set; } = new MeshBuffer();

        private RenderContext backgroundRenderContext;
        private RenderContext foregroundRenderContext;

        internal void InitializeBackForegroundRenderContext()
        {
            backgroundRenderContext = new RenderContext(backgroundGeometryRenderer, backgroundMeshList);
            foregroundRenderContext = new RenderContext(foregroundGeometryRenderer, foregroundMeshList);
        }

        internal void ForeBackGroundRenderOpen()
        {
            this.BackgroundDrawingContext = backgroundNode.RenderOpen();
            this.ForegroundDrawingContext = foregroundNode.RenderOpen();
        }

        internal void ForeBackGroundRenderClose()
        {
            this.BackgroundDrawingContext.Close();
            this.ForegroundDrawingContext.Close();
        }

        internal void RenderToBackgroundList()
        {
            backgroundNode.Render(this.backgroundRenderContext);

            //rebuild mesh buffer
            backgroundMeshBuffer.Clear();
            backgroundMeshBuffer.Init();
            backgroundMeshBuffer.Append(this.backgroundMeshList);

            backgroundMeshList.Clear();
        }

        internal void RenderToForegroundList()
        {
            foregroundNode.Render(this.foregroundRenderContext);
        }
    }
}
