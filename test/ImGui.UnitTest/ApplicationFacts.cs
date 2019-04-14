using Xunit;
using ImGui;


namespace ImGui.UnitTest
{
    public class ApplicationFacts
    {
        public class TheRunMethod
        {
            /// <summary>
            /// demonstrate the application entry point
            /// </summary>
            [Fact]
            public void ProgramMain()
            {
                Application.Init();
                Application.Run(new MainForm());
            }
        }
    }
}