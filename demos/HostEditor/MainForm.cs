using ImGui;
using System.Collections.Generic;
using System;

namespace HostEditor
{
    public class MainForm : Form
    {
        public MainForm() : base(new Rect(60, 60, 500, 600))
        {
        }

        bool open = true;

        List<string> fileLines = new List<string>();

        protected override void OnGUI()
        {
            GUILayout.Begin("HostEditor", ref open);
            if(GUILayout.Button("Load host file"))
            {
                LoadHost();
            }
            for (int i = 0; i < fileLines.Count; i++)
            {
                fileLines[i] = GUILayout.Textbox("hostline"+i, new Size(400, 20), fileLines[i]);
            }
            GUILayout.End();
        }

        private void LoadHost()
        {
            fileLines.Clear();
            var hostFileContent = System.IO.File.ReadAllLines(@"C:\Windows\System32\drivers\etc\hosts");
            fileLines.AddRange(hostFileContent);
        }
    }
}