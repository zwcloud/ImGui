namespace IMGUI
{
    public class Tree<T>
    {
        private TreeNode<T> root;
        public TreeNode<T> Root { get { return root; } }

        public Tree(T t)
        {
            root = new TreeNode<T>(t);
        }
    }
}