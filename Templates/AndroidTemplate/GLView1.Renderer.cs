using OpenTK;
using OpenTK.Graphics.ES11;

namespace AndroidTemplate
{
    partial class GLView1
    {
        // This gets called on each frame render
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            
            GL.ClearColor(0.5f, 0.5f, 0f, 1.0f);
            GL.Clear((uint) All.ColorBufferBit);
            
            ImGui.Application.RunLoop(mainForm);

            SwapBuffers();

        }
    }
}