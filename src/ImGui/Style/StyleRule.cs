using ImGui.Rendering;

namespace ImGui
{
    public interface IStyleRule {}
    internal class StyleRule<T> : IStyleRule
    {
        public GUIStyleName Name { get; set; }
        public GUIState State { get; set; }
        public T Value { get; set; }

        /// <summary>
        /// Primitive owned by this rule.
        /// </summary>
        public Primitive primitive;

        public StyleRule(GUIStyleName name, T value)
        {
            this.Name = name;
            this.Value = value;
            this.State = GUIState.Normal;
        }

        public StyleRule(GUIStyleName name, T value, GUIState state)
        {
            this.Name = name;
            this.Value = value;
            this.State = state;
        }
    }
}