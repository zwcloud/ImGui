using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES30;
using OpenTK.Platform;
using OpenTK.Platform.Android;
using Android.Views;
using Android.Content;
using Android.Util;

namespace AndroidTemplate
{
    partial class GLView1 : AndroidGameView
    {
        private MainForm mainForm;

        public GLView1(Context context) : base(context)
        {
        }

        // This gets called when the drawing surface is ready
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Create form
            mainForm = new MainForm(IntPtr.Zero/*dummy*/, ImGui.Point.Zero/*dummy*/, new ImGui.Size(this.Size.Width, this.Size.Height));
            ImGui.Application.Init(mainForm);

            // Run the render loop
            Run();
        }

        // This method is called everytime the context needs
        // to be recreated. Use it to set any egl-specific settings
        // prior to context creation
        //
        // In this particular case, we demonstrate how to set
        // the graphics mode and fallback in case the device doesn't
        // support the defaults
        protected override void CreateFrameBuffer()
        {
            this.GLContextVersion = GLContextVersion.Gles3_0;
            // the default GraphicsMode that is set consists of (16, 16, 0, 0, 2, false)
            try
            {
                Log.Verbose("ImGui", "Loading with default settings");
                base.CreateFrameBuffer();// if you don't call this, the context won't be created
                return;
            }
            catch (Exception ex)
            {
                Log.Verbose("ImGui", "{0}", ex);
            }

            // this is a graphics setting that sets everything to the lowest mode possible so
            // the device returns a reliable graphics setting.
            try
            {
                Log.Verbose("ImGui", "Loading with custom Android settings (low mode)");
                GraphicsMode = new AndroidGraphicsMode(0, 0, 0, 0, 0, false);
                base.CreateFrameBuffer();// if you don't call this, the context won't be created
                return;
            }
            catch (Exception ex)
            {
                Log.Verbose("GLCube", "{0}", ex);
            }
            throw new Exception("Can't load egl, aborting");
        }

        // This gets called on each frame render
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.ClearColor(0.5f, 0.5f, 0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            ImGui.Application.RunLoop(mainForm);

            SwapBuffers();
        }
    }
}
