using ImGui.Input;
using ImGui.Rendering;
using ImGui.Style;

namespace ImGui
{
    public partial class GUI
    {
        /// <summary>
        /// Create a button that acts like a toggle.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="text">text to display on the button</param>
        /// <param name="selected">whether this selectable is selected</param>
        /// <returns>new value of the toggle-button</returns>
        public static bool Selectable(Rect rect, string text, bool selected)
        {
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            //get or create the root node
            var id = window.GetID(text);
            var container = window.AbsoluteVisualList;
            var node = (Node)container.Find(visual => visual.Id == id);
            var renderedText = Utility.FindRenderedText(text);
            if (node == null)
            {
                node = new Node(id, $"Toggle<{text}>");
                container.Add(node);
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Selectable]);
            }

            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(rect);

            // interact
            selected = GUIBehavior.SelectableBehavior(node.Rect, id, selected, out bool hovered, out bool held);
            
            node.State = selected ? GUIState.Active : GUIState.Normal;

            if (hovered)
            {
                node.State = GUIState.Hover;
            }

            if (held)
            {
                node.State = GUIState.Active;
            }
            
            // last item state
            window.TempData.LastItemState = node.State;

            // render
            using (var dc = node.RenderOpen())
            {
                dc.DrawBoxModel(renderedText, node.RuleSet, node.Rect);
            }

            return selected;
        }
    }

    public partial class GUILayout
    {
        /// <summary>
        /// Create an auto-layout button that acts like a toggle.
        /// </summary>
        /// <param name="text">text to display on the button</param>
        /// <param name="selected">whether this selectable is selected</param>
        /// <param name="options">layout options that specify layouting properties. See also <see cref="GUILayout.Width"/>, <see cref="GUILayout.Height"/>, <see cref="GUILayout.ExpandWidth"/>, <see cref="GUILayout.ExpandHeight"/>, <see cref="GUILayout.StretchWidth"/>, <see cref="GUILayout.StretchHeight"/></param>
        /// <returns>new value of the toggle-button</returns>
        public static bool Selectable(string text, bool selected = false, LayoutOptions? options = null)
        {
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            //get or create the root node
            var id = window.GetID(text);
            var container = window.RenderTree.CurrentContainer;
            var node = container.GetNodeById(id);
            var renderedText = Utility.FindRenderedText(text);
            if (node == null)
            {
                node = new Node(id, $"Selectable<{text}>");
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Selectable]);
                var size = node.RuleSet.CalcContentBoxSize(text, GUIState.Normal);
                node.AttachLayoutEntry(size);
            }
            container.AppendChild(node);

            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(id);

            // interact
            selected = GUIBehavior.SelectableBehavior(node.Rect, id, selected, out bool hovered, out bool held);

            node.State = selected ? GUIState.Active : GUIState.Normal;

            if (hovered)
            {
                node.State = GUIState.Hover;
            }

            if (held)
            {
                node.State = GUIState.Active;
            }
            
            // last item state
            window.TempData.LastItemState = node.State;

            // render
            using (var dc = node.RenderOpen())
            {
                dc.DrawBoxModel(renderedText, node.RuleSet, node.Rect);
            }

            return selected;
        }
    }

    internal partial class GUIBehavior
    {
        public static bool SelectableBehavior(Rect rect, int id, bool selected, out bool out_hovered, out bool out_held)
        {
            GUIContext g = Form.current.uiContext;

            bool hovered = g.IsHovered(rect, id);
            if (hovered)
            {
                g.SetHoverID(id);

                if (Mouse.Instance.LeftButtonPressed)
                {
                    g.SetActiveID(id);
                }

                if (g.ActiveId == id && Mouse.Instance.LeftButtonReleased)
                {
                    selected = !selected;
                    g.SetActiveID(0);
                }
            }
            
            bool held = false;
            if (g.ActiveId == id)
            {
                if (Mouse.Instance.LeftButtonState == KeyState.Down)
                {
                    held = true;
                }
            }

            out_hovered = hovered;
            out_held = held;

            return selected;
        }
    }

    internal partial class GUISkin
    {
        private void InitSelectableStyles(StyleRuleSet ruleSet)
        {
            var builder = new StyleRuleSetBuilder(ruleSet);
            builder.Padding(2.0)
                .BackgroundColor(Color.Clear, GUIState.Normal)
                .BackgroundColor(new Color(0.26f, 0.59f, 0.98f, 0.80f), GUIState.Hover)
                .BackgroundColor(new Color(0.26f, 0.59f, 0.98f, 1.00f), GUIState.Active);
        }
    }
}
