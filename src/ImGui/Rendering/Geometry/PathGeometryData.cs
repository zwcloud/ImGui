namespace ImGui.Rendering
{
    internal class PathGeometryData
    {
        public FillRule FillRule { get; set; }

        public Vector Offset { get; set; }//TODO make this a matrix, namely transformation

        internal byte[] SerializedData;//serialized PathGeometryData
    }
}