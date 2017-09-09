namespace ImGui
{
    internal partial class GUIStyle
    {
        public void ApplySkin(GUIControlName control)
        {
            var modifiers = GUISkin.Current.GetStyleModifiers(control);
            foreach (var modifier in modifiers)
            {
                this.Push(modifier);
            }
        }

        public void ApplyOption(LayoutOptions? options)
        {
            if (!options.HasValue) return;

            var optionValue = options.Value;
            if (optionValue.fontSize.HasValue)
            {
                PushFontSize(optionValue.fontSize.Value);
            }

            if (optionValue.fontColor.HasValue)
            {
                PushFontColor(optionValue.fontColor.Value);
            }

            if (optionValue.fontFamily != null)
            {
                PushFontFamily(optionValue.fontFamily);
            }
        }

    }
}