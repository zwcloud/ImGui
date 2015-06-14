using Cairo;

namespace IMGUI
{
    public class Font
    {
        public string Family { get; set; }
        public FontSlant Slant { get; set; }
        public FontWeight Weight { get; set; }
        public double Size { get; set; }
        public Color Color { get; set; }

        public Font()
        {
            Family = "Consolas";
            Slant = FontSlant.Normal;
            Weight = FontWeight.Normal;
            Size = 12;
            Color = CairoEx.ColorBlack;
        }
    }
}