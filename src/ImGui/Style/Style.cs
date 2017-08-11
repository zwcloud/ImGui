using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ImGui.Common.Primitive;
using ImGui.OSAbstraction.Graphics;

namespace ImGui
{
    public class GUIStyle
    {
        private struct NameState
        {
            public GUIStyleName Name { get; set; }
            public GUIState State { get; set; }
        }

        public static GUIStyle Default { get; }

        private static readonly double DefaultFontSize;

        private static readonly string DefaultFontFamily;

        static GUIStyle()
        {
            DefaultFontSize = 12;
            if (CurrentOS.IsAndroid)
            {
                DefaultFontFamily = "DroidSans.ttf";
            }
            else
            {
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
            // set up default style values
            this.numberStyles = new Dictionary<NameState, double>
            {
                [new NameState { Name = GUIStyleName.MinWidth, State = GUIState.Normal }] = 1,
                [new NameState { Name = GUIStyleName.MaxWidth, State = GUIState.Normal }] = 9999,
                [new NameState { Name = GUIStyleName.MinHeight, State = GUIState.Normal }] = 1,
                [new NameState { Name = GUIStyleName.MaxHeight, State = GUIState.Normal }] = 9999,

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

            this.colorStyles = new Dictionary<NameState, Color>
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

            this.imageStyles = new Dictionary<NameState, ITexture>
            {
                [new NameState { Name = GUIStyleName.BorderImageSource, State = GUIState.Normal }] = null,
                [new NameState { Name = GUIStyleName.BorderImageSource, State = GUIState.Hover }] = null,
                [new NameState { Name = GUIStyleName.BorderImageSource, State = GUIState.Active }] = null,

                [new NameState { Name = GUIStyleName.BackgroundImage, State = GUIState.Normal }] = null,
                [new NameState { Name = GUIStyleName.BackgroundImage, State = GUIState.Hover }] = null,
                [new NameState { Name = GUIStyleName.BackgroundImage, State = GUIState.Active }] = null,

            };

            this.intStyles = new Dictionary<NameState, int>
            {
                [new NameState { Name = GUIStyleName.HorizontalStretchFactor, State = GUIState.Normal }] = 0,
                [new NameState { Name = GUIStyleName.VerticalStretchFactor, State = GUIState.Normal }] = 0,

                [new NameState { Name = GUIStyleName.TextAlignment, State = GUIState.Normal }] = (int)TextAlignment.Leading,
                [new NameState { Name = GUIStyleName.TextAlignment, State = GUIState.Hover }] = (int)TextAlignment.Leading,
                [new NameState { Name = GUIStyleName.TextAlignment, State = GUIState.Active }] = (int)TextAlignment.Leading,

                [new NameState { Name = GUIStyleName.FontStyle, State = GUIState.Normal }] = (int)FontStyle.Normal,
                [new NameState { Name = GUIStyleName.FontStyle, State = GUIState.Hover }] = (int)FontStyle.Normal,
                [new NameState { Name = GUIStyleName.FontStyle, State = GUIState.Active }] = (int)FontStyle.Normal,

                [new NameState { Name = GUIStyleName.FontWeight, State = GUIState.Normal }] = (int)FontWeight.Normal,
                [new NameState { Name = GUIStyleName.FontWeight, State = GUIState.Hover }] = (int)FontWeight.Normal,
                [new NameState { Name = GUIStyleName.FontWeight, State = GUIState.Active }] = (int)FontWeight.Normal,

                [new NameState { Name = GUIStyleName.FontStretch, State = GUIState.Normal }] = (int)FontStretch.Normal,
                [new NameState { Name = GUIStyleName.FontStretch, State = GUIState.Hover }] = (int)FontStretch.Normal,
                [new NameState { Name = GUIStyleName.FontStretch, State = GUIState.Active }] = (int)FontStretch.Normal,

                [new NameState { Name = GUIStyleName.AlignmentHorizontal, State = GUIState.Normal }] = (int)Alignment.Start,
                [new NameState { Name = GUIStyleName.AlignmentHorizontal, State = GUIState.Hover }] = (int)Alignment.Start,
                [new NameState { Name = GUIStyleName.AlignmentHorizontal, State = GUIState.Active }] = (int)Alignment.Start,
                [new NameState { Name = GUIStyleName.AlignmentVertical, State = GUIState.Normal }] = (int)Alignment.Start,
                [new NameState { Name = GUIStyleName.AlignmentVertical, State = GUIState.Hover }] = (int)Alignment.Start,
                [new NameState { Name = GUIStyleName.AlignmentVertical, State = GUIState.Active }] = (int)Alignment.Start,
            };

            this.strStyles = new Dictionary<NameState, string>
            {
                [new NameState { Name = GUIStyleName.FontFamily, State = GUIState.Normal }] = DefaultFontFamily,
                [new NameState { Name = GUIStyleName.FontFamily, State = GUIState.Hover }] = DefaultFontFamily,
                [new NameState { Name = GUIStyleName.FontFamily, State = GUIState.Active }] = DefaultFontFamily,
            };

        }

        private readonly Dictionary<NameState, double> numberStyles;
        private readonly Dictionary<NameState, Color> colorStyles;
        private readonly Dictionary<NameState, ITexture> imageStyles;
        private readonly Dictionary<NameState, int> intStyles;
        private readonly Dictionary<NameState, string> strStyles;

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
            var nameState = new NameState { Name = styleName, State = state };
            var defalutDict = Default.GetDict<T>();
            if (defalutDict.TryGetValue(nameState, out T defaultValue))
            {
                return defaultValue;
            }

            throw new Exception(string.Format("Cannot find the style<{0},{1}>", styleName, state));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Dictionary<NameState, T> GetDict<T>()
        {
            return this.numberStyles as Dictionary<NameState, T>
                ?? this.intStyles as Dictionary<NameState, T>
                ?? this.colorStyles as Dictionary<NameState, T>
                ?? this.strStyles as Dictionary<NameState, T>
                ?? this.imageStyles as Dictionary<NameState, T>;
        }

        /// <summary>
        /// Get exact size of a text segment.
        /// </summary>
        internal Size CalcSize(string text, GUIState state)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            var width = -1d;
            var height = -1d;

            // apply font and text styles
            {
                var actualSize = MeasureText(state, text);
                width = actualSize.Width;
                height = actualSize.Height;
            }
            if (width < 0)
            {
                width = 0;
            }
            if (height < 0)
            {
                height = 0;
            }

            var size = new Size(Math.Ceiling(width), Math.Ceiling(height));
            return size;
        }

        /// <summary>
        /// Get border-box size of a texture
        /// </summary>
        internal Size CalcSize(ITexture texture, GUIState state, LayoutOption[] options)
        {
            if (texture == null) throw new ArgumentNullException(nameof(texture));

            double width;
            double height;

            // apply image size
            {
                width = texture.Width;
                height = texture.Height;
            }

            // apply options
            if (options != null)
            {
                foreach (var option in options)
                {
                    if (option.type == LayoutOptionType.FixedWidth)
                    {
                        width = (double)option.Value;
                    }
                    else if (option.type == LayoutOptionType.FixedHeight)
                    {
                        height = (double)option.Value;
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

        internal Size MeasureText(GUIState state, string text)
        {
            var measureContext = TextMeshUtil.GetTextContext(text, new Size(4096, 4096), this, state);
            var actualSize = measureContext.Measure();
            return actualSize;
        }

        #region short-cuts

        #region Box model
        // We use `box-sizing: border-box;`. So the width and height is the width and height of border-box.
        #region Width & Height
        public double MinWidth
        {
            get => Get<double>(GUIStyleName.MinWidth);
            set => Set<double>(GUIStyleName.MinWidth, value);
        }
        public double MaxWidth
        {
            get => Get<double>(GUIStyleName.MaxWidth);
            set => Set<double>(GUIStyleName.MaxWidth, value);
        }
        public double MinHeight
        {
            get => Get<double>(GUIStyleName.MinHeight);
            set => Set<double>(GUIStyleName.MinHeight, value);
        }
        public double MaxHeight
        {
            get => Get<double>(GUIStyleName.MaxHeight);
            set => Set<double>(GUIStyleName.MaxHeight, value);
        }
        #endregion Width & Height

        #region Padding
        public double PaddingTop => Get<double>(GUIStyleName.PaddingTop);
        public double PaddingRight => Get<double>(GUIStyleName.PaddingRight);
        public double PaddingBottom => Get<double>(GUIStyleName.PaddingBottom);
        public double PaddingLeft => Get<double>(GUIStyleName.PaddingLeft);
        public double PaddingHorizontal => Get<double>(GUIStyleName.PaddingLeft) + Get<double>(GUIStyleName.PaddingRight);
        public double PaddingVertical => Get<double>(GUIStyleName.PaddingTop) + Get<double>(GUIStyleName.PaddingBottom);
        public (double, double, double, double) Padding
        {
            get
            {
                var top = Get<double>(GUIStyleName.PaddingTop);
                var right = Get<double>(GUIStyleName.PaddingRight);
                var bottom = Get<double>(GUIStyleName.PaddingBottom);
                var left = Get<double>(GUIStyleName.PaddingLeft);
                return (top, right, bottom, left);
            }

            set
            {
                var (top, right, bottom, left) = value;
                Set<double>(GUIStyleName.PaddingTop, top);
                Set<double>(GUIStyleName.PaddingRight, right);
                Set<double>(GUIStyleName.PaddingBottom, bottom);
                Set<double>(GUIStyleName.PaddingLeft, left);
            }
        }
        #endregion Padding

        #region Border
        public double BorderTop => Get<double>(GUIStyleName.BorderTop);
        public double BorderRight => Get<double>(GUIStyleName.BorderRight);
        public double BorderBottom => Get<double>(GUIStyleName.BorderBottom);
        public double BorderLeft => Get<double>(GUIStyleName.BorderLeft);
        public double BorderHorizontal => Get<double>(GUIStyleName.BorderLeft) + Get<double>(GUIStyleName.BorderRight);
        public double BorderVertical => Get<double>(GUIStyleName.BorderTop) + Get<double>(GUIStyleName.BorderBottom);
        public (double, double, double, double) Border
        {
            get
            {
                var top = Get<double>(GUIStyleName.BorderTop);
                var right = Get<double>(GUIStyleName.BorderRight);
                var bottom = Get<double>(GUIStyleName.BorderBottom);
                var left = Get<double>(GUIStyleName.BorderLeft);
                return (top, right, bottom, left);
            }

            set
            {
                var (top, right, bottom, left) = value;
                Set<double>(GUIStyleName.BorderTop, top);
                Set<double>(GUIStyleName.BorderRight, right);
                Set<double>(GUIStyleName.BorderBottom, bottom);
                Set<double>(GUIStyleName.BorderLeft, left);
            }
        }
        public Color BorderColor
        {
            get
            {
                var color = BorderTopColor;
                if (color == BorderRightColor && color == BorderBottomColor && color == BorderLeftColor)
                {
                    return color;
                }
                throw new InvalidOperationException("Not all border share the same color! So we don't know which one should be returned.");
            }
            set
            {
                Set<Color>(GUIStyleName.BorderTopColor, value);
                Set<Color>(GUIStyleName.BorderRightColor, value);
                Set<Color>(GUIStyleName.BorderBottomColor, value);
                Set<Color>(GUIStyleName.BorderLeftColor, value);
            }
        }
        public Color BorderTopColor =>      Get<Color>(GUIStyleName.BorderTopColor);
        public Color BorderRightColor =>    Get<Color>(GUIStyleName.BorderRightColor);
        public Color BorderBottomColor =>   Get<Color>(GUIStyleName.BorderBottomColor);
        public Color BorderLeftColor =>     Get<Color>(GUIStyleName.BorderLeftColor);
        public ITexture BorderImageSource => Get<ITexture>(GUIStyleName.BorderImageSource);
        public (double, double, double, double) BorderImageSlice
        {
            get
            {
                var top = Get<double>(GUIStyleName.BorderImageSliceTop);
                var right = Get<double>(GUIStyleName.BorderImageSliceRight);
                var bottom = Get<double>(GUIStyleName.BorderImageSliceBottom);
                var left = Get<double>(GUIStyleName.BorderImageSliceLeft);
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
        #endregion Border

        #region Outline
        public double OutlineWidth => Get<double>(GUIStyleName.OutlineWidth);
        public Color OutlineColor => Get<Color>(GUIStyleName.OutlineColor);
        #endregion Outline

        #endregion Box model

        #region Layout

        public int HorizontalStretchFactor
        {
            get => Get<int>(GUIStyleName.HorizontalStretchFactor);
            set => Set<int>(GUIStyleName.HorizontalStretchFactor, value);
        }

        public int VerticalStretchFactor
        {
            get => Get<int>(GUIStyleName.VerticalStretchFactor);
            set => Set<int>(GUIStyleName.VerticalStretchFactor, value);
        }

        public double CellingSpacingHorizontal => Get<double>(GUIStyleName.CellingSpacingHorizontal);
        public double CellingSpacingVertical => Get<double>(GUIStyleName.CellingSpacingVertical);

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

        #endregion Layout



        public Color LineColor => Get<Color>(GUIStyleName.LineColor);
        public Color FillColor => Get<Color>(GUIStyleName.FillColor);

        public Color BackgroundColor
        {
            get => Get<Color>(GUIStyleName.BackgroundColor);
            set => Set<Color>(GUIStyleName.BackgroundColor, value);
        }

        public double FontSize
        {
            get => Get<double>(GUIStyleName.FontSize);
            set => Set<double>(GUIStyleName.FontSize, value);
        }

        public Color FontColor => Get<Color>(GUIStyleName.FontColor);


#endregion
    }
}
