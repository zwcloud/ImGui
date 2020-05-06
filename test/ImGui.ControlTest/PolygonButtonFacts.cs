using Xunit;

namespace ImGui.ControlTest
{
    public class PolygonButton
    {
        [Fact]
        public void ShowOneFixedPolygonButton()
        {
            Application.IsRunningInUnitTest = true;
            Application.InitialDebugWindowRect = new Rect(80, 80, 200, 240);
            Application.Init();

            var form = new MainForm(400, 400);
            var polygon = new[]
            {
                    new Point( 30, -52),
                    new Point(-30, -52),
                    new Point(-60,  0) ,
                    new Point(-30,  52),
                    new Point( 30,  52),
                    new Point( 60,  0)
                };
            for (var i = 0; i < polygon.Length; i++)
            {
                var point = polygon[i];
                polygon[i] = point + new Vector(65, 65);
            }
            var textRect = new Rect(new Point(-40, -20) + new Vector(65, 65), 80, 40);
            form.OnGUIAction = () =>
            {
                if (GUI.PolygonButton(new Rect(0, 0, 100, 100), polygon, textRect, "Apply"))
                {
                    Log.Msg("clicked");
                }
            };

            Application.Run(form);
        }

        [Fact]
        public void ShowTwoLayoutedPolygonButton()
        {
            Application.IsRunningInUnitTest = true;
            Application.InitialDebugWindowRect = new Rect(80, 80, 200, 300);
            Application.Init();

            var form = new MainForm(400, 400);
            var polygon = new[]
            {
                    new Point( 30, -52),
                    new Point(-30, -52),
                    new Point(-60,  0) ,
                    new Point(-30,  52),
                    new Point( 30,  52),
                    new Point( 60,  0)
                };
            for (var i = 0; i < polygon.Length; i++)
            {
                var point = polygon[i];
                polygon[i] = point + new Vector(65, 65);
            }
            var textRect = new Rect(new Point(-40, -20) + new Vector(65, 65), 80, 40);
            form.OnGUIAction = () =>
            {
                if (GUILayout.PolygonButton(polygon, textRect, "Button A"))
                {
                    Log.Msg("clicked A");
                }
                if (GUILayout.PolygonButton(polygon, textRect, "Button B"))
                {
                    Log.Msg("clicked B");
                }
            };

            Application.Run(form);
        }
    }
}
