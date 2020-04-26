namespace ImGui.OSImplementation.Android
{
    internal partial class OpenGLESRenderer
    {
        public void SwapBuffers()
        {
            // No need to do this on android, because Xamarin.Android did this later.
        }
    }
}