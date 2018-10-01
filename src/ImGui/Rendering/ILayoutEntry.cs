namespace ImGui.Rendering
{
    internal interface ILayoutEntry
    {
        bool ActiveSelf { get; }
        double X { get; set; }
        double Y { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        StyleRuleSet RuleSet { get; }
        LayoutEntry LayoutEntry { get; }
        LayoutGroup LayoutGroup { get; }
    }
}