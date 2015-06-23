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

        #region Text

        public Font Font { get; set; }

        public TextStyle TextStyle { get; set; }

        #endregion

        #region Background

        public BackgroundStyle BackgroundStyle { get; set; }

        #endregion

        #region Interaction

        private StyleState _normal;
        public StyleState Normal
        {
            get
            {
                if (this._normal == null)
                {
                    this._normal = new StyleState(this);
                }
                return this._normal;
            }
            internal set { _normal = value; }
        }

        private StyleState _active;
        public StyleState Active
        {
            get
            {
                if (this._active == null)
                {
                    this._active = new StyleState(this);
                }
                return this._active;
            }
            internal set { _active = value; }
        }

        private StyleState _hover;
        public StyleState Hover
        {
            get
            {
                if (this._hover == null)
                {
                    this._hover = new StyleState(this);
                }
                return this._hover;
            }
            internal set { _hover = value; }
        }
        
        #endregion
        

        public Style()
        {
            PaddingTop = PaddingRight = PaddingBottom = PaddingLeft = Length.Zero;
            BorderTop = BorderRight = BorderBottom = BorderLeft = Length.Zero;;
            BorderTopColor = CairoEx.ColorBlack;
            BorderRightColor = CairoEx.ColorBlack;
            BorderBottomColor = CairoEx.ColorBlack;
            BorderLeftColor = CairoEx.ColorBlack;
            MarginTop = MarginRight = MarginBottom = MarginLeft = Length.Zero;

            Font = new Font();

            TextStyle = new TextStyle
            {
                TextAlign = TextAlignment.Left,
                LineSpacing = 0,
                TabSize = 4
            };

            BackgroundStyle = new BackgroundStyle
            {
                Color = CairoEx.ColorWhite,
                Image = null,
                Pattern = null
            };
        }


    }
}
