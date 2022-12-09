using ImGui;
var demo = new Demo();
Application.Run(new Form(new Rect(320, 180, 1280, 720)), () =>
{
    demo.OnGUI();
    ImGui.GUILayout.Label("Hello, ImGui!");
});
