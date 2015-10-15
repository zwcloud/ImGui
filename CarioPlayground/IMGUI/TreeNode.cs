using System.Collections.Generic;

namespace IMGUI
{
    public class TreeNode<T>
    {
        private readonly List<TreeNode<T>> children = new List<TreeNode<T>>();

        public TreeNode(T t)
        {
            Data = t;
        }

        public T Data { get; set; }

        public List<TreeNode<T>> Children
        {
            get { return children; }
        }

        public void AddChild(T t)
        {
            Children.Add(new TreeNode<T>(t));
        }
    }
}