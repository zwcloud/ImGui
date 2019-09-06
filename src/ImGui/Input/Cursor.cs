namespace ImGui.Input
{
    /// <summary>
    /// Cursor types
    /// </summary>
    /// TODO implement all these cursor
    public enum Cursor
    {
#if false
        Auto,
#endif
        Default,
#if false
        None,
        ContextMenu,
        Help,
        Pointer,
        Progress,
        Wait,
        Cell,
        Crosshair,
#endif
        Text,
#if false
        VerticalText,
        Alias,
        Copy,
        Move,
        NoDrop,
        NotAllowed,
        Grab,
        Grabbing,
        EResize,
        NResize,
        NeResize,
        NwResize,
        SResize,
        SeResize,
        SwResize,
        WResize,
#endif
        EwResize,
        NsResize,
        NeswResize,
        NwseResize,
#if false
        ColResize,
        RowResize,
        AllScroll,
        ZoomIn,
        ZoomOut
#endif
    }
}