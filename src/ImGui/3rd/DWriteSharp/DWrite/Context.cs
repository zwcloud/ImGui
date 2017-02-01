using System;
using System.Runtime.InteropServices;

namespace DWriteSharp
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void PointAdder(float x, float y);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void BezierAdder(float x0, float y0, float x1, float y1, float x2, float y2);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void PathCloser();
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void FigureBeginner(float x, float y);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void FigureEnder();

    public struct Context
    {
        public float offsetX;
        public float offsetY;
        public PointAdder PointAdder;
        public BezierAdder BezierAdder;
        public PathCloser PathCloser;
        public FigureBeginner FigureBeginner;
        public FigureEnder FigureEnder;
    }
}
