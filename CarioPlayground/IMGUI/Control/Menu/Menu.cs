
namespace ImGui
{
    public class Menu
    {



        private ITree<MenuItem> menuTree = NodeTree<MenuItem>.NewTree();
        private INode<MenuItem> topItem;
        private INode<MenuItem> currentItem;

        public Menu(string name, BaseForm form, string text, Rect rect)
        {
            //topItem = menuTree.AddChild(MenuItem.Dummy);
        }

        void BeginGroup(string name, BaseForm form, string text, Rect rect)
        {
            if(currentItem == null)
            {
                currentItem = topItem.AddChild(new MenuItem(name, form, text, rect));
                //create window for this menu
            }
            else
            {
                currentItem = currentItem.AddChild(new MenuItem(name, form, text, rect));
            }
        }

        void EndGroup()
        {
            currentItem = currentItem.Parent;
        }

        void AddItem(string name, BaseForm form, string text, Rect rect)
        {
            if (currentItem == null)
            {
                throw new System.InvalidOperationException();
            }
            else
            {
                currentItem.AddChild(new MenuItem(name, form, text, rect));
            }
        }
    }
}