using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ImGui.Rendering.Composition
{
    internal enum RecordType
    {
        DrawLine,
        DrawRectangle,
        DrawGlyphRun,
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