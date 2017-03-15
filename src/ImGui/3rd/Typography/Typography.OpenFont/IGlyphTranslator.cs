//Apache2, 2016-2017, WinterDev
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
        /// <param name="countourCount"></param>
        void BeginRead(int countourCount);

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
        /// add Quadratic Bézier curve,begin from CURRENT pen pos, to (x2,y2), then set (x2,y2) as CURRENT pen pos
        /// </summary>
        /// <param name="x1">x of 1st control point</param>
        /// <param name="y1">y of 1st control point</param>
        /// <param name="x2">end point x</param>
        /// <param name="y2">end point y</param>
        void Curve3(float x1, float y1, float x2, float y2);

        /// <summary>
        /// add Quadratic Bézier curve,begin from CURRENT pen pos, to (x3,y3), then set (x3,y3) as CURRENT pen pos
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

    //static int s_POINTS_PER_INCH = 72; //default value, 
    //static int s_PIXELS_PER_INCH = 96; //default value
    //public static float ConvEmSizeInPointsToPixels(float emsizeInPoint)
    //{
    //    return (int)(((float)emsizeInPoint / (float)s_POINTS_PER_INCH) * (float)s_PIXELS_PER_INCH);
    //}

    ////from http://www.w3schools.com/tags/ref_pxtoemconversion.asp
    ////set default
    //// 16px = 1 em
    ////-------------------
    ////1. conv font design unit to em
    //// em = designUnit / unit_per_Em       
    ////2. conv font design unit to pixels 
    //// float scale = (float)(size * resolution) / (pointsPerInch * _typeface.UnitsPerEm);

    ////-------------------
    ////https://www.microsoft.com/typography/otspec/TTCH01.htm
    ////Converting FUnits to pixels
    ////Values in the em square are converted to values in the pixel coordinate system by multiplying them by a scale. This scale is:
    ////pointSize * resolution / ( 72 points per inch * units_per_em )
    ////where pointSize is the size at which the glyph is to be displayed, and resolution is the resolution of the output device.
    ////The 72 in the denominator reflects the number of points per inch.
    ////For example, assume that a glyph feature is 550 FUnits in length on a 72 dpi screen at 18 point. 
    ////There are 2048 units per em. The following calculation reveals that the feature is 4.83 pixels long.
    ////550 * 18 * 72 / ( 72 * 2048 ) = 4.83
    ////-------------------
    //public static float ConvFUnitToPixels(ushort reqFUnit, float fontSizeInPoint, ushort unitPerEm)
    //{
    //    //reqFUnit * scale             
    //    return reqFUnit * GetFUnitToPixelsScale(fontSizeInPoint, unitPerEm);
    //}
    //public static float GetFUnitToPixelsScale(float fontSizeInPoint, ushort unitPerEm)
    //{
    //    //reqFUnit * scale             
    //    return ((fontSizeInPoint * s_PIXELS_PER_INCH) / (s_POINTS_PER_INCH * unitPerEm));
    //}



}