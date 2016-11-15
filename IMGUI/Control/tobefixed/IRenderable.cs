namespace ImGui
{
    internal interface IRenderable
    {
        /// <summary>
        /// Does this control need repaint? TODO expand this into render tree
        /// </summary>
        bool NeedRepaint { get; set; }
        void OnRender(Cairo.Context g);
        void OnClear(Cairo.Context g);
    }
}