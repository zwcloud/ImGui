using System;
using System.IO;
using CSharpGL;
using System.Reflection;
using System.Runtime.CompilerServices;
using ImGui.assets;

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

        public static Stream ReadFile(string filePath)
        {
            Stream stream = null;
            if (CurrentOS.IsAndroid)
            {
                var androidAssetFilePath = filePath.Replace('\\', '/');
                var s = Application.OpenAndroidAssets(androidAssetFilePath);//TODO unify this
                using var ms = new MemoryStream();
                s.CopyTo(ms);
                stream = new MemoryStream(ms.ToArray());
            }
            else if (CurrentOS.IsLinux)
            {
                filePath = filePath.Replace('\\', '/');
                var s = new FileStream(filePath, FileMode.Open);
                stream = s;
            }
            else if (CurrentOS.IsBrowser)
            {
                //TODO use emscripten approaches

                filePath = filePath.Replace('\\', '/');
                if (filePath[0] != '/')
                {
                    filePath = filePath.Insert(0, "/");
                }
                var bytes = ImGuiRes.ResourceManager.GetObject(filePath) as byte[];
                stream = new MemoryStream(bytes ??
                    throw new InvalidOperationException($"Cannot find file {filePath} from resource."));
            }
            else//windows
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
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool HasAllFlags<T>(T value, T flags) where T : unmanaged, Enum
        {
            if (sizeof(T) == 1)
                return (Unsafe.As<T, byte>(ref value) | Unsafe.As<T, byte>(ref flags)) == Unsafe.As<T, byte>(ref value);
            if (sizeof(T) == 2)
                return (Unsafe.As<T, short>(ref value) | Unsafe.As<T, short>(ref flags)) == Unsafe.As<T, short>(ref value);
            if (sizeof(T) == 4)
                return (Unsafe.As<T, int>(ref value) | Unsafe.As<T, int>(ref flags)) == Unsafe.As<T, int>(ref value);
            return (Unsafe.As<T, long>(ref value) | Unsafe.As<T, long>(ref flags)) == Unsafe.As<T, long>(ref value);
        }
    }
}
