namespace ImGui.Rendering
{
    [System.Flags]
    public enum VisualFlags
    {
        None,

        // This flag indicates that the content of the visual resource needs to
        // be updated. This is done by calling the virtual method RenderContent.
        // When this flag is set usually the IsSubtreeDirtyForRender is propagated.
        IsContentDirty,

        // IsSubtreeDirtyForRender indicates that at least one Visual
        // in the sub-graph of this Visual needs to be re-rendered.
        IsSubtreeDirtyForRender
    }
}