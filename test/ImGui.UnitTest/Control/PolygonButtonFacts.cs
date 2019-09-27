using Xunit;

namespace ImGui.UnitTest
{
    public partial class GUIFacts
    {
        public class ThePolygonButtonMethod
        {
            [Fact]
            public void ShowOneFixedPolygonButton()
            {
                Application.IsRunningInUnitTest = true;
                Application.Init();

                var form = new MainForm();
                var polygon = new []
                {
                    new Point(30, -52),
                    new Point(-30, -52),
                    new Point(-60, 7),
                    new Point(-30, 52),
                    new Point(30, 52),
                    new Point(60, 0)
                };
                for (var i = 0; i < polygon.Length; i++)
                {
                    var point = polygon[i];
                    polygon[i] = point + new Vector(65, 65);
                }
                var textRect = new Rect(new Point(-40, -40) + new Vector(65, 65), 80, 80);
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
            public void ShowOneLayoutedPolygonButton()
            {
                Application.IsRunningInUnitTest = true;
                Application.Init();

                var form = new MainForm();
                var polygon = new[]
                {
                    new Point(30, -52),
                    new Point(-30, -52),
                    new Point(-60, 7),
                    new Point(-30, 52),
                    new Point(30, 52),
                    new Point(60, 0)
                };
                for (var i = 0; i < polygon.Length; i++)
                {
                    var point = polygon[i];
                    polygon[i] = point + new Vector(65, 65);
                }
                var textRect = new Rect(new Point(-40, -40) + new Vector(65, 65), 80, 80);
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
}