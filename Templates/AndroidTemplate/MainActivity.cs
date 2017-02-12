using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Util;

namespace AndroidTemplate
{
    [Activity(Label = "AndroidTemplate",
        MainLauncher = true,
        Icon = "@drawable/icon",
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.KeyboardHidden
#if __ANDROID_11__
		,HardwareAccelerated=false
#endif
        )]
    public class MainActivity : Activity
    {
        GLView1 view;

        private Action<float, float, bool> inputEventHandler;

        public override bool OnTouchEvent(MotionEvent e)
        {
            switch (e.ActionMasked)
            {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    {
                        var x = e.RawX;
                        var y = e.RawY;
                        this.inputEventHandler(x, y, true);
                    }
                    break;
                case MotionEventActions.Move:
                    break;
                case MotionEventActions.Up:
                case MotionEventActions.PointerUp:
                case MotionEventActions.Outside:
                    {
                        var x = e.RawX;
                        var y = e.RawY;
                        this.inputEventHandler(x, y, false);
                    }
                    break;
            }
            return base.OnTouchEvent(e);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            ImGui.Application.Init();
            this.inputEventHandler = ImGui.Application.inputEventHandler;

            // Create our OpenGL view, and display it
            view = new GLView1(this);
            SetContentView(view);
            
            //Log.Info("", "Environment Variables");
            //var envars = System.Environment.GetEnvironmentVariables();
            //var varEnumerator =
            //    envars.GetEnumerator();
            //while (varEnumerator.MoveNext())
            //{
            //    Log.Info("","\t{0}={1}",
            //        varEnumerator.Key,
            //        varEnumerator.Value);
            //}
        }

        protected override void OnPause()
        {
            base.OnPause();
            view.Pause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            view.Resume();
        }
    }
}

