using System;
using System.Diagnostics;

namespace ImGui
{
    internal class DWriteTextContext : ITextContext
    {
        private readonly DWriteSharp.TextFormat textFormat;
        private DWriteSharp.TextLayout textLayout;
        private string text;

        public DWriteTextContext(string text, string fontFamily, float fontSizeInDip,
            FontStretch stretch, FontStyle style, FontWeight weight,
            int maxWidth, int maxHeight,
            TextAlignment alignment)
        {
            textFormat = DWriteSharp.DWrite.CreateTextFormat(fontFamily,
                (DWriteSharp.FontWeight) weight, (DWriteSharp.FontStyle) style,
                (DWriteSharp.FontStretch) stretch,
                fontSizeInDip);
            this.Alignment = alignment;

            this.text = text;
            textLayout = DWriteSharp.DWrite.CreateTextLayout(text,
                textFormat, maxWidth, maxHeight);
        }

        public string FontFamily
        {
            get { return textFormat.FontFamilyName; }
        }

        public FontWeight FontWeight
        {
            get { return (FontWeight) textLayout.FontWeight; }
        }

        public FontStyle FontStyle
        {
            get { return (FontStyle) textLayout.FontStyle; }
        }

        public FontStretch FontStretch
        {
            get { return (FontStretch) textLayout.FontStretch; }
        }

        public int FontSize
        {
            get { return (int) textFormat.FontSize; }
        }

        public TextAlignment Alignment
        {
            get { return (TextAlignment) textFormat.TextAlignment; }
            set { textFormat.TextAlignment = (DWriteSharp.TextAlignment) value; }
        }

        public int MaxWidth
        {
            get { return textLayout.MaxWidth; }
            set
            {
                if(textLayout.MaxWidth == value)
                {
                    return;
                }
                textLayout.MaxWidth = value;
            }
        }

        public int MaxHeight
        {
            get { return textLayout.MaxHeight; }
            set
            {
                if(textLayout.MaxHeight == value)
                {
                    return;
                }
                textLayout.MaxHeight = value;
            }
        }

        public Rect Rect
        {
            get
            {
                float left, top, width, height;
                textLayout.GetRect(out left, out top, out width, out height);
                return new Rect(left, top, width, height);
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
                var tempMaxWidth = MaxWidth;
                var tempMaxHeight = MaxHeight;
                textLayout.Dispose();
                textLayout = DWriteSharp.DWrite.CreateTextLayout(
                    text,
                    textFormat,
                    tempMaxWidth, tempMaxHeight);
            }
        }

        public void Build(Point offset, TextMesh textMesh)
        {
            var t = textMesh;
            Point lastPoint = Point.Zero;
            DWriteSharp.DWrite.RenderLayoutToMesh(textLayout, (float)offset.X, (float)offset.Y,
                // point adder
                (x, y) =>
                {
                    t.PathLineTo(new Point(x, y));
                    lastPoint = new Point(x, y);
                },

                // bezier adder
                (c0x, c0y, c1x, c1y, p1x, p1y) =>
                {
                    var p0 = lastPoint;//The start point of the cubic Bezier segment.
                    var c0 = new Point(c0x, c0y);//The first control point of the cubic Bezier segment.
                    var p = new Point((c0x + c1x) / 2, (c0y + c1y) / 2);
                    var c1 = new Point(c1x, c1y);//The second control point of the cubic Bezier segment.
                    var p1 = new Point(p1x, p1y);//The end point of the cubic Bezier segment.

                    t.PathAddBezier(p0, c0, p);
                    t.PathAddBezier(p, c1, p1);

                    //set last point for next bezier
                    lastPoint = p1;
                },

                // path closer(dummy)
                () =>
                {
                },

                // figure beginner
                (x, y) =>
                {
                    lastPoint = new Point(x, y);
                    t.PathMoveTo(lastPoint);
                },

                // figure ender
                () =>
                {
                    t.PathClose();
                    t.AddContour(Color.Black);
                    t.PathClear();
                });
        }

        public Size Measure()
        {
            throw new NotImplementedException();
        }

        public uint XyToIndex(float pointX, float pointY, out bool isInside)
        {
            return textLayout.XyToIndex(pointX, pointY, out isInside);
        }

        public void IndexToXY(uint textPosition, bool isTrailingHit, out float pointX, out float pointY,
            out float height)
        {
            textLayout.IndexToXY(textPosition, isTrailingHit, out pointX, out pointY, out height);
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            textFormat.Dispose();
            textLayout.Dispose();
        }


        #endregion
    }
}