using System;
using ImGui.Common;
using ImGui.Common.Primitive;
using ImGui.GraphicsAbstraction;
using ImGui.Rendering;

namespace ImGui.GraphicsImplementation
{
    internal partial class BuiltinPrimitiveRenderer : IPrimitiveRenderer
    {
        public void DrawBoxModel(Rect rect, StyleRuleSet style)
        {
            GetBoxes(rect, style, out var borderBoxRect, out var paddingBoxRect, out _);

            // draw background in padding-box
            var gradient = (Gradient)style.Get<int>(GUIStyleName.BackgroundGradient);
            if (gradient == Gradient.None)
            {
                var bgColor = style.Get<Color>(GUIStyleName.BackgroundColor);
                var borderRounding = style.Get<double>(GUIStyleName.BorderTopLeftRadius);//FIXME only round or not round for all corners of a rectangle
                this.PathRect(paddingBoxRect, (float)borderRounding);
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

            //Content
            //Content-box
            //no content

            //Border
            //  Top
            if (!MathEx.AmostZero(borderBoxRect.Top))
            {
                var borderTopColor = style.Get<Color>(GUIStyleName.BorderTopColor);
                if (!MathEx.AmostZero(borderTopColor.A))
                {
                    PathLineTo(paddingBoxRect.TopLeft);
                    PathLineTo(borderBoxRect.TopLeft);
                    PathLineTo(borderBoxRect.TopRight);
                    PathLineTo(paddingBoxRect.TopRight);
                    PathFill(borderTopColor);
                }
            }
            //  Right
            if (!MathEx.AmostZero(borderBoxRect.Right))
            {
                var borderRightColor = style.Get<Color>(GUIStyleName.BorderRightColor);
                if(!MathEx.AmostZero(borderRightColor.A))
                {
                    PathLineTo(paddingBoxRect.TopRight);
                    PathLineTo(borderBoxRect.TopRight);
                    PathLineTo(borderBoxRect.BottomRight);
                    PathLineTo(paddingBoxRect.BottomRight);
                    PathFill(borderRightColor);
                }
            }
            //  Bottom
            if (!MathEx.AmostZero(borderBoxRect.Bottom))
            {
                var borderBottomColor = style.Get<Color>(GUIStyleName.BorderBottomColor);
                if (!MathEx.AmostZero(borderBottomColor.A))
                {
                    PathLineTo(paddingBoxRect.BottomRight);
                    PathLineTo(borderBoxRect.BottomRight);
                    PathLineTo(borderBoxRect.BottomLeft);
                    PathLineTo(paddingBoxRect.BottomLeft);
                    PathFill(borderBottomColor);
                }
            }
            //  Left
            if (!MathEx.AmostZero(borderBoxRect.Left))
            {
                var borderLeftColor = style.Get<Color>(GUIStyleName.BorderLeftColor);
                if (!MathEx.AmostZero(borderLeftColor.A))
                {
                    PathLineTo(paddingBoxRect.BottomLeft);
                    PathLineTo(borderBoxRect.BottomLeft);
                    PathLineTo(borderBoxRect.TopLeft);
                    PathLineTo(paddingBoxRect.TopLeft);
                    PathFill(borderLeftColor);
                }
            }

            //Outline
            var outlineWidth = style.Get<double>(GUIStyleName.OutlineWidth);
            if (!MathEx.AmostZero(outlineWidth))
            {
                var outlineColor = style.Get<Color>(GUIStyleName.OutlineColor);
                if(!MathEx.AmostZero(outlineColor.A))
                {
                    PathRect(borderBoxRect.TopLeft, borderBoxRect.BottomRight);
                    PathStroke(outlineColor, true, outlineWidth);
                }
            }

#if DrawPaddingBox
            PathRect(paddingBoxRect.TopLeft, paddingBoxRect.BottomRight);
            PathStroke(Color.Rgb(0, 100, 100), true, 1);
#endif

#if DrawContentBox
            PathRect(contentBoxRect.TopLeft, cbr);
            PathStroke(Color.Rgb(100, 0, 100), true, 1);
#endif
        }

        public void DrawBoxModel(TextPrimitive textPrimitive, Rect rect, StyleRuleSet style)
        {
            GetBoxes(rect, style, out var borderBoxRect, out var paddingBoxRect, out var contentBoxRect);

            // draw background in padding-box
            var gradient = (Gradient)style.Get<int>(GUIStyleName.BackgroundGradient);
            if (gradient == Gradient.None)
            {
                var bgColor = style.Get<Color>(GUIStyleName.BackgroundColor);
                var borderRounding = style.Get<double>(GUIStyleName.BorderTopLeftRadius);//FIXME only round or not round for all corners of a rectangle
                this.PathRect(paddingBoxRect, (float)borderRounding);
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

            //Content
            //Content-box
            if (contentBoxRect.TopLeft.X < contentBoxRect.TopRight.X)//content should not be visible when contentBoxRect.TopLeft.X > contentBoxRect.TopRight.X
            {
                if (textPrimitive != null)
                {
                    //var textSize = style.CalcSize(text);
                    /*HACK Don't check text size because the size calculated by Typography is not accurate. */
                    /*if (textSize.Height < contentBoxRect.Height && textSize.Width < contentBoxRect.Width)*/
                    {
                        DrawText(textPrimitive, contentBoxRect, style);
                    }
                }
            }

            //Border
            //  Top
            if (!MathEx.AmostZero(borderBoxRect.Top))
            {
                var borderTopColor = style.Get<Color>(GUIStyleName.BorderTopColor);
                if (!MathEx.AmostZero(borderTopColor.A))
                {
                    PathLineTo(paddingBoxRect.TopLeft);
                    PathLineTo(borderBoxRect.TopLeft);
                    PathLineTo(borderBoxRect.TopRight);
                    PathLineTo(paddingBoxRect.TopRight);
                    PathFill(borderTopColor);
                }
            }
            //  Right
            if (!MathEx.AmostZero(borderBoxRect.Right))
            {
                var borderRightColor = style.Get<Color>(GUIStyleName.BorderRightColor);
                if(!MathEx.AmostZero(borderRightColor.A))
                {
                    PathLineTo(paddingBoxRect.TopRight);
                    PathLineTo(borderBoxRect.TopRight);
                    PathLineTo(borderBoxRect.BottomRight);
                    PathLineTo(paddingBoxRect.BottomRight);
                    PathFill(borderRightColor);
                }
            }
            //  Bottom
            if (!MathEx.AmostZero(borderBoxRect.Bottom))
            {
                var borderBottomColor = style.Get<Color>(GUIStyleName.BorderBottomColor);
                if (!MathEx.AmostZero(borderBottomColor.A))
                {
                    PathLineTo(paddingBoxRect.BottomRight);
                    PathLineTo(borderBoxRect.BottomRight);
                    PathLineTo(borderBoxRect.BottomLeft);
                    PathLineTo(paddingBoxRect.BottomLeft);
                    PathFill(borderBottomColor);
                }
            }
            //  Left
            if (!MathEx.AmostZero(borderBoxRect.Left))
            {
                var borderLeftColor = style.Get<Color>(GUIStyleName.BorderLeftColor);
                if (!MathEx.AmostZero(borderLeftColor.A))
                {
                    PathLineTo(paddingBoxRect.BottomLeft);
                    PathLineTo(borderBoxRect.BottomLeft);
                    PathLineTo(borderBoxRect.TopLeft);
                    PathLineTo(paddingBoxRect.TopLeft);
                    PathFill(borderLeftColor);
                }
            }

            //Outline
            var outlineWidth = style.Get<double>(GUIStyleName.OutlineWidth);
            if (!MathEx.AmostZero(outlineWidth))
            {
                var outlineColor = style.Get<Color>(GUIStyleName.OutlineColor);
                if(!MathEx.AmostZero(outlineColor.A))
                {
                    PathRect(borderBoxRect.TopLeft, borderBoxRect.BottomRight);
                    PathStroke(outlineColor, true, outlineWidth);
                }
            }

#if DrawPaddingBox
            PathRect(paddingBoxRect.TopLeft, paddingBoxRect.BottomRight);
            PathStroke(Color.Rgb(0, 100, 100), true, 1);
#endif

#if DrawContentBox
            PathRect(contentBoxRect.TopLeft, cbr);
            PathStroke(Color.Rgb(100, 0, 100), true, 1);
#endif
        }

        public void DrawBoxModel(ImagePrimitive imagePrimitive, Rect rect, StyleRuleSet style)
        {
            GetBoxes(rect, style, out var borderBoxRect, out var paddingBoxRect, out var contentBoxRect);

            // draw background in padding-box
            var gradient = (Gradient)style.Get<int>(GUIStyleName.BackgroundGradient);
            if (gradient == Gradient.None)
            {
                var bgColor = style.Get<Color>(GUIStyleName.BackgroundColor);
                var borderRounding = style.Get<double>(GUIStyleName.BorderTopLeftRadius);//FIXME only round or not round for all corners of a rectangle
                this.PathRect(paddingBoxRect, (float)borderRounding);
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

            //Content
            //Content-box
            if (contentBoxRect.TopLeft.X < contentBoxRect.TopRight.X)//content should not be visible when contentBoxRect.TopLeft.X > contentBoxRect.TopRight.X
            {
                if (imagePrimitive != null)
                {
                    DrawImage(imagePrimitive, contentBoxRect, style);
                }
            }

            //Border
            //  Top
            if (!MathEx.AmostZero(borderBoxRect.Top))
            {
                var borderTopColor = style.Get<Color>(GUIStyleName.BorderTopColor);
                if (!MathEx.AmostZero(borderTopColor.A))
                {
                    PathLineTo(paddingBoxRect.TopLeft);
                    PathLineTo(borderBoxRect.TopLeft);
                    PathLineTo(borderBoxRect.TopRight);
                    PathLineTo(paddingBoxRect.TopRight);
                    PathFill(borderTopColor);
                }
            }
            //  Right
            if (!MathEx.AmostZero(borderBoxRect.Right))
            {
                var borderRightColor = style.Get<Color>(GUIStyleName.BorderRightColor);
                if(!MathEx.AmostZero(borderRightColor.A))
                {
                    PathLineTo(paddingBoxRect.TopRight);
                    PathLineTo(borderBoxRect.TopRight);
                    PathLineTo(borderBoxRect.BottomRight);
                    PathLineTo(paddingBoxRect.BottomRight);
                    PathFill(borderRightColor);
                }
            }
            //  Bottom
            if (!MathEx.AmostZero(borderBoxRect.Bottom))
            {
                var borderBottomColor = style.Get<Color>(GUIStyleName.BorderBottomColor);
                if (!MathEx.AmostZero(borderBottomColor.A))
                {
                    PathLineTo(paddingBoxRect.BottomRight);
                    PathLineTo(borderBoxRect.BottomRight);
                    PathLineTo(borderBoxRect.BottomLeft);
                    PathLineTo(paddingBoxRect.BottomLeft);
                    PathFill(borderBottomColor);
                }
            }
            //  Left
            if (!MathEx.AmostZero(borderBoxRect.Left))
            {
                var borderLeftColor = style.Get<Color>(GUIStyleName.BorderLeftColor);
                if (!MathEx.AmostZero(borderLeftColor.A))
                {
                    PathLineTo(paddingBoxRect.BottomLeft);
                    PathLineTo(borderBoxRect.BottomLeft);
                    PathLineTo(borderBoxRect.TopLeft);
                    PathLineTo(paddingBoxRect.TopLeft);
                    PathFill(borderLeftColor);
                }
            }

            //Outline
            var outlineWidth = style.Get<double>(GUIStyleName.OutlineWidth);
            if (!MathEx.AmostZero(outlineWidth))
            {
                var outlineColor = style.Get<Color>(GUIStyleName.OutlineColor);
                if(!MathEx.AmostZero(outlineColor.A))
                {
                    PathRect(borderBoxRect.TopLeft, borderBoxRect.BottomRight);
                    PathStroke(outlineColor, true, outlineWidth);
                }
            }

#if DrawPaddingBox
            PathRect(paddingBoxRect.TopLeft, paddingBoxRect.BottomRight);
            PathStroke(Color.Rgb(0, 100, 100), true, 1);
#endif

#if DrawContentBox
            PathRect(contentBoxRect.TopLeft, cbr);
            PathStroke(Color.Rgb(100, 0, 100), true, 1);
#endif
        }

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
            //if (ptl.X > ptr.X) return;//TODO what if (ptl.X > ptr.X) happens?
            paddingBoxRect = new Rect(ptl, pbr);

            //4 corner of the content-box
            var ctl = new Point(ptl.X + pl, ptl.Y + pt);
            var ctr = new Point(ptr.X - pr, ptr.Y + pr);
            var cbr = new Point(pbr.X - pr, pbr.Y - pb);
            var cbl = new Point(pbl.X + pl, pbl.Y - pb);
            contentBoxRect = new Rect(ctl, cbr);
        }
    }
}