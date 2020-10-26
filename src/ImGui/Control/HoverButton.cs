using ImGui.Rendering;
using ImGui.Style;

namespace ImGui
{
    public partial class GUI
    {
        /// <summary>
        /// Create a button that will be actived when the mouse is over it.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="text">text to display on the control</param>
        /// <returns>whether it is activated (the mouse is over it)</returns>
        public static bool HoverButton(Rect rect, string text)
        {
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            //get or create the root node
            var id = window.GetID(text);
            var container = window.AbsoluteVisualList;
            var node = (Node)container.Find(visual => visual.Id == id);
            text = Utility.FindRenderedText(text);
            if (node == null)
            {
                //create button node
                node = new Node(id, $"HoverButton<{text}>");
                container.Add(node);
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Button]);
            }
            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(rect);

            // interact
            GUIBehavior.ButtonBehavior(node.Rect, node.Id, out var hovered, out var held);

            // interact
            bool result = false;
            var uiState = Form.current.uiContext;
            uiState.KeepAliveID(id);
            if (hovered)
            {
                uiState.SetHoverID(id);
                if (uiState.ActiveId == 0)
                {
                    result = true;
                }
            }
            else
            {
                if (uiState.ActiveId == id)
                {
                    uiState.SetActiveID(0);
                }
                result = false;
            }
            var state = GUI.Normal;
            if (uiState.ActiveId == id)
            {
                state = GUI.Active;
            }
            node.State = state;
            
            // last item state
            window.TempData.LastItemState = node.State;

            // draw
            using (var dc = node.RenderOpen())
            {
                dc.DrawBoxModel(text, node.RuleSet, node.Rect);
            }

            return result;
        }
    }

    public partial class GUILayout
    {
        /// <summary>
        /// Create an auto-layout button that will be actived when the mouse is over it.
        /// </summary>
        /// <param name="text">text to display on the button</param>
        /// <param name="options">layout options that specify layouting properties. See also <see cref="GUILayout.Width"/>, <see cref="GUILayout.Height"/>, <see cref="GUILayout.ExpandWidth"/>, <see cref="GUILayout.ExpandHeight"/>, <see cref="GUILayout.StretchWidth"/>, <see cref="GUILayout.StretchHeight"/></param>
        /// <returns>whether it is activated (the mouse is over it)</returns>
        public static bool HoverButton(string text, LayoutOptions? options)
        {
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            //get or create the root node
            var id = window.GetID(text);
            var container = window.RenderTree.CurrentContainer;
            var node = container.GetNodeById(id);
            text = Utility.FindRenderedText(text);
            if (node == null)
            {
                //create button node
                node = new Node(id, $"HoverButton<{text}>");
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Button]);
                var size = node.RuleSet.CalcContentBoxSize(text, GUIState.Normal);
                node.AttachLayoutEntry(size);
                node.UseBoxModel = true;
            }
            container.AppendChild(node);
            node.RuleSet.ApplyOptions(options);
            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(id);

            // interact
            GUIBehavior.ButtonBehavior(node.Rect, node.Id, out var hovered, out var held);

            bool result = false;
            // interact
            var uiState = Form.current.uiContext;
            uiState.KeepAliveID(id);
            if (hovered)
            {
                uiState.SetHoverID(id);
                if (uiState.ActiveId == 0)
                {
                    uiState.SetActiveID(id);
                    result = true;
                }
            }
            else
            {
                if (uiState.ActiveId == id)
                {
                    uiState.SetActiveID(0);
                }
                result = false;
            }
            var state = GUI.Normal;
            if (uiState.ActiveId == id)
            {
                state = GUI.Active;
            }
            node.State = state;
            
            // last item state
            window.TempData.LastItemState = node.State;

            // draw
            using (var dc = node.RenderOpen())
            {
                dc.DrawBoxModel(text, node.RuleSet, node.Rect);
            }

            return result;
        }

        public static bool HoverButton(string text) => HoverButton(text, null);
    }
}
