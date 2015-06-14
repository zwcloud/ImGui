using Cairo;

namespace IMGUI
{
    public partial class Style
    {
        #region Padding

        /// <summary>
        /// padding-top
        /// </summary>
        public Length PaddingTop { get; set; }

        /// <summary>
        /// padding-right
        /// </summary>
        public Length PaddingRight { get; set; }

        /// <summary>
        /// padding-bottom
        /// </summary>
        public Length PaddingBottom { get; set; }

        /// <summary>
        /// padding-left
        /// </summary>
        public Length PaddingLeft { get; set; }

        #endregion
        
        #region Border

        /// <summary>
        /// border-top-width
        /// </summary>
        public Length BorderTop { get; set; }

        /// <summary>
        /// border-right-width
        /// </summary>
        public Length BorderRight { get; set; }

        /// <summary>
        /// border-bottom-width
        /// </summary>
        public Length BorderBottom { get; set; }

        /// <summary>
        /// border-left-width
        /// </summary>
        public Length BorderLeft { get; set; }

        /// <summary>
        /// Color of the top
        /// </summary>
        public Color BorderTopColor { get; set; }

        /// <summary>
        /// Color of the right
        /// </summary>
        public Color BorderRightColor { get; set; }

        /// <summary>
        /// Color of the bottom
        /// </summary>
        public Color BorderBottomColor { get; set; }

        /// <summary>
        /// Color of the left
        /// </summary>
        public Color BorderLeftColor { get; set; }

        #endregion
        
        #region Margin

        /// <summary>
        /// margin-top
        /// </summary>
        public Length MarginTop { get; set; }

        /// <summary>
        /// margin-right
        /// </summary>
        public Length MarginRight { get; set; }

        /// <summary>
        /// margin-bottom
        /// </summary>
        public Length MarginBottom { get; set; }

        /// <summary>
        /// margin-left
        /// </summary>
        public Length MarginLeft { get; set; }

        #endregion

        //Color
        /// <summary>
        /// Color of content background
        /// </summary>
        public Color BackgroundColor { get; set; }

        public Style()
        {
            PaddingTop = PaddingRight = PaddingBottom = PaddingLeft = Length.Zero;
            BorderTop = BorderRight = BorderBottom = BorderLeft = new Length(1, Unit.Pixel);
            BorderTopColor =new Color(1,0,0);
                BorderRightColor = new Color(0,1,0);
                BorderBottomColor = new Color(0,0,1);
                BorderLeftColor = new Color(1,1,0);
            MarginTop = MarginRight = MarginBottom = MarginLeft = Length.Zero;
            BackgroundColor = CairoEx.ColorWhite;
        }
    }
}
