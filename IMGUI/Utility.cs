using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using CSharpGL;

namespace ImGui
{
    static class Utility
    {
        /// <summary>
        /// Get rect of the context box
        /// </summary>
        /// <param name="rect">rect of the entire box</param>
        /// <param name="style">style</param>
        /// <returns>rect of the context box</returns>
        public static Rect GetContentRect(Rect rect, Style style)
        {
            //Widths of border
            var bt = style.BorderTop;
            var br = style.BorderRight;
            var bb = style.BorderBottom;
            var bl = style.BorderLeft;

            //Widths of padding
            var pt = style.PaddingTop;
            var pr = style.PaddingRight;
            var pb = style.PaddingBottom;
            var pl = style.PaddingLeft;

            //4 corner of the border-box
            var btl = new Point(rect.Left, rect.Top);
            var bbr = new Point(rect.Right, rect.Bottom);

            //4 corner of the padding-box
            var ptl = new Point(btl.X + bl, btl.Y + bt);
            var pbr = new Point(bbr.X - br, bbr.Y - bb);

            //4 corner of the content-box
            var ctl = new Point(ptl.X + pl, ptl.Y + pt);
            var cbr = new Point(pbr.X - pr, pbr.Y - pb);
            var contentBoxRect = new Rect(ctl, cbr);
            return contentBoxRect;
        }

        // Detects the current OS (Windows, Linux, MacOS)
        internal static class CurrentOS
        {
            static CurrentOS()
            {
                var envars = System.Environment.GetEnvironmentVariables();
                IsAndroid = envars.Contains("ANDROID_PROPERTY_WORKSPACE");
                if (IsAndroid) return;

                IsWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                if (IsWindows) return;
                IsMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
                if (IsMac) return;
                IsLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
                if (IsLinux) return;

                IsUnknown = true;
            }

            public static bool IsWindows { get; private set; }
            public static bool IsMac { get; private set; }
            public static bool IsLinux { get; private set; }

            public static bool IsAndroid { get; private set; }

            public static bool IsUnknown { get; private set; }

            public static bool Is64BitProcess
            {
                get { return (IntPtr.Size == 8); }
            }

            public static bool Is32BitProcess
            {
                get { return (IntPtr.Size == 4); }
            }
        }
        
        public static void GetId(string t, out string text, out string id)
        {
            if (t.Contains("###"))
            {
                var tmp0 = t.IndexOf("###");
                text = t.Substring(0, tmp0);
                id = t;
            }
            else if (t.Contains("##"))
            {
                var tmp0 = t.IndexOf("##");
                text = t.Substring(0, tmp0);
                id = t.Substring(tmp0 + 1);
            }
            else
            {
                text = id = t;
            }
        }

        [Conditional("DEBUG")]
        public static void SaveToObjFile(string path, IList<DrawVertex> vertexes, IList<DrawIndex> indexes)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("# MTE");
            sb.AppendLine();

            for (int i = 0; i < vertexes.Count; i++)
            {
                var position = vertexes[i].pos;
                sb.AppendFormat("v {0} {1} {2}", -position.x, position.y, 0);
                sb.AppendLine();
            }

            for (int i = 0; i < vertexes.Count; i++)
            {
                var uv = vertexes[i].uv;
                sb.AppendFormat("vt {0} {1}", uv.x, uv.y);
                sb.AppendLine();
            }

            for (int i = 0; i < indexes.Count; i += 3)
            {
                sb.AppendFormat("f {0}/{0} {1}/{1} {2}/{2}",
                    indexes[i] + 1,
                    indexes[i + 2] + 1,
                    indexes[i + 1] + 1);
                sb.AppendLine();
            }
            File.WriteAllText(path, sb.ToString());
        }

        [Conditional("DEBUG")]
        public static void SaveToObjFile(IList<DrawVertex> vertexes, IList<DrawIndex> indexes)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("# MTE");
            sb.AppendLine();

            for (int i = 0; i < vertexes.Count; i++)
            {
                var position = vertexes[i].pos;
                sb.AppendFormat("v {0} {1} {2}", -position.x, position.y, 0);
                sb.AppendLine();
            }

            for (int i = 0; i < vertexes.Count; i++)
            {
                var uv = vertexes[i].uv;
                sb.AppendFormat("vt {0} {1}", uv.x, uv.y);
                sb.AppendLine();
            }

            for (int i = 0; i < indexes.Count; i += 3)
            {
                sb.AppendFormat("f {0}/{0} {1}/{1} {2}/{2}",
                    indexes[i] + 1,
                    indexes[i + 2] + 1,
                    indexes[i + 1] + 1);
                sb.AppendLine();
            }
            File.WriteAllText("D:\\imgui_test.obj", sb.ToString());
        }

        public static void SaveToObjFile2(string filePath, float[] positions, int[] indexes)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("# Triangle");
            sb.AppendLine();

            for (int i = 0; i < positions.Length; i += 2)
            {
                sb.AppendFormat("v {0} {1} 0", positions[i], positions[i + 1]);
                sb.AppendLine();
            }

            for (int i = 0; i < positions.Length; i++)
            {
                sb.AppendFormat("vt {0} {1}", 0, 0);
                sb.AppendLine();
            }

            for (int i = 0; i < indexes.Length; i += 3)
            {
                sb.AppendFormat("f {0}/{0} {1}/{1} {2}/{2}",
                    indexes[i] + 1,
                    indexes[i + 1] + 1,
                    indexes[i + 2] + 1);
                sb.AppendLine();
            }

            File.WriteAllText(filePath, sb.ToString());
        }

#if false //to be moved to unit test
        public static void SavePathToPng(IList<Point> path)
        {
            if (path == null || path.Count <= 1) return;
            using (Cairo.ImageSurface surface = new Cairo.ImageSurface(Cairo.Format.Argb32, (int)Form.current.Size.Width, (int)Form.current.Size.Height))
            using (Cairo.Context g = new Cairo.Context(surface))
            {
                g.MoveTo(path[0].X, path[0].Y);
                for (int i = 1; i < path.Count; i++)
                {
                    var x0 = path[i - 1].X;
                    var y0 = path[i - 1].Y;
                    var x1 = path[i].X;
                    var y1 = path[i].Y;

                    g.LineTo(x1, y1);

                    var dx = x1 - x0;
                    var dy = y1 - y0;

                    if (MathEx.AmostZero(dx) && MathEx.AmostZero(dy)) continue;

                    var n0 = new Vector(-dy, dx); n0.Normalize();
                    var n1 = new Vector(dy, -dx); n1.Normalize();

                    var B = new Point(x1, y1);
                    var d = new Vector(x0 - x1, y0 - y1); d.Normalize();

                    var arrowEnd0 = B + 5 * (d + n0);
                    var arrowEnd1 = B + 5 * (d + n1);
                    g.MoveTo(x1, y1);
                    g.LineTo(arrowEnd0.ToPointD());
                    g.MoveTo(x1, y1);
                    g.LineTo(arrowEnd1.ToPointD());
                    g.MoveTo(x1, y1);
                }
                g.Stroke();
                surface.WriteToPng(@"D:\path_test.png");
            }
        }
        
        public static void SavePathToPng(List<LibTessDotNet.ContourVertex> path)
        {
            if (path == null || path.Count <= 1) return;
            using (Cairo.ImageSurface surface = new Cairo.ImageSurface(Cairo.Format.Argb32, (int)Form.current.Size.Width, (int)Form.current.Size.Height))
            using (Cairo.Context g = new Cairo.Context(surface))
            {
                g.MoveTo(path[0].Position.X , path[0].Position.Y);
                for (int i = 1; i < path.Count; i++)
                {
                    var x0 = path[i-1].Position.X;
                    var y0 = path[i-1].Position.Y;
                    var x1 = path[i].Position.X;
                    var y1 = path[i].Position.Y;

                    g.LineTo(x1, y1);
                    
                    {
                        // draw index number
                        g.RelMoveTo(5, 5);
                        g.ShowText(i.ToString());
                        g.MoveTo(x1, y1);

                        // draw arrow
                        var dx = x1 - x0;
                        var dy = y1 - y0;

                        if (MathEx.AmostZero(dx) && MathEx.AmostZero(dy)) continue;

                        var n0 = new Vector(-dy, dx); n0.Normalize();
                        var n1 = new Vector(dy, -dx); n1.Normalize();

                        var B = new Point(x1, y1);
                        var d = new Vector(x0 - x1, y0 - y1); d.Normalize();

                        var arrowEnd0 = B + 5 * (d + n0);
                        var arrowEnd1 = B + 5 * (d + n1);
                        g.MoveTo(x1, y1);
                        g.LineTo(arrowEnd0.ToPointD());
                        g.MoveTo(x1, y1);
                        g.LineTo(arrowEnd1.ToPointD());
                        g.MoveTo(x1, y1);
                    }
                }
                g.Stroke();
                surface.WriteToPng(@"D:\contour_test.png");
            }
        }
#endif
        /// <summary>
        /// convert pt to dip
        /// </summary>
        /// <remarks>dpi defaults to 96</remarks>
        internal static float PointToDip(int pt)
        {
            var dip = pt * 96.0f / 72.0f;
            return dip;
        }

        /// <summary>
        /// convert dip to pt
        /// </summary>
        /// <remarks>dpi defaults to 96</remarks>
        internal static float DipToPoint(int dip)
        {
            var pt = dip * 72.0f / 96.0f;
            return pt;
        }

        public static void CheckGLError(
            [CallerFilePath] string fileName = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string memberName = null)
        {
            var error = GL.GetError();
            string errorStr = "GL_NO_ERROR";
            switch (error)
            {
                case GL.GL_NO_ERROR:
                    errorStr = "GL_NO_ERROR";
                    break;
                case GL.GL_INVALID_ENUM:
                    errorStr = "GL_INVALID_ENUM";
                    break;
                case GL.GL_INVALID_VALUE:
                    errorStr = "GL_INVALID_VALUE";
                    break;
                case GL.GL_INVALID_OPERATION:
                    errorStr = "GL_INVALID_OPERATION";
                    break;
                case GL.GL_STACK_OVERFLOW:
                    errorStr = "GL_STACK_OVERFLOW";
                    break;
                case GL.GL_STACK_UNDERFLOW:
                    errorStr = "GL_STACK_UNDERFLOW";
                    break;
                case GL.GL_OUT_OF_MEMORY:
                    errorStr = "GL_OUT_OF_MEMORY";
                    break;
                case GL.GL_INVALID_FRAMEBUFFER_OPERATION:
                    errorStr = "GL_INVALID_FRAMEBUFFER_OPERATION";
                    break;
                case GL.GL_CONTEXT_LOST:
                    errorStr = "GL_CONTEXT_LOST";
                    break;
            }

            if (error != GL.GL_NO_ERROR)
            {
                Debug.WriteLine("{0}({1}): glError: 0x{2:X} ({3}) in {4}",
                    fileName, lineNumber, error, errorStr, memberName);
            }
        }

        public static void CheckGLESError(
            [CallerFilePath] string fileName = null,
            [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string memberName = null)
        {
            var error = CSharpGLES.GL.GetError();
            string errorStr = "GL_NO_ERROR";
            switch (error)
            {
                case GL.GL_NO_ERROR:
                    errorStr = "GL_NO_ERROR";
                    break;
                case GL.GL_INVALID_ENUM:
                    errorStr = "GL_INVALID_ENUM";
                    break;
                case GL.GL_INVALID_VALUE:
                    errorStr = "GL_INVALID_VALUE";
                    break;
                case GL.GL_INVALID_OPERATION:
                    errorStr = "GL_INVALID_OPERATION";
                    break;
                case GL.GL_STACK_OVERFLOW:
                    errorStr = "GL_STACK_OVERFLOW";
                    break;
                case GL.GL_STACK_UNDERFLOW:
                    errorStr = "GL_STACK_UNDERFLOW";
                    break;
                case GL.GL_OUT_OF_MEMORY:
                    errorStr = "GL_OUT_OF_MEMORY";
                    break;
                case GL.GL_INVALID_FRAMEBUFFER_OPERATION:
                    errorStr = "GL_INVALID_FRAMEBUFFER_OPERATION";
                    break;
                case GL.GL_CONTEXT_LOST:
                    errorStr = "GL_CONTEXT_LOST";
                    break;
            }

            if (error != GL.GL_NO_ERROR)
            {
                Debug.WriteLine("{0}({1}): glError: 0x{2:X} ({3}) in {4}",
                    fileName, lineNumber, error, errorStr, memberName);
            }
        }

    }
}
