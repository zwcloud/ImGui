using System;
using System.Collections.Generic;

namespace ImGui.Style
{
    public class CustomSkin
    {
        internal Dictionary<GUIControlName, StyleRuleSet> Rules { get; set; }
            = new Dictionary<GUIControlName, StyleRuleSet>();

        public StyleRuleSet this[GUIControlName index]
        {
            get
            {
                if (Rules.TryGetValue(index, out var value))
                {
                    return value;
                }
                else
                {
                    var ruleSet = Rules[index] = new StyleRuleSet();
                    return ruleSet;
                }
            }
        }
    }
}
