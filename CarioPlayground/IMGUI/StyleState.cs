using Cairo;

namespace IMGUI
{
    public sealed class StyleState
    {
        private Style _sourceStyle;

        public Texture BackgroundImage { get; set; }
        public Color BackgroundColor { get; set; }
        public FontWeight FontWeight { get; set; }
        public Color FontColor { get; set; }

        public StyleState(Style sourceStyle)
        {
            BackgroundImage = null;
            BackgroundColor = CairoEx.ColorWhite;
            FontColor = CairoEx.ColorBlack;
        }
    }
}