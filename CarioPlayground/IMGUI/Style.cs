using System;
using System.Collections.Generic;
using Cairo;

namespace ImGui
{
    public partial class Style
    {
        #region Box styles

        #region Padding

        /// <summary>
        /// padding-top
        /// </summary>
        public double PaddingTop { get; set; }

        /// <summary>
        /// padding-right
        /// </summary>
        public double PaddingRight { get; set; }

        /// <summary>
        /// padding-bottom
        /// </summary>
        public double PaddingBottom { get; set; }

        /// <summary>
        /// padding-left
        /// </summary>
        public double PaddingLeft { get; set; }

        public double PaddingHorizontal { get { return PaddingLeft + PaddingRight; } }

        public double PaddingVertical { get { return PaddingTop + PaddingBottom; } }

        #endregion

        #region Border

        /// <summary>
        /// border-top-width
        /// </summary>
        public double BorderTop { get; set; }

        /// <summary>
        /// border-right-width
        /// </summary>
        public double BorderRight { get; set; }

        /// <summary>
        /// border-bottom-width
        /// </summary>
        public double BorderBottom { get; set; }

        /// <summary>
        /// border-left-width
        /// </summary>
        public double BorderLeft { get; set; }

        public double BorderHorizontal { get { return (float)BorderLeft + (float)BorderRight; } }

        public double BorderVertical { get { return (float)BorderTop + (float)BorderBottom; } }

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

        #region Outline
        public double OutlineWidth { get; set; }
        public Color OutlineColor { get; set; }
        #endregion

        #region Background

        public BackgroundStyle BackgroundStyle { get; set; }

        #endregion

        #region Container Style

        public double CellingSpacingHorizontal { get; set; }

        public double CellingSpacingVertical { get; set; }

        public Alignment AlignmentHorizontal { get; set; }

        public Alignment AlignmentVertical { get; set; }

        #endregion

        #endregion

        #region Text

        public Font Font { get; set; }

        public TextStyle TextStyle { get; set; }

        #endregion

        #region Cursor

        public Cursor Cursor { get; set; }

        #endregion

        #region Verctor graphic styles
        public Color LineColor { get; set; }
        public int LineWidth { get; set; }
        public Color FillColor { get; set; }
        #endregion

        public Dictionary<string, object> ExtraStyles;

        public TextAlignment TextAlignment
        {
            get { return TextStyle.TextAlignment; }
            set
            {
                TextStyle = new TextStyle
                {
                    LineSpacing = TextStyle.LineSpacing,
                    TextAlignment = value,
                    TabSize = TextStyle.TabSize
                };
            }
        }


        /// <summary>
        /// Set defalut style for all kinds of box model
        /// </summary>
        private Style()
        {
            PaddingTop = PaddingRight = PaddingBottom = PaddingLeft = 0;
            BorderTop = BorderRight = BorderBottom = BorderLeft = 0;
            BorderTopColor = CairoEx.ColorBlack;
            BorderRightColor = CairoEx.ColorBlack;
            BorderBottomColor = CairoEx.ColorBlack;
            BorderLeftColor = CairoEx.ColorBlack;
            OutlineWidth = 0;
            OutlineColor = new Color(0, 0, 0, 0.5);

            BackgroundStyle = new BackgroundStyle
            {
                Color = CairoEx.ColorWhite,
                Image = null,
                Pattern = null
            };

            CellingSpacingHorizontal = 0;
            CellingSpacingVertical = 0;
            AlignmentHorizontal = Alignment.Start;
            AlignmentVertical = Alignment.Start;

            Font = new Font
            {
                FontFamily = "Consolas",
                FontStyle = FontStyle.Normal,
                FontWeight = FontWeight.Normal,
                FontStretch = FontStretch.Normal,
                Size = 12,
                Color = CairoEx.ColorBlack
            };

            TextStyle = new TextStyle
            {
                TextAlignment = TextAlignment.Center,
                LineSpacing = 0,
                TabSize = 4
            };

            Cursor = Cursor.Default;

            LineColor = CairoEx.ColorBlack;
            LineWidth = 1;
            FillColor = new Color(1,1,1);

            ExtraStyles = new Dictionary<string, object>();
        }

        public static readonly Style None = new Style();

        /// <summary>
        /// Get content's border-box size
        /// </summary>
        internal Size CalcSize(Content content, LayoutOption[] options)
        {
            var size = content.GetSize(this, options);
            size.Width += this.PaddingHorizontal + this.BorderHorizontal;
            size.Height += this.PaddingVertical + this.BorderVertical;
            return size;
        }

        /// <summary>
        /// get actual size of the text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public Size GetTextActualSize(string text)
        {
            Size actualSize;
            var font = this.Font;
            var textStyle = this.TextStyle;
            using (var measureContext = Application._map.CreateTextContext(
                text,
                font.FontFamily, font.Size, font.FontStretch, font.FontStyle, font.FontWeight,
                4096, 4096,
                textStyle.TextAlignment))
            {
                actualSize = measureContext.Rect.Size;
            }
            return actualSize;
        }

        internal static bool IsRebuildTextContextRequired(Style a, Style b)
        {
            return
                // font changed
                a.Font.FontFamily != b.Font.FontFamily
                || a.Font.Size != b.Font.Size
                || a.Font.FontStretch != b.Font.FontStretch
                || a.Font.FontStyle != b.Font.FontStyle
                || a.Font.FontWeight != b.Font.FontWeight
                // Text styles changed
                || a.TextStyle.TextAlignment != b.TextStyle.TextAlignment
                || !MathEx.AmostEqual(a.TextStyle.LineSpacing, b.TextStyle.LineSpacing)
                || a.TextStyle.TabSize != b.TextStyle.TabSize
                ;
        }
    }
}
