using ImGui.Rendering;
using System;

namespace ImGui
{
    public partial class GUILayout
    {
        public static bool TreeNode<T>(T obj, string text) where T: class
        {
            Window window = GetCurrentWindow();
            int id = window.GetID(obj);
            Storage storage = window.StateStorage;

            var open = storage.GetBool(id);
            DoTreeNode(id, text, ref open);
            storage.SetBool(id, open);
            return open;
        }

        public static bool TreeNode(string text, ref bool open)
        {
            Window window = GetCurrentWindow();
            int id = window.GetID(text);
            DoTreeNode(id, text, ref open);
            return open;
        }

        public static bool TreeNode(string text)
        {
            Window window = GetCurrentWindow();
            int id = window.GetID(text);
            Storage storage = window.StateStorage;
            var open = storage.GetBool(id);
            DoTreeNode(id, text, ref open);
            storage.SetBool(id, open);
            return open;
        }

        internal static bool DoTreeNode(int id, string text, ref bool open)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            window.TempData.LastItemId = id;

            //get or create the root node
            var container = window.RenderTree.CurrentContainer;
            var node = container.GetNodeById(id);
            text = Utility.FindRenderedText(text);
            if (node == null)
            {
                //create nodes
                node = new Node(id, $"TreeNode<{text}>");
                node.AttachLayoutEntry();
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.TreeNode]);
            }
            container.AppendChild(node);
            var lineHeight = node.RuleSet.GetLineHeight();
            node.RuleSet.ApplyOptions(Height(lineHeight));
            node.ActiveSelf = true;

            // rect
            Rect rect = window.GetRect(id);

            // interact
            var pressed = GUIBehavior.ButtonBehavior(rect, id, out var hovered, out var held, ButtonFlags.PressedOnClickRelease);
            node.State = (hovered && held) ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;
            if (pressed)
            {
                open = !open;
            }

            // last item state
            window.TempData.LastItemState = node.State;

            using (var dc = node.RenderOpen())
            {
                dc.DrawRectangle(new Brush(node.RuleSet.BackgroundColor), null, rect);
                dc.DrawGlyphRun(node.RuleSet, text, node.ContentRect.TopLeft + new Vector(rect.Height + node.PaddingLeft, 0));
                dc.RenderArrow(rect.Min + new Vector(node.RuleSet.PaddingTop, lineHeight * 0.15),
                    node.Height, node.RuleSet.FontColor, open ? Internal.Direcion.Down : Internal.Direcion.Right, 0.7);
            }

            if (open)
            {
                window.CheckStackSize(id, true);
                PushID(id);

                BeginHorizontal("#TreeContent");
                Space("Space", 20);
                BeginVertical("#Items");

                var cpId = HashCode.Combine(id, 23);
                window.CheckStackSize(cpId, true);
                PushID(cpId);
            }

            return open;
        }

        public static void TreePop()
        {
            var window = GetCurrentWindow();
            var cpId = PopID();
            window.CheckStackSize(cpId, false);

            EndVertical();
            EndHorizontal();

            var poppedId = PopID();
            window.CheckStackSize(poppedId, false);
        }
    }

    internal partial class GUISkin
    {
        private void InitTreeNodeStyles(StyleRuleSet button, out StyleRuleSet ruleSet)
        {
            ruleSet = new StyleRuleSet();
            ruleSet.Border = (1, 1, 1, 1);
            ruleSet.Padding = (1, 1, 1, 1);
            ruleSet.Set(StylePropertyName.BackgroundColor, Color.Clear, GUIState.Normal);
            ruleSet.Set(StylePropertyName.BackgroundColor, new Color(0.26f, 0.59f, 0.98f, 0.60f), GUIState.Hover);
            ruleSet.Set(StylePropertyName.BackgroundColor, new Color(0.26f, 0.59f, 0.98f, 1.00f), GUIState.Active);
            ruleSet.Set(StylePropertyName.HorizontalStretchFactor, 1, GUIState.Normal);
            ruleSet.Set(StylePropertyName.HorizontalStretchFactor, 1, GUIState.Hover);
            ruleSet.Set(StylePropertyName.HorizontalStretchFactor, 1, GUIState.Active);
        }
    }
}