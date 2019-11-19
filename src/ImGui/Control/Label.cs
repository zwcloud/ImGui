using ImGui.Rendering;
using ImGui.Style;

namespace ImGui
{
    public partial class GUI
    {
        /// <summary>
        /// Create a label.
        /// </summary>
        /// <param name="rect">position and size</param>
        /// <param name="text">text to display</param>
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
        /// <param name="text">text to display</param>
        /// <param name="options"></param>
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
                var size = node.RuleSet.CalcSize(text, GUIState.Normal);
                node.AttachLayoutEntry(size);
                container.AppendChild(node);
            }

            node.ActiveSelf = true;
            node.RuleSet.ApplyOptions(options);

            // rect
            node.Rect = window.GetRect(id);

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
        /// <param name="color">text color</param>
        /// <param name="text">text</param>
        public static void Label(Color color, string text)
        {
            Label(text, new LayoutOptions().FontColor(color));
        }

        /// <summary>
        /// Create a auto-layout and disabled label.
        /// </summary>
        /// <param name="text">text</param>
        public static void LabelDisabled(string text)
        {
            Label(Color.TextDisabled, text);
        }

        public static void Label(string format, object arg0)
        {
            Label(string.Format(format, arg0));
        }

        public static void Label(string format, object arg0, object arg1)
        {
            Label(string.Format(format, arg0, arg1));
        }

        public static void Label(string format, object arg0, object arg1, object arg2)
        {
            Label(string.Format(format, arg0, arg1, arg2));
        }

        public static void Label(string format, params object[] args)
        {
            Label(string.Format(format, args));
        }

        public static void Text(string text) => Label(text);

        public static void Text(string format, params object[] args) => Label(format, args);

        /// <summary>
        /// Create a labeled text.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="text"></param>
        /// <returns></returns>
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
        /// Create a bullet.
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
                var size = new Size(0, node.RuleSet.GetLineHeight());
                node.AttachLayoutEntry(size);
                container.AppendChild(node);
            }

            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(id);

            using (var dc = node.RenderOpen())
            {
                var bulletPosition = node.Rect.TopLeft + new Vector(node.Rect.Height * 0.5f, node.Rect.Height * 0.5f);
                GUIAppearance.RenderBullet(dc, bulletPosition, node.Rect.Height, node.RuleSet.FontColor);
            }
        }

        /// <summary>
        /// Create a label with a little bullet.
        /// </summary>
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
                var size = node.RuleSet.CalcSize(text, GUIState.Normal);
                node.AttachLayoutEntry(size);
                container.AppendChild(node);
            }

            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(id);

            using (var dc = node.RenderOpen())
            {
                var bulletPosition = node.Rect.TopLeft + new Vector(node.Rect.Height * 0.5f, node.Rect.Height * 0.5f);
                GUIAppearance.RenderBullet(dc, bulletPosition, node.Rect.Height, node.RuleSet.FontColor);
                var rect = node.Rect;
                rect.Offset(node.Rect.Height, 0);
                dc.DrawGlyphRun(node.RuleSet, text, node.Rect.TopLeft);
            }
        }

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
                .Padding(1.0, GUIState.Normal)
                .Padding(1.0, GUIState.Hover)
                .Padding(1.0, GUIState.Active)
                .AlignmentVertical(Alignment.Center, GUIState.Normal)
                .AlignmentVertical(Alignment.Center, GUIState.Hover)
                .AlignmentVertical(Alignment.Center, GUIState.Active)
                .AlignmentHorizontal(Alignment.Start, GUIState.Normal)
                .AlignmentHorizontal(Alignment.Start, GUIState.Hover)
                .AlignmentHorizontal(Alignment.Start, GUIState.Active);
        }
    }

}
