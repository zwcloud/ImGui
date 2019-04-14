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
        [FieldOffset(32)] public int PenIndex;
        [FieldOffset(36)] private int QuadWordPad0;

        public DrawLineCommand(int penIndex, Point startPoint, Point endPoint)
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
        [FieldOffset(32)] public int BrushHandle;
        [FieldOffset(36)] public int PenHandle;

        public DrawRectangleCommand(int brushIndex, int penIndex, Rect rectangle)
        {
            Rectangle = rectangle;
            BrushHandle = brushIndex;
            PenHandle = penIndex;
        }
    }
}