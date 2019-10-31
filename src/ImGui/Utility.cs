using System;
using System.Diagnostics;
using System.IO;
using CSharpGL;
using System.Reflection;
using ImGui.OSImplentation.Web;

namespace ImGui
{
    internal static class Utility
    {
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

        /// <summary>
        /// (Not using, performance is bad.)
        /// </summary>
        [Conditional("None")]
        public static void CheckGLError()
        {
            var error = GL.GetError();
            string errorStr = null;
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
                throw new Exception(String.Format("glError: 0x{0:X} ({1})", error, errorStr));
            }
        }

        /// <summary>
        /// (Not using, performance is bad.)
        /// </summary>
        [Conditional("None")]
        public static void CheckGLESError()
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
                throw new Exception(String.Format("glError: 0x{0:X} ({1})", error, errorStr));
            }
        }

        [Conditional("None")]
        public static void CheckWebGLError()
        {
            var error = WebGL.GetError();
            string errorStr = "GL_NO_ERROR";
            switch (error)
            {
                case WebGL.GL_NO_ERROR:
                    errorStr = "GL_NO_ERROR";
                    break;
                case WebGL.GL_INVALID_ENUM:
                    errorStr = "GL_INVALID_ENUM";
                    break;
                case WebGL.GL_INVALID_VALUE:
                    errorStr = "GL_INVALID_VALUE";
                    break;
                case WebGL.GL_INVALID_OPERATION:
                    errorStr = "GL_INVALID_OPERATION";
                    break;
                case WebGL.GL_STACK_OVERFLOW:
                    errorStr = "GL_STACK_OVERFLOW";
                    break;
                case WebGL.GL_STACK_UNDERFLOW:
                    errorStr = "GL_STACK_UNDERFLOW";
                    break;
                case WebGL.GL_OUT_OF_MEMORY:
                    errorStr = "GL_OUT_OF_MEMORY";
                    break;
                case WebGL.GL_INVALID_FRAMEBUFFER_OPERATION:
                    errorStr = "GL_INVALID_FRAMEBUFFER_OPERATION";
                    break;
                case WebGL.GL_CONTEXT_LOST:
                    errorStr = "GL_CONTEXT_LOST";
                    break;
            }

            if (error != GL.GL_NO_ERROR)
            {
                throw new Exception(String.Format("glError: 0x{0:X} ({1})", error, errorStr));
            }
        }

        public static Stream ReadFile(string filePath)
        {
            Stream stream = null;
            if (CurrentOS.IsAndroid)
            {
                var s = Application.OpenAndroidAssets(filePath);//TODO unify this
                using (var ms = new MemoryStream())
                {
                    s.CopyTo(ms);
                    stream = new MemoryStream(ms.ToArray());
                }
            }
            else
            {
                var s = new FileStream(filePath, FileMode.Open);
                stream = s;
            }
            return stream;
        }

        //HACK remove this if font-family in Typography is ready
        public static string FontDir = GetFontDir();

        static string GetFontDir()
        {
            return Path.GetDirectoryName(typeof(Application).GetTypeInfo().Assembly.Location) + Path.DirectorySeparatorChar + "assets/fonts" + Path.DirectorySeparatorChar;
        }

        /// <summary>
        /// Remove extra ID start with "##" from the text.
        /// </summary>
        public static string FindRenderedText(string text)
        {
            var indexHash = text.IndexOf("##", StringComparison.Ordinal);
            if (indexHash > 0)
            {
                text = text.Substring(0, indexHash);
            }

            return text;
        }

        public static Rect GetContentBox(Rect rect, StyleRuleSet style)
        {
            //Widths of border
            var bt = style.Get<double>(StylePropertyName.BorderTop);
            var br = style.Get<double>(StylePropertyName.BorderRight);
            var bb = style.Get<double>(StylePropertyName.BorderBottom);
            var bl = style.Get<double>(StylePropertyName.BorderLeft);

            //Widths of padding
            var pt = style.Get<double>(StylePropertyName.PaddingTop);
            var pr = style.Get<double>(StylePropertyName.PaddingRight);
            var pb = style.Get<double>(StylePropertyName.PaddingBottom);
            var pl = style.Get<double>(StylePropertyName.PaddingLeft);

            //4 corner of the border-box
            var btl = new Point(rect.Left, rect.Top);
            var btr = new Point(rect.Right, rect.Top);
            var bbr = new Point(rect.Right, rect.Bottom);
            var bbl = new Point(rect.Left, rect.Bottom);

            //4 corner of the padding-box
            var ptl = new Point(btl.X + bl, btl.Y + bt);
            var ptr = new Point(btr.X - br, btr.Y + bt);
            var pbr = new Point(bbr.X - br, bbr.Y - bb);
            var pbl = new Point(bbl.X + bl, bbl.Y - bb);
            Debug.Assert(ptl.X < ptr.X);//TODO what if (ptl.X > ptr.X) happens?

            //4 corner of the content-box
            var ctl = new Point(ptl.X + pl, ptl.Y + pt);
            var ctr = new Point(ptr.X - pr, ptr.Y + pr);
            var cbr = new Point(pbr.X - pr, pbr.Y - pb);
            var cbl = new Point(pbl.X + pl, pbl.Y - pb);
            if (ctl.X >= ctr.X)
            {
                Log.Warning("Content box is zero-sized.");
                return new Rect(ctl, Size.Zero);
            }

            return new Rect(ctl, cbr);
        }
    }
}
