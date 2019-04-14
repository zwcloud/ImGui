using System;
using ImGui.GraphicsImplementation;

namespace ImGui.Rendering
{
    /// <summary>
    /// This class accumulates state during a render pass of the scene.
    /// </summary>
    internal class RenderContext
    {
        public RenderContext(BuiltinGeometryRenderer renderer, MeshList meshList)
        {
            this.renderer = renderer;
            this.meshList = meshList;
        }

        public void ConsumeContent(DrawingContent content)
        {
            //TODO converts other content to mesh and save into meshList
        }

        /// <summary>
        /// Convert PathGeometryData into GPU renderable resources (Mesh) and save into the meshList.
        /// TODO this should be implemented carefully: minimum object creation and little CPU consumption.
        /// TODO consider how to optimize cached content -DrawingContent- data like PathGeometryData,
        /// to make it more suitable for converting to mesh.
        /// </summary>
        private void AppendPathGeometryData(PathGeometryData geometry)
        {
            BeginRenderShape();

            var offset = geometry.Offset;
            var path = geometry.Path;
            var fillRule = geometry.FillRule;//not used

            foreach (var command in path)
            {
                switch (command.Type)
                {
                    case PathCommandType.PathMoveTo:
                    {
                        var cmd = (MoveToCommand)command;
                        this.renderer.PathMoveTo(cmd.Point + offset);
                        break;
                    }
                    case PathCommandType.PathLineTo:
                    {
                        var cmd = (LineToCommand)command;
                        this.renderer.PathLineTo(cmd.Point + offset);
                        break;
                    }
                    case PathCommandType.PathCurveTo:
                    {
                        throw new NotImplementedException();
                        break;
                    }
                    case PathCommandType.PathClosePath:
                    {
                        this.renderer.PathClose();
                        break;
                    }
                    case PathCommandType.PathArc:
                    {
                        var cmd = (ArcCommand)command;
                        this.renderer.PathArcFast(cmd.Center + offset, cmd.Radius, cmd.Amin, cmd.Amax);
                        break;
                    }
                    case PathCommandType.Stroke:
                    {
                        var cmd = (StrokeCommand)command;
                        this.renderer.PathStroke(cmd.Color, false, cmd.LineWidth);
                        break;
                    }
                    case PathCommandType.Fill:
                    {
                        var cmd = (FillCommand)command;
                        this.renderer.PathFill(cmd.Color);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            EndRenderShape();
        }

        private void BeginRenderShape()
        {
            var shapeMesh = MeshPool.ShapeMeshPool.Get();
            shapeMesh.Clear();
            shapeMesh.CommandBuffer.Add(DrawCommand.Default);
            this.currentShapeMesh = shapeMesh;
        }

        private void EndRenderShape()
        {
            this.currentShapeMesh = null;
            this.meshList.AddOrUpdateShapeMesh(this.currentShapeMesh);
        }

        private readonly MeshList meshList;
        private readonly BuiltinGeometryRenderer renderer;
        private Mesh currentShapeMesh;
    }
}