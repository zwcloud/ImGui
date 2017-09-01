namespace ImGui.Layout
{
    public struct LayoutOption
    {
        internal LayoutOptionType Type { get; set; }
        internal int Value { get; set; }

        internal LayoutOption(LayoutOptionType type, int value)
        {
            this.Value = value;
            this.Type = type;
        }
    }
}
