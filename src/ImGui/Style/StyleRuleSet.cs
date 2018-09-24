using System.Collections.Generic;
using ImGui.Common.Primitive;
using ImGui.OSAbstraction.Graphics;

namespace ImGui
{
    internal class StyleRuleSet
    {
        private List<IStyleRule> rules;
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

        public void SetState(GUIState state)
        {
            this.currentState = state;
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

        #region short-cuts

        #region Box model
        // We use `box-sizing: border-box;`. So the width and height is the width and height of border-box.
        #region Width & Height
        public double MinWidth
        {
            get => Get<double>(GUIStyleName.MinWidth);
        }
        public double MaxWidth
        {
            get => Get<double>(GUIStyleName.MaxWidth);
        }
        public double MinHeight
        {
            get => Get<double>(GUIStyleName.MinHeight);
        }
        public double MaxHeight
        {
            get => Get<double>(GUIStyleName.MaxHeight);
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
        }
        public (Color top, Color right, Color bottom, Color left) BorderColor
        {
            get =>(
                this.Get<Color>(GUIStyleName.BorderTopColor),
                this.Get<Color>(GUIStyleName.BorderRightColor),
                this.Get<Color>(GUIStyleName.BorderBottomColor),
                this.Get<Color>(GUIStyleName.BorderLeftColor)
            );
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
        public double OutlineWidth => Get<double>(GUIStyleName.OutlineWidth);
        public Color OutlineColor => Get<Color>(GUIStyleName.OutlineColor);
        #endregion Outline

        public Color BackgroundColor
        {
            get => Get<Color>(GUIStyleName.BackgroundColor);
        }

        public Gradient BackgroundGradient
        {
            get => (Gradient)Get<int>(GUIStyleName.BackgroundGradient);
        }

        #endregion Box model

        #region Layout

        public int HorizontalStretchFactor
        {
            get => Get<int>(GUIStyleName.HorizontalStretchFactor);
        }

        public int VerticalStretchFactor
        {
            get => Get<int>(GUIStyleName.VerticalStretchFactor);
        }

        public double CellSpacingHorizontal
        {
            get => Get<double>(GUIStyleName.CellingSpacingHorizontal);
        }
        public double CellSpacingVertical
        {
            get => Get<double>(GUIStyleName.CellingSpacingVertical);
        }
        public (double, double) CellSpacing
        {
            get => (CellSpacingHorizontal, CellSpacingVertical);
        }

        public Alignment AlignmentHorizontal
        {
            get => (Alignment)Get<int>(GUIStyleName.AlignmentHorizontal);
        }
        public Alignment AlignmentVertical
        {
            get => (Alignment)Get<int>(GUIStyleName.AlignmentVertical);
        }
        public (Alignment, Alignment) GroupAlignment
        {
            get => (AlignmentHorizontal, AlignmentVertical);
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
        }

        public double FontSize
        {
            get => Get<double>(GUIStyleName.FontSize);
        }

        public FontStyle FontStyle//No effect in current Typography.
        {
            get => (FontStyle)Get<int>(GUIStyleName.FontStyle);
        }

        public FontWeight FontWeight//No effect in current Typography.
        {
            get => (FontWeight)Get<int>(GUIStyleName.FontWeight);
        }

        public Color FontColor
        {
            get => Get<Color>(GUIStyleName.FontColor);
        }

        #endregion

#endregion

    }
}
