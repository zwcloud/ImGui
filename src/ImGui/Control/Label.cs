using ImGui.Rendering;
using ImGui.Style;

namespace ImGui
{
    public partial class GUI
    {
        /// <summary>
        /// Create a fixed label.
        /// </summary>
        /// <param name="rect">The position and size of this control.</param>
        /// <param name="text">The text to display.</param>
        public static void Label(Rect rect, string text)
        {
            var window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            //get or create the root node
            var id = window.GetID(text);
            var container = window.AbsoluteVisualList;
            var node = (Node)container.Find(visual => visual.Id == id);
            text = Utility.FindRenderedText(text);
            if (node == null)
            {
                node = new Node(id, $"Label<{text}>");
                container.Add(node);
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Label]);
            }

            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(rect);
            
            // last item state
            window.TempData.LastItemState = node.State;

            using (var dc = node.RenderOpen())
            {
                dc.DrawBoxModel(text, node.RuleSet, node.Rect);
            }
        }
    }

    public partial class GUILayout
    {
        /// <summary>
        /// Create an auto-layout label.
        /// </summary>
        /// <param name="text">The text to display.</param>
        /// <param name="options">Layout options of this control.</param>
        public static void Label(string text, LayoutOptions? options)
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
                node = new Node(id, $"Label<{text}>");
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Label]);
                var size = node.RuleSet.CalcContentBoxSize(text, GUIState.Normal);
                node.AttachLayoutEntry(size);
            }
            container.AppendChild(node);

            node.ActiveSelf = true;
            node.RuleSet.ApplyOptions(options);

            // rect
            node.Rect = window.GetRect(id);
            
            // last item state
            window.TempData.LastItemState = node.State;

            using (var dc = node.RenderOpen())
            {
                dc.DrawBoxModel(text, node.RuleSet, node.Rect);
            }
        }

        /// <summary>
        /// Create an auto-layout label.
        /// </summary>
        /// <param name="text">text to display</param>
        public static void Label(string text)
        {
            Label(text, options: null);
        }

        /// <summary>
        /// Create a colored auto-layout label.
        /// </summary>
        /// <param name="color">The color of text.</param>
        /// <param name="text">The text to display.</param>
        public static void Label(Color color, string text)
        {
            Label(text, new LayoutOptions().FontColor(color));
        }

        /// <summary>
        /// Create an auto-layout and disabled label.
        /// </summary>
        /// <param name="text">The text to display.</param>
        public static void LabelDisabled(string text)
        {
            Label(Color.TextDisabled, text);
        }

        /// <summary>
        /// Create an auto-layout label with format string.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg">An object to format.</param>
        public static void Label(string format, object arg)
        {
            Label(string.Format(format, arg));
        }
        
        /// <summary>
        /// Create an auto-layout label with format string.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">First object to format.</param>
        /// <param name="arg1">Second object to format.</param>
        public static void Label(string format, object arg0, object arg1)
        {
            Label(string.Format(format, arg0, arg1));
        }
        
        /// <summary>
        /// Create an auto-layout label with format string.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="arg0">First object to format.</param>
        /// <param name="arg1">Second object to format.</param>
        /// <param name="arg2">Third object to format.</param>
        public static void Label(string format, object arg0, object arg1, object arg2)
        {
            Label(string.Format(format, arg0, arg1, arg2));
        }
        
        /// <summary>
        /// Create an auto-layout label with format string.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Label(string format, params object[] args)
        {
            Label(string.Format(format, args));
        }
        
        /// <summary>
        /// Create an auto-layout label.
        /// </summary>
        /// <param name="text">The text to display.</param>
        public static void Text(string text) => Label(text);
        
        /// <summary>
        /// Create an auto-layout label with format string.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void Text(string format, params object[] args) => Label(format, args);

        /// <summary>
        /// Create an auto-layout labeled text.
        /// </summary>
        /// <param name="label">The label of this control.</param>
        /// <param name="text">The text to display.</param>
        public static void LabelText(string label, string text)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            var id = window.GetID(label);

            BeginHorizontal("FieldGroup~"+id);
            {
                Label(text, GUILayout.ExpandWidth(true));
                Space("FieldSpacing", StyleRuleSet.Global.Get<double>("ControlLabelSpacing"));
                Label(label, GUILayout.Width((int)StyleRuleSet.Global.Get<double>("LabelWidth")));
            }
            EndHorizontal();
        }

        #region Bullets
        /// <summary>
        /// Create an auto-layout bullet mark.
        /// </summary>
        public static void Bullet(string str_id)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            //get or create the root node
            int id = window.GetID(str_id);
            var container = window.RenderTree.CurrentContainer;
            Node node = container.GetNodeById(id);
            if (node == null)
            {
                node = new Node(id, $"Bullet<{str_id}>");
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Label]);
                var lineheight = node.RuleSet.GetLineHeight();
                var size = new Size(lineheight, lineheight);
                node.AttachLayoutEntry(size);
            }
            container.AppendChild(node);

            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(id);
            
            // last item state
            window.TempData.LastItemState = node.State;

            using (var dc = node.RenderOpen())
            {
                var bulletPosition = node.Rect.TopLeft + new Vector(node.Rect.Height * 0.5f, node.Rect.Height * 0.5f);
                var lineheight = node.RuleSet.GetLineHeight();
                GUIAppearance.RenderBullet(dc, bulletPosition, lineheight, node.RuleSet.FontColor);
            }
        }

        /// <summary>
        /// Create a label with a little bullet.
        /// </summary>
        /// <param name="text">The text to display.</param>
        public static void BulletText(string text)
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
                node = new Node(id, $"BulletText<{text}>");
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Label]);
                var size = node.RuleSet.CalcContentBoxSize(text, GUIState.Normal);
                var lineHeight = node.RuleSet.GetLineHeight();
                size += new Vector(lineHeight, 0);
                node.AttachLayoutEntry(size);
            }
            container.AppendChild(node);

            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(id);
            
            // last item state
            window.TempData.LastItemState = node.State;

            using (var dc = node.RenderOpen())
            {
                var lineHeight = node.RuleSet.GetLineHeight();
                var bulletPosition = node.Rect.TopLeft + new Vector(lineHeight * 0.5f, lineHeight * 0.5f);
                GUIAppearance.RenderBullet(dc, bulletPosition, lineHeight, node.RuleSet.FontColor);
                var rect = node.Rect;
                rect.Offset(lineHeight, 0);
                dc.DrawGlyphRun(node.RuleSet, text, rect.TopLeft);
            }
        }

        /// <summary>
        /// Create an auto-layout label with a little bullet and format string.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public static void BulletText(string format, params object[] args)
        {
            BulletText(string.Format(format, args));
        }
        #endregion

    }

    internal partial class GUIAppearance
    {
        internal static void RenderBullet(DrawingContext dc, Point bulletPosition, double lineHeight, Color fontColor)
        {
            var radius = lineHeight * 0.20f;
            dc.DrawEllipse(new Brush(fontColor), null, bulletPosition, radius, radius);
        }
    }

    internal partial class GUISkin
    {
        private void InitLabelStyles(StyleRuleSet ruleSet)
        {
            StyleRuleSetBuilder builder = new StyleRuleSetBuilder(ruleSet);
            builder
                .Padding(1.0)
                .AlignmentVertical(Alignment.Center, GUIState.Normal)
                .AlignmentVertical(Alignment.Center, GUIState.Hover)
                .AlignmentVertical(Alignment.Center, GUIState.Active)
                .AlignmentHorizontal(Alignment.Start, GUIState.Normal)
                .AlignmentHorizontal(Alignment.Start, GUIState.Hover)
                .AlignmentHorizontal(Alignment.Start, GUIState.Active);
        }
    }

}
