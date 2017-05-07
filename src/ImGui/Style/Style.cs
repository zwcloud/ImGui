using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ImGui
{
    public enum GUIStyleName
    {
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

        OutlineWidth,
        OutlineColor,

        CellingSpacingHorizontal,
        CellingSpacingVertical,

        BackgroundColor,
        BackgroundImage,

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

        LineColor,
        FillColor,
        
        _FieldWidth,
        ResizeGripActiveColor,
        ResizeGripColor,
    }

    public enum GUIState
    {
        Normal,
        Hover,
        Active,
        Disabled,
    }

    public class GUIStyle
    {
        private struct NameState
        {
            public GUIStyleName Name { get; set; }
            public GUIState State { get; set; }
        }

        public static GUIStyle Default { get; private set; }

        private static readonly double DefaultFontSize;

        private static string DefaultFontFamily;

        static GUIStyle()
        {
            if (Utility.CurrentOS.IsAndroid)
            {
                DefaultFontSize = 42;
                DefaultFontFamily = "DroidSans.ttf";
            }
            else
            {
                DefaultFontSize = 12;
                DefaultFontFamily = Utility.FontDir + "msjh.ttf";
            }

            Default = new GUIStyle();
        }

        public static implicit operator GUIStyle(string str)
        {
            return GUISkin.Instance.GetStyle(str);
        }

        public GUIStyle()
        {
            NumberStyles = new Dictionary<NameState, double>
            {
                [new NameState { Name = GUIStyleName.BorderTop, State = GUIState.Normal }] = 0,
                [new NameState { Name = GUIStyleName.BorderTop, State = GUIState.Hover }] = 0,
                [new NameState { Name = GUIStyleName.BorderTop, State = GUIState.Active }] = 0,
                [new NameState { Name = GUIStyleName.BorderRight, State = GUIState.Normal }] = 0,
                [new NameState { Name = GUIStyleName.BorderRight, State = GUIState.Hover }] = 0,
                [new NameState { Name = GUIStyleName.BorderRight, State = GUIState.Active }] = 0,
                [new NameState { Name = GUIStyleName.BorderBottom, State = GUIState.Normal }] = 0,
                [new NameState { Name = GUIStyleName.BorderBottom, State = GUIState.Hover }] = 0,
                [new NameState { Name = GUIStyleName.BorderBottom, State = GUIState.Active }] = 0,
                [new NameState { Name = GUIStyleName.BorderLeft, State = GUIState.Normal }] = 0,
                [new NameState { Name = GUIStyleName.BorderLeft, State = GUIState.Hover }] = 0,
                [new NameState { Name = GUIStyleName.BorderLeft, State = GUIState.Active }] = 0,
                [new NameState { Name = GUIStyleName.BorderImageSliceTop, State = GUIState.Normal }] = 0,
                [new NameState { Name = GUIStyleName.BorderImageSliceTop, State = GUIState.Hover }] = 0,
                [new NameState { Name = GUIStyleName.BorderImageSliceTop, State = GUIState.Active }] = 0,
                [new NameState { Name = GUIStyleName.BorderImageSliceRight, State = GUIState.Normal }] = 0,
                [new NameState { Name = GUIStyleName.BorderImageSliceRight, State = GUIState.Hover }] = 0,
                [new NameState { Name = GUIStyleName.BorderImageSliceRight, State = GUIState.Active }] = 0,
                [new NameState { Name = GUIStyleName.BorderImageSliceBottom, State = GUIState.Normal }] = 0,
                [new NameState { Name = GUIStyleName.BorderImageSliceBottom, State = GUIState.Hover }] = 0,
                [new NameState { Name = GUIStyleName.BorderImageSliceBottom, State = GUIState.Active }] = 0,
                [new NameState { Name = GUIStyleName.BorderImageSliceLeft, State = GUIState.Normal }] = 0,
                [new NameState { Name = GUIStyleName.BorderImageSliceLeft, State = GUIState.Hover }] = 0,
                [new NameState { Name = GUIStyleName.BorderImageSliceLeft, State = GUIState.Active }] = 0,

                [new NameState { Name = GUIStyleName.PaddingTop, State = GUIState.Normal }] = 0,
                [new NameState { Name = GUIStyleName.PaddingTop, State = GUIState.Hover }] = 0,
                [new NameState { Name = GUIStyleName.PaddingTop, State = GUIState.Active }] = 0,
                [new NameState { Name = GUIStyleName.PaddingRight, State = GUIState.Normal }] = 0,
                [new NameState { Name = GUIStyleName.PaddingRight, State = GUIState.Hover }] = 0,
                [new NameState { Name = GUIStyleName.PaddingRight, State = GUIState.Active }] = 0,
                [new NameState { Name = GUIStyleName.PaddingBottom, State = GUIState.Normal }] = 0,
                [new NameState { Name = GUIStyleName.PaddingBottom, State = GUIState.Hover }] = 0,
                [new NameState { Name = GUIStyleName.PaddingBottom, State = GUIState.Active }] = 0,
                [new NameState { Name = GUIStyleName.PaddingLeft, State = GUIState.Normal }] = 0,
                [new NameState { Name = GUIStyleName.PaddingLeft, State = GUIState.Hover }] = 0,
                [new NameState { Name = GUIStyleName.PaddingLeft, State = GUIState.Active }] = 0,

                [new NameState { Name = GUIStyleName.CellingSpacingHorizontal, State = GUIState.Normal }] = 8,
                [new NameState { Name = GUIStyleName.CellingSpacingHorizontal, State = GUIState.Hover }] = 8,
                [new NameState { Name = GUIStyleName.CellingSpacingHorizontal, State = GUIState.Active }] = 8,
                [new NameState { Name = GUIStyleName.CellingSpacingVertical, State = GUIState.Normal }] = 4,
                [new NameState { Name = GUIStyleName.CellingSpacingVertical, State = GUIState.Hover }] = 4,
                [new NameState { Name = GUIStyleName.CellingSpacingVertical, State = GUIState.Active }] = 4,

                [new NameState { Name = GUIStyleName.OutlineWidth, State = GUIState.Normal }] = 0,
                [new NameState { Name = GUIStyleName.OutlineWidth, State = GUIState.Hover }] = 0,
                [new NameState { Name = GUIStyleName.OutlineWidth, State = GUIState.Active }] = 0,

                [new NameState { Name = GUIStyleName.FontSize, State = GUIState.Normal }] = DefaultFontSize,
                [new NameState { Name = GUIStyleName.FontSize, State = GUIState.Hover }] = DefaultFontSize,
                [new NameState { Name = GUIStyleName.FontSize, State = GUIState.Active }] = DefaultFontSize,

            };

            ColorStyles = new Dictionary<NameState, Color>
            {
                [new NameState { Name = GUIStyleName.BorderTopColor, State = GUIState.Normal }] = Color.Black,
                [new NameState { Name = GUIStyleName.BorderTopColor, State = GUIState.Hover }] = Color.Black,
                [new NameState { Name = GUIStyleName.BorderTopColor, State = GUIState.Active }] = Color.Black,

                [new NameState { Name = GUIStyleName.BorderRightColor, State = GUIState.Normal }] = Color.Black,
                [new NameState { Name = GUIStyleName.BorderRightColor, State = GUIState.Hover }] = Color.Black,
                [new NameState { Name = GUIStyleName.BorderRightColor, State = GUIState.Active }] = Color.Black,

                [new NameState { Name = GUIStyleName.BorderBottomColor, State = GUIState.Normal }] = Color.Black,
                [new NameState { Name = GUIStyleName.BorderBottomColor, State = GUIState.Hover }] = Color.Black,
                [new NameState { Name = GUIStyleName.BorderBottomColor, State = GUIState.Active }] = Color.Black,

                [new NameState { Name = GUIStyleName.BorderLeftColor, State = GUIState.Normal }] = Color.Black,
                [new NameState { Name = GUIStyleName.BorderLeftColor, State = GUIState.Hover }] = Color.Black,
                [new NameState { Name = GUIStyleName.BorderLeftColor, State = GUIState.Active }] = Color.Black,

                [new NameState { Name = GUIStyleName.BackgroundColor, State = GUIState.Normal }] = Color.Clear,
                [new NameState { Name = GUIStyleName.BackgroundColor, State = GUIState.Hover }] = Color.Clear,
                [new NameState { Name = GUIStyleName.BackgroundColor, State = GUIState.Active }] = Color.Clear,

                [new NameState { Name = GUIStyleName.FontColor, State = GUIState.Normal }] = new Color(0.90, 0.90, 0.90),
                [new NameState { Name = GUIStyleName.FontColor, State = GUIState.Hover }] = new Color(0.90, 0.90, 0.90),
                [new NameState { Name = GUIStyleName.FontColor, State = GUIState.Active }] = new Color(0.90, 0.90, 0.90),
            };

            ImageStyles = new Dictionary<NameState, ITexture>
            {
                [new NameState { Name = GUIStyleName.BorderImageSource, State = GUIState.Normal }] = null,
                [new NameState { Name = GUIStyleName.BorderImageSource, State = GUIState.Hover }] = null,
                [new NameState { Name = GUIStyleName.BorderImageSource, State = GUIState.Active }] = null,

                [new NameState { Name = GUIStyleName.BackgroundImage, State = GUIState.Normal }] = null,
                [new NameState { Name = GUIStyleName.BackgroundImage, State = GUIState.Hover }] = null,
                [new NameState { Name = GUIStyleName.BackgroundImage, State = GUIState.Active }] = null,

            };

            EnumStyles = new Dictionary<NameState, int>
            {
                [new NameState { Name = GUIStyleName.TextAlignment, State = GUIState.Normal }] = (int)TextAlignment.Leading,
                [new NameState { Name = GUIStyleName.TextAlignment, State = GUIState.Hover }] = (int)TextAlignment.Leading,
                [new NameState { Name = GUIStyleName.TextAlignment, State = GUIState.Active }] = (int)TextAlignment.Leading,

                [new NameState { Name = GUIStyleName.FontStyle, State = GUIState.Normal }] = (int)FontWeight.Normal,
                [new NameState { Name = GUIStyleName.FontStyle, State = GUIState.Hover }] = (int)FontWeight.Normal,
                [new NameState { Name = GUIStyleName.FontStyle, State = GUIState.Active }] = (int)FontWeight.Normal,

                [new NameState { Name = GUIStyleName.AlignmentHorizontal, State = GUIState.Normal }] = (int)Alignment.Start,
                [new NameState { Name = GUIStyleName.AlignmentHorizontal, State = GUIState.Hover }] = (int)Alignment.Start,
                [new NameState { Name = GUIStyleName.AlignmentHorizontal, State = GUIState.Active }] = (int)Alignment.Start,
                [new NameState { Name = GUIStyleName.AlignmentVertical, State = GUIState.Normal }] = (int)Alignment.Start,
                [new NameState { Name = GUIStyleName.AlignmentVertical, State = GUIState.Hover }] = (int)Alignment.Start,
                [new NameState { Name = GUIStyleName.AlignmentVertical, State = GUIState.Active }] = (int)Alignment.Start,
            };

            StrStyles = new Dictionary<NameState, string>
            {
                [new NameState { Name = GUIStyleName.FontFamily, State = GUIState.Normal }] = DefaultFontFamily,
                [new NameState { Name = GUIStyleName.FontFamily, State = GUIState.Hover }] = DefaultFontFamily,
                [new NameState { Name = GUIStyleName.FontFamily, State = GUIState.Active }] = DefaultFontFamily,
            };

        }

        private Dictionary<NameState, double> NumberStyles;
        private Dictionary<NameState, Color> ColorStyles;
        private Dictionary<NameState, ITexture> ImageStyles;
        private Dictionary<NameState, int> EnumStyles;
        private Dictionary<NameState, string> StrStyles;

        public T Get<T>(GUIStyleName styleName, GUIState state = GUIState.Normal)
        {
            var nameState = new NameState { Name = styleName, State = state };
            var dict = GetDict<T>();
            if (dict.TryGetValue(nameState, out T v))
            {
                return v;
            }
            return GetDefault<T>(styleName, state);
        }

        public void Set<T>(GUIStyleName styleName, T value, GUIState state = GUIState.Normal)
        {
            var nameState = new NameState { Name = styleName, State = state };
            var dict = GetDict<T>();
            dict[nameState] = value;
        }

        public T GetDefault<T>(GUIStyleName styleName, GUIState state)
        {
            return default(T);//TODO replace with real default styles
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Dictionary<NameState, T> GetDict<T>()
        {
            return NumberStyles as Dictionary<NameState, T>
                ?? EnumStyles as Dictionary<NameState, T>
                ?? ColorStyles as Dictionary<NameState, T>
                ?? StrStyles as Dictionary<NameState, T>
                ?? ImageStyles as Dictionary<NameState, T>;
        }

        /// <summary>
        /// Get border-box size of a segment of text
        /// </summary>
        internal Size CalcSize(string text, GUIState state, LayoutOption[] options)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            var width = -1d;
            var height = -1d;

            // apply options
            if (options != null)
            {
                foreach (var option in options)
                {
                    if (option.type == LayoutOption.Type.fixedWidth)
                    {
                        width = (double)option.value;
                    }
                    else if (option.type == LayoutOption.Type.fixedHeight)
                    {
                        height = (double)option.value;
                    }
                }
            }

            // apply font and text styles
            {
                var fontFamily = this.Get<string>(GUIStyleName.FontFamily, state);
                var fontSize = this.Get<double>(GUIStyleName.FontSize, state);
                var fontStretch = (FontStretch)this.Get<int>(GUIStyleName.FontStretch, state);
                var fontStyle = (FontStyle)this.Get<int>(GUIStyleName.FontStyle, state);
                var fontWeight = (FontWeight)this.Get<int>(GUIStyleName.FontWeight, state);
                var textAlignment = (TextAlignment)this.Get<int>(GUIStyleName.TextAlignment, state);

                if (width < 0 && height < 0) // auto-sized text
                {
                    var actualSize = MeasureText(fontFamily, fontSize, fontStretch, fontStyle, fontWeight, textAlignment, text);
                    width = actualSize.Width;
                    height = actualSize.Height;
                }
                else
                {
                    if (width < 0) // width-auto-sized text
                    {
                        var actualSize = MeasureText(fontFamily, fontSize, fontStretch, fontStyle, fontWeight, textAlignment, text);
                        width = actualSize.Width;
                    }
                    else if (height < 0) // height-auto-sized text
                    {
                        var actualSize = MeasureText(fontFamily, fontSize, fontStretch, fontStyle, fontWeight, textAlignment, text);
                        height = actualSize.Height;
                    }
                }
            }

            // apply padding and border
            if (width < 0)
            {
                width = 0;
            }
            if (height < 0)
            {
                height = 0;
            }
            var size = new Size(Math.Ceiling(width), Math.Ceiling(height));
            var paddingHorizontal = Get<double>(GUIStyleName.PaddingLeft) + Get<double>(GUIStyleName.PaddingRight);
            var borderHorizontal = Get<double>(GUIStyleName.BorderLeft) + Get<double>(GUIStyleName.BorderRight);
            var paddingVertical = Get<double>(GUIStyleName.PaddingTop) + Get<double>(GUIStyleName.PaddingBottom);
            var borderVertical = Get<double>(GUIStyleName.BorderTop) + Get<double>(GUIStyleName.BorderBottom);
            size.Width += paddingHorizontal + borderHorizontal;
            size.Height += paddingVertical + borderVertical;

            return size;
        }

        /// <summary>
        /// Get border-box size of a segment of text
        /// </summary>
        internal Size CalcSize(ITexture texture, GUIState state, LayoutOption[] options)
        {
            if (texture == null) throw new ArgumentNullException(nameof(texture));

            var width = -1d;
            var height = -1d;

            // apply options
            if (options != null)
            {
                foreach (var option in options)
                {
                    if (option.type == LayoutOption.Type.fixedWidth)
                    {
                        width = (double)option.value;
                    }
                    else if (option.type == LayoutOption.Type.fixedHeight)
                    {
                        height = (double)option.value;
                    }
                }
            }

            // apply image size
            {
                width = texture.Width;
                height = texture.Height;
            }

            // apply padding and border
            if (width < 0)
            {
                width = 0;
            }
            if (height < 0)
            {
                height = 0;
            }
            var size = new Size(Math.Ceiling(width), Math.Ceiling(height));

            var paddingHorizontal = Get<double>(GUIStyleName.PaddingLeft) + Get<double>(GUIStyleName.PaddingRight);
            var borderHorizontal = Get<double>(GUIStyleName.BorderLeft) + Get<double>(GUIStyleName.BorderRight);
            var paddingVertical = Get<double>(GUIStyleName.PaddingTop) + Get<double>(GUIStyleName.PaddingBottom);
            var borderVertical = Get<double>(GUIStyleName.BorderTop) + Get<double>(GUIStyleName.BorderBottom);
            size.Width += paddingHorizontal + borderHorizontal;
            size.Height += paddingVertical + borderVertical;
            return size;
        }

        private static Size MeasureText(string fontFamily, double fontSize,
            FontStretch fontStretch, FontStyle fontStyle, FontWeight fontWeight,
            TextAlignment textAlignment,
            string text)
        {
            Size actualSize;

            using (var measureContext = Application.platformContext.CreateTextContext(
                text,
                fontFamily, (int)fontSize, fontStretch, fontStyle, fontWeight,
                4096, 4096,
                textAlignment))
            {
                actualSize = measureContext.Measure();
            }
            return actualSize;
        }

#region common styles

        public double BorderTop    => Get<double>(GUIStyleName.BorderTop);
        public double BorderRight  => Get<double>(GUIStyleName.BorderRight);
        public double BorderBottom => Get<double>(GUIStyleName.BorderBottom);
        public double BorderLeft   => Get<double>(GUIStyleName.BorderLeft);

        public double PaddingTop => Get<double>(GUIStyleName.PaddingTop);
        public double PaddingRight => Get<double>(GUIStyleName.PaddingRight);
        public double PaddingBottom => Get<double>(GUIStyleName.PaddingBottom);
        public double PaddingLeft => Get<double>(GUIStyleName.PaddingLeft);

        public Color BorderTopColor => Get<Color>(GUIStyleName.BorderTopColor);
        public Color BorderRightColor => Get<Color>(GUIStyleName.BorderRightColor);
        public Color BorderBottomColor => Get<Color>(GUIStyleName.BorderBottomColor);
        public Color BorderLeftColor => Get<Color>(GUIStyleName.BorderLeftColor);

        public double PaddingHorizontal => Get<double>(GUIStyleName.PaddingLeft) + Get<double>(GUIStyleName.PaddingRight);
        public double BorderHorizontal => Get<double>(GUIStyleName.BorderLeft) + Get<double>(GUIStyleName.BorderRight);

        public ITexture BorderImageSource => Get<ITexture>(GUIStyleName.BorderImageSource);

        public (double, double, double, double) BorderImageSlice
        {
            get
            {
                var top =    Get<double>(GUIStyleName.BorderImageSliceTop);
                var right =  Get<double>(GUIStyleName.BorderImageSliceRight);
                var bottom = Get<double>(GUIStyleName.BorderImageSliceBottom);
                var left =   Get<double>(GUIStyleName.BorderImageSliceLeft);
                return (top, right, bottom, left);
            }

            set
            {
                var (top, right, bottom, left) = value;
                Set<double>(GUIStyleName.BorderImageSliceTop, top);
                Set<double>(GUIStyleName.BorderImageSliceRight, right);
                Set<double>(GUIStyleName.BorderImageSliceBottom, bottom);
                Set<double>(GUIStyleName.BorderImageSliceLeft, left);
            }
        }

        public double PaddingVertical => Get<double>(GUIStyleName.PaddingTop) + Get<double>(GUIStyleName.PaddingBottom);
        public double BorderVertical => Get<double>(GUIStyleName.BorderTop) + Get<double>(GUIStyleName.BorderBottom);

        public double CellingSpacingHorizontal => Get<double>(GUIStyleName.CellingSpacingHorizontal);
        public double CellingSpacingVertical => Get<double>(GUIStyleName.CellingSpacingVertical);

        public double OutlineWidth => Get<double>(GUIStyleName.OutlineWidth);
        public Color OutlineColor => Get<Color>(GUIStyleName.OutlineColor);

        public Alignment AlignmentHorizontal
        {
            get => (Alignment)Get<int>(GUIStyleName.AlignmentHorizontal);
            set => Set<int>(GUIStyleName.AlignmentHorizontal, (int)value);
        }
        public Alignment AlignmentVertical
        {
            get => (Alignment)Get<int>(GUIStyleName.AlignmentVertical);
            set => Set<int>(GUIStyleName.AlignmentVertical, (int)value);
        }

        public Color LineColor => Get<Color>(GUIStyleName.LineColor);
        public Color FillColor => Get<Color>(GUIStyleName.FillColor);

        public Color BackgroundColor => Get<Color>(GUIStyleName.BackgroundColor);

        public double FontSize => Get<double>(GUIStyleName.FontSize);
        public Color FontColor => Get<Color>(GUIStyleName.FontColor);


#endregion
    }
}
