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

            var o = options.Value;

            if(o.HorizontalStretchFactor.HasValue)
            {
                PushStretchFactor(false, o.HorizontalStretchFactor.Value);
            }

            if (o.VerticalStretchFactor.HasValue)
            {
                PushStretchFactor(true, o.VerticalStretchFactor.Value);
            }

            if(o.MinWidth.HasValue)
            {
                PushMinWidth(o.MinWidth.Value);
            }

            if (o.MaxWidth.HasValue)
            {
                PushMaxWidth(o.MaxWidth.Value);
            }

            if (o.MinHeight.HasValue)
            {
                PushMinHeight(o.MinHeight.Value);
            }

            if (o.MaxHeight.HasValue)
            {
                PushMaxHeight(o.MaxHeight.Value);
            }

            if (o.fontSize.HasValue)
            {
                PushFontSize(o.fontSize.Value);
            }

            if (o.fontColor.HasValue)
            {
                PushFontColor(o.fontColor.Value);
            }

            if (o.fontFamily != null)
            {
                PushFontFamily(o.fontFamily);
            }


        }

    }
}