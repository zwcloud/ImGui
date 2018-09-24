namespace ImGui
{
    public interface IStyleRule {}
    public class StyleRule<T> : IStyleRule
    {
        public GUIStyleName Name { get; set; }
        public GUIState State { get; set; }
        public T Value { get; set; }

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