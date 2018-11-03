using System;

namespace ImGui.UnitTest
{
    public class ApplicationFixture
    {
        public ApplicationFixture()
        {
            Application.IsRunningInUnitTest = true;
            Application.InitSysDependencies();
        }
    }
}