using System.Collections.Generic;
using System.Linq;
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

        private readonly List<IStyleRule> styleRuleStack = new List<IStyleRule>(16);
        public List<IStyleRule> StyleRuleStack => styleRuleStack;

        public StyleRuleSet()
        {
            this.rules = new List<IStyleRule>();
        }

        public void AppendRule<T>(StyleRule<T> rule)
        {
            this.rules.Add(rule);
        }

        public StyleRule<T> GetRule<T>(StylePropertyName name, GUIState state)
        {
            var rule = this.rules.Find(i =>
            {
                var r = i as StyleRule<T>;
                if (r != null && r.Name == name && r.State == state)
                {
                    return true;
                }

                return false;
            }) as StyleRule<T>;

            return rule;
        }

        public StyleRule<T> GetRule<T>(StylePropertyName name)
        {
            var rule = this.rules.Find(i =>
            {
                var r = i as StyleRule<T>;
                if (r != null && r.Name == name && r.State == this.currentState)
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

        public void Set<T>(StylePropertyName styleName, T value)
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

        public void Set<T>(StylePropertyName styleName, T value, GUIState state)
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

        public T Get<T>(StylePropertyName styleName, GUIState state)
        {
            if (GetFromStack<T>(styleName, state, out var value))
            {
                return value;
            }

            var rule = this.GetRule<T>(styleName, state);
            if (rule == null)
            {
                return GUIStyle.Default.Get<T>(styleName, this.currentState);
            }

            return rule.Value;
        }

        public T Get<T>(StylePropertyName styleName)
        {
            if (GetFromStack<T>(styleName, this.currentState, out var value))
            {
                return value;
            }

            var rule = this.GetRule<T>(styleName, this.currentState);
            if (rule == null)
            {
                rule = this.GetRule<T>(styleName, GUIState.Normal);
            }

            if (rule == null)
            {
                return GUIStyle.Default.Get<T>(styleName, this.currentState);
            }

            return rule.Value;
        }

        public void ApplyStack()
        {
            this.styleRuleStack.Clear();
            this.styleRuleStack.AddRange(GUILayout.StyleRuleStack);
        }

        private bool GetFromStack<T>(StylePropertyName styleName, GUIState state, out T value)
        {
            StyleRule<T> normalRule = null;
            foreach (var stackRule in this.styleRuleStack)
            {
                var styleRule = stackRule as StyleRule<T>;
                if (styleRule != null && styleRule.Name == styleName)
                {
                    if (styleRule.State == GUIState.Normal)
                    {
                        normalRule = styleRule;
                    }
                    if (styleRule.State == state)
                    {
                        value = styleRule.Value;
                        return true;
                    }
                }
            }

            if (normalRule != null)
            {
                value = normalRule.Value;
                return true;
            }

            value = default;
            return false;
        }

        #region Options
        
        public void ApplyOptions(LayoutOptions? options)
        {
            this.ApplyStack();

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
            if (options.MinWidth.HasValue && options.MaxWidth.HasValue)
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
            if (options.HorizontalStretchFactor.HasValue)
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
            get => Get<double>(StylePropertyName.MinWidth);
            set => Set<double>(StylePropertyName.MinWidth, value);
        }
        public double MaxWidth
        {
            get => Get<double>(StylePropertyName.MaxWidth);
            set => Set<double>(StylePropertyName.MaxWidth, value);
        }
        public double MinHeight
        {
            get => Get<double>(StylePropertyName.MinHeight);
            set => Set<double>(StylePropertyName.MinHeight, value);
        }
        public double MaxHeight
        {
            get => Get<double>(StylePropertyName.MaxHeight);
            set => Set<double>(StylePropertyName.MaxHeight, value);
        }
        #endregion Width & Height

        #region Padding
        public double PaddingTop => Get<double>(StylePropertyName.PaddingTop);
        public double PaddingRight => Get<double>(StylePropertyName.PaddingRight);
        public double PaddingBottom => Get<double>(StylePropertyName.PaddingBottom);
        public double PaddingLeft => Get<double>(StylePropertyName.PaddingLeft);
        public double PaddingHorizontal => Get<double>(StylePropertyName.PaddingLeft) + Get<double>(StylePropertyName.PaddingRight);
        public double PaddingVertical => Get<double>(StylePropertyName.PaddingTop) + Get<double>(StylePropertyName.PaddingBottom);
        public (double top, double right, double bottom, double left) Padding
        {
            get
            {
                var top = Get<double>(StylePropertyName.PaddingTop);
                var right = Get<double>(StylePropertyName.PaddingRight);
                var bottom = Get<double>(StylePropertyName.PaddingBottom);
                var left = Get<double>(StylePropertyName.PaddingLeft);
                return (top, right, bottom, left);
            }

            set
            {
                var (top, right, bottom, left) = value;
                Set<double>(StylePropertyName.PaddingTop, top);
                Set<double>(StylePropertyName.PaddingRight, right);
                Set<double>(StylePropertyName.PaddingBottom, bottom);
                Set<double>(StylePropertyName.PaddingLeft, left);
            }
        }
        #endregion Padding

        #region Border
        public double BorderTop => Get<double>(StylePropertyName.BorderTop);
        public double BorderRight => Get<double>(StylePropertyName.BorderRight);
        public double BorderBottom => Get<double>(StylePropertyName.BorderBottom);
        public double BorderLeft => Get<double>(StylePropertyName.BorderLeft);
        public double BorderHorizontal => Get<double>(StylePropertyName.BorderLeft) + Get<double>(StylePropertyName.BorderRight);
        public double BorderVertical => Get<double>(StylePropertyName.BorderTop) + Get<double>(StylePropertyName.BorderBottom);
        public (double top, double right, double bottom, double left) Border
        {
            get
            {
                var top = Get<double>(StylePropertyName.BorderTop);
                var right = Get<double>(StylePropertyName.BorderRight);
                var bottom = Get<double>(StylePropertyName.BorderBottom);
                var left = Get<double>(StylePropertyName.BorderLeft);
                return (top, right, bottom, left);
            }

            set
            {
                var (top, right, bottom, left) = value;
                Set<double>(StylePropertyName.BorderTop, top);
                Set<double>(StylePropertyName.BorderRight, right);
                Set<double>(StylePropertyName.BorderBottom, bottom);
                Set<double>(StylePropertyName.BorderLeft, left);
            }
        }
        public (Color top, Color right, Color bottom, Color left) BorderColor
        {
            get => (
                this.Get<Color>(StylePropertyName.BorderTopColor),
                this.Get<Color>(StylePropertyName.BorderRightColor),
                this.Get<Color>(StylePropertyName.BorderBottomColor),
                this.Get<Color>(StylePropertyName.BorderLeftColor)
            );
            set
            {
                this.Set<Color>(StylePropertyName.BorderTopColor, value.top);
                this.Set<Color>(StylePropertyName.BorderRightColor, value.right);
                this.Set<Color>(StylePropertyName.BorderBottomColor, value.bottom);
                this.Set<Color>(StylePropertyName.BorderLeftColor, value.left);
            }
        }

        public Color BorderTopColor => Get<Color>(StylePropertyName.BorderTopColor);
        public Color BorderRightColor => Get<Color>(StylePropertyName.BorderRightColor);
        public Color BorderBottomColor => Get<Color>(StylePropertyName.BorderBottomColor);
        public Color BorderLeftColor => Get<Color>(StylePropertyName.BorderLeftColor);
        public string BorderImageSource
        {
            get { return Get<string>(StylePropertyName.BorderImageSource); }
            set { Set<string>(StylePropertyName.BorderImageSource, value); }
        }
        public (double, double, double, double) BorderImageSlice
        {
            get
            {
                var top = Get<double>(StylePropertyName.BorderImageSliceTop);
                var right = Get<double>(StylePropertyName.BorderImageSliceRight);
                var bottom = Get<double>(StylePropertyName.BorderImageSliceBottom);
                var left = Get<double>(StylePropertyName.BorderImageSliceLeft);
                return (top, right, bottom, left);
            }

            set
            {
                var (top, right, bottom, left) = value;
                Set<double>(StylePropertyName.BorderImageSliceTop, top);
                Set<double>(StylePropertyName.BorderImageSliceRight, right);
                Set<double>(StylePropertyName.BorderImageSliceBottom, bottom);
                Set<double>(StylePropertyName.BorderImageSliceLeft, left);
            }
        }

        public (double TopLeft, double TopRight, double BottomRight, double BottomLeft) BorderRadius
        {
            get => (
                Get<double>(StylePropertyName.BorderTopLeftRadius),
                Get<double>(StylePropertyName.BorderTopRightRadius),
                Get<double>(StylePropertyName.BorderBottomRightRadius),
                Get<double>(StylePropertyName.BorderBottomLeftRadius)
            );
            set
            {
                Set<double>(StylePropertyName.BorderTopLeftRadius, value.TopLeft);
                Set<double>(StylePropertyName.BorderTopRightRadius, value.TopRight);
                Set<double>(StylePropertyName.BorderBottomRightRadius, value.BottomRight);
                Set<double>(StylePropertyName.BorderBottomLeftRadius, value.BottomLeft);
            }
        }
        #endregion Border

        #region Outline
        public double OutlineWidth
        {
            get => this.Get<double>(StylePropertyName.OutlineWidth);
            set => this.Set<double>(StylePropertyName.OutlineWidth, value);
        }

        public Color OutlineColor
        {
            get => this.Get<Color>(StylePropertyName.OutlineColor);
            set => this.Set<Color>(StylePropertyName.OutlineColor, value);
        }
        #endregion Outline

        public Color BackgroundColor
        {
            get => Get<Color>(StylePropertyName.BackgroundColor);
            set => Set<Color>(StylePropertyName.BackgroundColor, value);
        }

        public Gradient BackgroundGradient
        {
            get => (Gradient)Get<int>(StylePropertyName.BackgroundGradient);
            set => Set<int>(StylePropertyName.BackgroundGradient, (int)value);
        }

        #endregion Box model

        #region Layout

        public int HorizontalStretchFactor
        {
            get => Get<int>(StylePropertyName.HorizontalStretchFactor);
            set => Set<int>(StylePropertyName.HorizontalStretchFactor, value);
        }

        public int VerticalStretchFactor
        {
            get => Get<int>(StylePropertyName.VerticalStretchFactor);
            set => Set<int>(StylePropertyName.VerticalStretchFactor, value);
        }

        public double CellSpacingHorizontal
        {
            get => Get<double>(StylePropertyName.CellSpacingHorizontal);
            set => Set<double>(StylePropertyName.CellSpacingHorizontal, value);
        }
        public double CellSpacingVertical
        {
            get => Get<double>(StylePropertyName.CellSpacingVertical);
            set => Set<double>(StylePropertyName.CellSpacingVertical, value);
        }
        public (double, double) CellSpacing
        {
            get => (CellSpacingHorizontal, CellSpacingVertical);
            set { CellSpacingHorizontal = value.Item1; CellSpacingVertical = value.Item2; }
        }

        public Alignment AlignmentHorizontal
        {
            get => (Alignment)Get<int>(StylePropertyName.AlignmentHorizontal);
            set => Set<int>(StylePropertyName.AlignmentHorizontal, (int)value);
        }

        public Alignment AlignmentVertical
        {
            get => (Alignment)Get<int>(StylePropertyName.AlignmentVertical);
            set => Set<int>(StylePropertyName.AlignmentVertical, (int)value);
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


        #region Font & Text

        public string FontFamily
        {
            get => Get<string>(StylePropertyName.FontFamily);
            set => Set<string>(StylePropertyName.FontFamily, value);
        }

        public double FontSize
        {
            get => Get<double>(StylePropertyName.FontSize);
            set => Set<double>(StylePropertyName.FontSize, value);
        }

        public FontStyle FontStyle//No effect in current Typography.
        {
            get => (FontStyle)Get<int>(StylePropertyName.FontStyle);
            set => Set<int>(StylePropertyName.FontStyle, (int)value);
        }

        public FontWeight FontWeight//No effect in current Typography.
        {
            get => (FontWeight)Get<int>(StylePropertyName.FontWeight);
            set => Set<int>(StylePropertyName.FontWeight, (int)value);
        }

        public Color FontColor
        {
            get => Get<Color>(StylePropertyName.FontColor);
            set => Set<Color>(StylePropertyName.FontColor, value);
        }

        public TextAlignment TextAlignment
        {
            get => (TextAlignment) Get<int>(StylePropertyName.TextAlignment);
            set => Set<int>(StylePropertyName.TextAlignment, (int)value);
        }

        public double GetLineHeight()
        {
            var lineHeight = OSImplentation.TypographyTextContext.GetLineHeight(this.FontFamily, this.FontSize);
            lineHeight += this.PaddingVertical + this.BorderVertical;
            return lineHeight;
        }

        #endregion

        #region Pen & Brush

        public Color FillColor
        {
            get => Get<Color>(StylePropertyName.FillColor);
            set => Set<Color>(StylePropertyName.FillColor, value);
        }

        public Color GradientTopColor => Get<Color>(StylePropertyName.GradientTopColor);
        public Color GradientBottomColor => Get<Color>(StylePropertyName.GradientBottomColor);

        public double StrokeWidth
        {
            get => this.Get<double>(StylePropertyName.StrokeWidth);
            set => this.Set<double>(StylePropertyName.StrokeWidth, value);
        }

        public Color StrokeColor
        {
            get => this.Get<Color>(StylePropertyName.StrokeColor);
            set => this.Set<Color>(StylePropertyName.StrokeColor, value);
        }

        #endregion

        #endregion

    }
}
