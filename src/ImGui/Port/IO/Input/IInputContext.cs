namespace ImGui
{
    interface IInputContext
    {
        bool IsMouseLeftButtonDown { get; }

        bool IsMouseMiddleButtonDown { get; }

        bool IsMouseRightButtonDown { get; }

        Point MousePosition { get; }
    }
}
