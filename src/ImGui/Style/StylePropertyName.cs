using System.Collections.Generic;

namespace ImGui
{
    public class StylePropertyName
    {
        #region Box model
        public static StylePropertyName MinWidth = new StylePropertyName(nameof(MinWidth));
        public static StylePropertyName MaxWidth = new StylePropertyName(nameof(MaxWidth));
        public static StylePropertyName MinHeight = new StylePropertyName(nameof(MinHeight));
        public static StylePropertyName MaxHeight = new StylePropertyName(nameof(MaxHeight));

        public static StylePropertyName PaddingTop = new StylePropertyName(nameof(PaddingTop));
        public static StylePropertyName PaddingRight = new StylePropertyName(nameof(PaddingRight));
        public static StylePropertyName PaddingBottom = new StylePropertyName(nameof(PaddingBottom));
        public static StylePropertyName PaddingLeft = new StylePropertyName(nameof(PaddingLeft));

        public static StylePropertyName BorderTop = new StylePropertyName(nameof(BorderTop));
        public static StylePropertyName BorderRight = new StylePropertyName(nameof(BorderRight));
        public static StylePropertyName BorderBottom = new StylePropertyName(nameof(BorderBottom));
        public static StylePropertyName BorderLeft = new StylePropertyName(nameof(BorderLeft));
        public static StylePropertyName BorderTopColor = new StylePropertyName(nameof(BorderTopColor));
        public static StylePropertyName BorderRightColor = new StylePropertyName(nameof(BorderRightColor));
        public static StylePropertyName BorderBottomColor = new StylePropertyName(nameof(BorderBottomColor));
        public static StylePropertyName BorderLeftColor = new StylePropertyName(nameof(BorderLeftColor));

        public static StylePropertyName BorderImageSource = new StylePropertyName(nameof(BorderImageSource));
        public static StylePropertyName BorderImageSliceTop = new StylePropertyName(nameof(BorderImageSliceTop));
        public static StylePropertyName BorderImageSliceRight = new StylePropertyName(nameof(BorderImageSliceRight));
        public static StylePropertyName BorderImageSliceBottom = new StylePropertyName(nameof(BorderImageSliceBottom));
        public static StylePropertyName BorderImageSliceLeft = new StylePropertyName(nameof(BorderImageSliceLeft));

        public static StylePropertyName BorderTopLeftRadius = new StylePropertyName(nameof(BorderTopLeftRadius));
        public static StylePropertyName BorderTopRightRadius = new StylePropertyName(nameof(BorderTopRightRadius));
        public static StylePropertyName BorderBottomRightRadius = new StylePropertyName(nameof(BorderBottomRightRadius));
        public static StylePropertyName BorderBottomLeftRadius = new StylePropertyName(nameof(BorderBottomLeftRadius));


        public static StylePropertyName OutlineWidth = new StylePropertyName(nameof(OutlineWidth));
        public static StylePropertyName OutlineColor = new StylePropertyName(nameof(OutlineColor));
        #endregion Box model

        #region Object-Sizing and Object-Positioning
        public static StylePropertyName ObjectPosition = new StylePropertyName(nameof(ObjectPosition));
        #endregion

        public static StylePropertyName HorizontalStretchFactor = new StylePropertyName(nameof(HorizontalStretchFactor));
        public static StylePropertyName VerticalStretchFactor = new StylePropertyName(nameof(VerticalStretchFactor));

        public static StylePropertyName CellSpacingHorizontal = new StylePropertyName(nameof(CellSpacingHorizontal));
        public static StylePropertyName CellSpacingVertical = new StylePropertyName(nameof(CellSpacingVertical));

        public static StylePropertyName BackgroundColor = new StylePropertyName(nameof(BackgroundColor));
        public static StylePropertyName BackgroundImage = new StylePropertyName(nameof(BackgroundImage));
        public static StylePropertyName BackgroundGradient = new StylePropertyName(nameof(BackgroundGradient));

        public static StylePropertyName TextAlignment = new StylePropertyName(nameof(TextAlignment));
        public static StylePropertyName AlignmentHorizontal = new StylePropertyName(nameof(AlignmentHorizontal));
        public static StylePropertyName AlignmentVertical = new StylePropertyName(nameof(AlignmentVertical));

        public static StylePropertyName FontFamily = new StylePropertyName(nameof(FontFamily));
        public static StylePropertyName FontStyle = new StylePropertyName(nameof(FontStyle));
        public static StylePropertyName FontStretch = new StylePropertyName(nameof(FontStretch));
        public static StylePropertyName FontWeight = new StylePropertyName(nameof(FontWeight));
        public static StylePropertyName FontSize = new StylePropertyName(nameof(FontSize));
        public static StylePropertyName FontColor = new StylePropertyName(nameof(FontColor));

        public static StylePropertyName StrokeColor = new StylePropertyName(nameof(StrokeColor));
        public static StylePropertyName StrokeWidth = new StylePropertyName(nameof(StrokeWidth));
        public static StylePropertyName FillColor = new StylePropertyName(nameof(FillColor));

        public static StylePropertyName GradientLeftColor = new StylePropertyName(nameof(GradientLeftColor));
        public static StylePropertyName GradientTopColor = new StylePropertyName(nameof(GradientTopColor));
        public static StylePropertyName GradientRightColor = new StylePropertyName(nameof(GradientRightColor));
        public static StylePropertyName GradientBottomColor = new StylePropertyName(nameof(GradientBottomColor));

        public static StylePropertyName MinTextureCoordinateU = new StylePropertyName(nameof(MinTextureCoordinateU));
        public static StylePropertyName MaxTextureCoordinateU = new StylePropertyName(nameof(MaxTextureCoordinateU));
        public static StylePropertyName MinTextureCoordinateV = new StylePropertyName(nameof(MinTextureCoordinateV));
        public static StylePropertyName MaxTextureCoordinateV = new StylePropertyName(nameof(MaxTextureCoordinateV));

        public static StylePropertyName _FieldWidth = new StylePropertyName(nameof(_FieldWidth));
        public static StylePropertyName _LabelWidth = new StylePropertyName(nameof(_LabelWidth));
        public static StylePropertyName _LabelHeight = new StylePropertyName(nameof(_LabelHeight));

        public static StylePropertyName WindowRounding = new StylePropertyName(nameof(WindowRounding));
        public static StylePropertyName ResizeGripColor = new StylePropertyName(nameof(ResizeGripColor));
        public static StylePropertyName WindowBorderColor = new StylePropertyName(nameof(WindowBorderColor));
        public static StylePropertyName WindowShadowColor = new StylePropertyName(nameof(WindowShadowColor));
        public static StylePropertyName WindowShadowWidth = new StylePropertyName(nameof(WindowShadowWidth));

        public static StylePropertyName OverflowX = new StylePropertyName(nameof(OverflowX));
        public static StylePropertyName OverflowY = new StylePropertyName(nameof(OverflowY));

        public static StylePropertyName ScrollBarWidth = new StylePropertyName(nameof(ScrollBarWidth));
        public static StylePropertyName ScrollBarBackgroundColor = new StylePropertyName(nameof(ScrollBarBackgroundColor));
        public static StylePropertyName ScrollBarButtonColor = new StylePropertyName(nameof(ScrollBarButtonColor));

        protected StylePropertyName(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }
}