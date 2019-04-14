using System;
using System.Collections.Generic;
using ImGui.OSAbstraction.Graphics;
using ImGui.Rendering;

namespace ImGui
{
    internal interface IStyleRuleSet
    {
        StyleRuleSet RuleSet { get; }
    }

    internal class StyleRuleSet
    {
        //for test
        public Node refNode;

        private readonly List<IStyleRule> rules;
        private GUIState currentState = GUIState.Normal;

        public StyleRuleSet()
        {
            this.rules = new List<IStyleRule>();
        }

        public void AppendRule<T>(StyleRule<T> rule)
        {
            this.rules.Add(rule);
        }

        public StyleRule<T> GetRule<T>(GUIStyleName name, GUIState state)
        {
            var rule = this.rules.Find(i =>
            {
                var r = i as StyleRule<T>;
                if (r !=null && r.Name == name && r.State == state)
                {
                    return true;
                }

                return false;
            }) as StyleRule<T>;

            return rule;
        }

        public StyleRule<T> GetRule<T>(GUIStyleName name)
        {
            var rule = this.rules.Find(i =>
            {
                var r = i as StyleRule<T>;
                if (r !=null && r.Name == name && r.State == this.currentState)
                {
                    return true;
                }

                return false;
            }) as StyleRule<T>;

            return rule;
        }

        public void SetState(GUIState state)
        {
            this.currentState = state;
        }

        public void Set<T>(GUIStyleName styleName, T value)
        {
            var rule = this.GetRule<T>(styleName, this.currentState);
            if (rule == null)
            {
                rule = new StyleRule<T>(styleName, value);
                this.AppendRule(rule);
            }
            else
            {
                rule.Value = value;
            }
        }

        internal void Replace(StyleRuleSet styleRuleSet)
        {
            this.rules.Clear();
            this.rules.AddRange(styleRuleSet.rules);
        }

        public void Set<T>(GUIStyleName styleName, T value, GUIState state)
        {
            var rule = this.GetRule<T>(styleName, state);
            if (rule == null)
            {
                rule = new StyleRule<T>(styleName, value, state);
                this.AppendRule(rule);
            }
            else
            {
                rule.Value = value;
            }
        }

        public T Get<T>(GUIStyleName styleName, GUIState state)
        {
            var rule = this.GetRule<T>(styleName, state);
            if (rule == null)
            {
                return GUIStyle.Default.Get<T>(styleName, this.currentState);
            }

            return rule.Value;
        }

        public T Get<T>(GUIStyleName styleName)
        {
            var rule = this.GetRule<T>(styleName, this.currentState);
            if (rule == null)
            {
                return GUIStyle.Default.Get<T>(styleName, this.currentState);
            }

            return rule.Value;
        }

        #region Options

        public void ApplyOptions(LayoutOptions? options)
        {
            if (options.HasValue)
            {
                ApplyOptions(options.Value);
            }
        }

        /// <summary>
        /// Apply style options to this rule set.
        /// </summary>
        /// <param name="options"></param>
        /// TODO refactor
        public void ApplyOptions(LayoutOptions options)
        {
            if(options.MinWidth.HasValue && options.MaxWidth.HasValue)
            {
                var value = options.MinWidth.Value;
                if (value < this.PaddingHorizontal + this.BorderHorizontal)
                {
                    throw new LayoutException(
                        $"The specified width is too small. It must bigger than the horizontal padding and border size ({this.PaddingHorizontal + this.BorderHorizontal}).");
                }
                this.MinWidth = this.MaxWidth = value;
                this.HorizontalStretchFactor = 0;
            }
            if (options.MinHeight.HasValue && options.MaxHeight.HasValue)
            {
                var value = options.MinHeight.Value;
                if (value < this.PaddingVertical + this.BorderVertical)
                {
                    throw new LayoutException(
                        $"The specified height is too small. It must bigger than the vertical padding and border size ({this.PaddingVertical + this.BorderVertical}).");
                }
                this.MinHeight = this.MaxHeight = value;
                this.VerticalStretchFactor = 0;
            }
            if(options.HorizontalStretchFactor.HasValue)
            {
                this.HorizontalStretchFactor = options.HorizontalStretchFactor.Value;
            }
            if (options.VerticalStretchFactor.HasValue)
            {
                this.VerticalStretchFactor = options.VerticalStretchFactor.Value;
            }
            if (options.textAlignment.HasValue)
            {
                this.TextAlignment = options.textAlignment.Value;
            }
        }

        #endregion

        #region short-cuts

        #region Box model
        // We use `box-sizing: border-box;`. So the width and height is the width and height of border-box.
        #region Width & Height

        //TODO limit the range of min/max width/height
        //TODO ensure correct values of min/max width/height: min value must <= max value

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
        public (double top, double right, double bottom, double left) Padding
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
        public (double top, double right, double bottom, double left) Border
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
        public (Color top, Color right, Color bottom, Color left) BorderColor
        {
            get =>(
                this.Get<Color>(GUIStyleName.BorderTopColor),
                this.Get<Color>(GUIStyleName.BorderRightColor),
                this.Get<Color>(GUIStyleName.BorderBottomColor),
                this.Get<Color>(GUIStyleName.BorderLeftColor)
            );
            set
            {
                this.Set<Color>(GUIStyleName.BorderTopColor, value.top);
                this.Set<Color>(GUIStyleName.BorderRightColor, value.right);
                this.Set<Color>(GUIStyleName.BorderBottomColor, value.bottom);
                this.Set<Color>(GUIStyleName.BorderLeftColor, value.left);
            }
        }

        public Color BorderTopColor => Get<Color>(GUIStyleName.BorderTopColor);
        public Color BorderRightColor => Get<Color>(GUIStyleName.BorderRightColor);
        public Color BorderBottomColor => Get<Color>(GUIStyleName.BorderBottomColor);
        public Color BorderLeftColor => Get<Color>(GUIStyleName.BorderLeftColor);
        public string BorderImageSource => Get<string>(GUIStyleName.BorderImageSource);
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

        #endregion Border

        #region Outline
        public double OutlineWidth
        {
            get => this.Get<double>(GUIStyleName.OutlineWidth);
            set => this.Set<double>(GUIStyleName.OutlineWidth, value);
        }

        public Color OutlineColor
        {
            get => this.Get<Color>(GUIStyleName.OutlineColor);
            set => this.Set<Color>(GUIStyleName.OutlineColor, value);
        }
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
            get => Get<double>(GUIStyleName.CellSpacingHorizontal);
            set => Set<double>(GUIStyleName.CellSpacingHorizontal, value);
        }
        public double CellSpacingVertical
        {
            get => Get<double>(GUIStyleName.CellSpacingVertical);
            set => Set<double>(GUIStyleName.CellSpacingVertical, value);
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

        /// <summary>
        /// Is this entry or group horizontally stretched?
        /// </summary>
        public bool HorizontallyStretched => !this.IsFixedWidth && this.HorizontalStretchFactor > 0;

        /// <summary>
        /// Is this entry or group vertically stretched?
        /// </summary>
        public bool VerticallyStretched => !this.IsFixedHeight && this.VerticalStretchFactor > 0;

        /// <summary>
        /// Is this entry or group has a fixed width?
        /// </summary>
        public bool IsFixedWidth => MathEx.AmostEqual(this.MinWidth, this.MaxWidth);

        /// <summary>
        /// Is this entry or group has a fixed height?
        /// </summary>
        public bool IsFixedHeight => MathEx.AmostEqual(this.MinHeight, this.MaxHeight);

        public bool IsDefaultWidth => !IsFixedWidth && !HorizontallyStretched;

        public bool IsDefaultHeight => !IsFixedHeight && !VerticallyStretched;

        public bool IsStretchedWidth => HorizontallyStretched;

        public bool IsStretchedHeight => VerticallyStretched;

        #endregion Layout

        #region Misc

        public Color StrokeColor => Get<Color>(GUIStyleName.StrokeColor);
        public double StrokeWidth => Get<double>(GUIStyleName.StrokeWidth);

        public Color FillColor
        {
            get => Get<Color>(GUIStyleName.FillColor);
            set => Set<Color>(GUIStyleName.FillColor, value);
        }

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

        public TextAlignment TextAlignment
        {
            get => (TextAlignment) Get<int>(GUIStyleName.TextAlignment);
            set => Set<int>(GUIStyleName.TextAlignment, (int)value);
        }

        public double GetLineHeight()
        {
            var lineHeight = OSImplentation.TypographyTextContext.GetLineHeight(this.FontFamily, this.FontSize);
            lineHeight += this.PaddingVertical + this.BorderVertical;
            return lineHeight;
        }

        #endregion

        #endregion

    }
}
