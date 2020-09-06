using System;
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

            if (!GUI.Begin(WindowName, ref windowOpened))
            {
                GUI.End();
                return;
            }
            
            // Basic info
            GUIContext g = Form.current.uiContext;
            Text("Hello ImGui {0}##Metrics.Version", GetVersion());
            Text("Application average {0:F3} ms/frame ({1:F1} FPS)##Metrics.FPS", 1000.0f / g.fps, g.fps);
            Text("{0} vertices, {1} indices ({2} triangles)##Metrics.VI",
                VertexNumber, IndexNumber, IndexNumber / 3);
            Text("{0} active windows ({0} visible)##Metrics.Window", ActiveWindows, RenderWindows);
            Separator("separator0");

            //Windows
            if (TreeNode("Windows"))
            {
                var windowManager = GetCurrentContext().WindowManager;
                var windows = windowManager.Windows;
                for (int i = 0; i < windows.Count; i++)
                {
                    var window = windows[i];
                    if (window == null)
                    {
                        BulletText("Window: null");
                        continue;
                    }

                    if (TreeNode($"{(window.Collapsed ? "[ ]" : "[x]")}" +
                                 $"Window[{i}] '{window.Name}', " +
                                 $"{(window.Active || window.WasActive ? 1 : 0)}"))
                    {
                        Label($"(TODO: display window info here)#TODO_Window{i}");
                    }
                    TreePop();
                }
            }
            TreePop();

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
    }
}
