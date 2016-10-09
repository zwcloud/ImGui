
namespace ImGui
{
    internal interface IRenderBox
    {
        RenderBoxType Type { get; }

        Rect Rect { get; set; }
        Content Content { get; }
        Style Style { get; }

        bool NeedRepaint { get; set; }

        bool Active { get; set; }
    }

    internal enum RenderBoxType
    {
        Dummy,
        Space,
        VerticalGroup,
        HorizontalGroup,
        SimpleControl,
    }
}
