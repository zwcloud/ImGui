using System;
using System.Reflection;

namespace ImGui.Development
{
    public class Metrics
    {
        public static void ShowWindow(ref bool windowOpened)
        {
            if (!GUI.Begin("Dear ImGui Metrics", ref windowOpened))
            {
                GUI.End();
                return;
            }
            
            // Basic info
            GUIContext g = Form.current.uiContext;
            GUILayout.Text("Hello ImGui {0}", GetVersion());
            GUILayout.Text("Application average {0:F3} ms/frame ({1:F1} FPS)", 1000.0f / g.fps, g.fps);

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
