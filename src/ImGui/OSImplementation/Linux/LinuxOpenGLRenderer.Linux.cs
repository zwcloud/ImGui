using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ImGui.OSImplementation.Linux
{
    internal partial class LinuxOpenGLRenderer
    {
        #region Native

        const string libEGL = "libEGL.so.1";
        const int EGL_NONE = 0x3038;

        enum EGL_ERROR
        {
            EGL_SUCCESS = 0x3000,
            EGL_NOT_INITIALIZED = 0x3001,
            EGL_BAD_ACCESS = 0x3002,
            EGL_BAD_ALLOC = 0x3003,
            EGL_BAD_ATTRIBUTE = 0x3004,
            EGL_BAD_CONTEXT = 0x3006,
            EGL_BAD_CONFIG = 0x3005,
            EGL_BAD_CURRENT_SURFACE = 0x3007,
            EGL_BAD_DISPLAY = 0x3008,
            EGL_BAD_SURFACE = 0x300D,
            EGL_BAD_MATCH = 0x3009,
            EGL_BAD_PARAMETER = 0x300C,
            EGL_BAD_NATIVE_PIXMAP = 0x300A,
            EGL_BAD_NATIVE_WINDOW = 0x300B,
            EGL_CONTEXT_LOST = 0x300E,
        }

        Dictionary<EGL_ERROR, string> EGL_Error_Text = new Dictionary<EGL_ERROR, string>
        {
            [EGL_ERROR.EGL_SUCCESS] = "The last function succeeded without error.",
            [EGL_ERROR.EGL_NOT_INITIALIZED] = "EGL is not initialized, or could not be initialized, for the specified EGL display connection.",
            [EGL_ERROR.EGL_BAD_ACCESS] = "EGL cannot access a requested resource (for example a context is bound in another thread).",
            [EGL_ERROR.EGL_BAD_ALLOC] = "EGL failed to allocate resources for the requested operation.",
            [EGL_ERROR.EGL_BAD_ATTRIBUTE] = "An unrecognized attribute or attribute value was passed in the attribute list.",
            [EGL_ERROR.EGL_BAD_CONTEXT] = "An EGLContext argument does not name a valid EGL rendering context.",
            [EGL_ERROR.EGL_BAD_CONFIG] = "An EGLConfig argument does not name a valid EGL frame buffer configuration.",
            [EGL_ERROR.EGL_BAD_CURRENT_SURFACE] = "The current surface of the calling thread is a window, pixel buffer or pixmap that is no longer valid.",
            [EGL_ERROR.EGL_BAD_DISPLAY] = "An EGLDisplay argument does not name a valid EGL display connection.",
            [EGL_ERROR.EGL_BAD_SURFACE] = "An EGLSurface argument does not name a valid surface (window, pixel buffer or pixmap) configured for GL rendering.",
            [EGL_ERROR.EGL_BAD_MATCH] = "Arguments are inconsistent (for example, a valid context requires buffers not supplied by a valid surface).",
            [EGL_ERROR.EGL_BAD_PARAMETER] = "One or more argument values are invalid.",
            [EGL_ERROR.EGL_BAD_NATIVE_PIXMAP] = "A NativePixmapType argument does not refer to a valid native pixmap.",
            [EGL_ERROR.EGL_BAD_NATIVE_WINDOW] = "A NativeWindowType argument does not refer to a valid native window.",
            [EGL_ERROR.EGL_CONTEXT_LOST] = "A power management event has occurred. The application must destroy all contexts and reinitialise OpenGL ES state and objects to continue rendering.",
        };

        [DllImport(libEGL)]
        static extern IntPtr eglGetError();

        [DllImport(libEGL)]
        static extern IntPtr eglGetDisplay(IntPtr display_id);

        [DllImport(libEGL)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool eglInitialize(IntPtr dpy, out int major, out int minor);

        [DllImport(libEGL)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool eglChooseConfig(IntPtr dpy, int[] attrib_list,
            ref IntPtr configs, IntPtr config_size/*fixed to 1*/, ref IntPtr num_config);

        [DllImport(libEGL)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool eglBindAPI(int api);

        [DllImport(libEGL)]
        static extern IntPtr eglCreateContext(IntPtr/*EGLDisplay*/ dpy, IntPtr/*EGLConfig*/ config, IntPtr share_context, int[] attrib_list);

        [DllImport(libEGL)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool eglGetConfigAttrib(IntPtr/*EGLDisplay*/ dpy, IntPtr/*EGLConfig*/ config, IntPtr attribute, ref IntPtr value);

        [DllImport(libEGL)]
        public static extern IntPtr eglCreateWindowSurface(IntPtr dpy, IntPtr config, IntPtr win, IntPtr attrib_list/*fixed to NULL*/);

        [DllImport(libEGL)]
        static extern int eglDestroySurface(IntPtr dpy, IntPtr surface);

        [DllImport(libEGL)]
        static extern int eglDestroyContext(IntPtr dpy, IntPtr ctx);

        [DllImport(libEGL)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool eglMakeCurrent(IntPtr dpy, IntPtr draw, IntPtr read, IntPtr ctx);

        [DllImport(libEGL)]
        static extern int eglTerminate(IntPtr display);

        [DllImport(libEGL)]
        static extern int eglSwapBuffers(IntPtr display, IntPtr surface);

        [DllImport(libEGL)]
        static extern int eglSwapInterval(IntPtr display, int interval);

        #endregion

        IntPtr display;//EGLDisplay
        IntPtr config = IntPtr.Zero;//EGLConfig
        IntPtr context;//EGLContext
        IntPtr surface;//EGLSurface

        //https://www.khronos.org/opengl/wiki/Platform_specifics:_Linux#Source_Code
        private bool CreateEGLContext()
        {
            bool result = false;

            /* get an EGL display connection */
            this.display = eglGetDisplay(IntPtr.Zero/*EGL_DEFAULT_DISPLAY*/);
            if (this.display == IntPtr.Zero/*EGL_NO_DISPLAY*/)
            {
                Debug.WriteLine("Error: eglGetDisplay() failed. error: no default display connection is available.");
            }

            /* initialize the EGL display connection */
            int major, minor;
            result = eglInitialize(this.display, out major, out minor);
            if (!result)
            {
                var error = (EGL_ERROR)eglGetError();
                Debug.WriteLine(string.Format("Error: eglInitialize() failed. error<{0}>:{1}", error.ToString(), this.EGL_Error_Text[error]));
                return false;
            }
            Debug.WriteLine("EGL version: {0}.{1}", major, minor);

            /* get an appropriate EGL frame buffer configuration */
            /*
             All attributes in attrib_list, including boolean attributes, are immediately
             followed by the corresponding desired value. The list is terminated with EGL_NONE

             https://www.khronos.org/registry/egl/sdk/docs/man/html/eglChooseConfig.xhtml
             */
            int[] attribute_list = new int[]{
                0x3024/*EGL_RED_SIZE*/  , 8,
                0x3023/*EGL_GREEN_SIZE*/, 8,
                0x3022/*EGL_BLUE_SIZE*/ , 8,
                0x3025/*EGL_DEPTH_SIZE*/, 24,
                0x3026/*EGL_STENCIL_SIZE*/, 8,
                0x3033/*EGL_SURFACE_TYPE*/, 0x0004/*EGL_WINDOW_BIT*/,
                0x3040/*EGL_RENDERABLE_TYPE*/, 0x00000040/*EGL_OPENGL_ES3_BIT*/,
                //0x3031/*EGL_SAMPLES*/, 16,//MSAA, 16 samples
                EGL_NONE };

            IntPtr num_config = IntPtr.Zero;
            result = eglChooseConfig(this.display, attribute_list, ref this.config, (IntPtr)1, ref num_config);
            if (!result || num_config == IntPtr.Zero)
            {
                var error = (EGL_ERROR)eglGetError();
                Debug.WriteLine(string.Format("Error: eglChooseConfig() failed. error<{0}>:{1}", error.ToString(), this.EGL_Error_Text[error]));
                return false;
            }

            /* create an EGL rendering context */
            /*
             #define EGL_OPENGL_ES_API		0x30A0
             #define EGL_OPENVG_API			0x30A1
             #define EGL_OPENGL_API			0x30A2
             */
            if (!eglBindAPI(0x30A0/*EGL_OPENGL_ES_API*/))
            {
                var error = (EGL_ERROR)eglGetError();
                Debug.WriteLine(string.Format("Error: eglBindAPI() failed. error<{0}>:{1}", error.ToString(), this.EGL_Error_Text[error]));
                return false;
            }

            /*
             eglCreateContext — create a new EGL rendering context

             attrib_list specifies a list of attributes for the context.
             The list has the same structure as described for eglChooseConfig

             https://www.khronos.org/registry/egl/sdk/docs/man/html/eglCreateContext.xhtml
             */
            int[] ctx_attribs = new int[] { 0x3098/*EGL_CONTEXT_CLIENT_VERSION*/, 3, EGL_NONE };
            this.context = eglCreateContext(this.display, this.config, IntPtr.Zero/*EGL_NO_CONTEXT*/, ctx_attribs);
            if(this.context == IntPtr.Zero/*EGL_NO_CONTEXT*/)
            {
                var error = (EGL_ERROR)eglGetError();
                Debug.WriteLine(string.Format("Error: eglCreateContext() failed. error<{0}>:{1}", error.ToString(), this.EGL_Error_Text[error]));
                return false;
            }

            //Request eglVisualID for native window
            IntPtr eglConfAttrVisualID = IntPtr.Zero;
            result = eglGetConfigAttrib(this.display, this.config, (IntPtr)0x302E/*EGL_NATIVE_VISUAL_ID*/,
                    ref eglConfAttrVisualID);
            if (!result)
            {
                var error = (EGL_ERROR)eglGetError();
                Debug.WriteLine(string.Format("Error: eglGetConfigAttrib() failed. error<{0}>:{1}", error.ToString(), this.EGL_Error_Text[error]));
                return false;
            }

            return true;
        }

        private bool CreateEGLSurface(IntPtr nativeWindow)
        {
            bool result = false;

            /* create an EGL window surface */
            this.surface = eglCreateWindowSurface(this.display, this.config, nativeWindow/*for xcb window, this should be xcb_window_t*/, IntPtr.Zero);
            if(this.surface == IntPtr.Zero)
            {
                var error = (EGL_ERROR)eglGetError();
                Debug.WriteLine(string.Format("Error: eglCreateWindowSurface() failed. error<{0}>:{1}", error.ToString(), this.EGL_Error_Text[error]));
                return false;
            }

            /* connect the context to the surface */
            result = eglMakeCurrent(this.display, this.surface, this.surface, this.context);
            if (!result)
            {
                var error = (EGL_ERROR)eglGetError();
                Debug.WriteLine(string.Format("Error: eglMakeCurrent() failed. error<{0}>:{1}", error.ToString(), this.EGL_Error_Text[error]));
                return false;
            }

            return true;
        }

        private bool DestroyEGL()
        {
            eglDestroyContext(this.display, this.context);
            eglDestroySurface(this.display, this.surface);

            return true;
        }
    }
}
