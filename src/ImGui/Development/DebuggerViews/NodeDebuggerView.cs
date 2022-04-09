using System.Collections.Generic;
using ImGui.Rendering;

namespace ImGui.Development.DebuggerViews
{
    internal class NodeDebuggerView
    {
        private readonly Node node;
        public NodeDebuggerView(Node node)
        {
            this.node = node;
        }
        
        public List<Visual> Children => this.node.Children;
        public Visual Parent => this.node.Parent;
        public Rect Rect => node.Rect;
        public StyleRuleSet RuleSet => node.RuleSet;
    }
}