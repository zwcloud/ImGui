//MIT, 2016-2017, WinterDev
//MIT, 2015, Michael Popoloski 
//FTL, 3-clauses BSD, FreeType project
//-----------------------------------------------------

using System;
using System.Numerics;

namespace Typography.OpenFont
{
    //https://en.wikipedia.org/wiki/B%C3%A9zier_curve
    //--------------------
    //Line, has 2 points..
    //  (x0,y0) begin point
    //  (x1,y1) end point
    //--------------------
    //Curve3 (Quadratic Bézier curves), has 3 points
    //  (x0,y0)  begin point
    //  (x1,y1)  1st control point 
    //  (x2,y2)  end point
    //--------------------
    //Curve4 (Cubic  Bézier curves), has 4 points
    //  (x0,y0)  begin point
    //  (x1,y1)  1st control point 
    //  (x2,y2)  2nd control point
    //  (x3,y3)  end point    
    //-------------------- 
    //please note that TrueType font
    //compose of Quadractic Bezier Curve ***
    //--------------------- 
    public interface IGlyphTranslator
    {
        /// <summary>
        /// begin read a glyph
        /// </summary>
        /// <param name="contourCount"></param>
        void BeginRead(int contourCount);
        /// <summary>
        /// end read a glyph
        /// </summary>
        void EndRead();
        /// <summary>
        /// set CURRENT pen position to (x0,y0) And set the position as latest MOVETO position
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        void MoveTo(float x0, float y0);
        /// <summary>
        /// add line,begin from CURRENT pen position to (x1,y1) then set (x1,y1) as CURRENT pen position
        /// </summary>
        /// <param name="x1">end point x</param>
        /// <param name="y1">end point y</param>
        void LineTo(float x1, float y1);
        /// <summary>
        /// add Quadratic(2) Bézier curve,begin from CURRENT pen pos, to (x2,y2), then set (x2,y2) as CURRENT pen pos
        /// </summary>
        /// <param name="x1">x of 1st control point</param>
        /// <param name="y1">y of 1st control point</param>
        /// <param name="x2">end point x</param>
        /// <param name="y2">end point y</param>
        void Curve3(float x1, float y1, float x2, float y2);
        /// <summary>
        /// add Cubic(3) Bézier curve,begin from CURRENT pen pos, to (x3,y3), then set (x3,y3) as CURRENT pen pos
        /// </summary>
        /// <param name="x1">x of 1st control point</param>
        /// <param name="y1">y of 1st control point</param>
        /// <param name="x2">x of 2nd control point</param>
        /// <param name="y2">y of 2dn control point</param>
        /// <param name="x3">end point x</param>
        /// <param name="y3">end point y</param>
        void Curve4(float x1, float y1, float x2, float y2, float x3, float y3);

        /// <summary>
        /// close current contour, create line from CURRENT pen position to latest MOVETO position
        /// </summary>
        void CloseContour();
    }

    public static class IGlyphReaderExtensions
    {

        public static void Read(this IGlyphTranslator tx, GlyphPointF[] glyphPoints, ushort[] contourEndPoints, float scale = 1, float offsetX = 0, float offsetY = 0)
        {

            int startContour = 0;
            int cpoint_index = 0;//current point index

            int todoContourCount = contourEndPoints.Length;
            //----------------------------------- 
            //1. start read data from a glyph
            tx.BeginRead(todoContourCount);
            //-----------------------------------
            float latest_moveto_x = 0;
            float latest_moveto_y = 0;
            int curveControlPointCount = 0; // 1 curve control point => Quadratic, 2 curve control points => Cubic


            while (todoContourCount > 0)
            {
                //foreach contour...
                //next contour will begin at...
                int nextCntBeginAtIndex = contourEndPoints[startContour] + 1;

                //reset  ...

                bool has_c_begin = false;  //see below [A]
                Vector2 c_begin = new Vector2(); //special point if the glyph starts with 'off-curve' control point                 
                Vector2 c1 = new Vector2(); //control point of quadratic curve
                //-------------------------------------------------------------------
                bool offCurveMode = false;
                bool isFirstOnCurvePoint = true; //first point of this contour

                //-------------------------------------------------------------------
                //[A]
                //first point may start with 'ON CURVE" or 'OFF-CURVE'
                //1. if first point is 'ON-CURVE' => we just set moveto command to it
                //
                //2. if first point is 'OFF-CURVE' => we store it into c_begin and set has_c_begin= true
                //   the c_begin will be use when we close the contour   
                //
                //
                //eg. glyph '2' in Century font starts with 'OFF-CURVE' point, and ends with 'OFF-CURVE'
                //-------------------------------------------------------------------

                for (; cpoint_index < nextCntBeginAtIndex; ++cpoint_index)
                {    //for each point in this contour

                    GlyphPointF p = glyphPoints[cpoint_index];
                    float p_x = p.X * scale;
                    float p_y = p.Y * -scale;
                    p_x += offsetX;
                    p_y += offsetY;

                    if (p.onCurve)
                    {
                        //-------------------------------------------------------------------
                        //[B]
                        //point p is an 'on-curve' point (on outline).
                        //(not curve control point)***
                        //the point touch the outline.

                        //possible ways..
                        //1. if we are in offCurveMode, then p is a curve end point.
                        //   we must decide which curve to create (Curve3 or Curve4)
                        //   easy, ... 
                        //      if  curveControlPointCount == 1 , then create Curve3
                        //      else curveControlPointCount ==2 , then create Curve4 (BUT SHOULD NOT BE FOUND IN TRUE TYPEFONT'(
                        //2. if we are NOT in offCurveMode, 
                        //      if p is first point then set this to =>moveto(x0,y0)
                        //      else then p is end point of a line => lineto(x1,y1)
                        //-------------------------------------------------------------------

                        if (offCurveMode)
                        {
                            //as describe above [B.1]
                            switch (curveControlPointCount)
                            {
                                case 1:
                                    tx.Curve3(c1.X, c1.Y, p_x, p_y);
                                    break;
                                default:
                                    throw new NotSupportedException();
                            }

                            //reset curve control point count
                            curveControlPointCount = 0;
                            //we touch the curve, set offCurveMode= false
                            offCurveMode = false;
                        }
                        else
                        {
                            // p is ON CURVE, but now we are in OFF-CURVE mode.
                            //as describe above [B.2]
                            if (isFirstOnCurvePoint)
                            {
                                //special treament for first point
                                isFirstOnCurvePoint = false;
                                switch (curveControlPointCount)
                                {
                                    case 0:
                                        //describe above, see [A.1]
                                        tx.MoveTo(latest_moveto_x = p_x, latest_moveto_y = p_y);
                                        break;
                                    case 1:
                                        //describe above, see [A.2]
                                        c_begin = c1;
                                        has_c_begin = true;
                                        //since c1 is off curve
                                        //we skip the c1 for and use it when we close the curve 
                                        //tx.MoveTo(latest_moveto_x = c1.X, latest_moveto_y = c1.Y);
                                        //tx.LineTo(p_x, p_y); 
                                        tx.MoveTo(latest_moveto_x = p_x, latest_moveto_y = p_y);
                                        curveControlPointCount--;
                                        break;
                                    default:
                                        throw new NotSupportedException();
                                }
                            }
                            else
                            {
                                tx.LineTo(p_x, p_y);
                            }
                        }
                    }
                    else
                    {
                        //p is OFF-CURVE point (this is curve control point)
                        switch (curveControlPointCount)
                        {

                            case 0:
                                c1 = new Vector2(p_x, p_y);
                                if (!isFirstOnCurvePoint)
                                {
                                    //this point is curve control point
                                    //so set curve mode = true 
                                    //check number if existing curve control
                                    offCurveMode = true;
                                }
                                else
                                {
                                    //describe above, see [A.2]
                                }
                                break;
                            case 1:
                                {
                                    //we already have previous 1st control point (c1)
                                    //------------------------------------- 
                                    //please note that TrueType font
                                    //compose of Quadractic Bezier Curve (Curve3)
                                    //------------------------------------- 
                                    //in this case, this is NOT Cubic,
                                    //this is 2 CONNECTED Quadractic Bezier Curves
                                    //
                                    //we must create 'end point' of the first curve
                                    //and set it as 'begin point of the second curve.
                                    //
                                    //this is done by ...
                                    //1. calculate mid point between c1 and the latest point (p_x,p_y)
                                    Vector2 mid = GetMidPoint(c1, p_x, p_y);
                                    //2. generate curve3 ***
                                    tx.Curve3(c1.X, c1.Y, mid.X, mid.Y);
                                    //3. so curve control point number is reduce by 1
                                    curveControlPointCount--;
                                    //4. and set (p_x,p_y) as 1st control point for the new curve
                                    c1 = new Vector2(p_x, p_y);
                                    offCurveMode = true;
                                }
                                break;
                            default:
                                throw new NotSupportedException();
                        }
                        //count
                        curveControlPointCount++;
                    }
                }
                //when finish, ensure that the contour is closed.
                if (offCurveMode)
                {
                    switch (curveControlPointCount)
                    {
                        case 0: break;
                        case 1:
                            {
                                if (has_c_begin)
                                {
                                    Vector2 mid = GetMidPoint(c1, c_begin.X, c_begin.Y);
                                    //2. generate curve3
                                    tx.Curve3(c1.X, c1.Y, mid.X, mid.Y);
                                    //3. so curve control point number is reduce by 1
                                    curveControlPointCount--;
                                    tx.Curve3(c_begin.X, c_begin.Y,
                                         latest_moveto_x, latest_moveto_y);
                                }
                                else
                                {
                                    tx.Curve3(
                                        c1.X, c1.Y,
                                        latest_moveto_x, latest_moveto_y);
                                }
                            }
                            break;
                        default:
                            {
                                throw new NotSupportedException();
                            }
                    }
                    //reset
                    offCurveMode = false;
                    curveControlPointCount = 0;
                }

                tx.CloseContour();
                startContour++;
                todoContourCount--;
            }
            //finish
            tx.EndRead();
        }

        static Vector2 GetMidPoint(Vector2 v0, float x1, float y1)
        {
            //mid point between v0 and (x1,y1)
            return new Vector2(
                ((v0.X + x1) / 2f),
                ((v0.Y + y1) / 2f));
        }
    }

}
