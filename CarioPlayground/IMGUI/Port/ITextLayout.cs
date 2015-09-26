using System;

namespace IMGUI
{
    public interface ITextLayout : IDisposable
    {
        int MaxWidth { get; set; }
        int MaxHeight { get; set; }
        string Text { get; set; }
        ITextFormat TextFormat { get; set; }
        void Show(Cairo.Context context);
    }

}