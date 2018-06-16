using System.Collections.Generic;
using System.Threading;
using Xunit;
using ImGui;
using ImGui.Common.Primitive;
using ImGui.Core;
using ImGui.GraphicsAbstraction;
using ImGui.GraphicsImplementation;
using ImGui.Input;
using ImGui.OSImplentation.Windows;
using ImGui.Rendering;

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
    }
}