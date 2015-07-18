using System.Collections.Generic;
using Cairo;

using Weight = Pango.Weight;
using Color = Cairo.Color;

namespace IMGUI
{
    public sealed class Skin
    {
        public Dictionary<string, Style> Button { get; private set; }
        public Dictionary<string, Style> Label { get; private set; }
        public Dictionary<string, Style> Toggle { get; private set; }
        public Dictionary<string, Style> ComboBox { get; private set; }
        public Dictionary<string, Style> Image { get; private set; }
        public Dictionary<string, Style> Radio { get; set; }

        internal static Skin _current;

        static Skin()
        {
            _current = new Skin();
        }

        public Skin()
        {
            Button = new Dictionary<string, Style>(3);
            Label = new Dictionary<string, Style>(3);
            Toggle = new Dictionary<string, Style>(3);
            ComboBox = new Dictionary<string, Style>(3);
            Image = new Dictionary<string, Style>(1);
            Radio = new Dictionary<string, Style>(2);

            #region Label
            {
                Label["Normal"] = Style.Make();
                Label["Hover"] = Style.Make();

                var activeModifiers = new[]
                {
                    new StyleModifier
                    {
                        Name = "Font", Value = new Font
                        {
                            Family = "Consolas",
                            Slant = FontSlant.Normal,
                            Weight = Weight.Normal,
                            Size = 12,
                            Color = new Color(0,0,1)
                        }
                    }
                };
                Label["Active"] = Style.Make(activeModifiers);
            }
            #endregion

            #region Button
            {
                StyleModifier[] normalModifiers =
                {
                    new StyleModifier{Name = "BorderTop", Value = new Length(1, Unit.Pixel)},
                    new StyleModifier{Name = "BorderRight", Value = new Length(1, Unit.Pixel)},
                    new StyleModifier{Name = "BorderBottom", Value = new Length(1, Unit.Pixel)},
                    new StyleModifier{Name = "BorderLeft", Value = new Length(1, Unit.Pixel)},

                    new StyleModifier{Name = "BorderTopColor", Value = CairoEx.ColorArgb(0xFFB3B3B3)},
                    new StyleModifier{Name = "BorderRightColor", Value = CairoEx.ColorArgb(0xFF7A7A7A)},
                    new StyleModifier{Name = "BorderBottomColor", Value = CairoEx.ColorArgb(0xFF7A7A7A)},
                    new StyleModifier{Name = "BorderLeftColor", Value = CairoEx.ColorArgb(0xFFB3B3B3)},

                    new StyleModifier
                    {
                        Name = "TextStyle",
                        Value = new TextStyle
                        {
                            TextAlign = TextAlignment.Center,
                            LineSpacing = 0,
                            TabSize = 4
                        }
                    },
                    
                    new StyleModifier
                    {
                        Name = "BackgroundStyle",
                        Value = new BackgroundStyle
                        {
                            Color = CairoEx.ColorArgb(0xFF999999),
                            Image = null,
                            Pattern = null
                        }
                    },
                    //var gradient = new LinearGradient(0, 0, 0, 1);
                    //gradient.AddColorStop(0, new Color(0.87, 0.93, 0.96));
                    //gradient.AddColorStop(1, new Color(0.65, 0.85, 0.96));
                    //Button.Normal.BackgroundPattern = gradient;
                    //Button.Hover.BackgroundPattern = gradient;
                    //Button.Active.BackgroundPattern = gradient;
                };
                Button["Normal"] = Style.Make(normalModifiers);
                
                StyleModifier[] hoverModifiers =
                {
                    new StyleModifier
                    {
                        Name = "BackgroundStyle",
                        Value = new BackgroundStyle
                        {
                            Color = CairoEx.ColorArgb(0xFFAFAFAF),
                            Image = null,
                            Pattern = null
                        }
                    }
                };
                Button["Hover"] = Style.Make(Button["Normal"], hoverModifiers);
                
                StyleModifier[] activeModifiers =
                {
                    new StyleModifier{Name = "BorderBottomColor", Value = CairoEx.ColorArgb(0xFFB3B3B3)},
                    new StyleModifier{Name = "BorderLeftColor", Value = CairoEx.ColorArgb(0xFF7A7A7A)},
                    new StyleModifier{Name = "BorderTopColor", Value = CairoEx.ColorArgb(0xFF7A7A7A)},
                    new StyleModifier{Name = "BorderRightColor", Value = CairoEx.ColorArgb(0xFFB3B3B3)},

                    new StyleModifier
                    {
                        Name = "Font",
                        Value = new Font
                        {
                            Family = "Consolas",
                            Slant = FontSlant.Normal,
                            Weight = Weight.Bold,
                            Size = 12,
                            Color = CairoEx.ColorBlack
                        }
                    },
                    new StyleModifier
                    {
                        Name = "BackgroundStyle",
                        Value = new BackgroundStyle
                        {
                            Color = CairoEx.ColorArgb(0xFF8F8F8F),
                            Image = null,
                            Pattern = null
                        }
                    }
                };
                Button["Active"] = Style.Make(Button["Normal"], activeModifiers);
            }
            #endregion

            #region Toggle
            {
                StyleModifier[] normalModifiers =
                {
                    new StyleModifier
                    {
                        Name = "BackgroundStyle",
                        Value = new BackgroundStyle
                        {
                            Color = new Color(0x9F, 0x9F, 0x9F),
                            Image = null,
                            Pattern = null
                        }
                    }
                };
                Toggle["Normal"] = Style.Make(normalModifiers);
                Toggle["Hover"] = Style.Make(Toggle["Normal"]);
                Toggle["Active"] = Style.Make(Toggle["Normal"]);
            }
            #endregion

            #region ComboBox
            {
                StyleModifier[] normalModifiers =
                {
                    new StyleModifier{Name = "BorderTop", Value = new Length(1, Unit.Pixel)},
                    new StyleModifier{Name = "BorderRight", Value = new Length(1, Unit.Pixel)},
                    new StyleModifier{Name = "BorderBottom", Value = new Length(1, Unit.Pixel)},
                    new StyleModifier{Name = "BorderLeft", Value = new Length(1, Unit.Pixel)},

                    new StyleModifier{Name = "BorderTopColor", Value = CairoEx.ColorBlack},
                    new StyleModifier{Name = "BorderRightColor", Value = CairoEx.ColorBlack},
                    new StyleModifier{Name = "BorderBottomColor", Value = CairoEx.ColorBlack},
                    new StyleModifier{Name = "BorderLeftColor", Value = CairoEx.ColorBlack},

                    new StyleModifier
                    {
                        Name = "TextStyle",
                        Value = new TextStyle
                        {
                            TextAlign = TextAlignment.Center,
                            LineSpacing = 0,
                            TabSize = 4
                        }
                    },
                    
                    new StyleModifier
                    {
                        Name = "BackgroundStyle",
                        Value = new BackgroundStyle
                        {
                            Color = CairoEx.ColorWhite,
                            Image = null,
                            Pattern = null
                        }
                    },
                };
                ComboBox["Normal"] = Style.Make(normalModifiers);


                StyleModifier[] hoverModifiers =
                {
                    new StyleModifier
                    {
                        Name = "BackgroundStyle",
                        Value = new BackgroundStyle
                        {
                            Color = CairoEx.ColorArgb(255,46,167,224),
                            Image = null,
                            Pattern = null
                        }
                    }
                };
                ComboBox["Hover"] = Style.Make(ComboBox["Normal"], hoverModifiers);

                StyleModifier[] activeModifiers =
                {
                    new StyleModifier
                    {
                        Name = "BackgroundStyle",
                        Value = new BackgroundStyle
                        {
                            Color = CairoEx.ColorArgb(255,3,110,184),
                            Image = null,
                            Pattern = null
                        }
                    }
                };
                ComboBox["Active"] = Style.Make(ComboBox["Normal"], activeModifiers);

                ComboBox["Item"] = Style.Make(ComboBox["Normal"]);
                ComboBox["Item:Hover"] = Style.Make(ComboBox["Hover"]);
                ComboBox["Item:Active"] = Style.Make(ComboBox["Active"]);
            }
            #endregion

            #region Image
            {
                StyleModifier[] normalModifiers =
                {
                    new StyleModifier {Name = "BorderTop", Value = new Length(1, Unit.Pixel)},
                    new StyleModifier {Name = "BorderRight", Value = new Length(1, Unit.Pixel)},
                    new StyleModifier {Name = "BorderBottom", Value = new Length(1, Unit.Pixel)},
                    new StyleModifier {Name = "BorderLeft", Value = new Length(1, Unit.Pixel)},

                    new StyleModifier {Name = "BorderTopColor", Value = CairoEx.ColorBlack},
                    new StyleModifier {Name = "BorderRightColor", Value = CairoEx.ColorBlack},
                    new StyleModifier {Name = "BorderBottomColor", Value = CairoEx.ColorBlack},
                    new StyleModifier {Name = "BorderLeftColor", Value = CairoEx.ColorBlack},
                };
                Image["Normal"] = Style.Make(normalModifiers);
            }


            #endregion

            #region Radio
            {
                Radio["Normal"] = Style.Make();
                Radio["Hover"] = Style.Make();
                Radio["Active"] = Style.Make();
            }
            #endregion
        }


    }
}
