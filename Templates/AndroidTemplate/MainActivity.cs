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
    public class MainActivity : NativeActivity
    {
        GLView1 view;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

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

