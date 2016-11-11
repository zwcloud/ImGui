using ImGui;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Test
{
    [TestClass]
    public class ClipTest
    {
        [TestMethod, TestCategory("layout"), Description("Show an item clipped by the content box of a group.")]
        public void ShowAnItemClippedByTheContentBoxOfAGroup()
        {
            Group group = new Group(false, GUILayout.Width(400), GUILayout.Height(40));
            Item item = new Item(GUILayout.Width(20), GUILayout.Height(20));
            group.Add(item);

            group.CalcWidth();
            group.CalcHeight();
            group.SetX(0);
            group.SetY(0);

            group.ShowResult();
        }

        [TestMethod, TestCategory("layout"), Description("Show a 3 layer group")]
        public void ShowA3LayerGroup()
        {
            Group group1 = new Group(true, GUILayout.Width(600), GUILayout.Height(600));
            Group group3 = new Group(false, GUILayout.Width(100), GUILayout.Height(100));
            Group group5 = new Group(true, GUILayout.Width(100), GUILayout.Height(200));
            Item item1 = new Item(GUILayout.Width(50), GUILayout.Height(50));
            group1.Add(group3);
            group3.Add(group5);
            group3.Add(item1.Clone());

            group1.CalcWidth();
            group1.CalcHeight();
            group1.SetX(0);
            group1.SetY(0);

            group1.ShowResult();
        }
    }
}