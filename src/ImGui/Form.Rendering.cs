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
        private MeshList backgroundMeshList { get; set; } = new MeshList();
        private MeshBuffer backgroundMeshBuffer { get; set; } = new MeshBuffer();

        private MeshList foregroundMeshList { get; set; } = new MeshList();
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
        

        internal void RenderBackground(Size size, IRenderer renderer)
        {
            backgroundNode.Render(this.backgroundRenderContext);

            //rebuild mesh buffer
            backgroundMeshBuffer.Clear();
            backgroundMeshBuffer.Init();
            backgroundMeshBuffer.Append(this.backgroundMeshList);

            backgroundMeshList.Clear();

            //draw meshes in MeshBuffer with underlying native renderer(OpenGL\Direct3D\Vulkan)
            renderer.DrawMeshes((int)size.Width, (int)size.Height,
                (
                    shapeMesh: this.backgroundMeshBuffer.ShapeMesh,
                    imageMesh: this.backgroundMeshBuffer.ImageMesh,
                    textMesh: this.backgroundMeshBuffer.TextMesh
                )
            );

            Metrics.VertexNumber += backgroundMeshBuffer.ShapeMesh.VertexBuffer.Count
                                    + backgroundMeshBuffer.ImageMesh.VertexBuffer.Count
                                    + backgroundMeshBuffer.TextMesh.VertexBuffer.Count;
            Metrics.IndexNumber += backgroundMeshBuffer.ShapeMesh.IndexBuffer.Count
                                   + backgroundMeshBuffer.ImageMesh.IndexBuffer.Count
                                   + backgroundMeshBuffer.TextMesh.IndexBuffer.Count;
        }

        internal void RenderForeground(Size size, IRenderer renderer)
        {
            foregroundNode.Render(this.foregroundRenderContext);

            //rebuild mesh buffer
            foregroundMeshBuffer.Clear();
            foregroundMeshBuffer.Init();
            foregroundMeshBuffer.Append(this.foregroundMeshList);

            foregroundMeshList.Clear();

            //draw meshes in MeshBuffer with underlying native renderer(OpenGL\Direct3D\Vulkan)
            renderer.DrawMeshes((int)size.Width, (int)size.Height,
                (
                    shapeMesh: this.foregroundMeshBuffer.ShapeMesh,
                    imageMesh: this.foregroundMeshBuffer.ImageMesh,
                    textMesh: this.foregroundMeshBuffer.TextMesh
                )
            );
            
            Metrics.VertexNumber += foregroundMeshBuffer.ShapeMesh.VertexBuffer.Count
                                    + foregroundMeshBuffer.ImageMesh.VertexBuffer.Count
                                    + foregroundMeshBuffer.TextMesh.VertexBuffer.Count;
            Metrics.IndexNumber += foregroundMeshBuffer.ShapeMesh.IndexBuffer.Count
                                   + foregroundMeshBuffer.ImageMesh.IndexBuffer.Count
                                   + foregroundMeshBuffer.TextMesh.IndexBuffer.Count;
        }
    }
}
