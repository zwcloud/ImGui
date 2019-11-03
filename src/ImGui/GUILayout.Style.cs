using System;
using System.Collections.Generic;
using System.Text;

namespace ImGui
{
    public partial class GUILayout
    {
        public static void PushStyle(StylePropertyName name, int value)
        {
            var g = GetCurrentContext();
            //implement a shared stack that will be applied in subsequent GUI call
        }

        public static void PopStyle()
        {

        }
    }
}
