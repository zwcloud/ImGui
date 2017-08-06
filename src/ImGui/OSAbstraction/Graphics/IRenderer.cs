using System;
using ImGui.Common.Primitive;

namespace ImGui.OSAbstraction.Graphics
{
    /// <summary>
    /// Renderer-related functions
    /// </summary>
    /// <remarks>Renderers should implement this.</remarks>
    internal interface IRenderer
    {
        /// <summary>
        /// Initialize the renderer
        /// </summary>
        /// <param name="windowHandle">window handle, this could be some context info needed by the renderer. e.g. win32 HWND</param>
        /// <param name="size">size of default framebuffer</param>
        void Init(IntPtr windowHandle, Size size);

        /// <summary>
        /// Clear the rendered data
        /// </summary>
        void Clear();

        /// <summary>
        /// Render the drawList
        /// </summary>
        /// <param name="drawList">drawlist contains the mesh to be rendered</param>
        /// <param name="width">width of the rendering rectangle</param>
        /// <param name="height">height of the rendering rectangle</param>
        void RenderDrawList(DrawList drawList, int width, int height);

        /// <summary>
        /// swap front(what is on the screen) and back(what is rendered by the renderer) buffer
        /// </summary>
        void SwapBuffers();

        /// <summary>
        /// shut down the renderer
        /// </summary>
        void ShutDown();
    }
}
