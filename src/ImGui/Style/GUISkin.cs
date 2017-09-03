using System;
using System.Collections.Generic;
using ImGui.Common.Primitive;

namespace ImGui
{
    internal partial class GUISkin
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
                this.InternalStyle.Set<double>(GUIStyleName._FieldWidth, 200);
                this.InternalStyle.Set<double>(GUIStyleName._LabelWidth, 60);
                this.InternalStyle.Set<double>(GUIStyleName._LabelHeight, 70);
                this.InternalStyle.Set<double>(GUIStyleName._ControlLabelSpacing, 5);
            }

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
            //Set default styles for each control
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
        public double FieldWidth
        {
            get => InternalStyle.Get<double>(GUIStyleName._FieldWidth);
            set =>InternalStyle.Set<double>(GUIStyleName._FieldWidth, value);
        }
        #endregion
    }
}
