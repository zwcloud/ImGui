using System.Collections.Generic;

namespace ImGui
{
    internal class StyleState : Dictionary<string, ImGui.Style>
    {
        public StyleState(Dictionary<string, ImGui.Style> states) : base(states)
        {
        }

    }
}