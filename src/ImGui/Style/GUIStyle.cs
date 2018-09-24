using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using ImGui.Common.Primitive;
using ImGui.OSAbstraction.Graphics;

namespace ImGui
{
    internal partial class GUIStyle
    {
        private struct NameState
        {
            public GUIStyleName Name { get; set; }
            public GUIState State { get; set; }
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
                DefaultFontSize = 12;
                DefaultFontFamily = Utility.FontDir + "msjh.ttf";
            }

            Basic = Default = CreateDefaultStyle();
        }

        private static GUIStyle CreateDefaultStyle()
        {
            var style = new GUIStyle(true);

            style.numberStyles = new Dictionary<NameState, double>
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
                [new NameState { Name = GUIStyleName.BorderTopLeftRadius, State = GUIState.Normal }] = 0,
                [new NameState { Name = GUIStyleName.BorderTopLeftRadius, State = GUIState.Hover }] = 0,
                [new NameState { Name = GUIStyleName.BorderTopLeftRadius, State = GUIState.Active }] = 0,
                [new NameState { Name = GUIStyleName.BorderTopRightRadius, State = GUIState.Normal }] = 0,
                [new NameState { Name = GUIStyleName.BorderTopRightRadius, State = GUIState.Hover }] = 0,
                [new NameState { Name = GUIStyleName.BorderTopRightRadius, State = GUIState.Active }] = 0,
                [new NameState { Name = GUIStyleName.BorderBottomRightRadius, State = GUIState.Normal }] = 0,
                [new NameState { Name = GUIStyleName.BorderBottomRightRadius, State = GUIState.Hover }] = 0,
                [new NameState { Name = GUIStyleName.BorderBottomRightRadius, State = GUIState.Active }] = 0,
                [new NameState { Name = GUIStyleName.BorderBottomLeftRadius, State = GUIState.Normal }] = 0,
                [new NameState { Name = GUIStyleName.BorderBottomLeftRadius, State = GUIState.Hover }] = 0,
                [new NameState { Name = GUIStyleName.BorderBottomLeftRadius, State = GUIState.Active }] = 0,

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

                [new NameState { Name = GUIStyleName.MinTextureCoordinateU, State = GUIState.Normal }] = 0,
                [new NameState { Name = GUIStyleName.MinTextureCoordinateU, State = GUIState.Hover }] = 0,
                [new NameState { Name = GUIStyleName.MinTextureCoordinateU, State = GUIState.Active }] = 0,
                [new NameState { Name = GUIStyleName.MinTextureCoordinateV, State = GUIState.Normal }] = 0,
                [new NameState { Name = GUIStyleName.MinTextureCoordinateV, State = GUIState.Hover }] = 0,
                [new NameState { Name = GUIStyleName.MinTextureCoordinateV, State = GUIState.Active }] = 0,

                [new NameState { Name = GUIStyleName.MaxTextureCoordinateU, State = GUIState.Normal }] = 1,
                [new NameState { Name = GUIStyleName.MaxTextureCoordinateU, State = GUIState.Hover }] = 1,
                [new NameState { Name = GUIStyleName.MaxTextureCoordinateU, State = GUIState.Active }] = 1,
                [new NameState { Name = GUIStyleName.MaxTextureCoordinateV, State = GUIState.Normal }] = 1,
                [new NameState { Name = GUIStyleName.MaxTextureCoordinateV, State = GUIState.Hover }] = 1,
                [new NameState { Name = GUIStyleName.MaxTextureCoordinateV, State = GUIState.Active }] = 1,
            };

            style.colorStyles = new Dictionary<NameState, Color>
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

                [new NameState { Name = GUIStyleName.FontColor, State = GUIState.Normal }] = Color.Black,
                [new NameState { Name = GUIStyleName.FontColor, State = GUIState.Hover }] = Color.Black,
                [new NameState { Name = GUIStyleName.FontColor, State = GUIState.Active }] = Color.Black,

                [new NameState { Name = GUIStyleName.GradientLeftColor, State = GUIState.Normal }] = Color.Rgb(247, 247, 247),
                [new NameState { Name = GUIStyleName.GradientLeftColor, State = GUIState.Hover }] = Color.Rgb(247, 247, 247),
                [new NameState { Name = GUIStyleName.GradientLeftColor, State = GUIState.Active }] = Color.Rgb(247, 247, 247),

                [new NameState { Name = GUIStyleName.GradientTopColor, State = GUIState.Normal }] = Color.Rgb(247, 247, 247),
                [new NameState { Name = GUIStyleName.GradientTopColor, State = GUIState.Hover }] = Color.Rgb(247, 247, 247),
                [new NameState { Name = GUIStyleName.GradientTopColor, State = GUIState.Active }] = Color.Rgb(247, 247, 247),

                [new NameState { Name = GUIStyleName.GradientRightColor, State = GUIState.Normal }] = Color.Rgb(221, 221, 221),
                [new NameState { Name = GUIStyleName.GradientRightColor, State = GUIState.Hover }] = Color.Rgb(221, 221, 221),
                [new NameState { Name = GUIStyleName.GradientRightColor, State = GUIState.Active }] = Color.Rgb(221, 221, 221),

                [new NameState { Name = GUIStyleName.GradientBottomColor, State = GUIState.Normal }] = Color.Rgb(221, 221, 221),
                [new NameState { Name = GUIStyleName.GradientBottomColor, State = GUIState.Hover }] = Color.Rgb(221, 221, 221),
                [new NameState { Name = GUIStyleName.GradientBottomColor, State = GUIState.Active }] = Color.Rgb(221, 221, 221),
            };

            style.imageStyles = new Dictionary<NameState, ITexture>
            {
                [new NameState { Name = GUIStyleName.BorderImageSource, State = GUIState.Normal }] = null,
                [new NameState { Name = GUIStyleName.BorderImageSource, State = GUIState.Hover }] = null,
                [new NameState { Name = GUIStyleName.BorderImageSource, State = GUIState.Active }] = null,

                [new NameState { Name = GUIStyleName.BackgroundImage, State = GUIState.Normal }] = null,
                [new NameState { Name = GUIStyleName.BackgroundImage, State = GUIState.Hover }] = null,
                [new NameState { Name = GUIStyleName.BackgroundImage, State = GUIState.Active }] = null,

            };

            style.intStyles = new Dictionary<NameState, int>
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

                [new NameState { Name = GUIStyleName.BackgroundGradient, State = GUIState.Normal }] = (int)Gradient.None,
                [new NameState { Name = GUIStyleName.BackgroundGradient, State = GUIState.Hover }] = (int)Gradient.None,
                [new NameState { Name = GUIStyleName.BackgroundGradient, State = GUIState.Active }] = (int)Gradient.None,
            };

            style.strStyles = new Dictionary<NameState, string>
            {
                [new NameState { Name = GUIStyleName.FontFamily, State = GUIState.Normal }] = DefaultFontFamily,
                [new NameState { Name = GUIStyleName.FontFamily, State = GUIState.Hover }] = DefaultFontFamily,
                [new NameState { Name = GUIStyleName.FontFamily, State = GUIState.Active }] = DefaultFontFamily,
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

        public T Get<T>(GUIStyleName styleName, GUIState state = GUIState.Normal)
        {
            T value;
            var nameState = new NameState { Name = styleName, State = state };
            var dict = GetDict<T>();

            //try get the style value of specified state
            if (dict.TryGetValue(nameState, out value))
            {
                return value;
            }

            if(state != GUIState.Normal)
            {
                //try to get the style value of Normal state
                if (dict.TryGetValue(new NameState { Name = styleName, State = GUIState.Normal }, out value))
                {
                    return value;
                }
            }

            var defalutDict = Default.GetDict<T>();

            //try to get a default value of  specified state
            if (defalutDict.TryGetValue(nameState, out value))
            {
                return value;
            }

            if (state != GUIState.Normal)
            {
                // try to get a default value of Normal state
                if (defalutDict.TryGetValue(new NameState { Name = styleName, State = GUIState.Normal }, out value))
                {
                    return value;
                }
            }

            Log.Warning("Cannot find the style<{0},{1}>", styleName.ToString(), state.ToString());
            return default(T);//still not found, get defalut value of that types //FIXME may we should throw an exception instead?
        }

        public void Set<T>(GUIStyleName styleName, T value, GUIState state = GUIState.Normal)
        {
            var nameState = new NameState { Name = styleName, State = state };
            var dict = GetDict<T>();
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

            var paddingHorizontal = Get<double>(GUIStyleName.PaddingLeft) + Get<double>(GUIStyleName.PaddingRight);
            var borderHorizontal = Get<double>(GUIStyleName.BorderLeft) + Get<double>(GUIStyleName.BorderRight);
            var paddingVertical = Get<double>(GUIStyleName.PaddingTop) + Get<double>(GUIStyleName.PaddingBottom);
            var borderVertical = Get<double>(GUIStyleName.BorderTop) + Get<double>(GUIStyleName.BorderBottom);
            size.Width += paddingHorizontal + borderHorizontal;
            size.Height += paddingVertical + borderVertical;
            return size;
        }

        internal double GetLineHeight()
        {
            var lineHeight = OSImplentation.TypographyTextContext.GetLineHeight(this.FontFamily, this.FontSize);
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
            var bt = this.Get<double>(GUIStyleName.BorderTop, state);
            var br = this.Get<double>(GUIStyleName.BorderRight, state);
            var bb = this.Get<double>(GUIStyleName.BorderBottom, state);
            var bl = this.Get<double>(GUIStyleName.BorderLeft, state);

            //Widths of padding
            var pt = this.Get<double>(GUIStyleName.PaddingTop, state);
            var pr = this.Get<double>(GUIStyleName.PaddingRight, state);
            var pb = this.Get<double>(GUIStyleName.PaddingBottom, state);
            var pl = this.Get<double>(GUIStyleName.PaddingLeft, state);

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
        public Color BorderTopColor => Get<Color>(GUIStyleName.BorderTopColor);
        public Color BorderRightColor => Get<Color>(GUIStyleName.BorderRightColor);
        public Color BorderBottomColor => Get<Color>(GUIStyleName.BorderBottomColor);
        public Color BorderLeftColor => Get<Color>(GUIStyleName.BorderLeftColor);
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

        public (double topLeft, double topRight, double bottomRight, double bottomLeft) BorderRadius =>
            (
                Get<double>(GUIStyleName.BorderTopLeftRadius),
                Get<double>(GUIStyleName.BorderTopRightRadius),
                Get<double>(GUIStyleName.BorderBottomRightRadius),
                Get<double>(GUIStyleName.BorderBottomLeftRadius)
            );

        /// <summary>
        /// Get the border color. Throws InvalidOperationException when all borders' colors are not the same.
        /// </summary>
        public Color GetBorderColor(GUIState state)
        {
            //TODO check if all border color under state is the same
            return this.Get<Color>(GUIStyleName.BorderTopColor, state);
        }
        #endregion Border

        #region Outline
        public double OutlineWidth => Get<double>(GUIStyleName.OutlineWidth);
        public Color OutlineColor => Get<Color>(GUIStyleName.OutlineColor);
        #endregion Outline

        public Color BackgroundColor
        {
            get => Get<Color>(GUIStyleName.BackgroundColor);
            set => Set<Color>(GUIStyleName.BackgroundColor, value);
        }

        public Gradient BackgroundGradient
        {
            get => (Gradient)Get<int>(GUIStyleName.BackgroundGradient);
            set => Set<int>(GUIStyleName.BackgroundGradient, (int)value);
        }

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

        public double CellSpacingHorizontal
        {
            get => Get<double>(GUIStyleName.CellingSpacingHorizontal);
            set => Set<double>(GUIStyleName.CellingSpacingHorizontal, value);
        }
        public double CellSpacingVertical
        {
            get => Get<double>(GUIStyleName.CellingSpacingVertical);
            set => Set<double>(GUIStyleName.CellingSpacingVertical, value);
        }
        public (double, double) CellSpacing
        {
            get => (CellSpacingHorizontal, CellSpacingVertical);
            set { CellSpacingHorizontal = value.Item1; CellSpacingVertical = value.Item2; }
        }

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
        public (Alignment, Alignment) GroupAlignment
        {
            get => (AlignmentHorizontal, AlignmentVertical);
            set { AlignmentHorizontal = value.Item1; AlignmentVertical = value.Item2; }
        }

        #endregion Layout

        #region Misc

        public Color StrokeColor => Get<Color>(GUIStyleName.StrokeColor);
        public Color FillColor => Get<Color>(GUIStyleName.FillColor);

        public Color GradientTopColor => Get<Color>(GUIStyleName.GradientTopColor);
        public Color GradientBottomColor => Get<Color>(GUIStyleName.GradientBottomColor);

        #endregion

        #region Font & Text

        public string FontFamily
        {
            get => Get<string>(GUIStyleName.FontFamily);
            set => Set<string>(GUIStyleName.FontFamily, value);
        }

        public double FontSize
        {
            get => Get<double>(GUIStyleName.FontSize);
            set => Set<double>(GUIStyleName.FontSize, value);
        }

        public FontStyle FontStyle//No effect in current Typography.
        {
            get => (FontStyle)Get<int>(GUIStyleName.FontStyle);
            set => Set<int>(GUIStyleName.FontStyle, (int)value);
        }

        public FontWeight FontWeight//No effect in current Typography.
        {
            get => (FontWeight)Get<int>(GUIStyleName.FontWeight);
            set => Set<int>(GUIStyleName.FontWeight, (int)value);
        }

        public Color FontColor
        {
            get => Get<Color>(GUIStyleName.FontColor);
            set => Set<Color>(GUIStyleName.FontColor, value);
        }

        #endregion

#endregion
    }
}
