using System;
using System.Collections.Generic;
using System.Text;

namespace ImGui
{
    enum GUIControlName
    {
        Label,
        Image,
        ToolTip,
        Box,
        Space,

        //_StatefulStart,

        Button,
        Toggle,
        ComboBox,
        TextBox,
        Slider,
        PolygonButton,
    }


    class GUISkin
    {
        public static GUISkin Instance { get; private set; }

        static GUISkin() { Instance = new GUISkin(); }

        public GUIStyle this[GUIControlName name]
        {
            get { return styles[name]; }
        }

        /// <summary> Get a GUIStyle from a string. </summary>
        public GUIStyle GetStyle(string styleName)
        {
            bool exist = this.controlNames.TryGetValue(styleName, out GUIControlName controlName);
            if(!exist)
            {
                throw new ArgumentOutOfRangeException(nameof(styleName), string.Format("style<{0}> doesn't exist.", styleName));
            }
            return this.styles[controlName];
        }

        private Dictionary<GUIControlName, GUIStyle> styles = new Dictionary<GUIControlName, GUIStyle>();

        public GUIStyle InternalStyle;

        private Dictionary<string, GUIControlName> controlNames = new Dictionary<string, GUIControlName>();

        private GUISkin()
        {
            InternalStyle = new GUIStyle();//internal styles
            {
                InternalStyle.Set<double>(GUIStyleName._LabelWidth, 60);
                InternalStyle.Set<double>(GUIStyleName._LabelHeight, 70);
                InternalStyle.Set<double>(GUIStyleName._ControlLabelSpacing, 5);
            }

            GUIStyle Label      = new GUIStyle();//no modification
            GUIStyle Image      = new GUIStyle();
            GUIStyle Box        = new GUIStyle();
            GUIStyle Space      = new GUIStyle();
            GUIStyle Button     = new GUIStyle();
            GUIStyle Toggle     = new GUIStyle();
            GUIStyle ComboBox   = new GUIStyle();
            GUIStyle TextBox    = new GUIStyle();
            GUIStyle Slider     = new GUIStyle();
            GUIStyle PolygonButton = new GUIStyle();

            styles.Add(GUIControlName.Label        , Label        );
            styles.Add(GUIControlName.Image        , Image        );
            styles.Add(GUIControlName.Box          , Box          );
            styles.Add(GUIControlName.Space        , Space        );
            styles.Add(GUIControlName.Button       , Button       );
            styles.Add(GUIControlName.Toggle       , Toggle       );
            styles.Add(GUIControlName.ComboBox     , ComboBox     );
            styles.Add(GUIControlName.TextBox      , TextBox      );
            styles.Add(GUIControlName.Slider       , Slider       );
            styles.Add(GUIControlName.PolygonButton, PolygonButton);

            controlNames.Add("Label",         GUIControlName.Label        );
            controlNames.Add("Image",         GUIControlName.Image        );
            controlNames.Add("Box",           GUIControlName.Box          );
            controlNames.Add("Space",         GUIControlName.Space        );
            controlNames.Add("Button",        GUIControlName.Button       );
            controlNames.Add("Toggle",        GUIControlName.Toggle       );
            controlNames.Add("ComboBox",      GUIControlName.ComboBox     );
            controlNames.Add("TextBox",       GUIControlName.TextBox      );
            controlNames.Add("Slider",        GUIControlName.Slider       );
            controlNames.Add("PolygonButton", GUIControlName.PolygonButton);

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
                TextBox.Set<double>(GUIStyleName.PaddingLeft,   5, GUIState.Normal);
                TextBox.Set<double>(GUIStyleName.PaddingLeft,   5, GUIState.Hover);
                TextBox.Set<double>(GUIStyleName.PaddingLeft,   5, GUIState.Active);
                TextBox.Set<double>(GUIStyleName.PaddingTop,    5, GUIState.Normal);
                TextBox.Set<double>(GUIStyleName.PaddingTop,    5, GUIState.Hover);
                TextBox.Set<double>(GUIStyleName.PaddingTop,    5, GUIState.Active);
                TextBox.Set<double>(GUIStyleName.PaddingRight,  5, GUIState.Normal);
                TextBox.Set<double>(GUIStyleName.PaddingRight,  5, GUIState.Hover);
                TextBox.Set<double>(GUIStyleName.PaddingRight,  5, GUIState.Active);
                TextBox.Set<double>(GUIStyleName.PaddingBottom, 5, GUIState.Normal);
                TextBox.Set<double>(GUIStyleName.PaddingBottom, 5, GUIState.Hover);
                TextBox.Set<double>(GUIStyleName.PaddingBottom, 5, GUIState.Active);
                TextBox.Set<double>(GUIStyleName.FontSize, 13, GUIState.Normal);
                TextBox.Set<double>(GUIStyleName.FontSize, 13, GUIState.Hover);
                TextBox.Set<double>(GUIStyleName.FontSize, 13, GUIState.Active);
            }
        }

        #region Global Styles
        public double LabelWidth
        {
            get => InternalStyle.Get<double>(GUIStyleName._LabelWidth);
            set => InternalStyle.Set<double>(GUIStyleName._LabelWidth, value);
        }
        public double LabelHeight
        {
            get => InternalStyle.Get<double>(GUIStyleName._LabelHeight);
            set => InternalStyle.Set<double>(GUIStyleName._LabelHeight, value);
        }
        public double ControlLabelSpacing
        {
            get => InternalStyle.Get<double>(GUIStyleName._ControlLabelSpacing);
            set => InternalStyle.Set<double>(GUIStyleName._ControlLabelSpacing, value);
        }
        #endregion
    }
}
