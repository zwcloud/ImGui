using System;

namespace ImGui
{
    public enum StyleType
    {
        @int,
        @double,
        Color,
        @string,
    }

    public class StyleModifier
    {
        public StylePropertyName name;
        public StyleType styleType;
        public GUIState state;

        public int intOriginal;
        public double doubleOriginal;
        public Color colorOriginal;
        public string stringOriginal;

        public int intValue;
        public double doubleValue;
        public Color colorValue;
        public string stringValue;

        public StyleModifier(StylePropertyName name, StyleType styleType, int value, GUIState state = GUIState.Normal)
        {
            this.name = name;
            this.styleType = styleType;
            this.state = state;
            this.intValue = value;
        }
        public StyleModifier(StylePropertyName name, StyleType styleType, double value, GUIState state = GUIState.Normal)
        {
            this.name = name;
            this.styleType = styleType;
            this.state = state;
            this.doubleValue = value;
        }

        public StyleModifier(StylePropertyName name, StyleType styleType, Color value, GUIState state = GUIState.Normal)
        {
            this.name = name;
            this.styleType = styleType;
            this.state = state;
            this.colorValue = value;
        }

        public StyleModifier(StylePropertyName name, StyleType styleType, string value, GUIState state = GUIState.Normal)
        {
            this.name = name;
            this.styleType = styleType;
            this.state = state;
            this.stringValue = value;
        }

        internal void Modify(GUIStyle style)
        {
            switch (styleType)
            {
                case StyleType.@int:
                    this.intOriginal = style.Get<int>(this.name, this.state);
                    style.Set<int>(name, this.intValue, this.state);
                    break;
                case StyleType.@double:
                    this.doubleOriginal = style.Get<double>(this.name, this.state);
                    style.Set<double>(name, this.doubleValue, this.state);
                    break;
                case StyleType.Color:
                    this.colorOriginal = style.Get<Color>(this.name, this.state);
                    style.Set<Color>(name, this.colorValue, this.state);
                    break;
                case StyleType.@string:
                    this.stringOriginal = style.Get<string>(this.name, this.state);
                    style.Set<string>(name, this.stringValue, this.state);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        internal void Restore(GUIStyle style)
        {
            switch (styleType)
            {
                case StyleType.@int:
                    style.Set<int>(name, intOriginal, this.state);
                    break;
                case StyleType.@double:
                    style.Set<double>(name, doubleOriginal, this.state);
                    break;
                case StyleType.Color:
                    style.Set<Color>(name, this.colorOriginal, this.state);
                    break;
                case StyleType.@string:
                    style.Set<string>(name, this.stringOriginal, this.state);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
