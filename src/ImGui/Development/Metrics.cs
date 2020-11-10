using ImGui.Rendering;
using System;
using System.Collections.Generic;
using System.Reflection;
using static ImGui.GUILayout;

namespace ImGui.Development
{
    public class Metrics
    {
        public static long VertexNumber {get; internal set; }
        public static long IndexNumber {get; internal set; }
        public static int RenderWindows { get; set; }
        public static int ActiveWindows { get; set; }

        public const string WindowName = "Hello ImGui Metrics";

        public static void ShowWindow(ref bool windowOpened)
        {
            if(!GUI.Begin(WindowName, ref windowOpened))
            {
                GUI.End();
                return;
            }
            
            // Basic info
            GUIContext g = GetCurrentContext();
            Text("Hello ImGui {0}##Metrics.Version", GetVersion());
            Text("Application average {0:F3} ms/frame ({1:F1} FPS)##Metrics.FPS", 1000.0f / g.fps, g.fps);
            Text("{0} vertices, {1} indices ({2} triangles)##Metrics.VI",
                VertexNumber, IndexNumber, IndexNumber / 3);
            Text("{0} active windows ({0} visible)##Metrics.Window", ActiveWindows, RenderWindows);
            Separator("separator0");

            //Windows
            var windowManager = g.WindowManager;
            NodeWindows(windowManager.Windows.ToArray(), "Windows");

            GUI.End();
        }
        
        private static string GetVersion()
        {
            var entryAssembly = Assembly.GetCallingAssembly();
            var informationalVersion =
                entryAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                    .InformationalVersion;
            return informationalVersion;
        }

        private static void NodeWindows(IEnumerable<Window> windows, string label)
        {
            if (!TreeNode(label))
            {
                return;
            }
            foreach (var window in windows)
            {
                NodeWindow(window, "Window");
            }
            TreePop();
        }

        private static void NodeWindow(Window window, string label)
        {
            if(window == null)
            {
                BulletText("Window: null");
                return;
            }

            if(TreeNode($"{(window.Collapsed ? "[ ]" : "[x]")}" +
                         $"Window '{window.Name}', " +
                         $"{(window.Active || window.WasActive ? 1 : 0)}"))
            {
                NodeMeshBuffer(window, "MeshBuffer");
                BulletText(
                    "Pos: ({0:F1},{1:F1}), Size: ({2:F1},{3:F1}), ContentSize ({4:F1},{5:F1})",
                    window.Position.X, window.Position.Y, window.Size.Width, window.Size.Height,
                    window.ContentRect.Width, window.ContentRect.Height);
                BulletText("Scroll: ({0:F2},{1:F2})",
                    window.ClientAreaNode.ScrollOffset.X, window.ClientAreaNode.ScrollOffset.Y);
                BulletText("Active: {0}/{1}, ", window.Active, window.WasActive, window.Accessed);
                if (window.RootWindow != window)
                {
                    NodeWindow(window.RootWindow, "RootWindow");
                }
                BulletText("Storage: {0} entries, ~{1} bytes", window.StateStorage.EntryCount,
                    window.StateStorage.EstimatedDataSizeInBytes);
                TreePop();
            }
        }

        private static void NodeMeshBuffer(Window nodeWindow, string label)
        {
            var buffer = nodeWindow.MeshBuffer;
            var vertexCount = buffer.ImageMesh.VertexBuffer.Count
                              + buffer.ShapeMesh.VertexBuffer.Count
                              + buffer.TextMesh.VertexBuffer.Count;
            var indexCount = buffer.ImageMesh.IndexBuffer.Count
                             + buffer.ShapeMesh.IndexBuffer.Count
                             + buffer.TextMesh.IndexBuffer.Count;
            var cmdCount = buffer.ImageMesh.CommandBuffer.Count
                +buffer.ShapeMesh.CommandBuffer.Count
                +buffer.TextMesh.Commands.Count;

            bool nodeOpen = TreeNode(buffer,
                $"{label}: '{buffer.OwnerName}'" +
                $" {vertexCount} vtx, {indexCount} indices, {cmdCount} cmds");

            if (nodeOpen)
            {
                if (buffer == GetCurrentWindow().MeshBuffer)
                {
                    Text("CURRENTLY APPENDING"); // Can't display stats for active draw list! (we don't have the data double-buffered)
                }
                else
                {
                    NodeShapeMesh(buffer);
                    NodeImageMesh(buffer);
                    NodeTextMesh(buffer);
                }
                TreePop();
            }

            if(IsItemHovered())
            {
                var g = GetCurrentContext();
                g.ForegroundDrawingContext.DrawRectangle(
                    null, new Pen(Color.Yellow, 1), nodeWindow.Rect);
            }
        }

        private static void NodeShapeMesh(MeshBuffer buffer)
        {
            var mesh = buffer.ShapeMesh;
            var cmds = mesh.CommandBuffer;
            for (var i = 0; i < cmds.Count; i++)
            {
                NodeDrawCommand(mesh, i);
            }
        }

        private static void NodeImageMesh(MeshBuffer buffer)
        {
            var mesh = buffer.ShapeMesh;
            var cmds = mesh.CommandBuffer;
            for (var i = 0; i < cmds.Count; i++)
            {
                NodeDrawCommand(mesh, i);
            }
        }

        private static void NodeTextMesh(MeshBuffer buffer)
        {
            
        }

        private static void NodeDrawCommand(Mesh mesh, int cmdIndex)
        {
            DrawCommand cmd = mesh.CommandBuffer[cmdIndex];
            if (cmd.ElemCount == 0)
            {
                return;
            }
            var tex = cmd.TextureData;
            var texId = 0;
            if (tex != null)
            {
                texId = tex.GetNativeTextureId();
            }
            var clipRect = cmd.ClipRect;
            var minX = clipRect.Min.X;
            var minY = clipRect.Min.Y;
            var maxX = clipRect.Max.X;
            var maxY = clipRect.Max.Y;
            if (TreeNode($"Draw {cmd.ElemCount/3,4} triangles," +
                         $" tex 0x{texId:X8}," +
                         $" clip_rect ({minX,7:.0},{minY,7:.0})-({maxX,7:.0},{maxY,7:.0})"))
            {
                Selectable($"Mesh: ElemCount: {cmd.ElemCount}, ElemCount/3: {cmd.ElemCount/3}");
                if (IsItemHovered())
                {
                    var pen = new Pen(Color.Yellow, 1);
                    var g = GetCurrentContext();
                    for (int i = 0; i < cmd.ElemCount; i+=3)
                    {
                        var v0 = mesh.VertexBuffer[i].pos;
                        var v1 = mesh.VertexBuffer[i + 1].pos;
                        var v2 = mesh.VertexBuffer[i + 2].pos;
                        g.ForegroundDrawingContext.DrawLine(pen, v0, v1);
                        g.ForegroundDrawingContext.DrawLine(pen, v1, v2);
                        g.ForegroundDrawingContext.DrawLine(pen, v2, v0);
                    }
                }
                TreePop();
            }
        }
    }
}
