using System.Threading;
using ImGui.Common.Primitive;
using ImGui.Input;
using ImGui.OSImplentation.Windows;
using ImGui.Core;
using Xunit;

namespace ImGui.UnitTest
{
    public class KeyboardFacts
    {
        public class TheKeypressedMethod
        {
            [Fact]
            public void KeypressedIsWorking()
            {
                //Util.CheckEchoLogger();

                Application.IsRunningInUnitTest = true;
                Application.InitSysDependencies();

                var window = new Win32Window();
                window.Init(new Point(100, 100), new Size(300, 400), WindowTypes.Regular);

                var renderer = new Win32OpenGLRenderer();
                renderer.Init(window.Pointer, window.ClientSize);

                window.Show();

                while (true)
                {
                    Time.OnFrameBegin();
                    Keyboard.Instance.OnFrameBegin();

                    window.MainLoop(() =>
                    {
                        if (Keyboard.Instance.KeyPressed(Key.Space))
                        {
                            Log.Msg("Key.Space Pressed");
                        }
                        
                        Log.Msg("Key.Space "+Keyboard.Instance.keyStates[(int) Key.Space] + ", last Key.Space " + Keyboard.Instance.lastKeyStates[(int) Key.Space]);

                        if (Keyboard.Instance.KeyDown(Key.Escape))
                        {
                            Application.Quit();
                        }
                    });
                    
                    if (Application.RequestQuit)
                    {
                        break;
                    }

                    Thread.Sleep(16);
                    
                    Keyboard.Instance.OnFrameEnd();
                    Time.OnFrameEnd();
                }
            }
        }
    }
}