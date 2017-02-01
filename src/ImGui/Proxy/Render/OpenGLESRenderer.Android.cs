using System;

namespace ImGui
{
    internal partial class OpenGLESRenderer
    {
        public void SwapBuffers()
        {
            // No need to do this on android devices, because Xamarin did this.
            throw new InvalidOperationException();
        }
    }
}