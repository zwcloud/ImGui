using System;
using System.Collections;
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

        public const string WindowName = "Dear ImGui Metrics";

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
            if(TreeNode("Windows"))
            {
                foreach (var window in windows)
                {
                    NodeWindow(window, "Window");
                }
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
                NodeDrawList(window, "DrawList");
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
            }
            TreePop();
        }

        private static void NodeDrawList(Window window, string label)
        {
            //TODO display draw list of the window
        }
    }
}
