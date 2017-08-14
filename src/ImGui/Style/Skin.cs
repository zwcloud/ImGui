using System;
using System.Collections.Generic;
using ImGui.Common.Primitive;

namespace ImGui
{
    internal class GUISkin
    {
        public static GUISkin Instance { get; } = new GUISkin();

        public StyleModifier[] this[GUIControlName name] => this.styles[name];

        /// <summary>
        /// Get a GUIStyle from a string.
        /// </summary>
        public StyleModifier[] GetStyle(string styleName)
        {
            bool exist = this.controlNames.TryGetValue(styleName, out GUIControlName controlName);
            if (!exist)
            {
                throw new ArgumentOutOfRangeException(nameof(styleName), string.Format("style<{0}> doesn't exist.", styleName));
            }
            return this.styles[controlName];
        }

        private readonly Dictionary<GUIControlName, StyleModifier[]> styles = new Dictionary<GUIControlName, StyleModifier[]>();

        public GUIStyle InternalStyle;

        private readonly Dictionary<string, GUIControlName> controlNames = new Dictionary<string, GUIControlName>();

        private GUISkin()
        {
            //Set up default styles for controls
            this.InternalStyle = new GUIStyle();//internal styles
            {
                this.InternalStyle.Set<double>(GUIStyleName._FieldControlWidth, 60);
                this.InternalStyle.Set<double>(GUIStyleName._LabelWidth, 60);
                this.InternalStyle.Set<double>(GUIStyleName._LabelHeight, 70);
                this.InternalStyle.Set<double>(GUIStyleName._ControlLabelSpacing, 5);
            }

            var borderColor = Color.Black;
            var Button = new StyleModifier[]
            {
                //normal
                new StyleModifier(GUIStyleName.BorderTop, StyleType.@double, 10.0),
                new StyleModifier(GUIStyleName.BorderRight, StyleType.@double, 10.0),
                new StyleModifier(GUIStyleName.BorderBottom, StyleType.@double, 10.0),
                new StyleModifier(GUIStyleName.BorderLeft, StyleType.@double, 10.0),
                new StyleModifier(GUIStyleName.PaddingTop, StyleType.@double, 20.0),
                new StyleModifier(GUIStyleName.PaddingRight, StyleType.@double, 20.0),
                new StyleModifier(GUIStyleName.PaddingBottom, StyleType.@double, 20.0),
                new StyleModifier(GUIStyleName.PaddingLeft, StyleType.@double, 20.0),
                new StyleModifier(GUIStyleName.BackgroundColor, StyleType.Color, new Color(0.67f, 0.40f, 0.40f, 0.60f)),
                new StyleModifier(GUIStyleName.FontWeight, StyleType.@int, (int)FontWeight.Normal),

                //hover
                new StyleModifier(GUIStyleName.BorderTop, StyleType.@double, 15.0, GUIState.Hover),
                new StyleModifier(GUIStyleName.BorderRight, StyleType.@double, 15.0, GUIState.Hover),
                new StyleModifier(GUIStyleName.BorderBottom,StyleType.@double,  15.0, GUIState.Hover),
                new StyleModifier(GUIStyleName.BorderLeft, StyleType.@double, 15.0, GUIState.Hover),
                new StyleModifier(GUIStyleName.BorderTopColor, StyleType.Color, borderColor, GUIState.Hover),
                new StyleModifier(GUIStyleName.BorderRightColor, StyleType.Color, borderColor, GUIState.Hover),
                new StyleModifier(GUIStyleName.BorderBottomColor, StyleType.Color, borderColor, GUIState.Hover),
                new StyleModifier(GUIStyleName.BorderLeftColor, StyleType.Color, borderColor, GUIState.Hover),
                new StyleModifier(GUIStyleName.PaddingTop, StyleType.@double, 15.0, GUIState.Hover),
                new StyleModifier(GUIStyleName.PaddingRight, StyleType.@double, 15.0, GUIState.Hover),
                new StyleModifier(GUIStyleName.PaddingBottom, StyleType.@double, 15.0, GUIState.Hover),
                new StyleModifier(GUIStyleName.PaddingLeft, StyleType.@double, 15.0, GUIState.Hover),
                new StyleModifier(GUIStyleName.BackgroundColor, StyleType.Color, new Color(0.67f, 0.40f, 0.40f, 1.00f), GUIState.Hover),
                new StyleModifier(GUIStyleName.FontWeight, StyleType.@int, (int)FontWeight.Normal, GUIState.Hover),

                //active
                new StyleModifier(GUIStyleName.BorderTop, StyleType.@double, 2.0, GUIState.Active),
                new StyleModifier(GUIStyleName.BorderRight, StyleType.@double, 2.0, GUIState.Active),
                new StyleModifier(GUIStyleName.BorderBottom, StyleType.@double, 2.0, GUIState.Active),
                new StyleModifier(GUIStyleName.BorderLeft, StyleType.@double, 2.0, GUIState.Active),
                new StyleModifier(GUIStyleName.BorderTopColor, StyleType.Color, borderColor, GUIState.Active),
                new StyleModifier(GUIStyleName.BorderRightColor, StyleType.Color, borderColor, GUIState.Active),
                new StyleModifier(GUIStyleName.BorderBottomColor, StyleType.Color, borderColor, GUIState.Active),
                new StyleModifier(GUIStyleName.BorderLeftColor, StyleType.Color, borderColor, GUIState.Active),
                new StyleModifier(GUIStyleName.PaddingTop, StyleType.@double, 2.0, GUIState.Active),
                new StyleModifier(GUIStyleName.PaddingRight, StyleType.@double, 2.0, GUIState.Active),
                new StyleModifier(GUIStyleName.PaddingBottom, StyleType.@double, 2.0, GUIState.Active),
                new StyleModifier(GUIStyleName.PaddingLeft, StyleType.@double, 2.0, GUIState.Active),
                new StyleModifier(GUIStyleName.BackgroundColor, StyleType.Color, new Color(0.80f, 0.50f, 0.50f, 1.00f), GUIState.Active),
                new StyleModifier(GUIStyleName.FontWeight, StyleType.@int, (int)FontWeight.Bold, GUIState.Active),
            };

            this.styles.Add(GUIControlName.Button, Button);
            //TODO add others

            this.controlNames.Add("Label", GUIControlName.Label);
            this.controlNames.Add("Image", GUIControlName.Image);
            this.controlNames.Add("Box", GUIControlName.Box);
            this.controlNames.Add("Space", GUIControlName.Space);
            this.controlNames.Add("Button", GUIControlName.Button);
            this.controlNames.Add("Toggle", GUIControlName.Toggle);
            this.controlNames.Add("ComboBox", GUIControlName.ComboBox);
            this.controlNames.Add("TextBox", GUIControlName.TextBox);
            this.controlNames.Add("Slider", GUIControlName.Slider);
            this.controlNames.Add("PolygonButton", GUIControlName.PolygonButton);
            this.controlNames.Add("CollapsingHeader", GUIControlName.CollapsingHeader);
            this.controlNames.Add("ColorField", GUIControlName.ColorField);
#if old
            GUIStyle Label = new GUIStyle();
            GUIStyle Image = new GUIStyle();
            GUIStyle Box = new GUIStyle();
            GUIStyle Space = new GUIStyle();
            GUIStyle Toggle = new GUIStyle();
            GUIStyle ComboBox = new GUIStyle();
            GUIStyle TextBox = new GUIStyle();
            GUIStyle Slider = new GUIStyle();
            GUIStyle PolygonButton = new GUIStyle();
            GUIStyle CollapsingHeader = new GUIStyle();
            GUIStyle ColorField = new GUIStyle();

            this.styles.Add(GUIControlName.Label, Label);
            this.styles.Add(GUIControlName.Image, Image);
            this.styles.Add(GUIControlName.Box, Box);
            this.styles.Add(GUIControlName.Space, Space);
            this.styles.Add(GUIControlName.Toggle, Toggle);
            this.styles.Add(GUIControlName.ComboBox, ComboBox);
            this.styles.Add(GUIControlName.TextBox, TextBox);
            this.styles.Add(GUIControlName.Slider, Slider);
            this.styles.Add(GUIControlName.PolygonButton, PolygonButton);
            this.styles.Add(GUIControlName.CollapsingHeader, CollapsingHeader);
            this.styles.Add(GUIControlName.ColorField, ColorField);


            //Set default styles for each control
            {
                Image.Set(GUIStyleName.BorderTop, 1.0, GUIState.Normal);
                Image.Set(GUIStyleName.BorderRight, 1.0, GUIState.Normal);
                Image.Set(GUIStyleName.BorderBottom, 1.0, GUIState.Normal);
                Image.Set(GUIStyleName.BorderLeft, 1.0, GUIState.Normal);
                Image.Set(GUIStyleName.BorderTopColor, Color.Black, GUIState.Normal);
                Image.Set(GUIStyleName.BorderRightColor, Color.Black, GUIState.Normal);
                Image.Set(GUIStyleName.BorderBottomColor, Color.Black, GUIState.Normal);
                Image.Set(GUIStyleName.BorderLeftColor, Color.Black, GUIState.Normal);
            }

            {
                var borderColor = Color.Rgb(24, 131, 215);
                var bgColor = Color.Rgb(242, 242, 242);
                Box.Set(GUIStyleName.BorderTop, 2.0, GUIState.Normal);
                Box.Set(GUIStyleName.BorderRight, 2.0, GUIState.Normal);
                Box.Set(GUIStyleName.BorderBottom, 2.0, GUIState.Normal);
                Box.Set(GUIStyleName.BorderLeft, 2.0, GUIState.Normal);
                Box.Set(GUIStyleName.BorderTopColor, borderColor, GUIState.Normal);
                Box.Set(GUIStyleName.BorderRightColor, borderColor, GUIState.Normal);
                Box.Set(GUIStyleName.BorderBottomColor, borderColor, GUIState.Normal);
                Box.Set(GUIStyleName.BorderLeftColor, borderColor, GUIState.Normal);
                Box.Set(GUIStyleName.PaddingTop, 2.0, GUIState.Normal);
                Box.Set(GUIStyleName.PaddingRight, 2.0, GUIState.Normal);
                Box.Set(GUIStyleName.PaddingBottom, 2.0, GUIState.Normal);
                Box.Set(GUIStyleName.PaddingLeft, 2.0, GUIState.Normal);
                //Box.Set(GUIStyleName.CellingSpacingHorizontal, 0.0, GUIState.Normal);
                //Box.Set(GUIStyleName.CellingSpacingVertical, 15.0, GUIState.Normal);
                Box.Set(GUIStyleName.BackgroundColor, bgColor, GUIState.Normal);
            }

            {
                var borderColor = Color.Black;
                //normal
                Button.Set(GUIStyleName.BorderTop, 2.0, GUIState.Normal);
                Button.Set(GUIStyleName.BorderRight, 2.0, GUIState.Normal);
                Button.Set(GUIStyleName.BorderBottom, 2.0, GUIState.Normal);
                Button.Set(GUIStyleName.BorderLeft, 2.0, GUIState.Normal);
                Button.Set(GUIStyleName.PaddingTop, 2.0, GUIState.Normal);
                Button.Set(GUIStyleName.PaddingRight, 2.0, GUIState.Normal);
                Button.Set(GUIStyleName.PaddingBottom, 2.0, GUIState.Normal);
                Button.Set(GUIStyleName.PaddingLeft, 2.0, GUIState.Normal);
                Button.Set(GUIStyleName.BackgroundColor, new Color(0.67f, 0.40f, 0.40f, 0.60f), GUIState.Normal);
                Button.Set(GUIStyleName.FontWeight, (int)FontWeight.Normal, GUIState.Normal);
                //hover
                Button.Set(GUIStyleName.BorderTop, 2.0, GUIState.Hover);
                Button.Set(GUIStyleName.BorderRight, 2.0, GUIState.Hover);
                Button.Set(GUIStyleName.BorderBottom, 2.0, GUIState.Hover);
                Button.Set(GUIStyleName.BorderLeft, 2.0, GUIState.Hover);
                Button.Set(GUIStyleName.BorderTopColor, borderColor, GUIState.Hover);
                Button.Set(GUIStyleName.BorderRightColor, borderColor, GUIState.Hover);
                Button.Set(GUIStyleName.BorderBottomColor, borderColor, GUIState.Hover);
                Button.Set(GUIStyleName.BorderLeftColor, borderColor, GUIState.Hover);
                Button.Set(GUIStyleName.PaddingTop, 2.0, GUIState.Hover);
                Button.Set(GUIStyleName.PaddingRight, 2.0, GUIState.Hover);
                Button.Set(GUIStyleName.PaddingBottom, 2.0, GUIState.Hover);
                Button.Set(GUIStyleName.PaddingLeft, 2.0, GUIState.Hover);
                Button.Set(GUIStyleName.BackgroundColor, new Color(0.67f, 0.40f, 0.40f, 1.00f), GUIState.Hover);
                Button.Set(GUIStyleName.FontWeight, (int)FontWeight.Normal, GUIState.Hover);
                //active
                Button.Set(GUIStyleName.BorderTop, 2.0, GUIState.Active);
                Button.Set(GUIStyleName.BorderRight, 2.0, GUIState.Active);
                Button.Set(GUIStyleName.BorderBottom, 2.0, GUIState.Active);
                Button.Set(GUIStyleName.BorderLeft, 2.0, GUIState.Active);
                Button.Set(GUIStyleName.BorderTopColor, borderColor, GUIState.Active);
                Button.Set(GUIStyleName.BorderRightColor, borderColor, GUIState.Active);
                Button.Set(GUIStyleName.BorderBottomColor, borderColor, GUIState.Active);
                Button.Set(GUIStyleName.BorderLeftColor, borderColor, GUIState.Active);
                Button.Set(GUIStyleName.PaddingTop, 2.0, GUIState.Active);
                Button.Set(GUIStyleName.PaddingRight, 2.0, GUIState.Active);
                Button.Set(GUIStyleName.PaddingBottom, 2.0, GUIState.Active);
                Button.Set(GUIStyleName.PaddingLeft, 2.0, GUIState.Active);
                Button.Set(GUIStyleName.BackgroundColor, new Color(0.80f, 0.50f, 0.50f, 1.00f), GUIState.Active);
                Button.Set(GUIStyleName.FontWeight, (int)FontWeight.Bold, GUIState.Active);
            }

            {
                var bgColor = new Color(0x9F, 0x9F, 0x9F);
                Toggle.Set(GUIStyleName.BackgroundColor, bgColor, GUIState.Normal);
                Toggle.Set(GUIStyleName.BackgroundColor, bgColor, GUIState.Hover);
                Toggle.Set(GUIStyleName.BackgroundColor, bgColor, GUIState.Active);
            }

            {
                Slider.Set(GUIStyleName.Slider_LineUsed, Color.Rgb(0, 151, 167), GUIState.Normal);
                Slider.Set(GUIStyleName.Slider_LineUsed, Color.Rgb(0, 151, 167), GUIState.Hover);
                Slider.Set(GUIStyleName.Slider_LineUsed, Color.Rgb(0, 151, 167), GUIState.Active);
                Slider.Set(GUIStyleName.Slider_LineUnused, Color.Rgb(117, 117, 117), GUIState.Normal);
                Slider.Set(GUIStyleName.Slider_LineUnused, Color.Rgb(255, 128, 171), GUIState.Hover);
                Slider.Set(GUIStyleName.Slider_LineUnused, Color.Rgb(255, 128, 171), GUIState.Active);
            }

            {
                PolygonButton.Set(GUIStyleName.TextAlignment, (int)TextAlignment.Center, GUIState.Normal);
                PolygonButton.Set(GUIStyleName.BackgroundColor, Color.Clear, GUIState.Normal);

                PolygonButton.Set(GUIStyleName.TextAlignment, (int)TextAlignment.Center, GUIState.Hover);
                PolygonButton.Set(GUIStyleName.BackgroundColor, Color.Clear, GUIState.Hover);
                PolygonButton.Set(GUIStyleName.LineColor, Color.Blue, GUIState.Hover);
                PolygonButton.Set(GUIStyleName.FillColor, Color.Blue, GUIState.Hover);

                PolygonButton.Set(GUIStyleName.TextAlignment, (int)TextAlignment.Center, GUIState.Active);
                PolygonButton.Set(GUIStyleName.BackgroundColor, Color.Clear, GUIState.Active);
                PolygonButton.Set(GUIStyleName.LineColor, Color.Blue, GUIState.Active);
                PolygonButton.Set(GUIStyleName.FillColor, Color.Red, GUIState.Active);
            }

            {
                TextBox.Set<double>(GUIStyleName.PaddingLeft, 5, GUIState.Normal);
                TextBox.Set<double>(GUIStyleName.PaddingLeft, 5, GUIState.Hover);
                TextBox.Set<double>(GUIStyleName.PaddingLeft, 5, GUIState.Active);
                TextBox.Set<double>(GUIStyleName.PaddingTop, 5, GUIState.Normal);
                TextBox.Set<double>(GUIStyleName.PaddingTop, 5, GUIState.Hover);
                TextBox.Set<double>(GUIStyleName.PaddingTop, 5, GUIState.Active);
                TextBox.Set<double>(GUIStyleName.PaddingRight, 5, GUIState.Normal);
                TextBox.Set<double>(GUIStyleName.PaddingRight, 5, GUIState.Hover);
                TextBox.Set<double>(GUIStyleName.PaddingRight, 5, GUIState.Active);
                TextBox.Set<double>(GUIStyleName.PaddingBottom, 5, GUIState.Normal);
                TextBox.Set<double>(GUIStyleName.PaddingBottom, 5, GUIState.Hover);
                TextBox.Set<double>(GUIStyleName.PaddingBottom, 5, GUIState.Active);
                TextBox.Set<double>(GUIStyleName.FontSize, CurrentOS.IsAndroid ? 32 : 13, GUIState.Normal);
                TextBox.Set<double>(GUIStyleName.FontSize, CurrentOS.IsAndroid ? 32 : 13, GUIState.Hover);
                TextBox.Set<double>(GUIStyleName.FontSize, CurrentOS.IsAndroid ? 32 : 13, GUIState.Active);
            }

            {
                var borderColor = Color.Black;
                //normal
                CollapsingHeader.Set(GUIStyleName.BorderTop, 2.0, GUIState.Normal);
                CollapsingHeader.Set(GUIStyleName.BorderRight, 2.0, GUIState.Normal);
                CollapsingHeader.Set(GUIStyleName.BorderBottom, 2.0, GUIState.Normal);
                CollapsingHeader.Set(GUIStyleName.BorderLeft, 2.0, GUIState.Normal);
                CollapsingHeader.Set(GUIStyleName.PaddingTop, 2.0, GUIState.Normal);
                CollapsingHeader.Set(GUIStyleName.PaddingRight, 2.0, GUIState.Normal);
                CollapsingHeader.Set(GUIStyleName.PaddingBottom, 2.0, GUIState.Normal);
                CollapsingHeader.Set(GUIStyleName.PaddingLeft, 2.0, GUIState.Normal);
                CollapsingHeader.Set(GUIStyleName.BackgroundColor, new Color(0.67f, 0.40f, 0.40f, 0.60f), GUIState.Normal);
                CollapsingHeader.Set(GUIStyleName.FontWeight, (int)FontWeight.Normal, GUIState.Normal);
                //hover
                CollapsingHeader.Set(GUIStyleName.BorderTop, 2.0, GUIState.Hover);
                CollapsingHeader.Set(GUIStyleName.BorderRight, 2.0, GUIState.Hover);
                CollapsingHeader.Set(GUIStyleName.BorderBottom, 2.0, GUIState.Hover);
                CollapsingHeader.Set(GUIStyleName.BorderLeft, 2.0, GUIState.Hover);
                CollapsingHeader.Set(GUIStyleName.BorderTopColor, borderColor, GUIState.Hover);
                CollapsingHeader.Set(GUIStyleName.BorderRightColor, borderColor, GUIState.Hover);
                CollapsingHeader.Set(GUIStyleName.BorderBottomColor, borderColor, GUIState.Hover);
                CollapsingHeader.Set(GUIStyleName.BorderLeftColor, borderColor, GUIState.Hover);
                CollapsingHeader.Set(GUIStyleName.PaddingTop, 2.0, GUIState.Hover);
                CollapsingHeader.Set(GUIStyleName.PaddingRight, 2.0, GUIState.Hover);
                CollapsingHeader.Set(GUIStyleName.PaddingBottom, 2.0, GUIState.Hover);
                CollapsingHeader.Set(GUIStyleName.PaddingLeft, 2.0, GUIState.Hover);
                CollapsingHeader.Set(GUIStyleName.BackgroundColor, new Color(0.67f, 0.40f, 0.40f, 1.00f), GUIState.Hover);
                CollapsingHeader.Set(GUIStyleName.FontWeight, (int)FontWeight.Normal, GUIState.Hover);
                //active
                CollapsingHeader.Set(GUIStyleName.BorderTop, 2.0, GUIState.Active);
                CollapsingHeader.Set(GUIStyleName.BorderRight, 2.0, GUIState.Active);
                CollapsingHeader.Set(GUIStyleName.BorderBottom, 2.0, GUIState.Active);
                CollapsingHeader.Set(GUIStyleName.BorderLeft, 2.0, GUIState.Active);
                CollapsingHeader.Set(GUIStyleName.BorderTopColor, borderColor, GUIState.Active);
                CollapsingHeader.Set(GUIStyleName.BorderRightColor, borderColor, GUIState.Active);
                CollapsingHeader.Set(GUIStyleName.BorderBottomColor, borderColor, GUIState.Active);
                CollapsingHeader.Set(GUIStyleName.BorderLeftColor, borderColor, GUIState.Active);
                CollapsingHeader.Set(GUIStyleName.PaddingTop, 2.0, GUIState.Active);
                CollapsingHeader.Set(GUIStyleName.PaddingRight, 2.0, GUIState.Active);
                CollapsingHeader.Set(GUIStyleName.PaddingBottom, 2.0, GUIState.Active);
                CollapsingHeader.Set(GUIStyleName.PaddingLeft, 2.0, GUIState.Active);
                CollapsingHeader.Set(GUIStyleName.BackgroundColor, new Color(0.80f, 0.50f, 0.50f, 1.00f), GUIState.Active);
                CollapsingHeader.Set(GUIStyleName.FontWeight, (int)FontWeight.Bold, GUIState.Active);
            }

            {
                ColorField.Set(GUIStyleName.BorderTop, 1.0, GUIState.Normal);
                ColorField.Set(GUIStyleName.BorderRight, 1.0, GUIState.Normal);
                ColorField.Set(GUIStyleName.BorderBottom, 1.0, GUIState.Normal);
                ColorField.Set(GUIStyleName.BorderLeft, 1.0, GUIState.Normal);
                ColorField.Set(GUIStyleName.BorderTopColor, Color.Metal, GUIState.Normal);
                ColorField.Set(GUIStyleName.BorderRightColor, Color.Metal, GUIState.Normal);
                ColorField.Set(GUIStyleName.BorderBottomColor, Color.Metal, GUIState.Normal);
                ColorField.Set(GUIStyleName.BorderLeftColor, Color.Metal, GUIState.Normal);
            }
#endif
        }

        #region short-cuts
        public double FieldControlWidth
        {
            get => InternalStyle.Get<double>(GUIStyleName._FieldControlWidth);
            set =>InternalStyle.Set<double>(GUIStyleName._FieldControlWidth, value);
        }
        #endregion
}
}
