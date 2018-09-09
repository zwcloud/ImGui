using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Cairo;
using ImGui.Common.Primitive;
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

        private static void Draw(Context context, Node node)
        {
            var isGroup = node.Children != null;

            if (!isGroup)
            {
                if (node.LayoutEntry.HorizontallyStretched || node.LayoutEntry.VerticallyStretched)
                {
                    context.FillRectangle(node.Rect, CairoEx.ColorLightBlue);
                }
                else if (node.LayoutEntry.IsFixedWidth || node.LayoutEntry.IsFixedHeight)
                {
                    context.FillRectangle(node.Rect, CairoEx.ColorOrange);
                }
                else
                {
                    context.FillRectangle(node.Rect, CairoEx.ColorGreen);
                }
            }

            context.StrokeRectangle(node.Rect, CairoEx.ColorBlack);

            if (!isGroup) return;

            context.Save();
            foreach (var childNode in node.Children)
            {
                Draw(context, childNode);
            }
            context.Restore();
        }
        
        private static void Draw(Context context, PathPrimitive primitive)
        {
            foreach (var command in primitive.Path)
            {
                switch (command.Type)
                {
                    case PathCommandType.PathMoveTo:
                    {
                        var cmd = (MoveToCommand)command;
                        context.MoveTo(cmd.Point.ToPointD());
                        break;
                    }
                    case PathCommandType.PathLineTo:
                    {
                        var cmd = (LineToCommand)command;
                        context.LineTo(cmd.Point.ToPointD());
                        break;
                    }
                    case PathCommandType.PathCurveTo:
                    {
                        var cmd = (CurveToCommand) command;
                        context.CurveTo(cmd.ControlPoint0.ToPointD(), cmd.ControlPoint1.ToPointD(), cmd.EndPoint.ToPointD());
                        break;
                    }
                    case PathCommandType.PathClosePath:
                    {
                        context.ClosePath();
                        break;
                    }
                    case PathCommandType.Stroke:
                    {
                        var cmd = (StrokeCommand) command;
                        context.Color = cmd.Color.ToCairoColor();
                        context.LineWidth = cmd.LineWidth;
                        context.Stroke();
                        break;
                    }
                    case PathCommandType.Fill:
                    {
                        var cmd = (FillCommand) command;
                        context.Color = cmd.Color.ToCairoColor();
                        context.Fill();
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private static Size GetPrimitiveSize(PathPrimitive primitive, out Common.Primitive.Point min)
        {
            var minX = 0.0;
            var minY = 0.0;
            var maxX = 0.0;
            var maxY = 0.0;

            void updateMinMax(Common.Primitive.Point point)
            {
                minX = Math.Min(minX, point.x);
                minY = Math.Min(minY, point.y);
                maxX = Math.Max(maxX, point.x);
                maxY = Math.Max(maxY, point.y);
            }
            foreach (var command in primitive.Path)
            {
                switch (command.Type)
                {
                    case PathCommandType.PathMoveTo:
                    {
                        var cmd = (MoveToCommand)command;
                        var point = cmd.Point;
                        updateMinMax(point);
                        break;
                    }
                    case PathCommandType.PathLineTo:
                    {
                        var cmd = (LineToCommand)command;
                        var point = cmd.Point;
                        updateMinMax(point);
                        break;
                    }
                    case PathCommandType.PathCurveTo:
                    {
                        var cmd = (CurveToCommand) command;
                        var c0 = cmd.ControlPoint0;
                        var c1 = cmd.ControlPoint1;
                        var end = cmd.EndPoint;
                        updateMinMax(c0);
                        updateMinMax(c1);
                        updateMinMax(end);
                        break;
                    }
                    case PathCommandType.PathClosePath:
                    case PathCommandType.Stroke:
                    case PathCommandType.Fill:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            min = new Common.Primitive.Point(minX, minY);
            return new Size(maxX - minX, maxY - minY);
        }

        internal static void DrawPathPrimitive(PathPrimitive primitive, [System.Runtime.CompilerServices.CallerMemberName]
            string memberName = "")
        {
            var size = GetPrimitiveSize(primitive, out Common.Primitive.Point minPoint);
            using (Cairo.ImageSurface surface = new Cairo.ImageSurface(Cairo.Format.Argb32, (int)size.Width, (int)size.Height))
            using (Cairo.Context context = new Cairo.Context(surface))
            {
                context.Translate(-minPoint.x, -minPoint.y);
                Draw(context, primitive);

                if (!System.IO.Directory.Exists(OutputPath))
                {
                    System.IO.Directory.CreateDirectory(OutputPath);
                }

                string filePath = OutputPath + "\\" + DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss-fff_") + surface.GetHashCode() + memberName + ".png";
                surface.WriteToPng(filePath);
                Util.OpenImage(filePath);
            }
        }
    }
}
