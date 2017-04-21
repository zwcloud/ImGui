using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using Android.Util;
using ImGui;

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
        MainView view;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            ImGui.Application.FontFileRead = this.Assets.Open;


            ImGui.Application.Init();

            // Create our OpenGL view, and display it
            view = new MainView(this);
            SetContentView(view);
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

        public override bool OnTouchEvent(MotionEvent e)
        {
            switch (e.ActionMasked)
            {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    {
                        var x = e.RawX;
                        var y = e.RawY;
                        Input.Mouse.MousePos = new Point(x, y);
                        Input.Mouse.LeftButtonState = InputState.Down;
                    }
                    break;
                case MotionEventActions.Move:
                    {
                        var x = e.RawX;
                        var y = e.RawY;
                        Input.Mouse.MousePos = new Point(x, y);
                    }
                    break;
                case MotionEventActions.Up:
                case MotionEventActions.PointerUp:
                case MotionEventActions.Outside:
                    {
                        var x = e.RawX;
                        var y = e.RawY;
                        Input.Mouse.MousePos = new Point(x, y);
                        Input.Mouse.LeftButtonState = InputState.Up;
                    }
                    break;
            }
            return base.OnTouchEvent(e);
        }
    }
}

