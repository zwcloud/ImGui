using System;

namespace ImGui
{
    internal interface ITextContext : IDisposable
    {
        int FontSize { get; }
        TextAlignment Alignment { get; set; }

        int MaxWidth { get; set; }
        int MaxHeight { get; set; }
        Rect Rect { get; }
        string Text { get; set; }

        void Build(Point offset, PointAdder pointAdder, BezierAdder bezierAdder, PathCloser pathCloser, FigureBeginner figureBeginner, FigureEnder figureEnder);

        uint XyToIndex(float pointX, float pointY, out bool isInside);
        void IndexToXY(uint textPosition, bool isTrailingHit,
            out float pointX, out float pointY, out float height);
    }
}