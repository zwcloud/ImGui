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
        MainView view;

        private Action<ImGui.InputType, float, float> inputEventHandler;

        public override bool OnTouchEvent(MotionEvent e)
        {
            switch (e.ActionMasked)
            {
                case MotionEventActions.Down:
                case MotionEventActions.PointerDown:
                    {
                        var x = e.RawX;
                        var y = e.RawY;
                        this.inputEventHandler(ImGui.InputType.TouchDown, x, y);
                    }
                    break;
                case MotionEventActions.Move:
                    {
                        var x = e.RawX;
                        var y = e.RawY;
                        this.inputEventHandler(ImGui.InputType.TouchMove, x, y);
                    }
                    break;
                case MotionEventActions.Up:
                case MotionEventActions.PointerUp:
                case MotionEventActions.Outside:
                    {
                        var x = e.RawX;
                        var y = e.RawY;
                        this.inputEventHandler(ImGui.InputType.TouchUp, x, y);
                    }
                    break;
            }
            return base.OnTouchEvent(e);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            ImGui.Application.AssetManager = this.Assets;

            ImGui.Application.Init();
            this.inputEventHandler = ImGui.Application.inputEventHandler;

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
    }
}

