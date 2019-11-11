using System;
using System.Collections.Generic;
using System.Text;

namespace ImGui
{
    public class CustomStylePropertyName : StylePropertyName
    {
        private CustomStylePropertyName(string name) : base(name)
        {
        }

        private static readonly Dictionary<string, CustomStylePropertyName> s_customStylePropertyNameMap = new Dictionary<string, CustomStylePropertyName>();

        public static CustomStylePropertyName Get(string name)
        {
            if (s_customStylePropertyNameMap.TryGetValue(name, out var value))
            {
                return value;
            }

            return null;
        }

        public static StylePropertyName GetOrAdd(string name)
        {
            if (s_customStylePropertyNameMap.TryGetValue(name, out var value))
            {
                return value;
            }

            var o = new CustomStylePropertyName(name);
            s_customStylePropertyNameMap.Add(name, o);
            return o;
        }
    }
}
