namespace IMGUI
{
    public struct Length
    {
        public float Value;

        public Unit Unit;
        
        public Length(int value, Unit unitType)
        {
            this.Value = value;
            this.Unit = unitType;
        }

        public Length(float value, Unit unitType)
        {
            this.Value = value;
            this.Unit = unitType;
        }
        
        public static Length OnePixel = new Length(1, Unit.Pixel);
        public static Length Zero = new Length(0, Unit.Pixel);
        public static Length Undefined = new Length(0xFFFF, Unit.Pixel);

        /// <summary>
        /// Convert Length to float
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        /// <remarks>
        /// TODO: percent value should refer to a value
        /// </remarks>
        public static implicit operator float(Length length)
        {
            return length.Value;
        }
    }

    public enum Unit
    {
        Pixel,
        Percent,
    }
}
