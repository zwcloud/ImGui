using ImGui;

namespace HostEditor
{
    public class TextPad
    {
        private bool open = true;
        private string filePath = "";
        private string fileContent = "";
        private bool fileOpened = false;

        public void OnGUI()
        {
            GUI.Begin("TextPad", ref open, (10, 10), (800, 600));
            GUILayout.Label("Only *.txt file smaller than 1KB are supported.");
            if(!fileOpened)
            {
                filePath = GUILayout.InputText("Path", filePath);
            }
            else
            {
                GUILayout.InputText("Path", filePath);
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

            fileContent = GUILayout.TextBox("file content textbox", new Size(400, 400), fileContent);

            GUI.End();
        }
    }
}