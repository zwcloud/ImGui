using CSharpGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ImGui
{
    internal partial class OpenGLRenderer
    {
        #region Native
        [DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [StructLayout(LayoutKind.Sequential)]
        public struct PIXELFORMATDESCRIPTOR
        {
            public void Init()
            {
                nSize = (ushort)Marshal.SizeOf<PIXELFORMATDESCRIPTOR>();
                nVersion = 1;
                dwFlags = PFD_FLAGS.PFD_DRAW_TO_WINDOW | PFD_FLAGS.PFD_SUPPORT_OPENGL | PFD_FLAGS.PFD_DOUBLEBUFFER | PFD_FLAGS.PFD_SUPPORT_COMPOSITION;
                iPixelType = PFD_PIXEL_TYPE.PFD_TYPE_RGBA;
                cColorBits = 24;
                cRedBits = cRedShift = cGreenBits = cGreenShift = cBlueBits = cBlueShift = 0;
                cAlphaBits = cAlphaShift = 0;
                cAccumBits = cAccumRedBits = cAccumGreenBits = cAccumBlueBits = cAccumAlphaBits = 0;
                cDepthBits = 32;
                cStencilBits = cAuxBuffers = 0;
                iLayerType = PFD_LAYER_TYPES.PFD_MAIN_PLANE;
                bReserved = 0;
                dwLayerMask = dwVisibleMask = dwDamageMask = 0;
            }
            ushort nSize;
            ushort nVersion;
            PFD_FLAGS dwFlags;
            PFD_PIXEL_TYPE iPixelType;
            byte cColorBits;
            byte cRedBits;
            byte cRedShift;
            byte cGreenBits;
            byte cGreenShift;
            byte cBlueBits;
            byte cBlueShift;
            byte cAlphaBits;
            byte cAlphaShift;
            byte cAccumBits;
            byte cAccumRedBits;
            byte cAccumGreenBits;
            byte cAccumBlueBits;
            byte cAccumAlphaBits;
            byte cDepthBits;
            byte cStencilBits;
            byte cAuxBuffers;
            PFD_LAYER_TYPES iLayerType;
            byte bReserved;
            uint dwLayerMask;
            uint dwVisibleMask;
            uint dwDamageMask;
        }

        [Flags]
        public enum PFD_FLAGS : uint
        {
            PFD_DOUBLEBUFFER = 0x00000001,
            PFD_STEREO = 0x00000002,
            PFD_DRAW_TO_WINDOW = 0x00000004,
            PFD_DRAW_TO_BITMAP = 0x00000008,
            PFD_SUPPORT_GDI = 0x00000010,
            PFD_SUPPORT_OPENGL = 0x00000020,
            PFD_GENERIC_FORMAT = 0x00000040,
            PFD_NEED_PALETTE = 0x00000080,
            PFD_NEED_SYSTEM_PALETTE = 0x00000100,
            PFD_SWAP_EXCHANGE = 0x00000200,
            PFD_SWAP_COPY = 0x00000400,
            PFD_SWAP_LAYER_BUFFERS = 0x00000800,
            PFD_GENERIC_ACCELERATED = 0x00001000,
            PFD_SUPPORT_DIRECTDRAW = 0x00002000,
            PFD_DIRECT3D_ACCELERATED = 0x00004000,
            PFD_SUPPORT_COMPOSITION = 0x00008000,
            PFD_DEPTH_DONTCARE = 0x20000000,
            PFD_DOUBLEBUFFER_DONTCARE = 0x40000000,
            PFD_STEREO_DONTCARE = 0x80000000
        }

        public enum PFD_LAYER_TYPES : byte
        {
            PFD_MAIN_PLANE = 0,
            PFD_OVERLAY_PLANE = 1,
            PFD_UNDERLAY_PLANE = 255
        }

        public enum PFD_PIXEL_TYPE : byte
        {
            PFD_TYPE_RGBA = 0,
            PFD_TYPE_COLORINDEX = 1
        }

        [DllImport("gdi32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern int ChoosePixelFormat(IntPtr hdc, [In] ref PIXELFORMATDESCRIPTOR ppfd);

        [DllImport("gdi32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool SetPixelFormat(IntPtr hdc, int iPixelFormat, ref PIXELFORMATDESCRIPTOR ppfd);

        [DllImport("opengl32.dll", SetLastError = true)]
        public static extern IntPtr wglCreateContext(IntPtr hDC);

        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern bool wglMakeCurrent(IntPtr hDC, IntPtr hRC);

        [DllImport("gdi32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool SwapBuffers(IntPtr hdc);

        [DllImport("opengl32.dll", SetLastError = true)]
        private static extern bool wglDeleteContext(IntPtr hRC);

        [DllImport("user32.dll")]
        static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        #region glew

        [DllImport("glew32.dll")]
        static extern uint glewInit();

        [DllImport("glew32.dll", EntryPoint = "glewGetErrorString")]
        static extern IntPtr _glewGetErrorString(uint error);
        public static string glewGetErrorString(uint error)
        {
            IntPtr pStr = _glewGetErrorString(error);
            var str = Marshal.PtrToStringAnsi(pStr);
            return str;
        }

        [DllImport("glew32.dll", EntryPoint = "glewGetString")]
        static extern IntPtr _glewGetString(uint name);
        public static string glewGetString(uint name)
        {
            IntPtr pStr = _glewGetString(name);
            var str = Marshal.PtrToStringAnsi(pStr);
            return str;
        }

        #endregion

        #endregion

        IntPtr hDC;
        IntPtr hglrc;
        IntPtr hwnd;

        private void InitGLEW()
        {
            uint err = glewInit();
            if (0/* GLEW_OK */ != err)
            {
                throw new Exception("Error: " + glewGetErrorString(err));
            }

            Debug.WriteLine(string.Format("Status: Using GLEW {0}", glewGetString(1/* GLEW_VERSION */)));
        }

        private void CreateOpenGLContext(IntPtr hwnd)
        {
            this.hwnd = hwnd;

            hDC = GetDC(hwnd);

            var pixelformatdescriptor = new PIXELFORMATDESCRIPTOR();
            pixelformatdescriptor.Init();

            var pixelFormat = ChoosePixelFormat(hDC, ref pixelformatdescriptor);
            if (!SetPixelFormat(hDC, pixelFormat, ref pixelformatdescriptor))
                throw new Exception(string.Format("SetPixelFormat failed: error {0}", Marshal.GetLastWin32Error()));

            if ((hglrc = wglCreateContext(hDC)) == IntPtr.Zero)
                throw new Exception(string.Format("SetPixelFormat failed: error {0}", Marshal.GetLastWin32Error()));

            if(!wglMakeCurrent(hDC, hglrc))
            {
                throw new Exception(string.Format("wglMakeCurrent failed: error {0}", Marshal.GetLastWin32Error()));
            }

            PrintGraphicInfo();
        }

        private void PrintGraphicInfo()
        {
            string version = GL.GetString(CSharpGL.GL.GL_VERSION);
            Debug.WriteLine("OpenGL version info: " + version);
            int[] tmp = { 0 };
            GL.GetIntegerv(CSharpGL.GL.GL_MAX_TEXTURE_SIZE, tmp);
            int max_texture_size = tmp[0];
            Debug.WriteLine("Max texture size: " + max_texture_size);
        }

        private void MakeCurrent()
        {
            if (!wglMakeCurrent(hDC, hglrc))
            {
                throw new Exception(string.Format("wglMakeCurrent failed: error {0}", Marshal.GetLastWin32Error()));
            }
        }

        public void SwapBuffers()
        {
            SwapBuffers(hDC);
        }

        private void DeleteOpenGLContext()
        {
            wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);
            wglDeleteContext(hglrc);
            ReleaseDC(hwnd, hDC);
        }
    }
}
