using System;
using Cairo;

namespace IMGUI
{
    public struct BackgroundStyle
    {
        /// <summary>
        /// Image that used as a background
        /// </summary>
        public Texture Image { get; set; }
        
        /// <summary>
        /// Background color
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// Brush pattern of this background, whose size is 1 unit. You need to set proper scaling matrix to use it.
        /// </summary>
        /// <see cref="Cairo.Pattern.set_Matrix"/>
        public Pattern Pattern { get; set; }
    }
}