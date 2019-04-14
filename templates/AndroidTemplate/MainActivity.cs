using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content.PM;
using ImGui;
using ImGui.Input;

namespace AndroidTemplate
{
    [Activity(Label = "AndroidTemplate",
        MainLauncher = true,
        Icon = "@drawable/icon",
        ConfigurationChanges = ConfigChanges.Orientation
#if __ANDROID_11__
		,HardwareAccelerated=false
#endif
        )]
    public class MainActivity : Activity
    {
        MainView view;

        private static TaskCompletionSource<string> tcs;
        private static AlertDialog alert;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            ImGui.Application.OpenAndroidAssets = this.Assets.Open;
            ImGui.Application.Init();

            // Set up callback to show keyboard and get input from it.
            //https://github.com/MonoGame/MonoGame/tree/7c3d6870a38f8a5e479e64d935d692f2610e1cda/MonoGame.Framework/Input#L24
            Keyboard.ShowCallback = (text) =>
            {
                tcs = new TaskCompletionSource<string>();

                RunOnUiThread(() =>
                {
                    alert = new AlertDialog.Builder(this).Create();

                    alert.SetTitle("Input");

                    var input = new EditText(this) { Text = text };

                    alert.SetView(input);

                    alert.SetButton((int)DialogButtonType.Positive, "Ok", (sender, args) =>
                    {
                        if (!tcs.Task.IsCompleted)
                            tcs.SetResult(input.Text);
                    });

                    alert.SetButton((int)DialogButtonType.Negative, "Cancel", (sender, args) =>
                    {
                        if (!tcs.Task.IsCompleted)
                            tcs.SetResult(text);
                    });

                    alert.CancelEvent += (sender, args) =>
                    {
                        if (!tcs.Task.IsCompleted)
                            tcs.SetResult(text);
                    };

                    alert.Show();
                });

                return tcs.Task;
            };

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
                        Mouse.Instance.Position = new Point(x, y);
                        Mouse.Instance.LeftButtonState = KeyState.Down;
                    }
                    break;
                case MotionEventActions.Move:
                    {
                        var x = e.RawX;
                        var y = e.RawY;
                        Mouse.Instance.Position = new Point(x, y);
                    }
                    break;
                case MotionEventActions.Up:
                case MotionEventActions.PointerUp:
                case MotionEventActions.Outside:
                    {
                        var x = e.RawX;
                        var y = e.RawY;
                        Mouse.Instance.Position = new Point(x, y);
                        Mouse.Instance.LeftButtonState = KeyState.Up;
                    }
                    break;
            }
            return base.OnTouchEvent(e);
        }
    }
}

