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
                form.ForegroundDrawingContext.DrawGeometry(null, pen, geometry);
            };

            Application.Run(form);
        }
    }
}
