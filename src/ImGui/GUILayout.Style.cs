using System;
using System.Collections.Generic;
using S = ImGui.StylePropertyName;

namespace ImGui
{
    public partial class GUILayout
    {
        private static readonly List<IStyleRule> styleRuleStack = new List<IStyleRule>(16);

        internal static List<IStyleRule> StyleRuleStack => styleRuleStack;

        public static void PopStyle(int number = 1)
        {
            for (int i = 0; i < number && styleRuleStack.Count > 0; i++)
            {
                styleRuleStack.RemoveAt(styleRuleStack.Count - 1);
            }
        }

        public static void PushStyle<T>(StylePropertyName name, T value)
        {
            styleRuleStack.Add(new StyleRule<T>(name, value, GUIState.Normal));
        }

        public static void PushStyle<T>(StylePropertyName name, T value, GUIState state)
        {
            styleRuleStack.Add(new StyleRule<T>(name, value, state));
        }

        public static void PushCustomStyle<T>(string name, T value, GUIState state)
        {
            var propertyName = CustomStylePropertyName.GetOrAdd(name);
            styleRuleStack.Add(new StyleRule<T>(propertyName, value, state));
        }

        #region short-cut

        public static void PushBorder((int top, int left, int right, int bottom) border, GUIState state = GUIState.Normal)
        {
            PushStyle<double>(S.BorderTop, border.top, state);
            PushStyle<double>(S.BorderRight, border.right, state);
            PushStyle<double>(S.BorderBottom, border.bottom, state);
            PushStyle<double>(S.BorderLeft, border.left, state);
        }

        public static void PushPadding((int top, int left, int right, int bottom) padding, GUIState state = GUIState.Normal)
        {
            PushStyle<double>(S.PaddingTop, padding.top, state);
            PushStyle<double>(S.PaddingRight, padding.right, state);
            PushStyle<double>(S.PaddingBottom, padding.bottom, state);
            PushStyle<double>(S.PaddingLeft, padding.left, state);
        }

        public static void PushFixedWidth(int width, GUIState state = GUIState.Normal)
        {
            PushStyle<double>(S.MinWidth, width, state);
            PushStyle<double>(S.MaxWidth, width, state);
        }
        public static void PushFixedHeight(int height, GUIState state = GUIState.Normal)
        {
            PushStyle<double>(S.MinHeight, height, state);
            PushStyle<double>(S.MaxHeight, height, state);
        }

        #endregion
    }
}
