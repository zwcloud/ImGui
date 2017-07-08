using CSharpGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ImGui
{
    internal partial class Win32OpenGLRenderer
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
        public static extern IntPtr wglCreateContextAttribsARB(IntPtr hdc, IntPtr hShareContext, int[] attribList);

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

        class Wgl
        {
            [DllImport("OPENGL32.DLL", EntryPoint = "wglGetProcAddress", ExactSpelling = true, SetLastError = true)]
            internal static extern IntPtr GetProcAddress(string lpszProc);

            [DllImport("OPENGL32.DLL", EntryPoint = "wglGetCurrentContext", ExactSpelling = true, SetLastError = true)]
            internal static extern IntPtr GetCurrentContext();

            [DllImport("OPENGL32.DLL", EntryPoint = "wglCreateContext", ExactSpelling = true, SetLastError = true)]
            internal static extern IntPtr CreateContext(IntPtr hDc);

            [DllImport("OPENGL32.DLL", EntryPoint = "wglMakeCurrent", ExactSpelling = true, SetLastError = true)]
            internal static extern bool MakeCurrent(IntPtr hDc, IntPtr newContext);

            [DllImport("OPENGL32.DLL", EntryPoint = "wglDeleteContext", ExactSpelling = true, SetLastError = true)]
            internal static extern bool DeleteContext(IntPtr oldContext);

            private static string[] EntryPointNames;

            private static IntPtr[] EntryPoints;

            static Wgl()
            {
                Wgl.EntryPointNames = new string[]
                {
                    "wglCreateContextAttribsARB",
                    "wglGetExtensionsStringARB",
                    "wglGetPixelFormatAttribivARB",
                    "wglGetPixelFormatAttribfvARB",
                    "wglChoosePixelFormatARB",//4
                    "wglMakeContextCurrentARB",
                    "wglGetCurrentReadDCARB",
                    "wglCreatePbufferARB",
                    "wglGetPbufferDCARB",
                    "wglReleasePbufferDCARB",
                    "wglDestroyPbufferARB",
                    "wglQueryPbufferARB",
                    "wglBindTexImageARB",
                    "wglReleaseTexImageARB",
                    "wglSetPbufferAttribARB",
                    "wglGetExtensionsStringEXT",
                    "wglSwapIntervalEXT",
                    "wglGetSwapIntervalEXT"
                };
                Wgl.EntryPoints = new IntPtr[Wgl.EntryPointNames.Length];
            }

            private static bool IsValid(IntPtr address)
            {
                long a = address.ToInt64();
                return a < -1L || a > 3L;
            }

            private IntPtr GetAddress(string function_string)
            {
                IntPtr address = Wgl.GetProcAddress(function_string);
                if (!Wgl.IsValid(address))
                {
                    address = Win32.GetProcAddress(function_string);
                }
                return address;
            }
            internal void LoadEntryPoints(IntPtr hdc)
            {
                for (int i = 0; i < Wgl.EntryPointNames.Length; i++)
                {
                    Wgl.EntryPoints[i] = GetAddress(Wgl.EntryPointNames[i]);
                }
            }

            //  Delegates
            delegate bool wglChoosePixelFormatARB(IntPtr hdc, int[] piAttribIList, Single[] pfAttribFList, UInt32 nMaxFormats, [Out] out int piFormats, [Out] out UInt32 nNumFormats);

            internal static bool ChoosePixelFormatARB(IntPtr hdc, int[] piAttribIList, Single[] pfAttribFList, UInt32 nMaxFormats, [Out] out int piFormats, [Out] out UInt32 nNumFormats)
            {
                return Marshal.GetDelegateForFunctionPointer<wglChoosePixelFormatARB>(Wgl.EntryPoints[4])(hdc, piAttribIList, pfAttribFList, nMaxFormats, out piFormats, out nNumFormats);
            }
        }

        private IntPtr TempWindowProc(IntPtr hWnd, uint msg, UIntPtr wParam, IntPtr lParam)
        {
            return DefWindowProc(hWnd, msg, wParam, lParam);
        }
        delegate IntPtr WndProc(IntPtr hWnd, uint msg, UIntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        struct WNDCLASS
        {
            public uint style;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public WndProc lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpszMenuName;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpszClassName;
        }

        [DllImport("user32.dll")]
        static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, UIntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern ushort RegisterClassW([In] ref WNDCLASS lpWndClass);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CreateWindowEx(
            uint dwExStyle,
            IntPtr lpClassName,
            string lpWindowName,
            uint dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool DestroyWindow(IntPtr hwnd);

        #endregion

        #endregion

        IntPtr hDC;
        IntPtr hglrc;
        IntPtr hwnd;

        enum WGL
        {
            //WGL_ARB_pixel_format
            WGL_NUMBER_PIXEL_FORMATS_ARB = 0x2000,
            WGL_DRAW_TO_WINDOW_ARB = 0x2001,
            WGL_DRAW_TO_BITMAP_ARB = 0x2002,
            WGL_ACCELERATION_ARB = 0x2003,
            WGL_NEED_PALETTE_ARB = 0x2004,
            WGL_NEED_SYSTEM_PALETTE_ARB = 0x2005,
            WGL_SWAP_LAYER_BUFFERS_ARB = 0x2006,
            WGL_SWAP_METHOD_ARB = 0x2007,
            WGL_NUMBER_OVERLAYS_ARB = 0x2008,
            WGL_NUMBER_UNDERLAYS_ARB = 0x2009,
            WGL_TRANSPARENT_ARB = 0x200A,
            WGL_SHARE_DEPTH_ARB = 0x200C,
            WGL_SHARE_STENCIL_ARB = 0x200D,
            WGL_SHARE_ACCUM_ARB = 0x200E,
            WGL_SUPPORT_GDI_ARB = 0x200F,
            WGL_SUPPORT_OPENGL_ARB = 0x2010,
            WGL_DOUBLE_BUFFER_ARB = 0x2011,
            WGL_STEREO_ARB = 0x2012,
            WGL_PIXEL_TYPE_ARB = 0x2013,
            WGL_COLOR_BITS_ARB = 0x2014,
            WGL_RED_BITS_ARB = 0x2015,
            WGL_RED_SHIFT_ARB = 0x2016,
            WGL_GREEN_BITS_ARB = 0x2017,
            WGL_GREEN_SHIFT_ARB = 0x2018,
            WGL_BLUE_BITS_ARB = 0x2019,
            WGL_BLUE_SHIFT_ARB = 0x201A,
            WGL_ALPHA_BITS_ARB = 0x201B,
            WGL_ALPHA_SHIFT_ARB = 0x201C,
            WGL_ACCUM_BITS_ARB = 0x201D,
            WGL_ACCUM_RED_BITS_ARB = 0x201E,
            WGL_ACCUM_GREEN_BITS_ARB = 0x201F,
            WGL_ACCUM_BLUE_BITS_ARB = 0x2020,
            WGL_ACCUM_ALPHA_BITS_ARB = 0x2021,
            WGL_DEPTH_BITS_ARB = 0x2022,
            WGL_STENCIL_BITS_ARB = 0x2023,
            WGL_AUX_BUFFERS_ARB = 0x2024,
            WGL_NO_ACCELERATION_ARB = 0x2025,
            WGL_GENERIC_ACCELERATION_ARB = 0x2026,
            WGL_FULL_ACCELERATION_ARB = 0x2027,
            WGL_SWAP_EXCHANGE_ARB = 0x2028,
            WGL_SWAP_COPY_ARB = 0x2029,
            WGL_SWAP_UNDEFINED_ARB = 0x202A,
            WGL_TYPE_RGBA_ARB = 0x202B,
            WGL_TYPE_COLORINDEX_ARB = 0x202C,
            WGL_TRANSPARENT_RED_VALUE_ARB = 0x2037,
            WGL_TRANSPARENT_GREEN_VALUE_ARB = 0x2038,
            WGL_TRANSPARENT_BLUE_VALUE_ARB = 0x2039,
            WGL_TRANSPARENT_ALPHA_VALUE_ARB = 0x203A,
            WGL_TRANSPARENT_INDEX_VALUE_ARB = 0x203B,

            //WGL_ARB_multisample
            WGL_SAMPLE_BUFFERS_ARB = 0x2041,
            WGL_SAMPLES_ARB = 0x2042
        }

        private void CreateOpenGLContext(IntPtr hwnd)
        {
            this.hwnd = hwnd;
            hDC = GetDC(hwnd);

            IntPtr tempContext = IntPtr.Zero;
            IntPtr tempHwnd = IntPtr.Zero;
            //Create temporary window
            IntPtr hInstance = Process.GetCurrentProcess().SafeHandle.DangerousGetHandle();
            string szClassName = "tmpWindow~";

            WNDCLASS wndclass;
            wndclass.style = 0x0002 /*CS_HREDRAW*/ | 0x0001/*CS_VREDRAW*/;
            wndclass.lpfnWndProc = TempWindowProc;
            wndclass.cbClsExtra = 0;
            wndclass.cbWndExtra = 0;
            wndclass.hInstance = hInstance;
            wndclass.hIcon = IntPtr.Zero;
            wndclass.hCursor = IntPtr.Zero;
            wndclass.hbrBackground = IntPtr.Zero;
            wndclass.lpszMenuName = null;
            wndclass.lpszClassName = szClassName;

            ushort atom = RegisterClassW(ref wndclass);
            if (atom == 0)
            {
                throw new WindowCreateException(string.Format("RegisterClass error: {0}", Marshal.GetLastWin32Error()));
            }

            tempHwnd = CreateWindowEx(
                0,
                new IntPtr(atom),
                "tmpWindow~",
                (uint)(WindowStyles.WS_VISIBLE | WindowStyles.WS_CHILD),
                0, 0, 10, 10, hwnd, IntPtr.Zero,
                hInstance,
                IntPtr.Zero);

            if (tempHwnd == IntPtr.Zero)
            {
                throw new WindowCreateException(string.Format("CreateWindowEx for tempContext error: {0}", Marshal.GetLastWin32Error()));
            }

            IntPtr tempHdc = GetDC(tempHwnd);

            var pixelformatdescriptor = new PIXELFORMATDESCRIPTOR();
            pixelformatdescriptor.Init();

            if(!SetPixelFormat(tempHdc, 1, ref pixelformatdescriptor))
            {
                throw new Exception(string.Format("SetPixelFormat failed: error {0}", Marshal.GetLastWin32Error()));
            }

            tempContext = Wgl.CreateContext(tempHdc);//Crate temp context to load entry points
            if(tempContext == IntPtr.Zero)
            {
                throw new Exception(string.Format("wglCreateContext for tempHdc failed: error {0}", Marshal.GetLastWin32Error()));
            }

            if (!Wgl.MakeCurrent(tempHdc, tempContext))
            {
                throw new Exception(string.Format("wglMakeCurrent for tempContext failed: error {0}", Marshal.GetLastWin32Error()));
            }

            //load wgl entry points for wglXXXARB functions
            new Wgl().LoadEntryPoints(tempHdc);

            //Init glew
            uint err = glewInit();
            if (0/* GLEW_OK */ != err)
            {
                throw new Exception("Error: " + glewGetErrorString(err));
            }
            Debug.WriteLine(string.Format("Status: Using GLEW {0}", glewGetString(1/* GLEW_VERSION */)));

            //Destroy temp window and temp WGL context
            Wgl.MakeCurrent(IntPtr.Zero, IntPtr.Zero);
            DestroyWindow(tempHwnd);
            Wgl.DeleteContext(tempContext);

            int[] iPixAttribs = {
                (int)WGL.WGL_SUPPORT_OPENGL_ARB, 1,
                (int)WGL.WGL_DRAW_TO_WINDOW_ARB, 1,
                (int)WGL.WGL_ACCELERATION_ARB,   (int)WGL.WGL_FULL_ACCELERATION_ARB,
                (int)WGL.WGL_COLOR_BITS_ARB,     32,
                (int)WGL.WGL_DEPTH_BITS_ARB,     24,
                (int)WGL.WGL_DOUBLE_BUFFER_ARB,(int)GL.GL_TRUE,
                (int)WGL.WGL_PIXEL_TYPE_ARB,      (int)WGL.WGL_TYPE_RGBA_ARB,
                (int)WGL.WGL_STENCIL_BITS_ARB, 8,
                (int)WGL.WGL_SAMPLE_BUFFERS_ARB, (int)GL.GL_TRUE,//Enable MXAA
                (int)WGL.WGL_SAMPLES_ARB,        8,
            0};

            int pixelFormat;
            uint numFormats;
            if(!Wgl.ChoosePixelFormatARB(hDC, iPixAttribs, null, 1, out pixelFormat, out numFormats))
            {
                throw new Exception(string.Format("wglChoosePixelFormatARB failed: error {0}", Marshal.GetLastWin32Error()));
            }

            if (!SetPixelFormat(hDC, pixelFormat, ref pixelformatdescriptor))
            {
                throw new Exception(string.Format("SetPixelFormat failed: error {0}", Marshal.GetLastWin32Error()));
            }

            if ((hglrc = wglCreateContext(hDC)) == IntPtr.Zero)
            {
                throw new Exception(string.Format("wglCreateContext failed: error {0}", Marshal.GetLastWin32Error()));
            }

            if (!wglMakeCurrent(hDC, hglrc))
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
