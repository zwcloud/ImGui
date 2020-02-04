#define ShowImage
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using ImageSharp.Extension;
using ImGui.GraphicsImplementation;
using ImGui.Rendering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Xunit;

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

        public static void SelectFileInExplorer(string path)
        {
            if(!File.Exists(path))
            {
                throw new FileNotFoundException();
            }
            string argument = "/select, \"" + path +"\"";
            Process.Start("explorer", argument);
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
            using (Cairo.ImageSurface surface = new Cairo.ImageSurface(Cairo.Format.Argb32, (int)node.Rect.Width, (int)node.Rect.Height))
            using (Cairo.Context context = new Cairo.Context(surface))
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

        private static void Draw(Cairo.Context context, Visual visual)
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
            var dirPath = Path.GetDirectoryName(path);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            using (var stream = File.OpenWrite(path))
            {
                image.SaveAsPng(stream);
            }
        }

        internal static bool CompareImage(Image<Rgba32> a, Image<Rgba32> b)
        {
            var diffPercentage = ImageComparer.PercentageDifference(a,b, 20);
            return diffPercentage < 0.1;
        }

        internal static bool CompareImage(Image<Rgba32> a, Image<Bgra32> b)
        {
            var diffPercentage = ImageComparer.PercentageDifference(a,b, 20);
            return diffPercentage < 0.1;
        }

        public static Image<Rgba32> LoadImage(string filePath)
        {
            return Image.Load<Rgba32>(filePath);
        }

        internal static void CheckExpectedImage(byte[] imageRawBytes, int width, int height, string expectedImageFilePath)
        {
            var image = Util.CreateImage(imageRawBytes, width, height, flip: true);
#if DEBUG//check if it matches expected image
            var expectedImage = Util.LoadImage(expectedImageFilePath);

#if ShowImage
            var actualImagePath = Environment.ExpandEnvironmentVariables("%TEMP%") + Path.DirectorySeparatorChar + "actual.png";
            var expectedImagePath = Util.UnitTestRootDir + expectedImageFilePath;
            if (File.Exists(actualImagePath))
            {
                File.Delete(actualImagePath);
            }
            Util.SaveImage(image, actualImagePath);
            Util.OpenImage(actualImagePath);
            Util.OpenImage(expectedImagePath);
#endif

            Assert.True(Util.CompareImage(expectedImage, image));
#else//generate expected image
            var path = Util.UnitTestRootDir + expectedImageFilePath;
            Util.SaveImage(image, path);
            Util.SelectFileInExplorer(path);
            Util.OpenImage(path);
#endif
        }

        internal static void ShowImage(byte[] imageRawBytes, int width, int height, string expectedImageFilePath)
        {
            var image = Util.CreateImage(imageRawBytes, width, height, flip: true);
            var path = expectedImageFilePath;
            Util.SaveImage(image, path);
            Util.SelectFileInExplorer(path);
            Util.OpenImage(path);
        }

        internal static void ShowImageNotOpenFolder(byte[] imageRawBytes, int width, int height, string expectedImageFilePath)
        {
            var image = Util.CreateImage(imageRawBytes, width, height, flip: true);
            var path = expectedImageFilePath;
            Util.SaveImage(image, path);
            Util.OpenImage(path);
        }

        internal static void DrawNodeToImage(out byte[] imageRawBytes, Node node, int width, int height)
        {
            Application.EnableMSAA = false;

            MeshBuffer meshBuffer = new MeshBuffer();
            MeshList meshList = new MeshList();
            BuiltinGeometryRenderer geometryRenderer = new BuiltinGeometryRenderer();
            using (var context = new RenderContextForTest(width, height))
            {
                //This must be called after the context is created, for uploading textures to GPU via OpenGL.
                node.Render(new RenderContext(geometryRenderer, meshList));

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
            BuiltinGeometryRenderer geometryRenderer = new BuiltinGeometryRenderer();

            using (var context = new RenderContextForTest(width, height))
            {
                //This must be called after the context is created, for uploading textures to GPU via OpenGL.
                foreach (var node in nodes)
                {
                    node.Render(new RenderContext(geometryRenderer, meshList));
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
            if (root == null)
            {
                throw new ArgumentNullException(nameof(root));
            }

            MeshBuffer meshBuffer = new MeshBuffer();
            MeshList meshList = new MeshList();
            BuiltinGeometryRenderer geometryRenderer = new BuiltinGeometryRenderer();

            using (var context = new RenderContextForTest(width, height))
            {
                if (root is Node rootNode)
                {
                    using(var dc = rootNode.RenderOpen())
                    {
                        dc.DrawBoxModel(rootNode.RuleSet, rootNode.Rect);
                    }
                }

                root.Foreach(visual =>
                {
                    if (!(visual is Node node))
                    {
                        return true;
                    }

                    using(var dc = node.RenderOpen())
                    {
                        dc.DrawBoxModel(node.RuleSet, node.Rect);
                    }

                    return true;
                });

                //This must be called after the context is created, for uploading textures to GPU via OpenGL.
                root.Render(new RenderContext(geometryRenderer, meshList));

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

        #region new rendering pipeline

        /// <summary>
        /// Show a rendered image of a node. Used to inspect a node temporarily.
        /// </summary>
        /// <param name="node"></param>
        internal static void Show(Node node, string path)
        {
            var width = (int) node.Width + 50;
            var height = (int) node.Height + 50;
            Util.DrawNodeTreeToImage(out var imageRawBytes, node, width, height);
            ShowImageNotOpenFolder(imageRawBytes, width, height, path);
        }

        /// <summary>
        /// Show a rendered image of a node with specified image size. Used to inspect a node temporarily.
        /// </summary>
        /// <param name="node"></param>
        internal static void Show(Node node, Size size, string path)
        {
            var width = (int)size.Width;
            var height = (int)size.Height;
            Util.DrawNodeTreeToImage(out var imageRawBytes, node, width, height);
            ShowImageNotOpenFolder(imageRawBytes, width, height, path);
        }

        internal static void DrawNodeToImage_NewPipeline(out byte[] imageRawBytes, Node node, int width, int height)
        {
            Application.EnableMSAA = false;

            MeshBuffer meshBuffer = new MeshBuffer();
            MeshList meshList = new MeshList();
            BuiltinGeometryRenderer geometryRenderer = new BuiltinGeometryRenderer();

            using (var context = new RenderContextForTest(width, height))
            {
                RenderContext renderContext = new RenderContext(geometryRenderer, meshList);
                //This must be called after the RenderContextForTest is created, for uploading textures to GPU via OpenGL.
                node.Render(renderContext);

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

        #endregion
    }
}
