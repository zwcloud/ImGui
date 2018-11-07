using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Cairo;
using ImageSharp.Extension;
using ImGui.Common.Primitive;
using ImGui.GraphicsAbstraction;
using ImGui.GraphicsImplementation;
using ImGui.OSImplentation.Windows;
using ImGui.Rendering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Xunit;
using Color = ImGui.Common.Primitive.Color;
using Image = SixLabors.ImageSharp.Image;
using Path = System.IO.Path;
using Point = ImGui.Common.Primitive.Point;

namespace ImGui.UnitTest
{
    public static class Util
    {
        //FIXME this only works on Windows
        //For macOS and Linux the path should start with "~\" instead.
        public static readonly string OutputPath = Assembly.GetExecutingAssembly().Location.Substring(0, 2) + "/ImGui.UnitTest.Output";

        public static readonly string UnitTestRootDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..")) + Path.DirectorySeparatorChar;

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


        internal static void DrawNode(Node node, [CallerMemberName] string memberName = "")
        {
            using (ImageSurface surface = new ImageSurface(Format.Argb32, (int)node.Rect.Width, (int)node.Rect.Height))
            using (Context context = new Context(surface))
            {
                Draw(context, node);

                if (!Directory.Exists(OutputPath))
                {
                    Directory.CreateDirectory(OutputPath);
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
                if (node.RuleSet.HorizontallyStretched || node.RuleSet.VerticallyStretched)
                {
                    context.FillRectangle(node.Rect, CairoEx.ColorLightBlue);
                }
                else if (node.RuleSet.IsFixedWidth || node.RuleSet.IsFixedHeight)
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
                    case PathCommandType.PathArc:
                    {
                        var cmd = (ArcCommand)command;
                        var xc = cmd.Center.x;
                        var yc = cmd.Center.y;
                        var r = cmd.Radius;
                        var angle1 = cmd.Amin * Math.PI / 6;
                        var angle2 = cmd.Amax * Math.PI / 6;
                        context.Arc(xc, yc, r, angle1, angle2);
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

        private static Size GetPrimitiveSize(PathPrimitive primitive, out Point min)
        {
            var minX = 0.0;
            var minY = 0.0;
            var maxX = 0.0;
            var maxY = 0.0;

            void updateMinMax(Point point)
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
                    case PathCommandType.PathArc:
                    {
                        var cmd = (ArcCommand)command;
                        var c0 = cmd.Center;
                        var v = new Vector(cmd.Radius, cmd.Radius);
                        var minPoint = c0 - v;
                        var maxPoint = c0 + v;
                        updateMinMax(minPoint);
                        updateMinMax(maxPoint);
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

            min = new Point(minX, minY);
            return new Size(maxX - minX, maxY - minY);
        }

        internal static void DrawPathPrimitive(PathPrimitive primitive, [CallerMemberName]
            string memberName = "")
        {
            var size = GetPrimitiveSize(primitive, out Point minPoint);
            using (ImageSurface surface = new ImageSurface(Format.Argb32, (int)size.Width, (int)size.Height))
            using (Context context = new Context(surface))
            {
                context.Translate(-minPoint.x, -minPoint.y);
                Draw(context, primitive);

                if (!Directory.Exists(OutputPath))
                {
                    Directory.CreateDirectory(OutputPath);
                }

                string filePath = OutputPath + "\\" + DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss-fff_") + surface.GetHashCode() + memberName + ".png";
                surface.WriteToPng(filePath);
                Util.OpenImage(filePath);
            }
        }

        public static Image<Rgba32> CreateImage(byte[] data, int width, int height, bool flip)
        {
            var img = Image.LoadPixelData<Rgba32>(Configuration.Default, data, width, height);
            if (flip)
            {
                img.Mutate(x => x.Flip(FlipMode.Vertical));
            }
            return img;
        }

        public static void SaveImage(Image<Rgba32> image, string path)
        {
            using (var stream = File.OpenWrite(path))
            {
                image.SaveAsPng(stream);
            }
        }

        internal static bool CompareImage(Image<Rgba32> a, Image<Rgba32> b)
        {
            var diffPercentage = ImageComparer.PercentageDifference(a,b);
            return diffPercentage < 0.1;
        }

        internal static bool CompareImage(Image<Rgba32> a, Image<Bgra32> b)
        {
            var diffPercentage = ImageComparer.PercentageDifference(a,b);
            return diffPercentage < 0.1;
        }

        public static Image<Rgba32> LoadImage(string filePath)
        {
            return Image.Load<Rgba32>(filePath);
        }

        internal static Image<Rgba32> RenderTextMeshToImage(TextMesh textMesh, Size imageSize)
        {
            //TODO de-cuple this method with Windows platform

            var window = new Win32Window();
            window.Init(Point.Zero, imageSize, WindowTypes.Regular);

            var renderer = new Win32OpenGLRenderer();
            renderer.Init(window.Pointer, window.ClientSize);

            renderer.Clear(Color.FrameBg);
            Win32OpenGLRenderer.DrawTextMesh(renderer.shapeMaterial, textMesh,
                (int)window.ClientSize.Width, (int)window.ClientSize.Height);

            var imageRawBytes = renderer.GetRawBackBuffer(out var width, out var height);

            var image = Util.CreateImage(imageRawBytes, width, height, flip: true);

            renderer.ShutDown();
            window.Close();

            return image;
        }

        internal static void CheckExpectedImage(byte[] imageRawBytes, int width, int height, string expectedImageFilePath)
        {
            var image = Util.CreateImage(imageRawBytes, width, height, flip: true);
#if DEBUG
            var expectedImage = Util.LoadImage(expectedImageFilePath);
            Assert.True(Util.CompareImage(expectedImage, image));
#else
            Util.SaveImage(image, Util.UnitTestRootDir + expectedImageFilePath);//generate expected image
#endif
        }

        internal static void DrawNodeToImage(out byte[] imageRawBytes, Node node, int width, int height)
        {
            MeshBuffer meshBuffer = new MeshBuffer();
            MeshList meshList = new MeshList();
            IPrimitiveRenderer primitiveRenderer = new BuiltinPrimitiveRenderer();

            using (var context = new RenderContextForTest(width, height))
            {
                //This must be called after the context is created, for creating textures when the OpenGL context is built.
                node.Draw(primitiveRenderer, meshList);

                //rebuild mesh buffer
                meshBuffer.Clear();
                meshBuffer.Init();
                meshBuffer.Build(meshList);

                //draw mesh buffer to screen
                context.Clear();
                context.DrawMeshes(meshBuffer);

                imageRawBytes = context.GetRenderedRawBytes();
            }
        }

        internal static void DrawNodesToImage(out byte[] imageRawBytes, IList<Node> nodes, int width, int height)
        {
            MeshBuffer meshBuffer = new MeshBuffer();
            MeshList meshList = new MeshList();
            IPrimitiveRenderer primitiveRenderer = new BuiltinPrimitiveRenderer();

            using (var context = new RenderContextForTest(width, height))
            {
                //This must be called after the context is created, for creating textures when the OpenGL context is built.
                foreach (var node in nodes)
                {
                    node.Draw(primitiveRenderer, meshList);
                }

                //rebuild mesh buffer
                meshBuffer.Clear();
                meshBuffer.Init();
                meshBuffer.Build(meshList);

                //draw mesh buffer to screen
                context.Clear();
                context.DrawMeshes(meshBuffer);

                imageRawBytes = context.GetRenderedRawBytes();
            }
        }

        internal static void CheckExpectedImage(PathPrimitive primitive, int width, int height, string expectedImageFilePath)
        {
            byte[] imageRawBytes;
            using (var context = new RenderContextForTest(width, height))
            {
                BuiltinPrimitiveRenderer primitiveRenderer = new BuiltinPrimitiveRenderer();
                var mesh = new Mesh();
                primitiveRenderer.DrawPathPrimitive(mesh, primitive, Vector.Zero);

                context.Clear();
                context.DrawShapeMesh(mesh);

                imageRawBytes = context.GetRenderedRawBytes();
            }
            var image = Util.CreateImage(imageRawBytes, width, height, flip: true);
#if DEBUG
            var expectedImage = Util.LoadImage(expectedImageFilePath);
            Assert.True(Util.CompareImage(expectedImage, image));
#else
            Util.SaveImage(image, Util.UnitTestRootDir + expectedImageFilePath); //generate expected image
#endif
        }

        internal static void CheckExpectedImage(ImagePrimitive primitive, int width, int height, StyleRuleSet style, string expectedImageFilePath)
        {
            byte[] imageRawBytes;
            using (var context = new RenderContextForTest(width, height))
            {
                BuiltinPrimitiveRenderer primitiveRenderer = new BuiltinPrimitiveRenderer();
                var mesh = new Mesh();
                primitiveRenderer.DrawImagePrimitive(mesh, primitive,
                    new Rect(10, 10, primitive.Image.Width, primitive.Image.Height), style, Vector.Zero);

                context.Clear();
                context.DrawImageMesh(mesh);

                imageRawBytes = context.GetRenderedRawBytes();
            }
            var image = Util.CreateImage(imageRawBytes, width, height, flip: true);
#if DEBUG
            var expectedImage = Util.LoadImage(expectedImageFilePath);
            Assert.True(Util.CompareImage(expectedImage, image));
#else
            Util.SaveImage(image, Util.UnitTestRootDir + expectedImageFilePath); //generate expected image
#endif
        }

        internal static void CheckExpectedImage(TextPrimitive primitive, int width, int height, Rect contentRect, string expectedImageFilePath)
        {
            byte[] imageRawBytes;
            using (var context = new RenderContextForTest(width, height))
            {
                BuiltinPrimitiveRenderer primitiveRenderer = new BuiltinPrimitiveRenderer();
                var textMesh = new TextMesh();
                primitiveRenderer.DrawTextPrimitive(textMesh, primitive, contentRect, new StyleRuleSet(), Vector.Zero);

                context.Clear();
                context.DrawTextMesh(textMesh);

                imageRawBytes = context.GetRenderedRawBytes();
            }
            var image = Util.CreateImage(imageRawBytes, width, height, flip: true);
#if DEBUG
            var expectedImage = Util.LoadImage(expectedImageFilePath);
            Assert.True(Util.CompareImage(expectedImage, image));
#else
            Util.SaveImage(image, Util.UnitTestRootDir + expectedImageFilePath); //generate expected image
#endif
        }

        internal static void CheckExpectedImageSliced(ImagePrimitive primitive, int width, int height, Rect rect, StyleRuleSet style, string expectedImageFilePath)
        {
            byte[] imageRawBytes;
            using (var context = new RenderContextForTest(width, height))
            {
                BuiltinPrimitiveRenderer primitiveRenderer = new BuiltinPrimitiveRenderer();
                var mesh = new Mesh();
                primitiveRenderer.SetImageMesh(mesh);
                primitiveRenderer.DrawSlicedImage(primitive, rect, style);

                context.Clear();
                context.DrawImageMesh(mesh);

                imageRawBytes = context.GetRenderedRawBytes();
            }
            var image = Util.CreateImage(imageRawBytes, width, height, flip: true);
#if DEBUG
            var expectedImage = Util.LoadImage(expectedImageFilePath);
            Assert.True(Util.CompareImage(expectedImage, image));
#else
            Util.SaveImage(image, Util.UnitTestRootDir + expectedImageFilePath); //generate expected image
#endif
        }

    }
}
