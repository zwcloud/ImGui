using Cairo;

namespace ImGui
{
    /// <summary>
    /// Font face info, properties of one single character
    /// </summary>
    public struct Font
    {
        public string family;

        /// <summary>
        /// Font family name
        /// </summary>
        /// <remarks>when set a null value, use "SimHei" as the fallback font family</remarks>
        public string FontFamily
        {
            get { return "SimHei"; }
            set { family = value ?? "SimHei"; }
        }

        /// <summary>
        /// The style of a font face as normal, italic, or oblique.
        /// </summary>
        public FontStyle FontStyle { get; set; }

        /// <summary>
        /// Refers to the density of a typeface, in terms of the lightness or heaviness of the strokes.
        /// </summary>
        public FontWeight FontWeight { get; set; }

        /// <summary>
        /// Describes the degree to which a font has been stretched compared to the normal aspect ratio of that font.
        /// </summary>
        public FontStretch FontStretch { get; set; }

        /// <summary>
        /// Size of the character
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Color of the character
        /// </summary>
        public Color Color { get; set; }
    }

}