using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using ImGui.Rendering;

namespace ImGui.UnitTest
{
    public static class Util
    {
        private static readonly string OutputPath = Assembly.GetExecutingAssembly().Location.Substring(0, 2) + "\\ImGui.UnitTest.Output";

        public static void CheckEchoLogger()
        {
            var processes = Process.GetProcessesByName("EchoLogger.Server");
            if (processes.Length == 0)
            {
                throw new InvalidOperationException("EchoLogger.Server not started.");
            }
        }

        public static void SelectFile(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new FileNotFoundException(path);
            }
            Process.Start("explorer", "/select," + path);
        }

        public static void OpenDirectory(string path)
        {
            if(!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException(path);
            }
            Process.Start("explorer", path);
        }

        public static void OpenImage(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }
            if (!File.Exists(@"C:\Program Files\Windows Photo Viewer\PhotoViewer.dll"))
            {
                throw new FileNotFoundException("Windows Photo Viewer not found.");
            }
            Process.Start("rundll32.exe", @"""C:\Program Files\Windows Photo Viewer\PhotoViewer.dll"",ImageView_Fullscreen " + path);
        }

        public static void OpenModel(string path)
        {
            const string ModelViewerPath = @"E:\Program Files (green)\open3mod_1_1_standalone\open3mod.exe";
            if(!File.Exists(ModelViewerPath))
            {
                throw new FileNotFoundException("ModelViewer(open3mod) not found.");
            }
            if (!Directory.Exists(path))
            {
                throw new FileNotFoundException(path);
            }
            Process.Start(ModelViewerPath, path);
        }


        internal static void DrawNode(Node node, [System.Runtime.CompilerServices.CallerMemberName] string memberName = "")
        {
            using (Cairo.ImageSurface surface = new Cairo.ImageSurface(Cairo.Format.Argb32, (int)node.Rect.Width, (int)node.Rect.Height))
            using (Cairo.Context context = new Cairo.Context(surface))
            {
                Draw(context, node);

                if (!System.IO.Directory.Exists(OutputPath))
                {
                    System.IO.Directory.CreateDirectory(OutputPath);
                }

                string filePath = OutputPath + "\\" + DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss-fff_") + surface.GetHashCode() + memberName + ".png";
                surface.WriteToPng(filePath);
                Util.OpenImage(filePath);
            }
        }

        private static void Draw(Cairo.Context context, Node node)
        {
            foreach (var entry in node.Children)
            {
                if (entry.HorizontallyStretched || entry.VerticallyStretched)
                {
                    context.FillRectangle(entry.Rect, CairoEx.ColorLightBlue);
                }
                else if (entry.IsFixedWidth || entry.IsFixedHeight)
                {
                    context.FillRectangle(entry.Rect, CairoEx.ColorOrange);
                }
                else
                {
                    context.FillRectangle(entry.Rect, CairoEx.ColorPink);
                }
                context.StrokeRectangle(entry.Rect, CairoEx.ColorBlack);
                var innerGroup = entry;
                if (innerGroup.Children != null)
                {
                    context.Save();
                    Draw(context, innerGroup);
                    context.Restore();
                }
            }
        }

    }
}
