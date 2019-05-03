using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ImGui.Rendering.Composition
{
    internal enum RecordType
    {
        DrawLine,
        DrawRectangle,
        DrawRoundedRectangle,
        DrawGlyphRun,
        DrawGeometry,
        DrawImage,
        DrawSlicedImage,
    }

    internal struct RecordHeader
    {
        public int Size;
        public RecordType Type;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct DrawLineCommand
    {
        [FieldOffset(0)] public Point StartPoint;
        [FieldOffset(16)] public Point EndPoint;
        [FieldOffset(32)] public uint PenIndex;
        [FieldOffset(36)] private uint QuadWordPad0;

        public DrawLineCommand(uint penIndex, Point startPoint, Point endPoint)
        {
            PenIndex = penIndex;
            StartPoint = startPoint;
            EndPoint = endPoint;
            QuadWordPad0 = 0;
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct DrawRectangleCommand
    {
        [FieldOffset(0)] public Rect Rectangle;
        [FieldOffset(32)] public uint BrushHandle;
        [FieldOffset(36)] public uint PenHandle;

        public DrawRectangleCommand(uint brushIndex, uint penIndex, Rect rectangle)
        {
            Rectangle = rectangle;
            BrushHandle = brushIndex;
            PenHandle = penIndex;
        }
    }


    [StructLayout(LayoutKind.Explicit)]
    internal struct DrawRoundedRectangleCommand
    {
        public DrawRoundedRectangleCommand
        (
            uint hBrush,
            uint hPen,
            Rect rectangle,
            double radiusX,
            double radiusY
        )
        {
            this.hBrush = hBrush;
            this.hPen = hPen;
            this.rectangle = rectangle;
            this.radiusX = radiusX;
            this.radiusY = radiusY;
        }

        [FieldOffset(0)] public Rect rectangle;
        [FieldOffset(32)] public double radiusX;
        [FieldOffset(40)] public double radiusY;
        [FieldOffset(48)] public uint hBrush;
        [FieldOffset(52)] public uint hPen;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct DrawGlyphRunCommand
    {
        public DrawGlyphRunCommand
        (
            uint hForegroundBrush,
            uint hGlyphRun,
            Point origin,
            double maxTextWidth,
            double maxTextHeight
        )
        {
            this.hForegroundBrush = hForegroundBrush;
            this.hGlyphRun = hGlyphRun;
            this.origin = origin;
            this.maxTextWidth = maxTextWidth;
            this.maxTextHeight = maxTextHeight;
        }

        [FieldOffset(0)] public uint hForegroundBrush;
        [FieldOffset(4)] public uint hGlyphRun;
        [FieldOffset(8)] public Point origin;
        [FieldOffset(16)] public double maxTextWidth;
        [FieldOffset(24)] public double maxTextHeight;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct DrawGeometryCommand
    {
        public DrawGeometryCommand (
            uint hBrush,
            uint hPen,
            uint hGeometry
        )
        {
            this.hBrush = hBrush;
            this.hPen = hPen;
            this.hGeometry = hGeometry;
            this.QuadWordPad0 = 0;
        }

        [FieldOffset(0)] public uint hBrush;
        [FieldOffset(4)] public uint hPen;
        [FieldOffset(8)] public uint hGeometry;
        [FieldOffset(12)] private uint QuadWordPad0;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct DrawImageCommand
    {
        public DrawImageCommand(uint hImageSource, Rect rectangle)
        {
            this.hImageSource = hImageSource;
            this.rectangle = rectangle;
            this.QuadWordPad0 = 0;
        }

        [FieldOffset(0)] public Rect rectangle;
        [FieldOffset(32)] public uint hImageSource;
        [FieldOffset(36)] private uint QuadWordPad0;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct DrawSlicedImageCommand
    {
        public DrawSlicedImageCommand(uint hImageSource, Rect rectangle, (double top, double right, double bottom, double left) slice)
        {
            this.hImageSource = hImageSource;
            this.rectangle = rectangle;
            this.sliceTop = slice.top;
            this.sliceRight = slice.right;
            this.sliceBottom = slice.left;
            this.sliceLeft = slice.bottom;
            this.QuadWordPad0 = 0;
        }

        [FieldOffset(0)] public Rect rectangle;
        [FieldOffset(32)] public uint hImageSource;
        [FieldOffset(36)] public double sliceTop;
        [FieldOffset(44)] public double sliceRight;
        [FieldOffset(52)] public double sliceBottom;
        [FieldOffset(60)] public double sliceLeft;
        [FieldOffset(68)] public uint QuadWordPad0;
    }

    internal abstract class RecordReader : DrawingContext
    {
        public bool ShouldStopWalking { get; set; }

        /// <summary>
        /// RecordReader implementations are never opened, so they shouldn't be closed.
        /// </summary>
        public sealed override void Close()
        {
            Debug.Assert(false);
        }

        /// <summary>
        /// RecordReader implementations are never opened, so they shouldn't be disposed.
        /// </summary>
        protected override void DisposeCore()
        {
            Debug.Assert(false);
        }
    }
}