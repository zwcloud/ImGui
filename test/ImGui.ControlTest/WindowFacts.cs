using Xunit;

namespace ImGui.ControlTest
{
    public class Window
    {
        [Fact]
        public void CreateAWindow()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm(new Rect(320, 180, 800, 600));
            bool open = true;
            form.OnGUIAction = () =>
            {
                GUI.Begin("test window", ref open, new Point(100, 100), new Size(100, 100));
                GUI.End();
            };

            Application.Run(form);
        }

        [Fact]
        public void DrawOverlayOnForeground()
        {
            Rendering.PathGeometryBuilder g = new Rendering.PathGeometryBuilder();

            g.BeginPath();
            g.MoveTo(50, 140);
            g.LineTo(150, 60);
            g.LineTo(250, 140);
            g.ClosePath();
            g.Stroke();
            var geometry = g.ToGeometry();

            Rendering.Pen pen = new Rendering.Pen(Color.LightGreen, 2);

            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm(new Rect(320, 180, 800, 600));
            form.OnGUIAction = () =>
            {
                form.uiContext.ForegroundDrawingContext.DrawGeometry(null, pen, geometry);
            };

            Application.Run(form);
        }
        
        [Fact]
        public void WindowOverflowByManyButtons()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm(new Rect(320, 180, 800, 600));
            bool open = true;
            form.OnGUIAction = () =>
            {
                GUI.Begin("test window", ref open, new Point(100, 100), new Size(100, 100));
                GUILayout.Button("Button 0");
                GUILayout.Button("Button 1");
                GUILayout.Button("Button 2");
                GUILayout.Button("Button 3");
                GUILayout.Button("Button 4");
                GUILayout.Button("Button 5");
                GUILayout.Button("Button 6");
                GUILayout.Button("Button 7");
                GUILayout.Button("Button 8");
                GUILayout.Button("Button 9");
                GUILayout.Button("Button 10");
                GUILayout.Button("Button 11");
                GUILayout.Button("Button 12");
                GUILayout.Button("Button 13");
                GUILayout.Button("Button 14");
                GUILayout.Button("Button 15");
                GUILayout.Button("Button 16");
                GUILayout.Button("Button 17");
                GUILayout.Button("Button 18");
                GUILayout.Button("Button 19");
                GUI.End();
            };

            Application.Run(form);
        }

        [Fact]
        public void WindowOverflowByListBox()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm(new Rect(320, 180, 800, 600));
            bool open = true;
            string[] listBoxItems = { "Apple", "Banana", "Cherry", "Kiwi", "Mango", "Orange", "Pineapple", "Strawberry", "Watermelon" };
            int currentListBoxItem = 0;
            form.OnGUIAction = () =>
            {
                GUI.Begin("test window", ref open, new Point(100, 100), new Size(200, 100));
                GUILayout.Button("Button 0");
                currentListBoxItem = GUILayout.ListBox("listbox\n(single select)", listBoxItems, currentListBoxItem);
                GUILayout.Button("Button 1");
                GUI.End();
            };

            Application.Run(form);
        }
    }
}
