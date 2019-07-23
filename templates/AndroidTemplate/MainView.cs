using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES30;
using OpenTK.Platform;
using OpenTK.Platform.Android;
using Android.Views;
using Android.Content;
using Android.Util;
using ImGui;
using Size = ImGui.Size;

namespace AndroidTemplate
{
    internal partial class MainView : AndroidGameView
    {
        private MainForm mainForm;

        public MainView(Context context) : base(context)
        {
        }

        // This gets called when the drawing surface is ready
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            //Create form
            mainForm = new MainForm(Point.Zero/*dummy*/, new Size(this.Size.Width, this.Size.Height));
            ImGui.Application.Init(mainForm);
            
            Run();// Run the render loop
        }

        // This method is called everytime the context needs
        // to be recreated. Use it to set any egl-specific settings
        // prior to context creation
        protected override void CreateFrameBuffer()
        {
            // using OpenGLES3.0
            this.GLContextVersion = GLContextVersion.Gles3_0;
            // the default GraphicsMode that is set consists of (16, 16, 0, 0, 2, false)
            try
            {
                this.GraphicsMode = new GraphicsMode(new ColorFormat(32), 16, 8, 4);
                base.CreateFrameBuffer();// if you don't call this, the context won't be created
                return;
            }
            catch (Exception ex)
            {
                Log.Verbose("ImGui", "{0}", ex);
            }
            throw new Exception("Can't load egl, aborting");
        }

        // This gets called on each frame render
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            ImGui.Application.RunLoop(mainForm);

            SwapBuffers();
        }
    }
}
