using System;
using System.Diagnostics;
using ImGui.Common.Primitive;
using ImGui.Development.DebuggerViews;

namespace ImGui.Rendering
{
    [DebuggerTypeProxy(typeof(PathDataDebuggerView))]
    internal struct PathData
    {
        private static int[] PointNumbers = { 1, 1, 3, 0 };

        public PathDataType Type { get; set; }
        public Point[] Points;

        public PathData(PathDataType type)
        {
            this.Type = type;
            if(type != PathDataType.PathClosePath)
            {
                Points = new Point[PointNumbers[(int)type]];
            }
            else
            {
                Points = null;
            }
        }
    }
}
