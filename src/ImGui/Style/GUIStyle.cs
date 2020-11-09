using ImGui.OSAbstraction.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ImGui
{
    internal partial class GUIStyle
    {
        [DebuggerDisplay("{" + nameof(Name) + "}" + ("{" + nameof(State) + "}"))]
        private struct NameState
        {
            public StylePropertyName Name { get; set; }
            public GUIState State { get; set; }

            public override int GetHashCode()
            {
                unchecked // Overflow is fine, just wrap
                {
                    int hash = 17;
                    hash = hash * 23 + this.Name.GetHashCode();
                    hash = hash * 23 + this.State.GetHashCode();
                    return hash;
                }
            }

            public override bool Equals(object obj)
            {
                if (obj is NameState)
                {
                    return this.Equals((NameState)obj);
                }
                return false;
            }

            public bool Equals(NameState nameState)
            {
                return nameState.Name == this.Name && nameState.State == this.State;
            }
        }

        /// <summary>
        /// The app-level style.
        /// </summary>
        public static GUIStyle Default { get; }

        public static GUIStyle Basic { get; }

        private static readonly double DefaultFontSize;

        private static readonly string DefaultFontFamily;

        static GUIStyle()
        {
            if (CurrentOS.IsAndroid)
            {
                DefaultFontSize = 24;
                DefaultFontFamily = "DroidSans.ttf";
            }
            else
            {
                DefaultFontSize = 10.5;
                DefaultFontFamily = Utility.FontDir + "Terminus.ttf";
            }

            Basic = Default = CreateDefaultStyle();
        }

        private static GUIStyle CreateDefaultStyle()
        {
            var style = new GUIStyle(true);

            style.numberStyles = new Dictionary<NameState, double>
            {
                [new NameState { Name = StylePropertyName.MinWidth, State = GUIState.Normal }] = 1,
                [new NameState { Name = StylePropertyName.MaxWidth, State = GUIState.Normal }] = 9999,
                [new NameState { Name = StylePropertyName.MinHeight, State = GUIState.Normal }] = 1,
                [new NameState { Name = StylePropertyName.MaxHeight, State = GUIState.Normal }] = 9999,

                [new NameState { Name = StylePropertyName.BorderTop, State = GUIState.Normal }] = 0,
                [new NameState { Name = StylePropertyName.BorderTop, State = GUIState.Hover }] = 0,
                [new NameState { Name = StylePropertyName.BorderTop, State = GUIState.Active }] = 0,
                [new NameState { Name = StylePropertyName.BorderRight, State = GUIState.Normal }] = 0,
                [new NameState { Name = StylePropertyName.BorderRight, State = GUIState.Hover }] = 0,
                [new NameState { Name = StylePropertyName.BorderRight, State = GUIState.Active }] = 0,
                [new NameState { Name = StylePropertyName.BorderBottom, State = GUIState.Normal }] = 0,
                [new NameState { Name = StylePropertyName.BorderBottom, State = GUIState.Hover }] = 0,
                [new NameState { Name = StylePropertyName.BorderBottom, State = GUIState.Active }] = 0,
                [new NameState { Name = StylePropertyName.BorderLeft, State = GUIState.Normal }] = 0,
                [new NameState { Name = StylePropertyName.BorderLeft, State = GUIState.Hover }] = 0,
                [new NameState { Name = StylePropertyName.BorderLeft, State = GUIState.Active }] = 0,
                [new NameState { Name = StylePropertyName.BorderImageSliceTop, State = GUIState.Normal }] = 0,
                [new NameState { Name = StylePropertyName.BorderImageSliceTop, State = GUIState.Hover }] = 0,
                [new NameState { Name = StylePropertyName.BorderImageSliceTop, State = GUIState.Active }] = 0,
                [new NameState { Name = StylePropertyName.BorderImageSliceRight, State = GUIState.Normal }] = 0,
                [new NameState { Name = StylePropertyName.BorderImageSliceRight, State = GUIState.Hover }] = 0,
                [new NameState { Name = StylePropertyName.BorderImageSliceRight, State = GUIState.Active }] = 0,
                [new NameState { Name = StylePropertyName.BorderImageSliceBottom, State = GUIState.Normal }] = 0,
                [new NameState { Name = StylePropertyName.BorderImageSliceBottom, State = GUIState.Hover }] = 0,
                [new NameState { Name = StylePropertyName.BorderImageSliceBottom, State = GUIState.Active }] = 0,
                [new NameState { Name = StylePropertyName.BorderImageSliceLeft, State = GUIState.Normal }] = 0,
                [new NameState { Name = StylePropertyName.BorderImageSliceLeft, State = GUIState.Hover }] = 0,
                [new NameState { Name = StylePropertyName.BorderImageSliceLeft, State = GUIState.Active }] = 0,
                [new NameState { Name = StylePropertyName.BorderTopLeftRadius, State = GUIState.Normal }] = 0,
                [new NameState { Name = StylePropertyName.BorderTopLeftRadius, State = GUIState.Hover }] = 0,
                [new NameState { Name = StylePropertyName.BorderTopLeftRadius, State = GUIState.Active }] = 0,
                [new NameState { Name = StylePropertyName.BorderTopRightRadius, State = GUIState.Normal }] = 0,
                [new NameState { Name = StylePropertyName.BorderTopRightRadius, State = GUIState.Hover }] = 0,
                [new NameState { Name = StylePropertyName.BorderTopRightRadius, State = GUIState.Active }] = 0,
                [new NameState { Name = StylePropertyName.BorderBottomRightRadius, State = GUIState.Normal }] = 0,
                [new NameState { Name = StylePropertyName.BorderBottomRightRadius, State = GUIState.Hover }] = 0,
                [new NameState { Name = StylePropertyName.BorderBottomRightRadius, State = GUIState.Active }] = 0,
                [new NameState { Name = StylePropertyName.BorderBottomLeftRadius, State = GUIState.Normal }] = 0,
                [new NameState { Name = StylePropertyName.BorderBottomLeftRadius, State = GUIState.Hover }] = 0,
                [new NameState { Name = StylePropertyName.BorderBottomLeftRadius, State = GUIState.Active }] = 0,

                [new NameState { Name = StylePropertyName.PaddingTop, State = GUIState.Normal }] = 0,
                [new NameState { Name = StylePropertyName.PaddingTop, State = GUIState.Hover }] = 0,
                [new NameState { Name = StylePropertyName.PaddingTop, State = GUIState.Active }] = 0,
                [new NameState { Name = StylePropertyName.PaddingRight, State = GUIState.Normal }] = 0,
                [new NameState { Name = StylePropertyName.PaddingRight, State = GUIState.Hover }] = 0,
                [new NameState { Name = StylePropertyName.PaddingRight, State = GUIState.Active }] = 0,
                [new NameState { Name = StylePropertyName.PaddingBottom, State = GUIState.Normal }] = 0,
                [new NameState { Name = StylePropertyName.PaddingBottom, State = GUIState.Hover }] = 0,
                [new NameState { Name = StylePropertyName.PaddingBottom, State = GUIState.Active }] = 0,
                [new NameState { Name = StylePropertyName.PaddingLeft, State = GUIState.Normal }] = 0,
                [new NameState { Name = StylePropertyName.PaddingLeft, State = GUIState.Hover }] = 0,
                [new NameState { Name = StylePropertyName.PaddingLeft, State = GUIState.Active }] = 0,

                [new NameState { Name = StylePropertyName.CellSpacingHorizontal, State = GUIState.Normal }] = 0,
                [new NameState { Name = StylePropertyName.CellSpacingHorizontal, State = GUIState.Hover }] = 0,
                [new NameState { Name = StylePropertyName.CellSpacingHorizontal, State = GUIState.Active }] = 0,
                [new NameState { Name = StylePropertyName.CellSpacingVertical, State = GUIState.Normal }] = 0,
                [new NameState { Name = StylePropertyName.CellSpacingVertical, State = GUIState.Hover }] = 0,
                [new NameState { Name = StylePropertyName.CellSpacingVertical, State = GUIState.Active }] = 0,

                [new NameState { Name = StylePropertyName.OutlineWidth, State = GUIState.Normal }] = 0,
                [new NameState { Name = StylePropertyName.OutlineWidth, State = GUIState.Hover }] = 0,
                [new NameState { Name = StylePropertyName.OutlineWidth, State = GUIState.Active }] = 0,

                [new NameState { Name = StylePropertyName.FontSize, State = GUIState.Normal }] = DefaultFontSize,
                [new NameState { Name = StylePropertyName.FontSize, State = GUIState.Hover }] = DefaultFontSize,
                [new NameState { Name = StylePropertyName.FontSize, State = GUIState.Active }] = DefaultFontSize,

                [new NameState { Name = StylePropertyName.MinTextureCoordinateU, State = GUIState.Normal }] = 0,
                [new NameState { Name = StylePropertyName.MinTextureCoordinateU, State = GUIState.Hover }] = 0,
                [new NameState { Name = StylePropertyName.MinTextureCoordinateU, State = GUIState.Active }] = 0,
                [new NameState { Name = StylePropertyName.MinTextureCoordinateV, State = GUIState.Normal }] = 0,
                [new NameState { Name = StylePropertyName.MinTextureCoordinateV, State = GUIState.Hover }] = 0,
                [new NameState { Name = StylePropertyName.MinTextureCoordinateV, State = GUIState.Active }] = 0,

                [new NameState { Name = StylePropertyName.MaxTextureCoordinateU, State = GUIState.Normal }] = 1,
                [new NameState { Name = StylePropertyName.MaxTextureCoordinateU, State = GUIState.Hover }] = 1,
                [new NameState { Name = StylePropertyName.MaxTextureCoordinateU, State = GUIState.Active }] = 1,
                [new NameState { Name = StylePropertyName.MaxTextureCoordinateV, State = GUIState.Normal }] = 1,
                [new NameState { Name = StylePropertyName.MaxTextureCoordinateV, State = GUIState.Hover }] = 1,
                [new NameState { Name = StylePropertyName.MaxTextureCoordinateV, State = GUIState.Active }] = 1,

                [new NameState {Name = StylePropertyName.StrokeWidth, State = GUIState.Normal}] = 1,
                [new NameState {Name = StylePropertyName.StrokeWidth, State = GUIState.Hover}] = 1,
                [new NameState {Name = StylePropertyName.StrokeWidth, State = GUIState.Active}] = 1,

                [new NameState { Name = StylePropertyName.ScrollBarWidth, State = GUIState.Normal}] = 20,
                [new NameState { Name = StylePropertyName.ScrollBarWidth, State = GUIState.Hover }] = 20,
                [new NameState { Name = StylePropertyName.ScrollBarWidth, State = GUIState.Active }] = 20,

                //TODO Following custom styles should be moved into GUISkin
                [new NameState {Name = CustomStylePropertyName.GetOrAdd("ControlLabelSpacing"), State = GUIState.Normal}] = 5,
                [new NameState {Name = CustomStylePropertyName.GetOrAdd("ControlLabelSpacing"), State = GUIState.Hover }] = 5,
                [new NameState {Name = CustomStylePropertyName.GetOrAdd("ControlLabelSpacing"), State = GUIState.Active }] = 5,

                [new NameState {Name = CustomStylePropertyName.GetOrAdd("FieldWidth"), State = GUIState.Normal}] = 200,
                [new NameState {Name = CustomStylePropertyName.GetOrAdd("FieldWidth"), State = GUIState.Hover }] = 200,
                [new NameState {Name = CustomStylePropertyName.GetOrAdd("FieldWidth"), State = GUIState.Active }] = 200,

                [new NameState {Name = CustomStylePropertyName.GetOrAdd("LabelWidth"), State = GUIState.Normal}] = 80,
                [new NameState {Name = CustomStylePropertyName.GetOrAdd("LabelWidth"), State = GUIState.Hover }] = 80,
                [new NameState {Name = CustomStylePropertyName.GetOrAdd("LabelWidth"), State = GUIState.Active }] = 80,

                [new NameState {Name = CustomStylePropertyName.GetOrAdd("LabelHeight"), State = GUIState.Normal}] = 70,
                [new NameState {Name = CustomStylePropertyName.GetOrAdd("LabelHeight"), State = GUIState.Hover }] = 70,
                [new NameState {Name = CustomStylePropertyName.GetOrAdd("LabelHeight"), State = GUIState.Active }] = 70,
            };

            style.colorStyles = new Dictionary<NameState, Color>
            {
                [new NameState { Name = StylePropertyName.BorderTopColor, State = GUIState.Normal }] = Color.Black,
                [new NameState { Name = StylePropertyName.BorderTopColor, State = GUIState.Hover }] = Color.Black,
                [new NameState { Name = StylePropertyName.BorderTopColor, State = GUIState.Active }] = Color.Black,

                [new NameState { Name = StylePropertyName.BorderRightColor, State = GUIState.Normal }] = Color.Black,
                [new NameState { Name = StylePropertyName.BorderRightColor, State = GUIState.Hover }] = Color.Black,
                [new NameState { Name = StylePropertyName.BorderRightColor, State = GUIState.Active }] = Color.Black,

                [new NameState { Name = StylePropertyName.BorderBottomColor, State = GUIState.Normal }] = Color.Black,
                [new NameState { Name = StylePropertyName.BorderBottomColor, State = GUIState.Hover }] = Color.Black,
                [new NameState { Name = StylePropertyName.BorderBottomColor, State = GUIState.Active }] = Color.Black,

                [new NameState { Name = StylePropertyName.BorderLeftColor, State = GUIState.Normal }] = Color.Black,
                [new NameState { Name = StylePropertyName.BorderLeftColor, State = GUIState.Hover }] = Color.Black,
                [new NameState { Name = StylePropertyName.BorderLeftColor, State = GUIState.Active }] = Color.Black,

                [new NameState { Name = StylePropertyName.BackgroundColor, State = GUIState.Normal }] = Color.Clear,
                [new NameState { Name = StylePropertyName.BackgroundColor, State = GUIState.Hover }] = Color.Clear,
                [new NameState { Name = StylePropertyName.BackgroundColor, State = GUIState.Active }] = Color.Clear,

                [new NameState { Name = StylePropertyName.OutlineColor, State = GUIState.Normal }] = Color.Clear,
                [new NameState { Name = StylePropertyName.OutlineColor, State = GUIState.Hover }] = Color.Clear,
                [new NameState { Name = StylePropertyName.OutlineColor, State = GUIState.Active }] = Color.Clear,

                [new NameState { Name = StylePropertyName.FontColor, State = GUIState.Normal }] = Color.Black,
                [new NameState { Name = StylePropertyName.FontColor, State = GUIState.Hover }] = Color.Black,
                [new NameState { Name = StylePropertyName.FontColor, State = GUIState.Active }] = Color.Black,

                [new NameState { Name = StylePropertyName.GradientLeftColor, State = GUIState.Normal }] = Color.Rgb(247, 247, 247),
                [new NameState { Name = StylePropertyName.GradientLeftColor, State = GUIState.Hover }] = Color.Rgb(247, 247, 247),
                [new NameState { Name = StylePropertyName.GradientLeftColor, State = GUIState.Active }] = Color.Rgb(247, 247, 247),

                [new NameState { Name = StylePropertyName.GradientTopColor, State = GUIState.Normal }] = Color.Rgb(247, 247, 247),
                [new NameState { Name = StylePropertyName.GradientTopColor, State = GUIState.Hover }] = Color.Rgb(247, 247, 247),
                [new NameState { Name = StylePropertyName.GradientTopColor, State = GUIState.Active }] = Color.Rgb(247, 247, 247),

                [new NameState { Name = StylePropertyName.GradientRightColor, State = GUIState.Normal }] = Color.Rgb(221, 221, 221),
                [new NameState { Name = StylePropertyName.GradientRightColor, State = GUIState.Hover }] = Color.Rgb(221, 221, 221),
                [new NameState { Name = StylePropertyName.GradientRightColor, State = GUIState.Active }] = Color.Rgb(221, 221, 221),

                [new NameState { Name = StylePropertyName.GradientBottomColor, State = GUIState.Normal }] = Color.Rgb(221, 221, 221),
                [new NameState { Name = StylePropertyName.GradientBottomColor, State = GUIState.Hover }] = Color.Rgb(221, 221, 221),
                [new NameState { Name = StylePropertyName.GradientBottomColor, State = GUIState.Active }] = Color.Rgb(221, 221, 221),

                [new NameState { Name = StylePropertyName.StrokeColor, State = GUIState.Normal }] = Color.Black,
                [new NameState { Name = StylePropertyName.StrokeColor, State = GUIState.Hover }] = Color.Black,
                [new NameState { Name = StylePropertyName.StrokeColor, State = GUIState.Active }] = Color.Black,

                [new NameState { Name = StylePropertyName.FillColor, State = GUIState.Normal }] = Color.White,
                [new NameState { Name = StylePropertyName.FillColor, State = GUIState.Hover }] = Color.White,
                [new NameState { Name = StylePropertyName.FillColor, State = GUIState.Active }] = Color.White,

                [new NameState { Name = StylePropertyName.ScrollBarBackgroundColor, State = GUIState.Normal }] = Color.Rgb(232),
                [new NameState { Name = StylePropertyName.ScrollBarBackgroundColor, State = GUIState.Hover }] = Color.Rgb(232),
                [new NameState { Name = StylePropertyName.ScrollBarBackgroundColor, State = GUIState.Active }] = Color.Rgb(232),

                [new NameState { Name = StylePropertyName.ScrollBarButtonColor, State = GUIState.Normal }] = Color.Rgb(194,195,201),
                [new NameState { Name = StylePropertyName.ScrollBarButtonColor, State = GUIState.Hover }] = Color.Rgb(104),
                [new NameState { Name = StylePropertyName.ScrollBarButtonColor, State = GUIState.Active }] = Color.Rgb(91),
            };

            style.imageStyles = new Dictionary<NameState, ITexture>
            {
                [new NameState { Name = StylePropertyName.BackgroundImage, State = GUIState.Normal }] = null,
                [new NameState { Name = StylePropertyName.BackgroundImage, State = GUIState.Hover }] = null,
                [new NameState { Name = StylePropertyName.BackgroundImage, State = GUIState.Active }] = null,

            };

            style.intStyles = new Dictionary<NameState, int>
            {
                [new NameState { Name = StylePropertyName.HorizontalStretchFactor, State = GUIState.Normal }] = 0,
                [new NameState { Name = StylePropertyName.VerticalStretchFactor, State = GUIState.Normal }] = 0,

                [new NameState { Name = StylePropertyName.TextAlignment, State = GUIState.Normal }] = (int)TextAlignment.Leading,
                [new NameState { Name = StylePropertyName.TextAlignment, State = GUIState.Hover }] = (int)TextAlignment.Leading,
                [new NameState { Name = StylePropertyName.TextAlignment, State = GUIState.Active }] = (int)TextAlignment.Leading,

                [new NameState { Name = StylePropertyName.FontStyle, State = GUIState.Normal }] = (int)FontStyle.Normal,
                [new NameState { Name = StylePropertyName.FontStyle, State = GUIState.Hover }] = (int)FontStyle.Normal,
                [new NameState { Name = StylePropertyName.FontStyle, State = GUIState.Active }] = (int)FontStyle.Normal,

                [new NameState { Name = StylePropertyName.FontWeight, State = GUIState.Normal }] = (int)FontWeight.Normal,
                [new NameState { Name = StylePropertyName.FontWeight, State = GUIState.Hover }] = (int)FontWeight.Normal,
                [new NameState { Name = StylePropertyName.FontWeight, State = GUIState.Active }] = (int)FontWeight.Normal,

                [new NameState { Name = StylePropertyName.FontStretch, State = GUIState.Normal }] = (int)FontStretch.Normal,
                [new NameState { Name = StylePropertyName.FontStretch, State = GUIState.Hover }] = (int)FontStretch.Normal,
                [new NameState { Name = StylePropertyName.FontStretch, State = GUIState.Active }] = (int)FontStretch.Normal,

                [new NameState { Name = StylePropertyName.AlignmentHorizontal, State = GUIState.Normal }] = (int)Alignment.Start,
                [new NameState { Name = StylePropertyName.AlignmentHorizontal, State = GUIState.Hover }] = (int)Alignment.Start,
                [new NameState { Name = StylePropertyName.AlignmentHorizontal, State = GUIState.Active }] = (int)Alignment.Start,
                [new NameState { Name = StylePropertyName.AlignmentVertical, State = GUIState.Normal }] = (int)Alignment.Start,
                [new NameState { Name = StylePropertyName.AlignmentVertical, State = GUIState.Hover }] = (int)Alignment.Start,
                [new NameState { Name = StylePropertyName.AlignmentVertical, State = GUIState.Active }] = (int)Alignment.Start,

                [new NameState { Name = StylePropertyName.BackgroundGradient, State = GUIState.Normal }] = (int)Gradient.None,
                [new NameState { Name = StylePropertyName.BackgroundGradient, State = GUIState.Hover }] = (int)Gradient.None,
                [new NameState { Name = StylePropertyName.BackgroundGradient, State = GUIState.Active }] = (int)Gradient.None,

                [new NameState { Name = StylePropertyName.OverflowX, State = GUIState.Normal }] = (int)OverflowPolicy.Hidden,
                [new NameState { Name = StylePropertyName.OverflowX, State = GUIState.Hover }] = (int)OverflowPolicy.Hidden,
                [new NameState { Name = StylePropertyName.OverflowX, State = GUIState.Active }] = (int)OverflowPolicy.Hidden,
                [new NameState { Name = StylePropertyName.OverflowY, State = GUIState.Normal }] = (int)OverflowPolicy.Hidden,
                [new NameState { Name = StylePropertyName.OverflowY, State = GUIState.Hover }] = (int)OverflowPolicy.Hidden,
                [new NameState { Name = StylePropertyName.OverflowY, State = GUIState.Active }] = (int)OverflowPolicy.Hidden,
            };

            style.strStyles = new Dictionary<NameState, string>
            {
                [new NameState { Name = StylePropertyName.FontFamily, State = GUIState.Normal }] = DefaultFontFamily,
                [new NameState { Name = StylePropertyName.FontFamily, State = GUIState.Hover }] = DefaultFontFamily,
                [new NameState { Name = StylePropertyName.FontFamily, State = GUIState.Active }] = DefaultFontFamily,

                [new NameState { Name = StylePropertyName.BorderImageSource, State = GUIState.Normal }] = null,
                [new NameState { Name = StylePropertyName.BorderImageSource, State = GUIState.Hover }] = null,
                [new NameState { Name = StylePropertyName.BorderImageSource, State = GUIState.Active }] = null,
            };

            return style;
        }

        private Dictionary<NameState, double> numberStyles;
        private Dictionary<NameState, Color> colorStyles;
        private Dictionary<NameState, ITexture> imageStyles;
        private Dictionary<NameState, int> intStyles;
        private Dictionary<NameState, string> strStyles;

        private GUIStyle(bool dummy)//only used to create Default and Basic GUIStyle
        {
        }

        /// <summary>
        /// Create a custom style.
        /// </summary>
        public GUIStyle()
        {
            this.numberStyles = new Dictionary<NameState, double>();
            this.colorStyles = new Dictionary<NameState, Color>();
            this.imageStyles = new Dictionary<NameState, ITexture>();
            this.intStyles = new Dictionary<NameState, int>();
            this.strStyles = new Dictionary<NameState, string>();
        }

        public T Get<T>(StylePropertyName styleName, GUIState state = GUIState.Normal)
        {
            T value;
            var nameState = new NameState { Name = styleName, State = state };
            var dict = this.GetDict<T>();

            //try get the style value of specified state
            if (dict.TryGetValue(nameState, out value))
            {
                return value;
            }

            //try to get the style value of Normal state
            var normalNameState = new NameState { Name = styleName, State = GUIState.Normal };
            if (dict.TryGetValue(normalNameState, out value))
            {
                return value;
            }

            var defalutDict = Default.GetDict<T>();

            //try to get a default value of  specified state
            if (defalutDict.TryGetValue(nameState, out value))
            {
                return value;
            }

            // try to get a default value of Normal state
            if (defalutDict.TryGetValue(normalNameState, out value))
            {
                return value;
            }

            throw new InvalidOperationException($"Cannot find the style<{styleName},{state}>");
        }

        public void Set<T>(StylePropertyName styleName, T value, GUIState state = GUIState.Normal)
        {
            var nameState = new NameState { Name = styleName, State = state };
            var dict = this.GetDict<T>();
            dict[nameState] = value;
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
                var actualSize = this.MeasureText(state, text);
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
        internal Size CalcSize(ITexture texture, GUIState state)
        {
            if (texture == null) throw new ArgumentNullException(nameof(texture));

            double width;
            double height;

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

            var paddingHorizontal = this.Get<double>(StylePropertyName.PaddingLeft) + this.Get<double>(StylePropertyName.PaddingRight);
            var borderHorizontal = this.Get<double>(StylePropertyName.BorderLeft) + this.Get<double>(StylePropertyName.BorderRight);
            var paddingVertical = this.Get<double>(StylePropertyName.PaddingTop) + this.Get<double>(StylePropertyName.PaddingBottom);
            var borderVertical = this.Get<double>(StylePropertyName.BorderTop) + this.Get<double>(StylePropertyName.BorderBottom);
            size.Width += paddingHorizontal + borderHorizontal;
            size.Height += paddingVertical + borderVertical;
            return size;
        }

        internal double GetLineHeight()
        {
            var lineHeight = OSImplementation.TypographyTextContext.GetLineHeight(this.FontFamily, this.FontSize);
            lineHeight += this.PaddingVertical + this.BorderVertical;
            return lineHeight;
        }

        /// <summary>
        /// Get rect of the context box
        /// </summary>
        /// <param name="rect">rect of the entire box</param>
        /// <param name="style">style</param>
        /// <param name="state">state</param>
        /// <returns>rect of the context box</returns>
        public Rect GetContentRect(Rect rect, GUIState state = GUIState.Normal)
        {
            //Widths of border
            var bt = this.Get<double>(StylePropertyName.BorderTop, state);
            var br = this.Get<double>(StylePropertyName.BorderRight, state);
            var bb = this.Get<double>(StylePropertyName.BorderBottom, state);
            var bl = this.Get<double>(StylePropertyName.BorderLeft, state);

            //Widths of padding
            var pt = this.Get<double>(StylePropertyName.PaddingTop, state);
            var pr = this.Get<double>(StylePropertyName.PaddingRight, state);
            var pb = this.Get<double>(StylePropertyName.PaddingBottom, state);
            var pl = this.Get<double>(StylePropertyName.PaddingLeft, state);

            //4 corner of the border-box
            var btl = new Point(rect.Left, rect.Top);
            var bbr = new Point(rect.Right, rect.Bottom);

            //4 corner of the padding-box
            var ptl = new Point(btl.X + bl, btl.Y + bt);
            var pbr = new Point(bbr.X - br, bbr.Y - bb);

            //4 corner of the content-box
            var ctl = new Point(ptl.X + pl, ptl.Y + pt);
            var cbr = new Point(pbr.X - pr, pbr.Y - pb);
            var contentBoxRect = new Rect(ctl, cbr);
            return contentBoxRect;
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
            get => this.Get<double>(StylePropertyName.MinWidth);
            set => this.Set(StylePropertyName.MinWidth, value);
        }
        public double MaxWidth
        {
            get => this.Get<double>(StylePropertyName.MaxWidth);
            set => this.Set(StylePropertyName.MaxWidth, value);
        }
        public double MinHeight
        {
            get => this.Get<double>(StylePropertyName.MinHeight);
            set => this.Set(StylePropertyName.MinHeight, value);
        }
        public double MaxHeight
        {
            get => this.Get<double>(StylePropertyName.MaxHeight);
            set => this.Set(StylePropertyName.MaxHeight, value);
        }
        #endregion Width & Height

        #region Padding
        public double PaddingTop => this.Get<double>(StylePropertyName.PaddingTop);
        public double PaddingRight => this.Get<double>(StylePropertyName.PaddingRight);
        public double PaddingBottom => this.Get<double>(StylePropertyName.PaddingBottom);
        public double PaddingLeft => this.Get<double>(StylePropertyName.PaddingLeft);
        public double PaddingHorizontal => this.Get<double>(StylePropertyName.PaddingLeft) + this.Get<double>(StylePropertyName.PaddingRight);
        public double PaddingVertical => this.Get<double>(StylePropertyName.PaddingTop) + this.Get<double>(StylePropertyName.PaddingBottom);
        public (double, double, double, double) Padding
        {
            get
            {
                var top = this.Get<double>(StylePropertyName.PaddingTop);
                var right = this.Get<double>(StylePropertyName.PaddingRight);
                var bottom = this.Get<double>(StylePropertyName.PaddingBottom);
                var left = this.Get<double>(StylePropertyName.PaddingLeft);
                return (top, right, bottom, left);
            }

            set
            {
                var (top, right, bottom, left) = value;
                this.Set(StylePropertyName.PaddingTop, top);
                this.Set(StylePropertyName.PaddingRight, right);
                this.Set(StylePropertyName.PaddingBottom, bottom);
                this.Set(StylePropertyName.PaddingLeft, left);
            }
        }
        #endregion Padding

        #region Border
        public double BorderTop => this.Get<double>(StylePropertyName.BorderTop);
        public double BorderRight => this.Get<double>(StylePropertyName.BorderRight);
        public double BorderBottom => this.Get<double>(StylePropertyName.BorderBottom);
        public double BorderLeft => this.Get<double>(StylePropertyName.BorderLeft);
        public double BorderHorizontal => this.Get<double>(StylePropertyName.BorderLeft) + this.Get<double>(StylePropertyName.BorderRight);
        public double BorderVertical => this.Get<double>(StylePropertyName.BorderTop) + this.Get<double>(StylePropertyName.BorderBottom);
        public (double, double, double, double) Border
        {
            get
            {
                var top = this.Get<double>(StylePropertyName.BorderTop);
                var right = this.Get<double>(StylePropertyName.BorderRight);
                var bottom = this.Get<double>(StylePropertyName.BorderBottom);
                var left = this.Get<double>(StylePropertyName.BorderLeft);
                return (top, right, bottom, left);
            }

            set
            {
                var (top, right, bottom, left) = value;
                this.Set(StylePropertyName.BorderTop, top);
                this.Set(StylePropertyName.BorderRight, right);
                this.Set(StylePropertyName.BorderBottom, bottom);
                this.Set(StylePropertyName.BorderLeft, left);
            }
        }

        public Color BorderColor
        {
            get
            {
                var color = this.BorderTopColor;
                if (color == this.BorderRightColor && color == this.BorderBottomColor && color == this.BorderLeftColor)
                {
                    return color;
                }
                throw new InvalidOperationException("Not all border share the same color! So we don't know which one should be returned.");
            }
            set
            {
                this.Set<Color>(StylePropertyName.BorderTopColor, value);
                this.Set<Color>(StylePropertyName.BorderRightColor, value);
                this.Set<Color>(StylePropertyName.BorderBottomColor, value);
                this.Set<Color>(StylePropertyName.BorderLeftColor, value);
            }
        }

        public Color BorderTopColor => this.Get<Color>(StylePropertyName.BorderTopColor);
        public Color BorderRightColor => this.Get<Color>(StylePropertyName.BorderRightColor);
        public Color BorderBottomColor => this.Get<Color>(StylePropertyName.BorderBottomColor);
        public Color BorderLeftColor => this.Get<Color>(StylePropertyName.BorderLeftColor);
        public ITexture BorderImageSource => this.Get<ITexture>(StylePropertyName.BorderImageSource);
        public (double, double, double, double) BorderImageSlice
        {
            get
            {
                var top = this.Get<double>(StylePropertyName.BorderImageSliceTop);
                var right = this.Get<double>(StylePropertyName.BorderImageSliceRight);
                var bottom = this.Get<double>(StylePropertyName.BorderImageSliceBottom);
                var left = this.Get<double>(StylePropertyName.BorderImageSliceLeft);
                return (top, right, bottom, left);
            }

            set
            {
                var (top, right, bottom, left) = value;
                this.Set(StylePropertyName.BorderImageSliceTop, top);
                this.Set(StylePropertyName.BorderImageSliceRight, right);
                this.Set(StylePropertyName.BorderImageSliceBottom, bottom);
                this.Set(StylePropertyName.BorderImageSliceLeft, left);
            }
        }

        public (double topLeft, double topRight, double bottomRight, double bottomLeft) BorderRadius =>
            (
                this.Get<double>(StylePropertyName.BorderTopLeftRadius),
                this.Get<double>(StylePropertyName.BorderTopRightRadius),
                this.Get<double>(StylePropertyName.BorderBottomRightRadius),
                this.Get<double>(StylePropertyName.BorderBottomLeftRadius)
            );

        #endregion Border

        #region Outline
        public double OutlineWidth => this.Get<double>(StylePropertyName.OutlineWidth);
        public Color OutlineColor => this.Get<Color>(StylePropertyName.OutlineColor);
        #endregion Outline

        public Color BackgroundColor
        {
            get => this.Get<Color>(StylePropertyName.BackgroundColor);
            set => this.Set(StylePropertyName.BackgroundColor, value);
        }

        public Gradient BackgroundGradient
        {
            get => (Gradient)this.Get<int>(StylePropertyName.BackgroundGradient);
            set => this.Set(StylePropertyName.BackgroundGradient, (int)value);
        }

        #endregion Box model

        #region Layout

        public int HorizontalStretchFactor
        {
            get => this.Get<int>(StylePropertyName.HorizontalStretchFactor);
            set => this.Set(StylePropertyName.HorizontalStretchFactor, value);
        }

        public int VerticalStretchFactor
        {
            get => this.Get<int>(StylePropertyName.VerticalStretchFactor);
            set => this.Set(StylePropertyName.VerticalStretchFactor, value);
        }

        public double CellSpacingHorizontal
        {
            get => this.Get<double>(StylePropertyName.CellSpacingHorizontal);
            set => this.Set(StylePropertyName.CellSpacingHorizontal, value);
        }
        public double CellSpacingVertical
        {
            get => this.Get<double>(StylePropertyName.CellSpacingVertical);
            set => this.Set(StylePropertyName.CellSpacingVertical, value);
        }
        public (double, double) CellSpacing
        {
            get => (this.CellSpacingHorizontal, this.CellSpacingVertical);
            set { this.CellSpacingHorizontal = value.Item1; this.CellSpacingVertical = value.Item2; }
        }

        public Alignment AlignmentHorizontal
        {
            get => (Alignment)this.Get<int>(StylePropertyName.AlignmentHorizontal);
            set => this.Set(StylePropertyName.AlignmentHorizontal, (int)value);
        }
        public Alignment AlignmentVertical
        {
            get => (Alignment)this.Get<int>(StylePropertyName.AlignmentVertical);
            set => this.Set(StylePropertyName.AlignmentVertical, (int)value);
        }
        public (Alignment, Alignment) GroupAlignment
        {
            get => (this.AlignmentHorizontal, this.AlignmentVertical);
            set { this.AlignmentHorizontal = value.Item1; this.AlignmentVertical = value.Item2; }
        }

        #endregion Layout

        #region Misc

        public Color StrokeColor => this.Get<Color>(StylePropertyName.StrokeColor);
        public Color FillColor => this.Get<Color>(StylePropertyName.FillColor);

        public Color GradientTopColor => this.Get<Color>(StylePropertyName.GradientTopColor);
        public Color GradientBottomColor => this.Get<Color>(StylePropertyName.GradientBottomColor);

        #endregion

        #region Font & Text

        public string FontFamily
        {
            get => this.Get<string>(StylePropertyName.FontFamily);
            set => this.Set(StylePropertyName.FontFamily, value);
        }

        public double FontSize
        {
            get => this.Get<double>(StylePropertyName.FontSize);
            set => this.Set(StylePropertyName.FontSize, value);
        }

        public FontStyle FontStyle//No effect in current Typography.
        {
            get => (FontStyle)this.Get<int>(StylePropertyName.FontStyle);
            set => this.Set(StylePropertyName.FontStyle, (int)value);
        }

        public FontWeight FontWeight//No effect in current Typography.
        {
            get => (FontWeight)this.Get<int>(StylePropertyName.FontWeight);
            set => this.Set(StylePropertyName.FontWeight, (int)value);
        }

        public Color FontColor
        {
            get => this.Get<Color>(StylePropertyName.FontColor);
            set => this.Set(StylePropertyName.FontColor, value);
        }

        #endregion

        #endregion
    }
}
