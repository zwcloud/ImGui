namespace ImGui
{
    public interface BaseForm : IWindow
    {
        object InternalForm { get; }
    }
}