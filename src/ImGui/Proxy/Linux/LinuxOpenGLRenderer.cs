using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using CSharpGLES;

namespace ImGui
{

    class LinuxOpenGLRenderer : IRenderer
    {
        #region Native

        const string libEGL = "libEGL.so.1";
        IntPtr EGL_NONE = (IntPtr)0x3038;

        /*
EGL_SUCCESS
The last function succeeded without error.

EGL_NOT_INITIALIZED
EGL is not initialized, or could not be initialized, for the specified EGL display connection.

EGL_BAD_ACCESS
EGL cannot access a requested resource (for example a context is bound in another thread).

EGL_BAD_ALLOC
EGL failed to allocate resources for the requested operation.

EGL_BAD_ATTRIBUTE
An unrecognized attribute or attribute value was passed in the attribute list.

EGL_BAD_CONTEXT
An EGLContext argument does not name a valid EGL rendering context.

EGL_BAD_CONFIG
An EGLConfig argument does not name a valid EGL frame buffer configuration.

EGL_BAD_CURRENT_SURFACE
The current surface of the calling thread is a window, pixel buffer or pixmap that is no longer valid.

EGL_BAD_DISPLAY
An EGLDisplay argument does not name a valid EGL display connection.

EGL_BAD_SURFACE
An EGLSurface argument does not name a valid surface (window, pixel buffer or pixmap) configured for GL rendering.

EGL_BAD_MATCH
Arguments are inconsistent (for example, a valid context requires buffers not supplied by a valid surface).

EGL_BAD_PARAMETER
One or more argument values are invalid.

EGL_BAD_NATIVE_PIXMAP
A NativePixmapType argument does not refer to a valid native pixmap.

EGL_BAD_NATIVE_WINDOW
A NativeWindowType argument does not refer to a valid native window.

EGL_CONTEXT_LOST
A power management event has occurred. The application must destroy all contexts and reinitialise OpenGL ES state and objects to continue rendering.
*/

        enum EGL_ERROR
        {
            EGL_SUCCESS             = 0x3000,
            EGL_NOT_INITIALIZED     = 0x3001,
            EGL_BAD_ACCESS          = 0x3002,
            EGL_BAD_ALLOC           = 0x3003,
            EGL_BAD_ATTRIBUTE       = 0x3004,
            EGL_BAD_CONTEXT         = 0x3006,
            EGL_BAD_CONFIG          = 0x3005,
            EGL_BAD_CURRENT_SURFACE = 0x3007,
            EGL_BAD_DISPLAY         = 0x3008,
            EGL_BAD_SURFACE         = 0x300D,
            EGL_BAD_MATCH           = 0x3009,
            EGL_BAD_PARAMETER       = 0x300C,
            EGL_BAD_NATIVE_PIXMAP   = 0x300A,
            EGL_BAD_NATIVE_WINDOW   = 0x300B,
            EGL_CONTEXT_LOST        = 0x300E,
        }

        Dictionary<EGL_ERROR, string> EGL_Error_Text = new Dictionary<EGL_ERROR, string>
        {
            [EGL_ERROR.EGL_SUCCESS]             = "The last function succeeded without error.",
            [EGL_ERROR.EGL_NOT_INITIALIZED]     = "EGL is not initialized, or could not be initialized, for the specified EGL display connection.",
            [EGL_ERROR.EGL_BAD_ACCESS]          = "EGL cannot access a requested resource (for example a context is bound in another thread).",
            [EGL_ERROR.EGL_BAD_ALLOC]           = "EGL failed to allocate resources for the requested operation.",
            [EGL_ERROR.EGL_BAD_ATTRIBUTE]       = "An unrecognized attribute or attribute value was passed in the attribute list.",
            [EGL_ERROR.EGL_BAD_CONTEXT]         = "An EGLContext argument does not name a valid EGL rendering context.",
            [EGL_ERROR.EGL_BAD_CONFIG]          = "An EGLConfig argument does not name a valid EGL frame buffer configuration.",
            [EGL_ERROR.EGL_BAD_CURRENT_SURFACE] = "The current surface of the calling thread is a window, pixel buffer or pixmap that is no longer valid.",
            [EGL_ERROR.EGL_BAD_DISPLAY]         = "An EGLDisplay argument does not name a valid EGL display connection.",
            [EGL_ERROR.EGL_BAD_SURFACE]         = "An EGLSurface argument does not name a valid surface (window, pixel buffer or pixmap) configured for GL rendering.",
            [EGL_ERROR.EGL_BAD_MATCH]           = "Arguments are inconsistent (for example, a valid context requires buffers not supplied by a valid surface).",
            [EGL_ERROR.EGL_BAD_PARAMETER]       = "One or more argument values are invalid.",
            [EGL_ERROR.EGL_BAD_NATIVE_PIXMAP]   = "A NativePixmapType argument does not refer to a valid native pixmap.",
            [EGL_ERROR.EGL_BAD_NATIVE_WINDOW]   = "A NativeWindowType argument does not refer to a valid native window.",
            [EGL_ERROR.EGL_CONTEXT_LOST]        = "A power management event has occurred. The application must destroy all contexts and reinitialise OpenGL ES state and objects to continue rendering.",
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
        static extern bool eglChooseConfig(IntPtr dpy, IntPtr[] attrib_list,
            ref IntPtr configs, IntPtr config_size/*fixed to 1*/, ref IntPtr num_config);

        [DllImport(libEGL)]
        static extern int eglBindAPI(int api);

        [DllImport(libEGL)]
        static extern IntPtr eglCreateContext(IntPtr/*EGLDisplay*/ dpy, IntPtr/*EGLConfig*/ config, IntPtr share_context, IntPtr[] attrib_list);

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
        static extern int eglMakeCurrent(IntPtr dpy, IntPtr draw, IntPtr read, IntPtr ctx);

        [DllImport(libEGL)]
        static extern int eglTerminate(IntPtr display);

        [DllImport(libEGL)]
        static extern int eglSwapBuffers(IntPtr display, IntPtr surface);

        [DllImport(libEGL)]
        static extern int eglSwapInterval(IntPtr display, int interval);

        #endregion


        public void Clear()
        {
            GL.Clear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT);
        }

        public void Init(object windowHandle)
        {
            var nativeWindow = (IntPtr)windowHandle;
            if (!CreateOpenGLContext())
            {
                Debug.WriteLine("Failed to create EGL context.");
                return;
            }

            if (!CreateEGLSurface(nativeWindow))
            {
                Debug.WriteLine("Failed to create EGL surface.");
                return;
            }
        }

        public void RenderDrawList(DrawList drawList, int width, int height)
        {
            //TODO render drawlist
        }

        public void ShutDown()
        {
            eglDestroyContext(display, context);
            eglDestroySurface(display, surface);
        }

        public void SwapBuffers()
        {
            eglSwapBuffers(display, surface);
        }

        IntPtr display;//EGLDisplay
        IntPtr config = IntPtr.Zero;//EGLConfig
        IntPtr context;//EGLContext

        //https://www.khronos.org/opengl/wiki/Platform_specifics:_Linux#Source_Code
        private bool CreateOpenGLContext()
        {
            bool result = false;

            /* get an EGL display connection */
            display = eglGetDisplay(IntPtr.Zero/*EGL_DEFAULT_DISPLAY*/);
            if(display == IntPtr.Zero/*EGL_NO_DISPLAY*/)
            {
                Debug.WriteLine("Error: eglGetDisplay() failed. error: no default display connection is available.");
            }

            /* initialize the EGL display connection */
            int major, minor;
            result = eglInitialize(display, out major, out minor);
            if (!result)
            {
                var error = (EGL_ERROR)eglGetError();
                Debug.WriteLine("Error: eglInitialize() failed. error: {0}", EGL_Error_Text[error]);
                return false;
            }
            Debug.WriteLine("EGL version: {0}.{1}", major, minor);

            /* get an appropriate EGL frame buffer configuration */
            /*
             All attributes in attrib_list, including boolean attributes, are immediately
             followed by the corresponding desired value. The list is terminated with EGL_NONE

             https://www.khronos.org/registry/egl/sdk/docs/man/html/eglChooseConfig.xhtml
             */
            IntPtr[] attribute_list = new IntPtr[]{
                (IntPtr)0x3024/*EGL_RED_SIZE*/  , (IntPtr)1,
                (IntPtr)0x3023/*EGL_GREEN_SIZE*/, (IntPtr)1,
                (IntPtr)0x3022/*EGL_BLUE_SIZE*/ , (IntPtr)1,
                EGL_NONE };
            IntPtr num_config = IntPtr.Zero;
            result = eglChooseConfig(display, attribute_list, ref config, (IntPtr)1, ref num_config);
            if (!result)
            {
                var error = (EGL_ERROR)eglGetError();
                Debug.WriteLine("Error: eglChooseConfig() failed. error: {0}", EGL_Error_Text[error]);
                return false;
            }

            /* create an EGL rendering context */
            /*
             #define EGL_OPENGL_ES_API		0x30A0
             #define EGL_OPENVG_API			0x30A1
             #define EGL_OPENGL_API			0x30A2
             */
            eglBindAPI(0x30A0/*EGL_OPENGL_ES_API*/);
            /*
             eglCreateContext — create a new EGL rendering context

             attrib_list specifies a list of attributes for the context.
             The list has the same structure as described for eglChooseConfig

             https://www.khronos.org/registry/egl/sdk/docs/man/html/eglCreateContext.xhtml
             */
            IntPtr[] ctx_attribs = new IntPtr[] { (IntPtr)0x3098/*EGL_CONTEXT_CLIENT_VERSION*/, (IntPtr)2, EGL_NONE };

            context = eglCreateContext(display, config, IntPtr.Zero/*EGL_NO_CONTEXT*/, ctx_attribs);

            //Request eglVisualID for native window
            IntPtr eglConfAttrVisualID = IntPtr.Zero;
            result = eglGetConfigAttrib(display, config, (IntPtr)0x302E/*EGL_NATIVE_VISUAL_ID*/,
                    ref eglConfAttrVisualID);
            if (!result)
            {
                var error = (EGL_ERROR)eglGetError();
                Debug.WriteLine("Error: eglGetConfigAttrib() failed. error: {0}", EGL_Error_Text[error]);
                return false;
            }

            return true;
        }

        IntPtr surface;//EGLSurface
        private bool CreateEGLSurface(IntPtr nativeWindow)
        {
            /* create an EGL window surface */
            surface = eglCreateWindowSurface(display, config, nativeWindow/*for xcb window, this should be xcb_window_t*/, IntPtr.Zero);

            /* connect the context to the surface */
            eglMakeCurrent(display, surface, surface, context);

            return true;
        }
    }
}
