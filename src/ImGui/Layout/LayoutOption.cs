namespace ImGui
{
    /// <summary>
    /// Class internally used to pass layout options into GUILayout functions. You don't use these directly, but construct them with the layouting functions in the GUILayout class.
    /// </summary>
    public sealed class LayoutOption
    {
        internal LayoutOptionType type;

        internal object Value;

        internal LayoutOption(LayoutOptionType type, object value)
        {
            this.type = type;
            this.Value = value;
        }
    }
}
