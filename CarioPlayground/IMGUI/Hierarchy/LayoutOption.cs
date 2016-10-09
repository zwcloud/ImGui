using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImGui
{
    /// <summary>
    /// Class internally used to pass layout options into GUILayout functions. You don't use these directly, but construct them with the layouting functions in the GUILayout class.
    /// </summary>
    public sealed class LayoutOption
    {
        internal enum Type
        {
            fixedWidth,
            fixedHeight,
            minWidth,
            maxWidth,
            minHeight,
            maxHeight,
            stretchWidth,
            stretchHeight,
            alignStart,
            alignMiddle,
            alignEnd,
            alignJustify,
            equalSize,
            spacing
        }

        internal LayoutOption.Type type;

        internal object value;

        internal LayoutOption(LayoutOption.Type type, object value)
        {
            this.type = type;
            this.value = value;
        }
    }
}
