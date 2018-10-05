using System.Collections.Generic;

namespace ImGui.Rendering
{
    internal interface ILayoutGroup : ILayoutEntry, IEnumerable<ILayoutEntry>
    {
        int ChildCount { get; }
    }
}