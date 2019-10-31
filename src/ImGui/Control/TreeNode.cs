using ImGui.Rendering;
using System;

namespace ImGui
{
    public partial class GUILayout
    {
        public static bool TreeNode(string text, ref bool open)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            BeginVertical(text + "_Tree");

            //get or create the root node
            int id = window.GetID(text);
            var container = window.RenderTree.CurrentContainer;
            var node = container.GetNodeById(id);
            text = Utility.FindRenderedText(text);
            if (node == null)
            {
                //create nodes
                node = new Node(id, $"TreeNode<{text}>");
                node.AttachLayoutEntry();
                container.AppendChild(node);
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.TreeNode]);
            }
            var lineHeight = node.RuleSet.GetLineHeight();
            node.RuleSet.ApplyOptions(Height(lineHeight));
            node.ActiveSelf = true;

            // rect
            Rect rect = window.GetRect(id);

            // interact
            var pressed = GUIBehavior.ButtonBehavior(rect, id, out var hovered, out var held, ButtonFlags.PressedOnClick);
            node.State = (hovered && held) ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;
            if (pressed)
            {
                open = !open;
            }

            using (var dc = node.RenderOpen())
            {
                dc.DrawGlyphRun(node.RuleSet, text, node.ContentRect.TopLeft + new Vector(node.Rect.Height + node.PaddingLeft, 0));
                dc.RenderArrow(node.Rect.Min + new Vector(node.RuleSet.PaddingTop, lineHeight * 0.15),
                    node.Height, node.RuleSet.FontColor, open ? Internal.Direcion.Down : Internal.Direcion.Right, 0.7);
            }

                BeginHorizontal("#Content");
                    Space("Space", 20);
                    BeginVertical("#Items");
            return open;
        }

        public static void TreePop()
        {
                    EndVertical();
                EndHorizontal();
            EndVertical();
        }
    }

    internal partial class GUISkin
    {
        private void InitTreeNodeStyles(StyleRuleSet button, out StyleRuleSet ruleSet)
        {
            ruleSet = new StyleRuleSet();
            ruleSet.Replace(button);
            ruleSet.Set(StylePropertyName.BackgroundColor, new Color(0.26f, 0.59f, 0.98f, 0.31f), GUIState.Normal);
            ruleSet.Set(StylePropertyName.BackgroundColor, new Color(0.26f, 0.59f, 0.98f, 0.80f), GUIState.Hover);
            ruleSet.Set(StylePropertyName.BackgroundColor, new Color(0.26f, 0.59f, 0.98f, 1.00f), GUIState.Active);
            ruleSet.Set(StylePropertyName.HorizontalStretchFactor, 1, GUIState.Normal);
            ruleSet.Set(StylePropertyName.HorizontalStretchFactor, 1, GUIState.Hover);
            ruleSet.Set(StylePropertyName.HorizontalStretchFactor, 1, GUIState.Active);
        }
    }
}