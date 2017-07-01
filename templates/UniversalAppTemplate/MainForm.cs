using ImGui;
using System.Diagnostics;

namespace UniversalAppTemplate
{
    public class MainForm : Form
    {
        public MainForm() : base(new Rect(60, 60, 500, 600))
        {
        }

        TestUI testUI = new TestUI();
        
        bool DebugWindow1Open = true;
        bool DebugWindow2Open = true;
        string text = @"line 1 Hello World
line 2 你好，世界
line 3 こんにちは 世界";
        string text1 = "ABC\nDEFG\n";
        string text3 = "新进展：\nTextBox支持多行文本\n文本选择还在写\n对了 这个是可以修改的\n";
        protected override void OnGUI()
        {
            GUILayout.Begin("TextEditor", ref DebugWindow2Open, new Point(10, 10), new Size(800, 500), 1, WindowFlags.Default);
            text3 = GUI.Textbox(new Rect(10, 10, 700, 400), "Text Box", text3);
            GUILayout.End();
            Debug.WriteLine("CaretIndex: {0}", TextBoxDebug.CaretIndex);
#if false
            var clientRect = GUILayout.GetWindowClientRect();
            GUILayout.BeginHorizontal("example_h2", GUIStyle.Default, GUILayout.Width(clientRect.Width), GUILayout.Height(clientRect.Height));
            {
                GUILayout.Button("Left", GUILayout.StretchWidth(1));
                GUILayout.BeginVertical("example_v1", GUIStyle.Default,
                    GUILayout.StretchWidth(1), GUILayout.Width(clientRect.Width), GUILayout.Height(clientRect.Height));
                {
                    GUILayout.Button("Middle");
                    GUILayout.Button("Center");
                    GUILayout.Button("Bottom");
                }
                GUILayout.EndVertical();
                GUILayout.Button("Right", GUILayout.StretchWidth(1));
            }
            GUILayout.EndHorizontal();

            GUILayout.Begin("Debug2", ref DebugWindow2Open, new Point(43, 256), new Size(400, 300), 1, WindowFlags.Default);
            testUI.OnGUI();
            GUILayout.End();
#endif
        }
    }
}