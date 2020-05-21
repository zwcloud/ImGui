using Xunit;

namespace ImGui.ControlTest
{
    public class Button
    {
        public Button()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();
        }

        [Fact]
        public void ShowOneFixedButton()
        {
            var form = new MainForm();
            form.OnGUIAction = () =>
            {
                if (GUI.Button(new Rect(0, 0, 100, 30), "Apply"))
                {
                    Log.Msg("clicked");
                }
            };

            Application.Run(form);
        }

        [Fact]
        public void ShowTwoFixedButtons()
        {
            var form = new MainForm();
            form.OnGUIAction = () =>
            {
                if (GUI.Button(new Rect(5, 5, 100, 30), "Button1"))
                {
                    Log.Msg("clicked Button1");
                }
                if (GUI.Button(new Rect(5, 50, 100, 40), "Button2"))
                {
                    Log.Msg("clicked Button2");
                }
            };

            Application.Run(form);
        }

        [Fact]
        public void ShowOneLayoutedButton()
        {
            var form = new MainForm();
            form.OnGUIAction = () =>
            {
                if (GUILayout.Button("Apply"))
                {
                    Log.Msg("clicked");
                }
            };

            Application.Run(form);
        }

        [Fact]
        public void ShowTwoLayoutedButton()
        {
            var form = new MainForm();
            form.OnGUIAction = () =>
            {
                if (GUILayout.Button("Apply"))
                {
                    Log.Msg("clicked Apply");
                }
                if (GUILayout.Button("Revert"))
                {
                    Log.Msg("clicked Revert");
                }
            };

            Application.Run(form);
        }

        [Fact]
        public void ShowOneLayoutedImageButton()
        {
            var form = new MainForm(800, 600);
            form.OnGUIAction = () =>
            {
                GUILayout.ImageButton("assets/images/logo.png");
            };

            Application.Run(form);
        }
        
        [Fact]
        public void ShowPartialImageButtons()
        {
            var form = new MainForm(800, 800);
            var pressed_count = 0;
            form.OnGUIAction = () =>
            {
                GUILayout.BeginHorizontal("HGroup~1");
                for (int i = 0; i < 8; i++)
                {
                    GUILayout.PushID(i);
                    if (GUILayout.ImageButton("assets/images/checker.png", new Size(32, 32),
                        new Vector(32 * i, 0)))
                    {
                        pressed_count += 1;
                    }
                    GUILayout.PopID();
                }
                GUILayout.EndHorizontal();
                GUILayout.Label("Pressed {0} times.", pressed_count);
                GUILayout.ImageButton("assets/images/checker.png");
            };
            
            Application.InitialDebugWindowRect = new Rect(10, 10, 600, 700);
            Application.Run(form);
        }

        [Fact]
        public void ShowOneLayoutedSlicedImageButton()
        {
            var form = new MainForm(800, 600);
            form.OnGUIAction = () =>
            {
                GUILayout.ImageButton("assets/images/button.png", (306, 456), (83, 53, 53, 53));
            };

            Application.Run(form);
        }
    }
}
