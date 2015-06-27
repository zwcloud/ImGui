using Cairo;

namespace IMGUI
{
    public struct Font
    {
        public string Family { get; set; }
        public FontSlant Slant { get; set; }
        public FontWeight Weight { get; set; }
        public double Size { get; set; }
        public Color Color { get; set; }
    }
}