using System.Diagnostics;
using ImGui.Common.Primitive;
using ImGui.Rendering;

namespace ImGui.Development.DebuggerViews
{
    internal class PathDataDebuggerView
    {
        private readonly PathData pathData;

        public PathDataDebuggerView(PathData pathData)
        {
            this.pathData = pathData;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public Point[] Points
        {
            get {
                Point[] array = this.pathData.Points.Clone() as Point[];
                return array;
            }
        }

        public string Type => this.pathData.Type.ToString();
    }
}
