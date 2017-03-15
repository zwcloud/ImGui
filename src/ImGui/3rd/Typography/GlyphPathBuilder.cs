//MIT, 2016-2017, WinterDev

using System;
using System.Numerics;
using Typography.OpenFont;

using Vector = System.Numerics.Vector2;

namespace Typography.TextPrint
{
    /// <summary>
    /// Glyph Path Builder
    /// </summary>
    public class GlyphPathBuilder
    {
        private readonly TrueTypeInterpreter trueTypeInterpreter;
        private ushort[] outputContours;
        private GlyphPointF[] outputGlyphPoints;

        public GlyphPathBuilder(Typeface typeface)
        {
            this.Typeface = typeface;
            this.UseTrueTypeInstructions = false;
            this.trueTypeInterpreter = new TrueTypeInterpreter();
            this.trueTypeInterpreter.SetTypeFace(typeface);
        }

        public Typeface Typeface { get; }

        /// <summary>
        /// specific output glyph size (in points)
        /// </summary>
        public float SizeInPoints { get; private set; }

        /// <summary>
        /// use Maxim's Agg Vertical Hinting
        /// </summary>
        public bool UseVerticalHinting { get; set; }

        /// <summary>
        /// Whether the builder process glyph with true type instructions. This is false by default.
        /// </summary>
        public bool UseTrueTypeInstructions { get; set; }

        public bool PassHintInterpreterModule { get; private set; }

        /// <summary>
        /// Get glyph paths
        /// </summary>
        public void BuildFromGlyphIndex(ushort glyphIndex, float sizeInPoints, IGlyphTranslator tx, float scale,
            float offsetX, float offsetY)
        {
            this.SizeInPoints = sizeInPoints;
            var glyph = this.Typeface.GetGlyphByIndex(glyphIndex);

            //1. start with original points/contours from glyph
            this.outputGlyphPoints = glyph.GlyphPoints;
            this.outputContours = glyph.EndPoints;

            this.PassHintInterpreterModule = false;
            var currentTypeFace = this.Typeface;

            //2. process glyph points
            if (UseTrueTypeInstructions &&
                currentTypeFace.HasPrepProgramBuffer &&
                glyph.HasGlyphInstructions)
            {
                var newGlyphPoints = trueTypeInterpreter.HintGlyph(glyphIndex, SizeInPoints);
                this.outputGlyphPoints = newGlyphPoints;
                PassHintInterpreterModule = true;
            }

            // Read line segments and bezier segments from the contour of this glyph.
            {
                var startContour = 0;
                var cpoint_index = 0; //current point index

                var todoContourCount = this.outputContours.Length;

                //1. start read data from a glyph
                tx.BeginRead(todoContourCount);

                float latest_move_to_x = 0;
                float latest_move_to_y = 0;

                var curveControlPointCount = 0; // 1 curve control point => Quadratic, 2 curve control points => Cubic

                while (todoContourCount > 0) //foreach contour...
                {
                    //next contour will begin at...
                    var nextCntBeginAtIndex = this.outputContours[startContour] + 1;

                    //reset  ...
                    var c1 = new Vector();
                    var c2 = new Vector();
                    var curveMode = false;
                    var isFirstPoint = true; //first point of this contour

                    for (; cpoint_index < nextCntBeginAtIndex; ++cpoint_index) //for each point in this contour
                    {
                        var p = this.outputGlyphPoints[cpoint_index];
                        var p_x = p.X;
                        var p_y = p.Y;
                        p_x = p_x * scale + offsetX;
                        p_y = p_y * (-scale) + sizeInPoints + offsetY;

                        if (p.onCurve)
                        {
                            //point p is an on-curve point (on outline). (not curve control point)
                            //possible ways..
                            //1. if we are in curve mode, then p is end point
                            //   we must decide which curve to create (Curve3 or Curve4)
                            //   easy, ... 
                            //      if  curveControlPointCount == 1 , then create Curve3
                            //      else curveControlPointCount ==2 , then create Curve4
                            //2. if we are NOT in curve mode, 
                            //      if p is first point then set this to x0,y0
                            //      else then p is end point of a line.
                            if (curveMode)
                            {
                                switch (curveControlPointCount)
                                {
                                    case 1:
                                    {
                                        tx.Curve3(
                                            c1.X, c1.Y,
                                            p_x, p_y);
                                    }
                                        break;
                                    case 2:
                                    {
                                        //for TrueType font 
                                        //we should not be here?

                                        tx.Curve4(
                                            c1.X, c1.Y,
                                            c2.X, c2.Y,
                                            p_x, p_y);
                                    }
                                        break;
                                    default:
                                    {
                                        throw new NotSupportedException();
                                    }
                                }

                                //reset curve control point count
                                curveControlPointCount = 0;
                                //exist from curve mode
                                curveMode = false;
                            }
                            else
                            {
                                //as describe above,...

                                if (isFirstPoint)
                                {
                                    isFirstPoint = false;
                                    tx.MoveTo(latest_move_to_x = p_x, latest_move_to_y = p_y);
                                }
                                else
                                {
                                    tx.LineTo(p_x, p_y);
                                }
                            }
                        }
                        else
                        {
                            //this point is curve control point***
                            //so set curve mode = true
                            curveMode = true;
                            //check number if existing curve control 

                            switch (curveControlPointCount)
                            {
                                case 0:
                                    c1 = new Vector2(p_x, p_y);
                                    //this point may be part 1st control point of a curve,
                                    //store it and wait for next point before make decision *** 
                                    break;
                                case 1:
                                    //we already have previous 1st control point (c1)
                                    //------------------------------------- 
                                    //please note that TrueType font
                                    //compose of Quadractic Bezier Curve (Curve3) *** 
                                    //------------------------------------- 
                                    //in this case, this is NOT Cubic,
                                    //this is 2 CONNECTED Quadractic Bezier Curves***
                                    //
                                    //we must create 'end point' of the first curve
                                    //and set it as 'begin point of the second curve.
                                    //
                                    //this is done by ...
                                    //1. calculate mid point between c1 and the latest point (p_x,p_y)
                                    var mid = new Vector2(
                                        ((c1.X + p_x) / 2f),
                                        ((c1.Y + p_y) / 2f));

                                    //2. generate curve3 ***
                                    tx.Curve3(
                                        c1.X, c1.Y,
                                        mid.X, mid.Y);

                                    //3. so curve control point number is reduce by 1***
                                    curveControlPointCount--;

                                    //4. and set (p_x,p_y) as 1st control point for the new curve
                                    c1 = new Vector2(p_x, p_y);

                                    //printf("[%d] bzc2nd,  x: %d,y:%d \n", mm, vpoint.x, vpoint.y); 
                                    break;
                                default:
                                    throw new NotSupportedException();
                            }
                            //count
                            curveControlPointCount++;
                        }
                    }

                    //when finish,
                    //ensure that the contour is closed.
                    if (curveMode)
                    {
                        switch (curveControlPointCount)
                        {
                            case 0:
                                break;
                            case 1:
                            {
                                tx.Curve3(c1.X, c1.Y,
                                    latest_move_to_x, latest_move_to_y);
                            }
                                break;
                            case 2:
                            {
                                //for TrueType font 
                                //we should not be here? 
                                tx.Curve4(c1.X, c1.Y,
                                    c2.X, c2.Y,
                                    latest_move_to_x, latest_move_to_y);
                            }
                                break;
                            default:
                            {
                                throw new NotSupportedException();
                            }
                        }
                        //reset
                        curveControlPointCount = 0;
                    }

                    tx.CloseContour();
                    startContour++;
                    todoContourCount--;
                }

                tx.EndRead();
            }
        }
    }
}