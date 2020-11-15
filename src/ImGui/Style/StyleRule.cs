using System.Diagnostics;

namespace ImGui
{
    public interface IStyleRule {}

    [DebuggerDisplay("{Name}, {State}, {Value}")]
    public class StyleRule<T> : IStyleRule
    {
        public StylePropertyName Name { get; set; }
        public GUIState State { get; set; }
        public T Value { get; set; }

        public StyleRule(StylePropertyName name, T value)
        {
            this.Name = name;
            this.Value = value;
            this.State = GUIState.Normal;
        }

        public StyleRule(StylePropertyName name, T value, GUIState state)
        {
            this.Name = name;
            this.Value = value;
            this.State = state;
        }
    }
}