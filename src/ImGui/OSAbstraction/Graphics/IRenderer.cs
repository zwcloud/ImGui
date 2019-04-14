using System;

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
        /// Clear the front buffer
        /// </summary>
        void Clear(Color color);

        /// <summary>
        /// Draw meshes
        /// </summary>
        /// <param name="width">viewport width</param>
        /// <param name="height">viewport height</param>
        /// <param name="meshes">meshes</param>
        void DrawMeshes(int width, int height, (Mesh shapeMesh, Mesh imageMesh, TextMesh textMesh) meshes);

        /// <summary>
        /// swap front(what is on the screen) and back(what is rendered by the renderer) buffer
        /// </summary>
        void SwapBuffers();

        /// <summary>
        /// shut down the renderer
        /// </summary>
        void ShutDown();

        /// <summary>
        /// Get back buffer data
        /// </summary>
        byte[] GetRawBackBuffer(out int width, out int height);
    }
}
