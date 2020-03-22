using ImGui.Rendering;

namespace ImGui.UnitTest
{
    public class NodeRenderingFixture
    {
        public NodeRenderingFixture()
        {
            //mark as running unit tests
            Application.IsRunningInUnitTest = true;

            //use box-model for all nodes
            Node.DefaultUseBoxModel = true;

            //reset the style for rendering the rectangle of a node
            GUIStyle.Default.BackgroundColor = Color.White;
            GUIStyle.Default.Border = (1, 1, 1, 1);
            GUIStyle.Default.BorderColor = Color.Black;
            GUIStyle.Default.Padding = (1, 1, 1, 1);
            GUIStyle.Default.CellSpacing = (1, 1);
        }
    }
}