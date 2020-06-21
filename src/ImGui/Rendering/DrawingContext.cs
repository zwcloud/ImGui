using System;
using ImGui.OSAbstraction.Graphics;
using ImGui.OSAbstraction.Text;

namespace ImGui.Rendering
{
    /// <summary>
    /// Describes visual content using draw, push, and pop commands.
    /// </summary>
    public abstract class DrawingContext : IDisposable
    {
        /// <summary>
        ///     DrawLine -
        ///     Draws a line with the specified pen.
        ///     Note that this API does not accept a Brush, as there is no area to fill.
        /// </summary>
        /// <param name="pen"> The Pen with which to stroke the line. </param>
        /// <param name="point0"> The start Point for the line. </param>
        /// <param name="point1"> The end Point for the line. </param>
        public abstract void DrawLine(
            Pen pen,
            Point point0,
            Point point1);

        /// <summary>
        ///     DrawRectangle -
        ///     Draw a rectangle with the provided Brush and/or Pen.
        ///     If both the Brush and Pen are null this call is a no-op.
        /// </summary>
        /// <param name="brush">
        ///     The Brush with which to fill the rectangle.
        ///     This is optional, and can be null, in which case no fill is performed.
        /// </param>
        /// <param name="pen">
        ///     The Pen with which to stroke the rectangle.
        ///     This is optional, and can be null, in which case no stroke is performed.
        /// </param>
        /// <param name="rectangle"> The Rect to fill and/or stroke. </param>
        public abstract void DrawRectangle(
            Brush brush,
            Pen pen,
            Rect rectangle);

        /// <summary>
        ///     DrawRoundedRectangle -
        ///     Draw a rounded rectangle with the provided Brush and/or Pen.
        ///     If both the Brush and Pen are null this call is a no-op.
        /// </summary>
        /// <param name="brush">
        ///     The Brush with which to fill the rectangle.
        ///     This is optional, and can be null, in which case no fill is performed.
        /// </param>
        /// <param name="pen">
        ///     The Pen with which to stroke the rectangle.
        ///     This is optional, and can be null, in which case no stroke is performed.
        /// </param>
        /// <param name="rectangle"> The Rect to fill and/or stroke. </param>
        /// <param name="radiusX">
        ///     The radius in the X dimension of the rounded corners of this
        ///     rounded Rect.  This value will be clamped to the range [0..rectangle.Width/2]
        /// </param>
        /// <param name="radiusY">
        ///     The radius in the Y dimension of the rounded corners of this
        ///     rounded Rect.  This value will be clamped to the range [0..rectangle.Height/2].
        /// </param>
        public abstract void DrawRoundedRectangle(
            Brush brush,
            Pen pen,
            Rect rectangle,
            Double radiusX,
            Double radiusY);


        /// <summary>
        ///     DrawEllipse -
        ///     Draw an ellipse with the provided Brush and/or Pen.
        ///     If both the Brush and Pen are null this call is a no-op.
        /// </summary>
        /// <param name="brush">
        ///     The Brush with which to fill the ellipse.
        ///     This is optional, and can be null, in which case no fill is performed.
        /// </param>
        /// <param name="pen">
        ///     The Pen with which to stroke the ellipse.
        ///     This is optional, and can be null, in which case no stroke is performed.
        /// </param>
        /// <param name="center">
        ///     The center of the ellipse to fill and/or stroke.
        /// </param>
        /// <param name="radiusX">
        ///     The radius in the X dimension of the ellipse.
        ///     The absolute value of the radius provided will be used.
        /// </param>
        /// <param name="radiusY">
        ///     The radius in the Y dimension of the ellipse.
        ///     The absolute value of the radius provided will be used.
        /// </param>
        public abstract void DrawEllipse(
            Brush brush,
            Pen pen,
            Point center,
            Double radiusX,
            Double radiusY);

        /// <summary>
        ///     DrawGeometry -
        ///     Draw a Geometry with the provided Brush and/or Pen.
        ///     If both the Brush and Pen are null this call is a no-op.
        /// </summary>
        /// <param name="brush">
        ///     The Brush with which to fill the Geometry.
        ///     This is optional, and can be null, in which case no fill is performed.
        /// </param>
        /// <param name="pen">
        ///     The Pen with which to stroke the Geometry.
        ///     This is optional, and can be null, in which case no stroke is performed.
        /// </param>
        /// <param name="geometry"> The Geometry to fill and/or stroke. </param>
        public abstract void DrawGeometry(
            Brush brush,
            Pen pen,
            Geometry geometry);

        /// <summary>
        ///     DrawImage -
        ///     Draw an Image into the region specified by the Rect.
        ///     The Image will potentially be stretched and distorted to fit the Rect.
        ///     For more fine grained control, consider filling a Rect with an ImageBrush via
        ///     DrawRectangle.
        /// </summary>
        /// <param name="image"> The Image to draw. </param>
        /// <param name="rectangle">
        ///     The Rect into which the Image will be fit.
        /// </param>
        /// <param name="uvMin"></param>
        /// <param name="uvMax"></param>
        public abstract void DrawImage(ITexture image, Rect rectangle, Point uvMin, Point uvMax);

        public abstract void DrawImage(ITexture image, Rect rectangle,
            (double top, double right, double bottom, double left) slice);

        /// <summary>
        ///     DrawGlyphRun -
        ///     Draw a GlyphRun
        /// </summary>
        /// <param name="foregroundBrush">
        ///     Foreground brush to draw the GlyphRun with.
        /// </param>
        /// <param name="glyphRun"> The <see cref="GlyphRun"/> to draw.  </param>
        public abstract void DrawGlyphRun(Brush foregroundBrush, GlyphRun glyphRun);

        public abstract void DrawText(Brush foregroundBrush, FormattedText formattedText);
        
        /// <summary>
        ///     PushClip - 
        ///     Push a clip region, which will apply to all drawing primitives until the 
        ///     corresponding Pop call.
        /// </summary>
        /// <param name="clipGeometry"> The Geometry to which we will clip. </param>
        public abstract void PushClip(Geometry clipGeometry);

        /// <summary>
        /// Pop
        /// </summary>
        public abstract void Pop();

        //TODO Drawing, video, animation, effect, transform, etc.

        /// <summary>
        /// Closes the DrawingContext and flushes the content.
        /// Afterwards the DrawingContext can not be used anymore.
        /// This call does not require all Push calls to have been Popped.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// This call is illegal if this object has already been closed or disposed.
        /// </exception>
        public abstract void Close();

        /// <summary>
        /// This is the same as the Close call:
        /// Closes the DrawingContext and flushes the content.
        /// Afterwards the DrawingContext can not be used anymore.
        /// This call does not require all Push calls to have been Popped.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// This call is illegal if this object has already been closed or disposed.
        /// </exception>
        void IDisposable.Dispose()
        {
            // Call a virtual method for derived Dispose implementations
            DisposeCore();
        }

        /// <summary>
        /// Dispose functionality implemented by subclasses
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// This call is illegal if this object has already been closed or disposed.
        /// </exception>
        protected abstract void DisposeCore();
    }
}