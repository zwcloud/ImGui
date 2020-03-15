namespace ImGui.Rendering
{
    /// <summary>
    /// Base class for all Geometry
    /// </summary>
    public abstract class Geometry
    {
        /// <summary>
        /// Offset applied this this <see cref="Geometry"/>.
        /// </summary>
        public Vector Offset { get; set; }//TODO make this a matrix, namely transformation

        /// <summary>
        /// GetPathGeometryData - returns a struct which contains this Geometry represented
        /// as a path geometry's serialized format.
        /// </summary>
        internal abstract PathGeometryData GetPathGeometryData();

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
