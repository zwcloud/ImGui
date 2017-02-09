using System;

namespace ImGui
{
    internal partial class OpenGLESRenderer
    {
        public void SwapBuffers()
        {
            if(Utility.CurrentOS.IsAndroid)
            {
                // No need to do this on android devices, because Xamarin did this later.

            }
        }
    }
}