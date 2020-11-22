using ImGui.Rendering;
using ImGui.OSAbstraction.Graphics;

namespace ImGui
{
    public partial class GUI
    {
        /// <summary>
        /// Create an image.
        /// </summary>
        /// <param name="rect">position and size</param>
        /// <param name="filePath">file path of the image. The path should be relative to current dir or absolute.</param>
        public static void Image(Rect rect, string filePath)
        {
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            var id = window.GetID(filePath);
            var container = window.AbsoluteVisualList;
            var node = (Node)container.Find(visual => visual.Id == id);
            if (node == null)
            {
                //create node
                node = new Node(id, $"{nameof(Image)}<{filePath}>");
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Image]);
                container.Add(node);
            }
            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(rect);
            
            // last item state
            window.TempData.LastItemState = node.State;

            // draw
            using (var dc = node.RenderOpen())
            {
                var texture = TextureCache.Default.GetOrAdd(filePath, Form.current.renderer);
                dc.DrawBoxModel(texture, node.RuleSet, node.Rect);
            }
        }

        /// <summary>
        /// Create an image from an existing texture.
        /// </summary>
        /// <param name="rect">position and size</param>
        /// <param name="texture">texture, call<see cref="CreateTexture"/>to create it.</param>
        public static void Image(Rect rect, ITexture texture)
        {
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            var id = window.GetID(texture.GetHashCode());
            var container = window.AbsoluteVisualList;
            var node = (Node)container.Find(visual => visual.Id == id);
            if (node == null)
            {
                //create node
                node = new Node(id, $"{nameof(Image)}<Native_{texture.GetNativeTextureId()}>");
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Image]);
                container.Add(node);
            }
            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(rect);
            
            // last item state
            window.TempData.LastItemState = node.State;

            // draw
            using (var dc = node.RenderOpen())
            {
                dc.DrawBoxModel(texture, node.RuleSet, node.Rect);
            }
        }
    }

    public partial class GUILayout
    {
        /// <summary>
        /// Create an auto-layout image.
        /// </summary>
        /// <param name="filePath">file path of the image to display. The path should be relative to current dir or absolute.</param>
        public static void Image(string filePath)
        {
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            var id = window.GetID(filePath);
            var container = window.RenderTree.CurrentContainer;
            var node = container.GetNodeById(id);
            var texture = TextureCache.Default.GetOrAdd(filePath, Form.current.renderer);
            if (node == null)
            {
                //create node
                node = new Node(id, $"{nameof(Image)}<{filePath}>");
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Image]);
                node.AttachLayoutEntry(texture.Size);
            }
            container.AppendChild(node);
            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(id);
            
            // last item state
            window.TempData.LastItemState = node.State;

            // draw
            using (var dc = node.RenderOpen())
            {
                dc.DrawBoxModel(texture, node.RuleSet, node.Rect);
            }
        }

        public static void Image(ITexture texture)
        {
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            var id = window.GetID(texture.GetHashCode());
            var container = window.RenderTree.CurrentContainer;
            var node = container.GetNodeById(id);
            if (node == null)
            {
                //create node
                node = new Node(id, $"{nameof(Image)}<Native_{texture.GetNativeTextureId()}>");
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Image]);
                node.AttachLayoutEntry(texture.Size);
            }
            container.AppendChild(node);
            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(id);
            
            // last item state
            window.TempData.LastItemState = node.State;

            // draw
            using (var dc = node.RenderOpen())
            {
                dc.DrawBoxModel(texture, node.RuleSet, node.Rect);
            }
        }
    }

    internal partial class GUISkin
    {
        private void InitImageStyles(StyleRuleSet ruleSet)
        {
            var builder = new StyleRuleSetBuilder(ruleSet);
            builder
                .Border(1.0)
                .BorderColor(Color.Black);
        }
    }
}
