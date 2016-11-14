using System.Collections.Generic;

namespace ImGui
{
    public class StyleState : Dictionary<string, ImGui.Style>
    {
        public StyleState(Dictionary<string, ImGui.Style> states) : base(states)
        {
        }

    }
}