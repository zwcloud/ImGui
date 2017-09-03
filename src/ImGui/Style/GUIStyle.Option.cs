namespace ImGui
{
    internal partial class GUIStyle
    {
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