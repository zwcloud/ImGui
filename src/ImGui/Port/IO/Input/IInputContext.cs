namespace ImGui
{
    /// <summary>
    /// input-related functions
    /// </summary>
    interface IInputContext
    {
        bool IsMouseLeftButtonDown { get; }

        bool IsMouseMiddleButtonDown { get; }

        bool IsMouseRightButtonDown { get; }

        Point MousePosition { get; }

        Cursor MouseCursor { get; set; }
    }
}
