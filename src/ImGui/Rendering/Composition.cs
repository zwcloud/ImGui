using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ImGui.Rendering.Composition
{
    internal enum RecordType
    {
        DrawLine,
        DrawRectangle,
        DrawRoundedRectangle,
        DrawEllipse,
        DrawGlyphRun,
        DrawText,
        DrawGeometry,
        DrawImage,
        DrawSlicedImage,
        PushClip,
        Pop,
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
        [FieldOffset(32)] public uint BrushIndex;
        [FieldOffset(36)] public uint PenIndex;

        public DrawRectangleCommand(uint brushIndex, uint penIndex, Rect rectangle)
        {
            Rectangle = rectangle;
            this.BrushIndex = brushIndex;
            this.PenIndex = penIndex;
        }
    }


    [StructLayout(LayoutKind.Explicit)]
    internal struct DrawRoundedRectangleCommand
    {
        public DrawRoundedRectangleCommand
        (
            uint brushIndex,
            uint penIndex,
            Rect rectangle,
            double radiusX,
            double radiusY
        )
        {
            this.BrushIndex = brushIndex;
            this.PenIndex = penIndex;
            this.Rectangle = rectangle;
            this.radiusX = radiusX;
            this.radiusY = radiusY;
        }

        [FieldOffset(0)] public Rect Rectangle;
        [FieldOffset(32)] public double radiusX;
        [FieldOffset(40)] public double radiusY;
        [FieldOffset(48)] public uint BrushIndex;
        [FieldOffset(52)] public uint PenIndex;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct DrawEllipseCommand
    {
        public DrawEllipseCommand(
            uint brushIndex,
            uint penIndex,
            Point center,
            double radiusX,
            double radiusY
        )
        {
            this.BrushIndex = brushIndex;
            this.PenIndex = penIndex;
            this.Center = center;
            this.RadiusX = radiusX;
            this.RadiusY = radiusY;
        }

        [FieldOffset(0)] public Point Center;
        [FieldOffset(16)] public double RadiusX;
        [FieldOffset(24)] public double RadiusY;
        [FieldOffset(32)] public uint BrushIndex;
        [FieldOffset(36)] public uint PenIndex;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct DrawGlyphRunCommand
    {
        public DrawGlyphRunCommand
        (
            uint brushIndex,
            uint glyphRunIndex
        )
        {
            this.BrushIndex = brushIndex;
            this.GlyphRunIndex = glyphRunIndex;
        }

        [FieldOffset(0)] public uint BrushIndex;
        [FieldOffset(4)] public uint GlyphRunIndex;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct DrawTextCommand
    {
        public DrawTextCommand
        (
            uint brushIndex,
            uint formattedTextIndex
        )
        {
            this.BrushIndex = brushIndex;
            this.FormattedTextIndex = formattedTextIndex;
        }

        [FieldOffset(0)] public uint BrushIndex;
        [FieldOffset(4)] public uint FormattedTextIndex;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct DrawGeometryCommand
    {
        public DrawGeometryCommand (
            uint brushIndex,
            uint penIndex,
            uint geometryIndex
        )
        {
            this.BrushIndex = brushIndex;
            this.PenIndex = penIndex;
            this.GeometryIndex = geometryIndex;
            this.QuadWordPad0 = 0;
        }

        [FieldOffset(0)] public uint BrushIndex;
        [FieldOffset(4)] public uint PenIndex;
        [FieldOffset(8)] public uint GeometryIndex;
        [FieldOffset(12)] private uint QuadWordPad0;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct DrawImageCommand
    {
        public DrawImageCommand(uint imageSourceIndex, Rect rectangle, Point uvMin, Point uvMax)
        {
            this.ImageSourceIndex = imageSourceIndex;
            this.rectangle = rectangle;
            this.UVMin = uvMin;
            this.UVMax = uvMax;
            this.QuadWordPad0 = 0;
        }

        [FieldOffset(0)] public Rect rectangle;
        [FieldOffset(32)] public uint ImageSourceIndex;
        [FieldOffset(36)] public Point UVMin;
        [FieldOffset(52)] public Point UVMax;
        [FieldOffset(68)] public uint QuadWordPad0;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct DrawSlicedImageCommand
    {
        public DrawSlicedImageCommand(uint imageSourceIndex, Rect rectangle, (double top, double right, double bottom, double left) slice)
        {
            this.ImageSourceIndex = imageSourceIndex;
            this.Rectangle = rectangle;
            this.sliceTop = slice.top;
            this.sliceRight = slice.right;
            this.sliceBottom = slice.left;
            this.sliceLeft = slice.bottom;
            this.QuadWordPad0 = 0;
        }

        [FieldOffset(0)] public Rect Rectangle;
        [FieldOffset(32)] public uint ImageSourceIndex;
        [FieldOffset(36)] public double sliceTop;
        [FieldOffset(44)] public double sliceRight;
        [FieldOffset(52)] public double sliceBottom;
        [FieldOffset(60)] public double sliceLeft;
        [FieldOffset(68)] public uint QuadWordPad0;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct PushClipCommand
    {
        public PushClipCommand(uint clipGeometryIndex)
        {
            this.ClipGeometryIndex = clipGeometryIndex;
            this.QuadWordPad0 = 0;
        }

        [FieldOffset(0)] public uint ClipGeometryIndex;
        [FieldOffset(4)] public uint QuadWordPad0;
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct PopCommand
    {
    }

    internal abstract class RecordReader : DrawingContext
    {
        public bool ShouldStopWalking { get; set; }

        public abstract void OnBeforeRead();

        public abstract void OnAfterRead(MeshList meshList);

        public abstract override void PushClip(Geometry clipGeometry);

        public abstract override void Pop();

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