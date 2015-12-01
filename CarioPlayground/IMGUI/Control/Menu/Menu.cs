
namespace IMGUI
{
    public class Menu
    {
        private ITree<MenuItem> menuTree = NodeTree<MenuItem>.NewTree();
        private INode<MenuItem> topItem;
        private INode<MenuItem> currentItem;

        public Menu()
        {
            topItem = menuTree.AddChild(new MenuItem());
        }

        void BeginGroup(string text)
        {
            if(currentItem == null)
            {
                currentItem = topItem.AddChild(new MenuItem());
            }
            else
            {
                currentItem = currentItem.AddChild(new MenuItem());
            }
        }

        void EndGroup()
        {
            currentItem = currentItem.Parent;
        }

        void AddItem(string text)
        {
            if (currentItem == null)
            {
                throw new System.InvalidOperationException();
            }
            else
            {
                currentItem.AddChild(new MenuItem());
            }
        }
    }
}