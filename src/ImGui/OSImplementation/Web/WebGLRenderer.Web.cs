using System;
using System.Runtime.InteropServices;
using EMSCRIPTEN_RESULT = System.Int32;
using EM_BOOL = System.Int32;
using EMSCRIPTEN_WEBGL_CONTEXT_HANDLE = System.Int32;

namespace ImGui.OSImplementation.Web
{
    internal partial class WebGLRenderer
    {
        #region Native
        enum EM_WEBGL_POWER_PREFERENCE : int
        {
            EM_WEBGL_POWER_PREFERENCE_DEFAULT = 0,
            EM_WEBGL_POWER_PREFERENCE_LOW_POWER = 1,
            EM_WEBGL_POWER_PREFERENCE_HIGH_PERFORMANCE= 2,
        }

        enum EMSCRIPTEN_WEBGL_CONTEXT_PROXY_MODE : int
        {
            EMSCRIPTEN_WEBGL_CONTEXT_PROXY_DISALLOW = 0,
            EMSCRIPTEN_WEBGL_CONTEXT_PROXY_FALLBACK = 1,
            EMSCRIPTEN_WEBGL_CONTEXT_PROXY_ALWAYS = 2
        }

        //It's needed to link emscripten webgl when building for Web.
        [DllImport("*")]
        private static extern double emscripten_get_device_pixel_ratio();

        [DllImport("*")]
        private static extern Int32 emscripten_set_element_css_size(string target, double width, double height);
        
        [DllImport("*")]
        private static extern Int32 emscripten_set_canvas_element_size(string target, double width, double height);
        
        [StructLayout(LayoutKind.Sequential)]
        struct EmscriptenWebGLContextAttributes
        {
            public EM_BOOL alpha;
            public EM_BOOL depth;
            public EM_BOOL stencil;
            public EM_BOOL antialias;
            public EM_BOOL premultipliedAlpha;
            public EM_BOOL preserveDrawingBuffer;
            public EM_WEBGL_POWER_PREFERENCE powerPreference;
            public EM_BOOL failIfMajorPerformanceCaveat;

            public int majorVersion;
            public int minorVersion;

            public EM_BOOL enableExtensionsByDefault;
            public EM_BOOL explicitSwapControl;
            public EMSCRIPTEN_WEBGL_CONTEXT_PROXY_MODE proxyContextToMainThread;
            public EM_BOOL renderViaOffscreenBackBuffer;

            //TODO add ToString method for debug
        }

        [DllImport("*")]
        private static extern void emscripten_webgl_init_context_attributes(out EmscriptenWebGLContextAttributes attributes);

        [DllImport("*")]
        private static extern EMSCRIPTEN_WEBGL_CONTEXT_HANDLE emscripten_webgl_create_context(string target, ref EmscriptenWebGLContextAttributes attributes);
        
        [DllImport("*")]
        private static extern EMSCRIPTEN_WEBGL_CONTEXT_HANDLE emscripten_webgl_make_context_current(Int32 context);
        #endregion

        private EMSCRIPTEN_WEBGL_CONTEXT_HANDLE glContext;
        //ref: emscripten init_webgl()
        private void CreateWebGLContext(Size size)
        {
            var width = size.Width;
            var height = size.Height;
            double dpr = emscripten_get_device_pixel_ratio();
            emscripten_set_element_css_size("canvas", width / dpr, height / dpr);
            emscripten_set_canvas_element_size("canvas", width, height);
            EmscriptenWebGLContextAttributes attrs;
            emscripten_webgl_init_context_attributes(out attrs);
            attrs.alpha = 0;
            attrs.majorVersion = 2;//use WebGL2
            glContext = emscripten_webgl_create_context("canvas", ref attrs);
            if (glContext == 0)
            {
                throw new Exception("Failed to create WebGL context via emscripten_webgl_create_context");
            }
            emscripten_webgl_make_context_current(glContext);
        }
    }
}