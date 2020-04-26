namespace ImGui.OSImplementation.Web
{
    internal partial class WebGLRenderer
    {
        public void SwapBuffers()
        {
            // No need to do this for the web, because the browser will do this later.
        }
    }
}