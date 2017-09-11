using ImGui;
using ImGui.Common.Primitive;

namespace HostEditor
{
    public class MainForm : Form
    {
        public MainForm() : base(new Rect(320, 180, 1280, 720))
        {
        }

        bool open = true;
        string filePath = "";
        string fileContent = "";
        bool fileOpened = false;

        protected override void OnGUI()
        {
            GUI.Begin("TextPad", ref open, (10, 10), (800, 600));
            GUILayout.Label("Only *.txt file smaller than 1KB are supported.");
            if(!fileOpened)
            {
                filePath = GUILayout.InputText("Path", filePath);
            }
            else
            {
                GUILayout.PushFontColor(Color.TextDisabled);
                GUILayout.InputText("Path", filePath);
                GUILayout.PopStyleVar();
            }

            GUILayout.BeginHorizontal("CommandButtons");
            bool requestOpen = GUILayout.Button("Open");
            bool requestSave = GUILayout.Button("Save");
            bool requestClose = GUILayout.Button("Close");
            GUILayout.EndHorizontal();

            if(requestOpen)
            {
                if (filePath.EndsWith(".txt") && System.IO.File.Exists(filePath))
                {
                    if(new System.IO.FileInfo(filePath).Length < 10000)
                    {
                        fileContent = System.IO.File.ReadAllText(filePath);
                    }
                    fileOpened = true;
                }
            }
            if (requestSave && fileOpened)
            {
                System.IO.File.WriteAllText(filePath, fileContent);
            }
            if (requestClose && fileOpened)
            {
                filePath = "";
                fileContent = "";
                fileOpened = false;
            }

            GUILayout.PushHStretchFactor(1);
            fileContent = GUILayout.TextBox("file content textbox", new Size(400, 400), fileContent);
            GUILayout.PopStyleVar();

            GUI.End();
        }

    }
}