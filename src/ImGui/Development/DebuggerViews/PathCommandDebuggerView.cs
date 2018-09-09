using System.Diagnostics;
using ImGui.Common.Primitive;
using ImGui.Rendering;

namespace ImGui.Development.DebuggerViews
{
    internal class PathCommandDebuggerView
    {
        private readonly PathCommand pathCommand;

        public PathCommandDebuggerView(PathCommand pathCommand)
        {
            this.pathCommand = pathCommand;
        }

        public string Type => this.pathCommand.Type.ToString();
    }
}
