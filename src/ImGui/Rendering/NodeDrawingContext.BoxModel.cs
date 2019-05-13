using System;
using System.Diagnostics;

namespace ImGui.Rendering
{
    internal partial class NodeDrawingContext
    {
        private void DrawOutline(Rect borderBoxRect)
        {
            var style = this.ruleSet;
            var outlineWidth = style.Get<double>(GUIStyleName.OutlineWidth);
            if (MathEx.AmostZero(outlineWidth)) return;
            var outlineColor = style.Get<Color>(GUIStyleName.OutlineColor);
            if (MathEx.AmostZero(outlineColor.A)) return;
            base.DrawRectangle(null, new Pen(outlineColor, outlineWidth), borderBoxRect);
        }

        private void DrawBorder(Rect borderBoxRect, Rect paddingBoxRect)
        {
            var style = this.ruleSet;
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
                //TODO this.DrawSlicedImage((ImageGeometry) rule.Geometry, borderBoxRect, style);
            }
            else
            {
                var border = style.Border;
                var borderColor = style.BorderColor;
                var borderRadius = style.BorderRadius;
                PathGeometry topBorder = GenerateTopBorderGeometry(borderBoxRect, paddingBoxRect, border, borderRadius);
                PathGeometry leftBorder = GenerateRightBorderGeometry(borderBoxRect, paddingBoxRect, border, borderRadius);
                PathGeometry rightBorder = GenerateBottomBorderGeometry(borderBoxRect, paddingBoxRect, border, borderRadius);
                PathGeometry bottomBorder = GenerateLeftBorderGeometry(borderBoxRect, paddingBoxRect, border, borderRadius);
                base.DrawGeometry(new Brush(borderColor.top),     new Pen(Color.Black, 2)/*TEMP stroke border for testing*/, topBorder);
                base.DrawGeometry(new Brush(borderColor.left),    new Pen(Color.Black, 2)/*TEMP stroke border for testing*/, leftBorder);
                base.DrawGeometry(new Brush(borderColor.right),   new Pen(Color.Black, 2)/*TEMP stroke border for testing*/, rightBorder);
                base.DrawGeometry(new Brush(borderColor.bottom),  new Pen(Color.Black, 2)/*TEMP stroke border for testing*/, bottomBorder);
            }
        }

        private static PathGeometry GenerateTopBorderGeometry(
            Rect borderRect,
            Rect paddingRect,
            (double top, double right, double bottom, double left) border,
            (double TopLeft, double TopRight, double BottomRight, double BottomLeft) borderRadius)
        {
            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            geometry.Figures.Add(figure);
            figure.IsFilled = true;

            //start from top-left
            {
                Point startPoint;
                var bl = border.left;
                var bt = border.top;
                var r = borderRadius.TopLeft;
                var arcCenter = borderRect.TopLeft + new Vector(r, r);
                var paddingCorner = paddingRect.TopLeft;
                var halfArcPoint = MathEx.EvaluateCircle(arcCenter, r, MathEx.Deg2Rad(-135));
                var arcEndPoint = new Point(borderRect.Left + r, borderRect.Top);
                if (r == bt && bt == bl || bl <= r && r <= bt || bt <= r && r <= bl || r <= bl && bl <= bt || r <= bt && bt <= bl)
                {
                    startPoint = paddingCorner;
                    figure.Segments.Add(new LineSegment(halfArcPoint, false));
                    figure.Segments.Add(new ArcSegment(arcEndPoint,new Size(r, r), 0,false, SweepDirection.Clockwise, false));
                }
                else // if (bl < bt && bt < r || bt<bl && bl <r)//inner ellipse curve occurs
                {
                    var ellipseCenter = arcCenter;
                    var ellipseXRadius = r - bl;
                    var ellipseYRadius = r - bt;
                    startPoint = paddingCorner + new Vector(ellipseXRadius, 0);
                    var halfEllipsePoint = MathEx.EvaluateEllipse(ellipseCenter, ellipseXRadius, ellipseYRadius, MathEx.Deg2Rad(-135));
                    figure.Segments.Add(new ArcSegment(halfEllipsePoint, new Size(ellipseXRadius, ellipseYRadius), 0,false,SweepDirection.Counterclockwise, false));
                    figure.Segments.Add(new LineSegment(halfArcPoint, false));
                    figure.Segments.Add(new ArcSegment(arcEndPoint, new Size(r, r), 0, false, SweepDirection.Clockwise,false));
                }
                figure.StartPoint = startPoint;
            }

            //connect left to right
            figure.Segments.Add(new LineSegment(borderRect.TopRight + new Vector(-borderRadius.TopRight, 0), false));

            //to top-right
            {
                var bt = border.top;
                var br = border.right;
                var r = borderRadius.TopRight;
                var arcCenter = borderRect.TopRight + new Vector(-r, r);
                var halfArcPoint = MathEx.EvaluateCircle(arcCenter, r, MathEx.Deg2Rad(-45));
                var paddingCorner = paddingRect.TopRight;

                if (r == bt && bt == br || br <= r && r <= bt || bt <= r && r <= br || r <= br && br <= bt || r <= bt && bt <= br)
                {
                    figure.Segments.Add(new ArcSegment(halfArcPoint, new Size(r, r), 0, false, SweepDirection.Clockwise,false));
                    figure.Segments.Add(new LineSegment(paddingCorner, false));
                }
                else //(br < bt && bt < r || bt<br && br <r)//inner ellipse curve occurs
                {
                    var ellipseCenter = arcCenter;
                    var ellipseXRadius = r - br;
                    var ellipseYRadius = r - bt;
                    var halfEllipsePoint = MathEx.EvaluateEllipse(ellipseCenter, ellipseXRadius, ellipseYRadius, MathEx.Deg2Rad(-45));
                    var ellipseEndPoint = new Point(borderRect.Right - r, borderRect.Top + border.top);
                    figure.Segments.Add(new ArcSegment(halfArcPoint, new Size(r, r), 0, false, SweepDirection.Clockwise,false));
                    figure.Segments.Add(new LineSegment(halfEllipsePoint, false));
                    figure.Segments.Add(new ArcSegment(ellipseEndPoint, new Size(ellipseXRadius, ellipseYRadius), 0, false, SweepDirection.Counterclockwise, false));
                }
            }

            //close the figure
            figure.Segments.Add(new LineSegment(figure.StartPoint, false));
            figure.IsClosed = true;

            return geometry;
        }

        private static PathGeometry GenerateRightBorderGeometry(
            Rect borderRect,
            Rect paddingRect,
            (double top, double right, double bottom, double left) border,
            (double TopLeft, double TopRight, double BottomRight, double BottomLeft) borderRadius)
        {
            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            geometry.Figures.Add(figure);
            figure.IsFilled = true;

            //top-right
            {
                Point startPoint;
                var br = border.right;
                var bt = border.top;
                var r = borderRadius.TopRight;
                var arcCenter = borderRect.TopRight + new Vector(-r, r);
                var halfArcPoint = MathEx.EvaluateCircle(arcCenter, r, MathEx.Deg2Rad(-45));
                var paddingCorner = paddingRect.TopRight;
                var arcEndPoint = new Point(borderRect.Right, borderRect.Top + r);
                if (r == bt && bt == br || br <= r && r <= bt || bt <= r && r <= br || r <= br && br <= bt || r <= bt && bt <= br)
                {
                    startPoint = paddingCorner;
                    figure.Segments.Add(new LineSegment(halfArcPoint, false));
                    figure.Segments.Add(new ArcSegment(arcEndPoint, new Size(r, r), 0, false, SweepDirection.Clockwise,false));
                }
                else //(br < bt && bt < r || bt<br && br <r)//inner ellipse curve occurs
                {
                    var ellipseCenter = arcCenter;
                    var ellipseXRadius = r - br;
                    var ellipseYRadius = r - bt;
                    var halfEllipsePoint = MathEx.EvaluateEllipse(ellipseCenter, ellipseXRadius, ellipseYRadius, MathEx.Deg2Rad(-45));
                    var ellipseEndPoint = new Point(ellipseCenter.X + ellipseXRadius, ellipseCenter.Y);
                    startPoint = ellipseEndPoint;
                    figure.Segments.Add(new ArcSegment(halfEllipsePoint, new Size(ellipseXRadius, ellipseYRadius), 0, false, SweepDirection.Counterclockwise, false));
                    figure.Segments.Add(new LineSegment(halfArcPoint, false));
                    figure.Segments.Add(new ArcSegment(arcEndPoint, new Size(r, r), 0, false, SweepDirection.Clockwise, false));
                }
                figure.StartPoint = startPoint;
            }


            //bottom-right
            {
                var br = border.right;
                var bb = border.bottom;
                var r = borderRadius.BottomRight;
                var arcCenter = borderRect.BottomRight + new Vector(-r, -r);
                var halfArcPoint = MathEx.EvaluateCircle(arcCenter, r, MathEx.Deg2Rad(45));
                var paddingCorner = paddingRect.BottomRight;
                var arcEndPoint = new Point(borderRect.Right, borderRect.Bottom - r);

                figure.Segments.Add(new LineSegment(arcEndPoint, false));

                if (r == bb && bb == br || br <= r && r <= bb || bb <= r && r <= br || r <= br && br <= bb || r <= bb && bb <= br)
                {
                    figure.Segments.Add(new ArcSegment(halfArcPoint, new Size(r, r), 0, false, SweepDirection.Clockwise,false));
                    figure.Segments.Add(new LineSegment(paddingCorner, false));
                }
                else //(br < bb && bt < r || bb<br && br <r)//inner ellipse curve occurs
                {
                    var ellipseCenter = arcCenter;
                    var ellipseXRadius = r - br;
                    var ellipseYRadius = r - bb;
                    var halfEllipsePoint = MathEx.EvaluateEllipse(ellipseCenter, ellipseXRadius, ellipseYRadius, MathEx.Deg2Rad(45));
                    var ellipseEndPoint = new Point(ellipseCenter.X + ellipseXRadius, ellipseCenter.Y);
                    figure.Segments.Add(new ArcSegment(halfArcPoint, new Size(r, r), 0, false, SweepDirection.Clockwise,false));
                    figure.Segments.Add(new LineSegment(halfEllipsePoint, false));
                    figure.Segments.Add(new ArcSegment(ellipseEndPoint, new Size(ellipseXRadius, ellipseYRadius), 0, false, SweepDirection.Counterclockwise, false));
                }
            }

            //close the figure
            figure.Segments.Add(new LineSegment(figure.StartPoint, false));
            figure.IsClosed = true;

            return geometry;
        }

        private static PathGeometry GenerateBottomBorderGeometry(
            Rect borderRect,
            Rect paddingRect,
            (double top, double right, double bottom, double left) border,
            (double TopLeft, double TopRight, double BottomRight, double BottomLeft) borderRadius
            )
        {
            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            geometry.Figures.Add(figure);
            figure.IsFilled = true;

            //bottom right
            {
                Point startPoint;
                var br = border.right;
                var bb = border.bottom;
                var r = borderRadius.BottomRight;
                var arcCenter = borderRect.BottomRight + new Vector(-r, -r);
                var halfArcPoint = MathEx.EvaluateCircle(arcCenter, r, MathEx.Deg2Rad(45));
                var paddingCorner = paddingRect.BottomRight;
                var arcEndPoint = new Point(borderRect.Right - r, borderRect.Bottom);

                if (r == bb && bb == br || br <= r && r <= bb || bb <= r && r <= br || r <= br && br <= bb || r <= bb && bb <= br)
                {
                    startPoint = paddingCorner;
                    figure.Segments.Add(new LineSegment(halfArcPoint, false));
                    figure.Segments.Add(new ArcSegment(arcEndPoint, new Size(r, r), 0, false, SweepDirection.Clockwise,false));
                }
                else //(br < bb && bt < r || bb<br && br <r)//inner ellipse curve occurs
                {
                    var ellipseCenter = arcCenter;
                    var ellipseXRadius = r - br;
                    var ellipseYRadius = r - bb;
                    var halfEllipsePoint = MathEx.EvaluateEllipse(ellipseCenter, ellipseXRadius, ellipseYRadius, MathEx.Deg2Rad(45));
                    var ellipseEndPoint = new Point(ellipseCenter.X, ellipseCenter.Y + ellipseYRadius);
                    startPoint = ellipseEndPoint;
                    figure.Segments.Add(new ArcSegment(halfEllipsePoint, new Size(ellipseXRadius, ellipseYRadius), 0, false, SweepDirection.Counterclockwise, false));
                    figure.Segments.Add(new LineSegment(halfArcPoint, false));
                    figure.Segments.Add(new ArcSegment(arcEndPoint, new Size(r, r), 0, false, SweepDirection.Clockwise,false));
                }
                figure.StartPoint = startPoint;
            }

            //bottom left
            {
                var bl = border.left;
                var bb = border.bottom;
                var r = borderRadius.BottomLeft;
                var arcCenter = borderRect.BottomLeft + new Vector(r, -r);
                var halfArcPoint = MathEx.EvaluateCircle(arcCenter, r, MathEx.Deg2Rad(135));
                var paddingCorner = paddingRect.BottomLeft;
                var arcEndPoint = new Point(borderRect.Left + r, borderRect.Bottom);

                figure.Segments.Add(new LineSegment(arcEndPoint, false));

                if (r == bb && bb == bl || bl <= r && r <= bb || bb <= r && r <= bl || r <= bl && bl <= bb || r <= bb && bb <= bl)
                {
                    figure.Segments.Add(new ArcSegment(halfArcPoint, new Size(r, r), 0, false, SweepDirection.Clockwise,false));
                    figure.Segments.Add(new LineSegment(paddingCorner, false));
                }
                else //(bl < bb && bt < r || bb<bl && bl <r)//inner ellipse curve occurs
                {
                    var ellipseCenter = arcCenter;
                    var ellipseXRadius = r - bl;
                    var ellipseYRadius = r - bb;
                    var halfEllipsePoint = MathEx.EvaluateEllipse(ellipseCenter, ellipseXRadius, ellipseYRadius, MathEx.Deg2Rad(135));
                    var ellipseEndPoint = new Point(ellipseCenter.X, ellipseCenter.Y + ellipseYRadius);
                    figure.Segments.Add(new ArcSegment(halfArcPoint, new Size(r, r), 0, false, SweepDirection.Clockwise,false));
                    figure.Segments.Add(new LineSegment(halfEllipsePoint, false));
                    figure.Segments.Add(new ArcSegment(ellipseEndPoint, new Size(ellipseXRadius, ellipseYRadius), 0, false, SweepDirection.Counterclockwise, false));
                }
            }

            //close the figure
            figure.Segments.Add(new LineSegment(figure.StartPoint, false));
            figure.IsClosed = true;

            return geometry;
        }

        private static PathGeometry GenerateLeftBorderGeometry(
            Rect borderRect,
            Rect paddingRect,
            (double top, double right, double bottom, double left) border,
            (double TopLeft, double TopRight, double BottomRight, double BottomLeft) borderRadius
            )
        {
            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            geometry.Figures.Add(figure);
            figure.IsFilled = true;

            //bottom left
            {
                Point startPoint;
                var bl = border.left;
                var bb = border.bottom;
                var r = borderRadius.BottomLeft;
                var arcCenter = borderRect.BottomLeft + new Vector(r, -r);
                var halfArcPoint = MathEx.EvaluateCircle(arcCenter, r, MathEx.Deg2Rad(135));
                var paddingCorner = paddingRect.BottomLeft;
                var arcEndPoint = new Point(borderRect.Left, borderRect.Bottom - r);

                if (r == bb && bb == bl || bl <= r && r <= bb || bb <= r && r <= bl || r <= bl && bl <= bb || r <= bb && bb <= bl)
                {
                    startPoint = paddingCorner;
                    figure.Segments.Add(new LineSegment(halfArcPoint, false));
                    figure.Segments.Add(new ArcSegment(arcEndPoint, new Size(r, r), 0, false, SweepDirection.Clockwise,false));
                }
                else //(bl < bb && bt < r || bb<bl && bl <r)//inner ellipse curve occurs
                {
                    var ellipseCenter = arcCenter;
                    var ellipseXRadius = r - bl;
                    var ellipseYRadius = r - bb;
                    var halfEllipsePoint = MathEx.EvaluateEllipse(ellipseCenter, ellipseXRadius, ellipseYRadius, MathEx.Deg2Rad(135));
                    var ellipseEndPoint = new Point(ellipseCenter.X - ellipseXRadius, ellipseCenter.Y);
                    startPoint = ellipseEndPoint;
                    figure.Segments.Add(new ArcSegment(halfEllipsePoint, new Size(ellipseXRadius, ellipseYRadius), 0, false, SweepDirection.Counterclockwise, false));
                    figure.Segments.Add(new LineSegment(halfArcPoint, false));
                    figure.Segments.Add(new ArcSegment(arcEndPoint, new Size(r, r), 0, false, SweepDirection.Clockwise,false));
                }

                figure.StartPoint = startPoint;
            }

            //top left
            {
                var bl = border.left;
                var bt = border.top;
                var r = borderRadius.TopLeft;
                var arcCenter = borderRect.TopLeft + new Vector(r, r);
                var paddingCorner = paddingRect.TopLeft;
                var halfArcPoint = MathEx.EvaluateCircle(arcCenter, r, MathEx.Deg2Rad(-135));
                var arcEndPoint = new Point(borderRect.Left, borderRect.Top + r);
                figure.Segments.Add(new LineSegment(arcEndPoint, false));
                if (r == bt && bt == bl || bl <= r && r <= bt || bt <= r && r <= bl || r <= bl && bl <= bt || r <= bt && bt <= bl)
                {
                    figure.Segments.Add(new ArcSegment(halfArcPoint,new Size(r, r), 0,false, SweepDirection.Clockwise, false));
                    figure.Segments.Add(new LineSegment(paddingCorner, false));
                }
                else // if (bl < bt && bt < r || bt<bl && bl <r)//inner ellipse curve occurs
                {
                    var ellipseCenter = arcCenter;
                    var ellipseXRadius = r - bl;
                    var ellipseYRadius = r - bt;
                    var halfEllipsePoint = MathEx.EvaluateEllipse(ellipseCenter, ellipseXRadius, ellipseYRadius, MathEx.Deg2Rad(-135));
                    var ellipseEndPoint = new Point(ellipseCenter.X - ellipseXRadius, ellipseCenter.Y);
                    figure.Segments.Add(new ArcSegment(halfArcPoint, new Size(r, r), 0, false, SweepDirection.Clockwise,false));
                    figure.Segments.Add(new LineSegment(halfEllipsePoint, false));
                    figure.Segments.Add(new ArcSegment(ellipseEndPoint, new Size(ellipseXRadius, ellipseYRadius), 0,false,SweepDirection.Counterclockwise, false));
                }
            }

            //close the figure
            figure.Segments.Add(new LineSegment(figure.StartPoint, false));
            figure.IsClosed = true;

            return geometry;
        }

        private void DrawBackground(Rect paddingBoxRect)
        {
            var style = this.ruleSet;
            // draw background in padding-box
            var gradient = (Gradient) style.Get<int>(GUIStyleName.BackgroundGradient);
            if (gradient == Gradient.None)
            {
                var bgColor = style.BackgroundColor;
                DrawRectangle(new Brush(bgColor), null, paddingBoxRect);
                //TODO apply border-radius clip, which is determined by how border corners are rendered
            }
            else if (gradient == Gradient.TopBottom)
            {
                var topColor = style.Get<Color>(GUIStyleName.GradientTopColor);
                var bottomColor = style.Get<Color>(GUIStyleName.GradientBottomColor);
                //TODO this.AddRectFilledGradient(paddingBoxRect, topColor, bottomColor);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }
    }
}