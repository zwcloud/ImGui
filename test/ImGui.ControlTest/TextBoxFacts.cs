using ImGui;
using Xunit;

namespace ImGui.ControlTest
{
    public class TextBoxTest
    {
        public TextBoxTest()
        {
            Application.IsRunningInUnitTest = true;
            Application.InitSysDependencies();
        }

        [Fact]
        public void TestBoxWorksCorrectly()
        {
            var text = "Hello ImGui!你好";

            Application.Run(new MainForm(() =>
            {
                text = GUILayout.TextBox("Name", new Size(200, 30), text);
            }));
        }
        
        [Fact]
        public void TestBoxLayoutCorrectly()
        {
            string str0 = "Hello, world!";
            int i0 = 123;
            float f0 = 0.001f;
            Application.Run(new MainForm(() =>
            {
                str0 = GUILayout.InputText("input text", str0);
                i0 = GUILayout.InputInt("input int", i0);
                f0 = GUILayout.InputFloat("input float", f0);
            }));
        }
        
        [Fact]
        public void EmptyTextBoxSizeIsCorrect()
        {
            string str0 = "";
            Application.Run(new MainForm(() =>
            {
                str0 = GUILayout.InputText("default", str0);
            }));
        }

        [Fact]
        public void MultipleLineTextBoxWorksNormally()
        {
            string multiLineText = @"/*\n
 The Pentium F00F bug, shorthand for F0 0F C7 C8,
 the hexadecimal encoding of one offending instruction,
 more formally, the invalid operand with locked CMPXCHG8B
 instruction bug, is a design flaw in the majority of
 Intel Pentium, Pentium MMX, and Pentium OverDrive
 processors (all in the P5 microarchitecture).
*/

label:
\tlock cmpxchg8b eax
";
            Application.Run(new MainForm(800, 600, () =>
            {
                multiLineText = GUILayout.InputTextMultiline(
                    "Text Box", new Size(200, 200), multiLineText);
            }));
        }

        [Fact]
        public void MultipleLineTextBoxScrollNormally()
        {
            string multiLineText = @"Line0
Line1
Line2
Line3
Line4
Line5
Line6
Line7
Line8
Line9
Line10
Line11
Line12
Line13
Line14
Line15
Line16
Line17
Line18
Line19
";
            Application.InitialDebugWindowRect = new Rect(10, 10, 300, 400);
            Application.Run(new MainForm(800, 800, () =>
            {
                multiLineText = GUILayout.InputTextMultiline(
                    "Text Box", new Size(200, 200), multiLineText);
            }));
        }
    }

}
