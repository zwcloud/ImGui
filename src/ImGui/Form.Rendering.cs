using System;
using ImGui.OSAbstraction.Graphics;
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

        void InitializeBackForegroundRenderContext()
        {
            backgroundRenderContext = new RenderContext(backgroundGeometryRenderer, backgroundMeshList);
            foregroundRenderContext = new RenderContext(foregroundGeometryRenderer, foregroundMeshList);
        }

        void RenderBackground()
        {
            backgroundNode.Render(this.backgroundRenderContext);

            //rebuild mesh buffer
            backgroundMeshBuffer.Clear();
            backgroundMeshBuffer.Init();
            backgroundMeshBuffer.Build(this.backgroundMeshList);

            backgroundMeshList.Clear();

            var size = ClientSize;
            //draw meshes in MeshBuffer with underlying native renderer(OpenGL\Direct3D\Vulkan)
            renderer.DrawMeshes((int)size.Width, (int)size.Height,
                (
                    shapeMesh: this.backgroundMeshBuffer.ShapeMesh,
                    imageMesh: this.backgroundMeshBuffer.ImageMesh,
                    textMesh: this.backgroundMeshBuffer.TextMesh
                )
            );
        }

        void RenderForeground()
        {
            foregroundNode.Render(this.foregroundRenderContext);

            //rebuild mesh buffer
            foregroundMeshBuffer.Clear();
            foregroundMeshBuffer.Init();
            foregroundMeshBuffer.Build(this.foregroundMeshList);

            foregroundMeshList.Clear();

            var size = ClientSize;
            //draw meshes in MeshBuffer with underlying native renderer(OpenGL\Direct3D\Vulkan)
            renderer.DrawMeshes((int)size.Width, (int)size.Height,
                (
                    shapeMesh: this.foregroundMeshBuffer.ShapeMesh,
                    imageMesh: this.foregroundMeshBuffer.ImageMesh,
                    textMesh: this.foregroundMeshBuffer.TextMesh
                )
            );
        }
    }
}
