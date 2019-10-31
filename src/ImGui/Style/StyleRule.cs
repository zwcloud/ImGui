using System.Diagnostics;
using ImGui.Rendering;

namespace ImGui
{
    public interface IStyleRule {}

    [DebuggerDisplay("{Name}, {State}, {Value}")]
    internal class StyleRule<T> : IStyleRule
    {
        public StylePropertyName Name { get; set; }
        public GUIState State { get; set; }
        public T Value { get; set; }

        /// <summary>
        /// Geometry owned by this rule.
        /// </summary>
        public Geometry Geometry;

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