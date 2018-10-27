using System;
using System.Diagnostics;
using ImGui.Common.Primitive;
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
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            //get or create the root node
            int id = window.GetID(text);
            var container = window.NodeTreeRoot;
            Node node = container.GetNodeById(id);
            text = Utility.FindRenderedText(text);
            if (node == null)
            {
                node = new Node(id, $"Label<{text}>");
                container.AppendChild(node);
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.Label]);
                node.Primitive = new TextPrimitive(text);
            }

            var textPrimitive = node.Primitive as TextPrimitive;
            Debug.Assert(textPrimitive != null);
            textPrimitive.Text = text;

            // rect
            node.Rect = window.GetRect(rect);
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
                node.Primitive = new TextPrimitive(text);
            }
            node.RuleSet.ApplyOptions(options);

            //TODO check if text changes

            // rect
            node.Rect = window.GetRect(id);
        }

        /// <summary>
        /// Create an auto-layout label.
        /// </summary>
        /// <param name="text">text to display</param>
        public static void Label(string text)
        {
            Label(text, options: null);
        }

        #if false
        /// <summary>
        /// Create a colored auto-layout label.
        /// </summary>
        /// <param name="color">text color</param>
        /// <param name="text">text</param>
        public static void Label(Color color, string text)
        {
            PushFontColor(color);
            Label(text);
            PopStyleVar();
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
                Space("FieldSpacing", GUISkin.Current.FieldSpacing);
                Label(label, GUILayout.Width((int)GUISkin.Current.LabelWidth));
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

            GUIContext g = GetCurrentContext();
            int id = window.GetID(str_id);

            // style
            var style = GUIStyle.Basic;

            // rect
            var lineHeight = style.GetLineHeight();
            Rect rect = window.GetRect(id);

            // Render
            var d = window.DrawList;
            var bulletPosition = rect.TopLeft + new Vector(0, lineHeight * 0.5f);
            d.RenderBullet(bulletPosition, lineHeight, style.FontColor);
        }

        /// <summary>
        /// Create a label with a little bullet.
        /// </summary>
        public static void BulletText(string text)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return;

            GUIContext g = GetCurrentContext();
            int id = window.GetID(text);

            // style
            var style = GUIStyle.Basic;

            // rect
            Size contentSize = style.CalcSize(text, GUIState.Normal);
            var lineHeight = style.GetLineHeight();
            Rect rect = window.GetRect(id);

            // Render
            var d = window.DrawList;
            var bulletPosition = rect.TopLeft + new Vector(0, lineHeight * 0.5f);
            d.RenderBullet(bulletPosition, lineHeight, style.FontColor);
            rect.Offset(lineHeight, 0);
            d.AddText(rect, text, style, GUIState.Normal);
        }

        public static void BulletText(string format, params object[] args)
        {
            BulletText(string.Format(format, args));
        }
        #endregion

        #endif
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
    #if false
    internal static partial class DrawListExtension
    {
        public static void RenderBullet(this IPrimitiveRenderer drawList, Point pos, double lineHeight, Color color)
        {
            drawList.AddCircleFilled(pos, (float)lineHeight * 0.20f, color, 8);
        }
    }
    #endif

}
