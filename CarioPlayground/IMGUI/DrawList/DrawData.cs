namespace ImGui
{
    // All draw data to render an ImGui frame
    class DrawData
    {
        public bool Valid;                  // Only valid after Render() is called and before the next NewFrame() is called.
        public DrawList[] CmdLists;
        public int CmdListsCount;
        public int TotalVtxCount;          // For convenience, sum of all cmd_lists vtx_buffer.Size
        public int TotalIdxCount;          // For convenience, sum of all cmd_lists idx_buffer.Size

        // Functions
        public DrawData()
        {
            CmdListsCount = TotalVtxCount = TotalIdxCount = 0;
        }
    }
}