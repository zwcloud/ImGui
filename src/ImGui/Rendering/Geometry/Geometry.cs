namespace ImGui.Rendering
{
    /// <summary>
    /// Base class for all Geometry
    /// </summary>
    internal abstract class Geometry
    {
        /// <summary>
        /// Offset applied this this <see cref="Geometry"/>.
        /// </summary>
        public Vector Offset { get; set; }//TODO make this a matrix, namely transformation

        public abstract void UpdateContent(RenderContext context);

        internal static PathGeometryData GetEmptyPathGeometryData()
        {
            return emptyPathGeometryData;
        }

        private static readonly PathGeometryData emptyPathGeometryData = MakeEmptyGeometryData();

        private static PathGeometryData MakeEmptyGeometryData()
        {
            PathGeometryData data = new PathGeometryData();
            data.FillRule = FillRule.EvenOdd;
            data.Offset = Vector.Zero;
            return data;
        }
    }
}
