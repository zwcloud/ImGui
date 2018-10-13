using Xunit;
using ImGui.Input;

namespace ImGui.UnitTest
{
    public class FormFacts
    {
        public class TheGUILoopMethod
        {
            [Fact]
            public void RunGUILoop()
            {
                Application.IsRunningInUnitTest = true;
                Application.InitSysDependencies();

                var form = new MainForm();
                form.Show();

                while (!form.Closed)
                {
                    Time.OnFrameBegin();
                    Keyboard.Instance.OnFrameBegin();

                    if (Keyboard.Instance.KeyDown(Key.Escape))
                    {
                        Application.Quit();
                    }

                    form.MainLoop(form.GUILoop);
                    
                    if (Application.RequestQuit)
                    {
                        break;
                    }
                    
                    Keyboard.Instance.OnFrameEnd();
                    Time.OnFrameEnd();
                }
            }
        }

        public class TheSaveClientAreaToPngMethod
        {
            [Fact]
            public void Save()
            {
                Application.IsRunningInUnitTest = true;
                Application.InitSysDependencies();

                var form = new MainForm();
                form.Show();

                while (!form.Closed)
                {
                    Time.OnFrameBegin();
                    Keyboard.Instance.OnFrameBegin();

                    if (Keyboard.Instance.KeyDown(Key.Escape))
                    {
                        Application.Quit();
                    }
                    
                    if (Keyboard.Instance.KeyDown(Key.Space))
                    {
                        form.SaveClientAreaToPng(Util.OutputPath + "/1.png");
                    }

                    form.MainLoop(form.GUILoop);
                    
                    if (Application.RequestQuit)
                    {
                        break;
                    }
                    
                    Keyboard.Instance.OnFrameEnd();
                    Time.OnFrameEnd();
                }
            }
        }
    }
}