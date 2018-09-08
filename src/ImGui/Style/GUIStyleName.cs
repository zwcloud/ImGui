namespace ImGui
{
    public enum GUIStyleName//FIXME rename to StyleProperty(Name)
    {
        #region Box model
        MinWidth,
        MaxWidth,
        MinHeight,
        MaxHeight,

        PaddingTop,
        PaddingRight,
        PaddingBottom,
        PaddingLeft,

        BorderTop,
        BorderRight,
        BorderBottom,
        BorderLeft,
        BorderTopColor,
        BorderRightColor,
        BorderBottomColor,
        BorderLeftColor,

        BorderImageSource,
        BorderImageSliceTop,
        BorderImageSliceRight,
        BorderImageSliceBottom,
        BorderImageSliceLeft,

        BorderTopLeftRadius,
        BorderTopRightRadius,
        BorderBottomRightRadius,
        BorderBottomLeftRadius,


        OutlineWidth,
        OutlineColor,
        #endregion Box model

        HorizontalStretchFactor,
        VerticalStretchFactor,

        CellingSpacingHorizontal,
        CellingSpacingVertical,

        BackgroundColor,
        BackgroundImage,
        BackgroundGradient,

        TextAlignment,
        AlignmentHorizontal,
        AlignmentVertical,

        FontFamily,
        FontStyle,
        FontStretch,
        FontWeight,
        FontSize,
        FontColor,

        Slider_LineUsed,
        Slider_LineUnused,

        StrokeColor,
        FillColor,

        GradientLeftColor,
        GradientTopColor,
        GradientRightColor,
        GradientBottomColor,

        MinTextureCoordinateU,
        MaxTextureCoordinateU,
        MinTextureCoordinateV,
        MaxTextureCoordinateV,

        _FieldWidth,
        _ControlLabelSpacing,
        _LabelWidth,
        _LabelHeight,

        WindowRounding,
        ResizeGripColor,
        WindowBorderColor,
        WindowShadowColor,
        WindowShadowWidth,

        ScrollBarWidth,
        ScrollBarBackgroundColor,
        ScrollBarButtonColor,
    }
}