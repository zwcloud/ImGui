using System;

namespace ImGui
{
    public class PangoTextContext : ITextContext
    {
        private readonly Pango.FontDescription desc;
        private readonly Cairo.Context g;
        private readonly Pango.Layout layout;
        private readonly Cairo.Surface surface; //dummy
        private bool dirty;
        private string text;

        public PangoTextContext(string text, string fontFamily, int fontSize,
            FontStretch stretch, FontStyle style, FontWeight weight,
            int maxWidth, int maxHeight,
            TextAlignment alignment)
        {
            surface = new Cairo.ImageSurface(Cairo.Format.A1, 1, 1);
            g = new Cairo.Context(surface);
            layout = Pango.CairoHelper.CreateLayout(g);
            layout.SetText(text);
            layout.Wrap = Pango.WrapMode.Word;
            this.MaxWidth = maxWidth;
            this.MaxHeight = maxHeight;

            desc = new Pango.FontDescription();
            desc.Family = fontFamily;
            desc.Weight = weight.ToPango();
            desc.Style = style.ToPango();
            desc.Stretch = stretch.ToPango();
            this.FontSize = fontSize;
            this.Alignment = alignment;
        }

        public int MaxWidth
        {
            get { return (int) (layout.Width/Pango.Scale.PangoScale); }
            set
            {
                value = (int) (value*Pango.Scale.PangoScale);
                if (layout.Width == value)
                {
                    return;
                }
                layout.Width = value;
                dirty = true;
            }
        }

        public int MaxHeight
        {
            get { return (int) (layout.Height/Pango.Scale.PangoScale); }
            set
            {
                value = (int) (value*Pango.Scale.PangoScale);
                if (layout.Height == value)
                {
                    return;
                }
                layout.Height = value;
                dirty = true;
            }
        }

        public Rect Rect
        {
            get
            {
                Pango.Rectangle inkRectangle, logicRectangle;
                layout.GetExtents(out inkRectangle, out logicRectangle);
                var x = logicRectangle.X/Pango.Scale.PangoScale;
                var y = logicRectangle.Y/Pango.Scale.PangoScale;
                var width = logicRectangle.Width/Pango.Scale.PangoScale;
                var height = logicRectangle.Height/Pango.Scale.PangoScale;
                var rect = new Rect(x, y, width, height);
                return rect;
            }
        }

        public string Text
        {
            get
            {
                if(text == null)
                {
                    return string.Empty;
                }
                return text;
            }
            set
            {
                if(value == null)
                {
                    throw new ArgumentNullException();
                }
                if(text == value)
                {
                    return;
                }

                text = value;
                layout.SetText(value);
                dirty = true;
            }
        }

        public Cairo.Path Path { get; private set; }

        public void BuildPath(Cairo.Context context)
        {
            if(Path == null || dirty)
            {
                if(Path != null && dirty)
                {
                    Path.Dispose();
                }
                Pango.CairoHelper.LayoutPath(context, layout);
                Path = context.CopyPath();
                context.NewPath();
            }
        }

        public uint XyToIndex(float pointX, float pointY, out bool isInside)
        {
            int index, trailing;
			isInside = layout.XyToIndex(
				(int)(pointX * Pango.Scale.PangoScale),
                (int)(pointY * Pango.Scale.PangoScale),
                out index, out trailing);
            return (uint) index;
        }

        public void IndexToXY(uint textPosition, bool isTrailingHit, out float pointX, out float pointY,
            out float height)
        {
            Pango.Rectangle strongRect, weakRect;
            layout.GetCursorPos((int) textPosition, out strongRect, out weakRect);
			pointX = (int)(weakRect.X/Pango.Scale.PangoScale);
			pointY = (int)(weakRect.Y/Pango.Scale.PangoScale);
			height = (int)(weakRect.Height/Pango.Scale.PangoScale);
        }

        public int FontSize
        {
            get { return (int) (desc.Size/Pango.Scale.PangoScale); }
            set { desc.Size = (int) (value*Pango.Scale.PangoScale); }
        }

        public TextAlignment Alignment
        {
            get { return layout.Alignment.FromPango(); }
            set { layout.Alignment = value.ToPango(); }
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            desc.Dispose();
            layout.Dispose();
            surface.Dispose();
            g.Dispose();
        }

        #endregion
    }

    internal static class PangoEx
    {
        public static Pango.Weight ToPango(this FontWeight weight)
        {
            switch (weight)
            {
                case FontWeight.Thin:
                    return Pango.Weight.Thin;
                case FontWeight.ExtraLight:
                    return Pango.Weight.Ultralight;
                case FontWeight.Light:
                    return Pango.Weight.Light;
                case FontWeight.SemiLight:
                    return Pango.Weight.Semilight;
                case FontWeight.Normal:
                    return Pango.Weight.Normal;
                case FontWeight.Medium:
                    return Pango.Weight.Medium;
                case FontWeight.DemiBold:
                    return Pango.Weight.Semibold;
                case FontWeight.Bold:
                    return Pango.Weight.Bold;
                case FontWeight.ExtraBold:
                    return Pango.Weight.Ultrabold;
                case FontWeight.Black:
                    return Pango.Weight.Heavy;
                case FontWeight.ExtraBlack:
                    return Pango.Weight.Ultraheavy;
                default:
                    throw new ArgumentOutOfRangeException("weight", weight, null);
            }
        }

        public static Pango.Style ToPango(this FontStyle style)
        {
            switch (style)
            {
                case FontStyle.Normal:
                    return Pango.Style.Normal;
                case FontStyle.Oblique:
                    return Pango.Style.Oblique;
                case FontStyle.Italic:
                    return Pango.Style.Italic;
                default:
                    throw new ArgumentOutOfRangeException("style", style, null);
            }
        }

        public static Pango.Stretch ToPango(this FontStretch stretch)
        {
            switch (stretch)
            {
                case FontStretch.Undefined:
                    return Pango.Stretch.Normal;
                case FontStretch.UltraCondensed:
                    return Pango.Stretch.UltraCondensed;
                case FontStretch.ExtraCondensed:
                    return Pango.Stretch.ExtraCondensed;
                case FontStretch.Condensed:
                    return Pango.Stretch.Condensed;
                case FontStretch.SemiCondensed:
                    return Pango.Stretch.SemiCondensed;
                case FontStretch.Normal:
                    return Pango.Stretch.Normal;
                case FontStretch.SemiExpanded:
                    return Pango.Stretch.SemiExpanded;
                case FontStretch.Expanded:
                    return Pango.Stretch.Expanded;
                case FontStretch.ExtraExpanded:
                    return Pango.Stretch.ExtraExpanded;
                case FontStretch.UltraExpanded:
                    return Pango.Stretch.UltraExpanded;
                default:
                    throw new ArgumentOutOfRangeException("stretch", stretch, null);
            }
        }

        public static Pango.Alignment ToPango(this TextAlignment alignment)
        {
            switch (alignment)
            {
                case TextAlignment.Leading:
                    return Pango.Alignment.Left;
                case TextAlignment.Trailing:
                    return Pango.Alignment.Right;
                case TextAlignment.Center:
                    return Pango.Alignment.Center;
                case TextAlignment.Justified:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException("alignment", alignment, null);
            }
        }

        public static TextAlignment FromPango(this Pango.Alignment alignment)
        {
            switch (alignment)
            {
                case Pango.Alignment.Left:
                    return TextAlignment.Leading;
                case Pango.Alignment.Right:
                    return TextAlignment.Trailing;
                case Pango.Alignment.Center:
                    return TextAlignment.Center;
                default:
                    throw new ArgumentOutOfRangeException("alignment", alignment, null);
            }
        }
    }
}