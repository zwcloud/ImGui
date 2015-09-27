using System;

namespace IMGUI
{
    public interface ITextLayout : IDisposable
    {
        int MaxWidth { get; set; }
        int MaxHeight { get; set; }
        string Text { get; set; }
        ITextFormat TextFormat { get; }
        Cairo.Path Path { get; }
        void BuildPath(Cairo.Context context);
    }

}