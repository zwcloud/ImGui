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
using ImGui.Rendering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Xunit;
using Image = SixLabors.ImageSharp.Image;
using Path = System.IO.Path;

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

        private static void Draw(Context context, Visual visual)
        {
            var node = (Node)visual;
            var isGroup =  node.IsGroup;

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
            node.Foreach(v =>
            {
                Draw(context, v);
                return true;
            });
            context.Restore();
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
                //This must be called after the context is created, for uploading textures to GPU via OpenGL.
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
                //This must be called after the context is created, for uploading textures to GPU via OpenGL.
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

        internal static void DrawNodeTreeToImage(out byte[] imageRawBytes, Node root, int width, int height, Rect clipRect)
        {
            MeshBuffer meshBuffer = new MeshBuffer();
            MeshList meshList = new MeshList();
            var primitiveRenderer = new BuiltinPrimitiveRenderer();

            using (var context = new RenderContextForTest(width, height))
            {
                //This must be called after the context is created, for uploading textures to GPU via OpenGL.
                root.Foreach(n =>primitiveRenderer.Draw(n, clipRect, meshList));

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

        internal static void DrawNodeTreeToImage(out byte[] imageRawBytes, Node root, int width, int height)
            => DrawNodeTreeToImage(out imageRawBytes, root, width, height, Rect.Big);
    }
}
