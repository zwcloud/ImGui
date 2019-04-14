using ImGui.GraphicsAbstraction;
using ImGui.Rendering;
using System;
using System.Diagnostics;

namespace ImGui.GraphicsImplementation
{
    internal partial class BuiltinGeometryRenderer : IGeometryRenderer
    {
        public void DrawBoxModel(Rect rect, StyleRuleSet style)
        {
            GetBoxes(rect, style, out var borderBoxRect, out var paddingBoxRect, out var contentBoxRect);

            this.DrawBackground(style, paddingBoxRect);

            //Content
            //Content-box
            //no content

            this.DrawBorder(style, borderBoxRect, paddingBoxRect);
            this.DrawOutline(style, borderBoxRect);

            this.DrawDebug(paddingBoxRect, contentBoxRect);
        }

        public void DrawBoxModel(TextGeometry textGeometry, Rect rect, StyleRuleSet style)
        {
            GetBoxes(rect, style, out var borderBoxRect, out var paddingBoxRect, out var contentBoxRect);

            this.DrawBackground(style, paddingBoxRect);

            //Content
            //Content-box
            if (contentBoxRect.TopLeft.X < contentBoxRect.TopRight.X)//content should not be visible when contentBoxRect.TopLeft.X > contentBoxRect.TopRight.X
            {
                if (textGeometry != null)
                {
                    //var textSize = style.CalcSize(text);
                    /*HACK Don't check text size because the size calculated by Typography is not accurate. */
                    /*if (textSize.Height < contentBoxRect.Height && textSize.Width < contentBoxRect.Width)*/
                    {
                        this.DrawText(textGeometry, contentBoxRect, style);
                    }
                }
            }

            this.DrawBorder(style, borderBoxRect, paddingBoxRect);
            this.DrawOutline(style, borderBoxRect);

            this.DrawDebug(paddingBoxRect, contentBoxRect);
        }

        public void DrawBoxModel(ImageGeometry imageGeometry, Rect rect, StyleRuleSet style)
        {
            GetBoxes(rect, style, out var borderBoxRect, out var paddingBoxRect, out var contentBoxRect);

            this.DrawBackground(style, paddingBoxRect);

            //Content
            //Content-box
            if (contentBoxRect.TopLeft.X < contentBoxRect.TopRight.X)//content should not be visible when contentBoxRect.TopLeft.X > contentBoxRect.TopRight.X
            {
                if (imageGeometry != null)
                {
                    this.DrawImage(imageGeometry, contentBoxRect, style);
                }
            }

            this.DrawBorder(style, borderBoxRect, paddingBoxRect);
            this.DrawOutline(style, borderBoxRect);

            this.DrawDebug(paddingBoxRect, contentBoxRect);
        }

        private void DrawOutline(StyleRuleSet style, Rect borderBoxRect)
        {
            var outlineWidth = style.Get<double>(GUIStyleName.OutlineWidth);
            if (!MathEx.AmostZero(outlineWidth))
            {
                var outlineColor = style.Get<Color>(GUIStyleName.OutlineColor);
                if (!MathEx.AmostZero(outlineColor.A))
                {
                    this.PathRect(borderBoxRect.TopLeft, borderBoxRect.BottomRight);
                    this.PathStroke(outlineColor, true, outlineWidth);
                }
            }
        }

        private void DrawBorder(StyleRuleSet style, Rect borderBoxRect, Rect paddingBoxRect)
        {
            // draw border between border-box and padding-box
            var borderImageSource = style.BorderImageSource;
            if (borderImageSource != null)
            {
                var rule = style.GetRule<string>(GUIStyleName.BorderImageSource);
                if (rule.Geometry == null)
                {
                    rule.Geometry = new ImageGeometry(borderImageSource);
                }

                Debug.Assert(rule.Geometry is ImageGeometry);
                this.DrawSlicedImage((ImageGeometry) rule.Geometry, borderBoxRect, style);
            }
            else
            {
                //  Top
                if (!MathEx.AmostZero(borderBoxRect.Top))
                {
                    var borderTopColor = style.Get<Color>(GUIStyleName.BorderTopColor);
                    if (!MathEx.AmostZero(borderTopColor.A))
                    {
                        this.PathLineTo(paddingBoxRect.TopLeft);
                        this.PathLineTo(borderBoxRect.TopLeft);
                        this.PathLineTo(borderBoxRect.TopRight);
                        this.PathLineTo(paddingBoxRect.TopRight);
                        this.PathFill(borderTopColor);
                    }
                }

                //  Right
                if (!MathEx.AmostZero(borderBoxRect.Right))
                {
                    var borderRightColor = style.Get<Color>(GUIStyleName.BorderRightColor);
                    if (!MathEx.AmostZero(borderRightColor.A))
                    {
                        this.PathLineTo(paddingBoxRect.TopRight);
                        this.PathLineTo(borderBoxRect.TopRight);
                        this.PathLineTo(borderBoxRect.BottomRight);
                        this.PathLineTo(paddingBoxRect.BottomRight);
                        this.PathFill(borderRightColor);
                    }
                }

                //  Bottom
                if (!MathEx.AmostZero(borderBoxRect.Bottom))
                {
                    var borderBottomColor = style.Get<Color>(GUIStyleName.BorderBottomColor);
                    if (!MathEx.AmostZero(borderBottomColor.A))
                    {
                        this.PathLineTo(paddingBoxRect.BottomRight);
                        this.PathLineTo(borderBoxRect.BottomRight);
                        this.PathLineTo(borderBoxRect.BottomLeft);
                        this.PathLineTo(paddingBoxRect.BottomLeft);
                        this.PathFill(borderBottomColor);
                    }
                }

                //  Left
                if (!MathEx.AmostZero(borderBoxRect.Left))
                {
                    var borderLeftColor = style.Get<Color>(GUIStyleName.BorderLeftColor);
                    if (!MathEx.AmostZero(borderLeftColor.A))
                    {
                        this.PathLineTo(paddingBoxRect.BottomLeft);
                        this.PathLineTo(borderBoxRect.BottomLeft);
                        this.PathLineTo(borderBoxRect.TopLeft);
                        this.PathLineTo(paddingBoxRect.TopLeft);
                        this.PathFill(borderLeftColor);
                    }
                }
            }
        }

        private void DrawBackground(StyleRuleSet style, Rect paddingBoxRect)
        {
            // draw background in padding-box
            var gradient = (Gradient) style.Get<int>(GUIStyleName.BackgroundGradient);
            if (gradient == Gradient.None)
            {
                var bgColor = style.Get<Color>(GUIStyleName.BackgroundColor);
                var borderRounding =
                    style.Get<double>(GUIStyleName
                        .BorderTopLeftRadius); //FIXME only round or not round for all corners of a rectangle
                this.PathRect(paddingBoxRect, (float) borderRounding);
                this.PathFill(bgColor);
            }
            else if (gradient == Gradient.TopBottom)
            {
                var topColor = style.Get<Color>(GUIStyleName.GradientTopColor);
                var bottomColor = style.Get<Color>(GUIStyleName.GradientBottomColor);
                this.AddRectFilledGradient(paddingBoxRect, topColor, bottomColor);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        //TODO remove out parameter borderBoxRect
        private static void GetBoxes(Rect rect, StyleRuleSet style, out Rect borderBoxRect, out Rect paddingBoxRect,
            out Rect contentBoxRect)
        {
            //Widths of border
            var bt = style.Get<double>(GUIStyleName.BorderTop);
            var br = style.Get<double>(GUIStyleName.BorderRight);
            var bb = style.Get<double>(GUIStyleName.BorderBottom);
            var bl = style.Get<double>(GUIStyleName.BorderLeft);

            //Widths of padding
            var pt = style.Get<double>(GUIStyleName.PaddingTop);
            var pr = style.Get<double>(GUIStyleName.PaddingRight);
            var pb = style.Get<double>(GUIStyleName.PaddingBottom);
            var pl = style.Get<double>(GUIStyleName.PaddingLeft);

            //4 corner of the border-box
            var btl = new Point(rect.Left, rect.Top);
            var btr = new Point(rect.Right, rect.Top);
            var bbr = new Point(rect.Right, rect.Bottom);
            var bbl = new Point(rect.Left, rect.Bottom);
            borderBoxRect = new Rect(btl, bbr);

            //4 corner of the padding-box
            var ptl = new Point(btl.X + bl, btl.Y + bt);
            var ptr = new Point(btr.X - br, btr.Y + bt);
            var pbr = new Point(bbr.X - br, bbr.Y - bb);
            var pbl = new Point(bbl.X + bl, bbl.Y - bb);
            Debug.Assert(ptl.X < ptr.X);//TODO what if (ptl.X > ptr.X) happens?
            paddingBoxRect = new Rect(ptl, pbr);

            //4 corner of the content-box
            var ctl = new Point(ptl.X + pl, ptl.Y + pt);
            var ctr = new Point(ptr.X - pr, ptr.Y + pr);
            var cbr = new Point(pbr.X - pr, pbr.Y - pb);
            var cbl = new Point(pbl.X + pl, pbl.Y - pb);
            if (ctl.X >= ctr.X)
            {
                Log.Warning("Content box is zero-sized.");
                contentBoxRect = new Rect(ctl, Size.Zero);
            }
            else
            {
                contentBoxRect = new Rect(ctl, cbr);
            }
        }

        private void DrawDebug(Rect paddingBoxRect, Rect contentBoxRect)
        {
#if DrawPaddingBox
            this.PathRect(paddingBoxRect.TopLeft, paddingBoxRect.BottomRight);
            this.PathStroke(Color.Rgb(0, 100, 100), true, 1);
#endif

#if DrawContentBox
            this.PathRect(contentBoxRect.TopLeft, cbr);
            this.PathStroke(Color.Rgb(100, 0, 100), true, 1);
#endif
        }
    }
}