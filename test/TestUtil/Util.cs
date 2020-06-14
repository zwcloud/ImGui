#define ShowImage
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using CSharpGL;
using ImageSharp.Extension;
using ImGui.GraphicsImplementation;
using ImGui.Input;
using ImGui.OSAbstraction.Graphics;
using ImGui.OSAbstraction.Text;
using ImGui.OSImplementation.Shared;
using ImGui.OSImplementation.Windows;
using ImGui.Rendering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImGui.UnitTest
{
    public static class Util
    {
        public static readonly string OutputPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/ImGui.UnitTest.Output";

        public static readonly string UnitTestRootDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..")) + Path.DirectorySeparatorChar;

        static Util()
        {
            Directory.CreateDirectory(OutputPath);
        }

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
            path = Path.GetFullPath(path);
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
            const string ModelViewerPath = @"Z:\Program Files\Side Effects Software\Houdini 18.0.460\bin\gplay.exe";
            if(!File.Exists(ModelViewerPath))
            {
                throw new FileNotFoundException("ModelViewer(Houdini gplay) not found.");
            }
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }
            Process.Start(ModelViewerPath, path);
        }

        #region Image

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

        public static byte[] EncodeAsPng(byte[] openGLRawPixelBytes, int width, int height)
        {
            var image = CreateImage(openGLRawPixelBytes, width, height, true);
            using MemoryStream stream = new MemoryStream();
            image.SaveAsPng(stream);
            return stream.ToArray();
        }

        public static bool CompareImage(Image<Rgba32> a, Image<Rgba32> b)
        {
            var diffPercentage = ImageComparer.PercentageDifference(a,b, 20);
            return diffPercentage < 0.1;
        }

        public static bool CompareImage(Image<Rgba32> a, Image<Bgra32> b)
        {
            var diffPercentage = ImageComparer.PercentageDifference(a,b, 20);
            return diffPercentage < 0.1;
        }

        public static Image<Rgba32> LoadImage(string filePath)
        {
            return Image.Load<Rgba32>(filePath);
        }

        public static void CheckExpectedImage(byte[] imageRawBytes, int width, int height, string expectedImageFilePath)
        {
            var image = Util.CreateImage(imageRawBytes, width, height, flip: false);
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

            Debug.Assert(Util.CompareImage(expectedImage, image));
#else//generate expected image
            var path = Util.UnitTestRootDir + expectedImageFilePath;
            Util.SaveImage(image, path);
            Util.SelectFileInExplorer(path);
            Util.OpenImage(path);
#endif
        }

        public static void ShowImage(byte[] imageRawBytes, int width, int height, string path)
        {
            using var image = CreateImage(imageRawBytes, width, height, flip: false);
            SaveImage(image, (string)path);
            SelectFileInExplorer((string)path);
            OpenImage((string)path);
        }

        internal static void ShowImageNotOpenFolder(byte[] imageRawBytes, int width, int height, string path)
        {
            using var image = CreateImage(imageRawBytes, width, height, flip: false);
            Util.SaveImage(image, path);
            Util.OpenImage(path);
        }
        
        internal static void ShowRawPixelsFrom_glReadPixels_NotOpenFolder(byte[] imageRawBytes, int width, int height, string path)
        {
            using var image = CreateImage(imageRawBytes, width, height, flip: true);
            Util.SaveImage(image, path);
            Util.OpenImage(path);
        }

        #endregion

        #region Node

        #region Cairo

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
            var isGroup = node.IsGroup;

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
        #endregion

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
        #endregion

        #region Visual
        internal static void DrawDrawingVisualToImage(out byte[] imageRawBytes,
            int width, int height, DrawingVisual drawingVisual)
        {
            //convert geometries inside the drawingVisual's content to meshes stored in a MeshList with a BuiltinGeometryRenderer
            MeshList meshList = new MeshList();
            using BuiltinGeometryRenderer geometryRenderer = new BuiltinGeometryRenderer();
            RenderContext renderContext = new RenderContext(geometryRenderer, meshList);
            drawingVisual.RenderContent(renderContext);

            //merge meshes in the MeshList to a MeshBuffer
            MeshBuffer meshBuffer = new MeshBuffer();
            meshBuffer.Clear();
            meshBuffer.Init();
            meshBuffer.Build(meshList);

            //created a mesh IRenderer
            Application.Init();
            var window = Application.PlatformContext.CreateWindow(Point.Zero, new Size(100, 100),
                WindowTypes.Hidden);
            var renderer = Application.PlatformContext.CreateRenderer() as Win32OpenGLRenderer;//TEMP HACK
            Debug.Assert(renderer != null, nameof(renderer) + " != null");
            renderer.Init(window.Pointer, window.ClientSize);

            //clear the canvas and draw mesh in the MeshBuffer with the mesh renderer
            renderer.Clear(Color.White);
            renderer.StartDrawMeshToImage(width, height);
            imageRawBytes = renderer.DrawMeshToImage(width, height, meshBuffer.ShapeMesh, OpenGLMaterial.shapeMaterial);
            renderer.EndDrawMeshToImage();

            //clear native resources: window and IRenderer
            renderer.ShutDown();
            window.Close();
        }
        #endregion

        #region Mesh
        internal static void DrawMeshToImage(out byte[] imageRawBytes, int width, int height, Mesh mesh)
        {
            //created a mesh IRenderer
            Application.Init();
            var window = Application.PlatformContext.CreateWindow(Point.Zero, new Size(width, height),
                WindowTypes.Hidden);
            var renderer = Application.PlatformContext.CreateRenderer() as Win32OpenGLRenderer;//TEMP HACK
            Debug.Assert(renderer != null, nameof(renderer) + " != null");
            renderer.Init(window.Pointer, window.ClientSize);

            //clear the canvas and draw mesh in the MeshBuffer with the mesh renderer
            renderer.Clear(Color.White);
            renderer.StartDrawMeshToImage(width, height);
            imageRawBytes = renderer.DrawMeshToImage(width, height, mesh, OpenGLMaterial.shapeMaterial);
            renderer.EndDrawMeshToImage();

            //clear native resources: window and IRenderer
            renderer.ShutDown();
            window.Close();
        }
        
        internal static void DrawTextMeshToImage(out byte[] imageRawBytes,
            int width, int height, TextMesh textMesh)
        {
            Application.Init();

            var window = Application.PlatformContext.CreateWindow(Point.Zero, new Size(200, 200),
                WindowTypes.Hidden);
            var renderer = Application.PlatformContext.CreateRenderer() as Win32OpenGLRenderer;//TEMP HACK
            Debug.Assert(renderer != null, nameof(renderer) + " != null");
            renderer.Init(window.Pointer, window.ClientSize);

            renderer.StartDrawTextMeshToImage(width, height);
            Keyboard.Instance.OnFrameBegin();
            imageRawBytes = renderer.DrawTextMeshToImage(textMesh, width, height);
            Keyboard.Instance.OnFrameEnd();
            renderer.EndDrawTextMeshToImage();

            renderer.ShutDown();
            window.Close();
        }
        
        internal static void DrawTextMeshToImage_Realtime(int width, int height, TextMesh textMesh)
        {
            Application.Init();

            var window = Application.PlatformContext.CreateWindow(Point.Zero, new Size(200, 200),
                WindowTypes.Regular);
            var renderer = Application.PlatformContext.CreateRenderer() as Win32OpenGLRenderer;//TEMP HACK
            Debug.Assert(renderer != null, nameof(renderer) + " != null");
            renderer.Init(window.Pointer, window.ClientSize);

            renderer.StartDrawTextMeshToImage(width, height);
            while (true)
            {
                if (Keyboard.Instance.KeyDown(Key.Escape))
                {
                    break;
                }

                window.MainLoop(() =>
                {
                    Keyboard.Instance.OnFrameBegin();
                    renderer.Clear(Color.White);
                    renderer.DrawTextMeshToImage(textMesh, width, height);
                    Keyboard.Instance.OnFrameEnd();
                    renderer.SwapBuffers();
                });
            }
            renderer.EndDrawTextMeshToImage();

            renderer.ShutDown();
            window.Close();

            //clear native resources: window and IRenderer
            renderer.ShutDown();
            window.Close();
        }

        internal static void DrawTextMeshInWindow(int width, int height, TextMesh mesh)
        {
            Application.Init();

            var window = Application.PlatformContext.CreateWindow(Point.Zero, new Size(width, height),
                WindowTypes.Hidden);
            var renderer = Application.PlatformContext.CreateRenderer() as Win32OpenGLRenderer;//TEMP HACK
            Debug.Assert(renderer != null, nameof(renderer) + " != null");
            renderer.Init(window.Pointer, window.ClientSize);
            window.Show();
            while (true)
            {
                if (Keyboard.Instance.KeyDown(Key.Escape))
                {
                    break;
                }

                window.MainLoop(() =>
                {
                    Keyboard.Instance.OnFrameBegin();
                    renderer.Clear(Color.White);
                    Win32OpenGLRenderer.DrawTextMesh(mesh, width, height);
                    Keyboard.Instance.OnFrameEnd();
                    renderer.SwapBuffers();
                });
            }

            renderer.ShutDown();
            window.Close();
        }

        internal static byte[] DrawAsOpenGLPixelBytes(Mesh mesh, TextMesh textMesh,
            int width, int height)
        {
            //created a mesh IRenderer
            Application.Init();
            var window = Application.PlatformContext.CreateWindow(Point.Zero, new Size(10, 200),
                WindowTypes.Hidden);
            window.ClientSize = new Size(width, height);
            var renderer = Application.PlatformContext.CreateRenderer() as Win32OpenGLRenderer;//TEMP HACK
            Debug.Assert(renderer != null, nameof(renderer) + " != null");
            renderer.Init(window.Pointer, window.ClientSize);
            var textureRenderer = new OpenGLOffscreenRenderer();
            var openGLBytes = textureRenderer.DrawMeshToTexture(mesh, textMesh, width, height);

            //clear native resources: window and IRenderer
            renderer.ShutDown();
            window.Close();

            return openGLBytes;
        }

        #endregion

        #region Media
        public static byte[] DrawAsOpenGLPixelBytes(FormattedText formattedText,
            Brush brush, int width, int height, Rect clipRect)
        {
            BuiltinGeometryRenderer renderer = new BuiltinGeometryRenderer();
            renderer.PushClipRect(clipRect);
            renderer.OnBeforeRead();
            renderer.DrawEllipse(null, new Pen(Color.DarkSeaGreen, 1),
                formattedText.OriginPoint, 2, 2);
            renderer.DrawLine(new Pen(Color.Gray, 1), formattedText.OriginPoint,
                formattedText.OriginPoint + new Vector(width, 0));
            renderer.DrawText(brush, formattedText);
            var mesh = renderer.ShapeMesh;
            var textMesh = renderer.TextMesh;
            var bytes = Util.DrawAsOpenGLPixelBytes(mesh, textMesh, width, height);
            return bytes;
        }
        #endregion
    }
}
