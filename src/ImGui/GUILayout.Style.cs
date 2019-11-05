using System;
using System.Collections.Generic;
using System.Text;

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

        public static void CopyTo(List<IStyleRule> otherStyleRuleList)
        {
            otherStyleRuleList.Clear();
            otherStyleRuleList.AddRange(styleRuleStack);
        }
    }
}
