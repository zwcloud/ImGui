namespace ImGui
{
    public interface ITextContext : System.IDisposable
    {
        int FontSize { get; }
        TextAlignment Alignment { get; set; }

        int MaxWidth { get; set; }
        int MaxHeight { get; set; }
        Rect Rect { get; }
        string Text { get; set; }
        Cairo.Path Path { get; }
        void BuildPath(Cairo.Context context);

        uint XyToIndex(float pointX, float pointY, out bool isInside);
        void IndexToXY(uint textPosition, bool isTrailingHit,
            out float pointX, out float pointY, out float height);
    }
}