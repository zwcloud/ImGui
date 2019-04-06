using System.Diagnostics;
using ImGui.Common.Primitive;
using ImGui.Rendering;
using ImGui.Style;

namespace ImGui
{
    public partial class GUI
    {
        /// <summary>
        /// Create a static box with text.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="text">text to display in the box</param>
        /// <param name="options">style options</param>
        internal static void Box(Rect rect, string text, LayoutOptions? options)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            //get or create the root node
            int id = window.GetID(text);
            var container = window.RenderTree.CurrentContainer;
            Node node = container.GetNodeById(id);
            text = Utility.FindRenderedText(text);
            if (node == null)
            {
                //create node
                node = new Node(id, $"Box<{text}>");
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Box]);
                var size = node.RuleSet.CalcSize(text, GUIState.Normal);
                node.AttachLayoutEntry(size);
                container.AppendChild(node);
                node.Geometry = new TextGeometry(text);
            }
            node.RuleSet.ApplyOptions(options);
            node.ActiveSelf = true;

            var textPrimitive = node.Geometry as TextGeometry;
            Debug.Assert(textPrimitive != null);
            textPrimitive.Text = text;

            // rect
            node.Rect = window.GetRect(rect);
        }

        internal static void Box(Rect rect, string text) => Box(rect, text, null);
    }

    public partial class GUILayout
    {
        /// <summary>
        /// Create an auto-layout static box with text.
        /// </summary>
        /// <param name="text">text to display in the box</param>
        /// <param name="options">style options</param>
        public static void Box(string text, LayoutOptions? options)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            //get or create the root node
            int id = window.GetID(text);
            var container = window.RenderTree.CurrentContainer;
            Node node = container.GetNodeById(id);
            text = Utility.FindRenderedText(text);
            if (node == null)
            {
                //create node
                node = new Node(id, $"Box<{text}>");
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Box]);
                var size = node.RuleSet.CalcSize(text, GUIState.Normal);
                node.AttachLayoutEntry(size);
                container.AppendChild(node);
                node.Geometry = new TextGeometry(text);
            }
            node.RuleSet.ApplyOptions(options);
            node.ActiveSelf = true;

            var textPrimitive = node.Geometry as TextGeometry;
            Debug.Assert(textPrimitive != null);
            textPrimitive.Text = text;

            // rect
            node.Rect = window.GetRect(id);
        }

        public static void Box(string text) => Box(text, null);
    }

    internal partial class GUISkin
    {
        private void InitBoxStyles(StyleRuleSet ruleSet)
        {
            StyleRuleSetBuilder builder = new StyleRuleSetBuilder(ruleSet);
            builder
                .Border(1.0)
                .Padding(5.0)
                .BorderColor(Color.Black)
                .BackgroundColor(Color.White)
                .AlignmentVertical(Alignment.Center)
                .AlignmentHorizontal(Alignment.Center);
        }
    }
}