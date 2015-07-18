using Cairo;

using FontDescription = Pango.FontDescription;
using Weight = Pango.Weight;
using Color = Cairo.Color;

namespace IMGUI
{
    public struct Font
    {
        private string family;
        private int size;
        private Weight weight;

        public string Family
        {
            set
            {
                family = value ?? "SimHei";
                Description = FontDescription.FromString(string.Format("{0} {1} {2}", family, weight, size));
            }
        }

        public FontSlant Slant { get; set; }

        public Weight Weight
        {
            set
            {
                weight = value;
                Description = FontDescription.FromString(string.Format("{0} {1} {2}", family, weight, size));
            }
        }


        public int Size
        {
            set
            {
                size = value;
                Description = FontDescription.FromString(string.Format("{0} {1} {2}", family, weight, size));
            }
        }

        public Color Color { get; set; }

        public FontDescription Description { get; private set; }
    }
}