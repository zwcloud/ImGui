using System.Collections.Generic;
using Cairo;

namespace ImGui
{
    public sealed class Skin
    {
        public Dictionary<string, Style> Button { get; private set; }
        public Dictionary<string, Style> Label { get; private set; }
        public Dictionary<string, Style> Toggle { get; private set; }
        public Dictionary<string, Style> ComboBox { get; private set; }
        public Dictionary<string, Style> Image { get; private set; }
        public Dictionary<string, Style> Radio { get; set; }
        public Dictionary<string, Style> TextBox { get; set; }
        public Dictionary<string, Style> Slider { get; set; }
        public Dictionary<string, Style> PolygonButton { get; set; }

        /*Stateless styles*/
        public Style ToolTip { get; set; }
        public Style Box { get; set; }
        public Style Space { get; set; }

        public static readonly Skin current;

        static Skin()
        {
            current = new Skin();
        }

        public Skin()
        {
            Button = new Dictionary<string, Style>(3);
            Label = new Dictionary<string, Style>(3);
            Toggle = new Dictionary<string, Style>(3);
            ComboBox = new Dictionary<string, Style>(3);
            Image = new Dictionary<string, Style>(1);
            Radio = new Dictionary<string, Style>(2);
            TextBox = new Dictionary<string, Style>(3);
            Slider = new Dictionary<string, Style>(3);
            PolygonButton = new Dictionary<string, Style>(3);

            #region Label
            {
                Label["Normal"] = Style.Make();
                Label["Hover"] = Style.Make();
                Label["Active"] = Style.Make();
            }
            #endregion

            #region Button
            {
                var bgColor = CairoEx.ColorRgb(204, 204, 204);
                StyleModifier[] normalModifiers =
                {
                    new StyleModifier{Name = "BorderTop", Value = 2},
                    new StyleModifier{Name = "BorderRight", Value = 2},
                    new StyleModifier{Name = "BorderBottom", Value = 2},
                    new StyleModifier{Name = "BorderLeft", Value = 2},

                    new StyleModifier{Name = "BorderTopColor", Value = bgColor},
                    new StyleModifier{Name = "BorderRightColor", Value = bgColor},
                    new StyleModifier{Name = "BorderBottomColor", Value = bgColor},
                    new StyleModifier{Name = "BorderLeftColor", Value = bgColor},
                    
                    new StyleModifier{Name = "PaddingTop", Value = 2},
                    new StyleModifier{Name = "PaddingRight", Value = 2},
                    new StyleModifier{Name = "PaddingBottom", Value = 2},
                    new StyleModifier{Name = "PaddingLeft", Value = 2},

                    new StyleModifier
                    {
                        Name = "TextStyle",
                        Value = new TextStyle
                        {
                            TextAlignment = TextAlignment.Center,
                            LineSpacing = 0,
                            TabSize = 4
                        }
                    },
                    
                    new StyleModifier
                    {
                        Name = "BackgroundStyle",
                        Value = new BackgroundStyle
                        {
                            Color = bgColor,
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

                var hoverBorderColor = CairoEx.ColorRgb(122, 122, 122);
                StyleModifier[] hoverModifiers =
                {
                    new StyleModifier{Name = "BorderTopColor", Value = hoverBorderColor},
                    new StyleModifier{Name = "BorderRightColor", Value = hoverBorderColor},
                    new StyleModifier{Name = "BorderBottomColor", Value = hoverBorderColor},
                    new StyleModifier{Name = "BorderLeftColor", Value = hoverBorderColor},
                };
                Button["Hover"] = Style.Make(Button["Normal"], hoverModifiers);

                var activeBgColor = CairoEx.ColorRgb(153, 153, 153);
                StyleModifier[] activeModifiers =
                {
                    new StyleModifier{Name = "BorderBottomColor",   Value = activeBgColor},
                    new StyleModifier{Name = "BorderLeftColor",     Value = activeBgColor},
                    new StyleModifier{Name = "BorderTopColor",      Value = activeBgColor},
                    new StyleModifier{Name = "BorderRightColor",    Value = activeBgColor},

                    new StyleModifier
                    {
                        Name = "Font",
                        Value = new Font
                        {
                            FontFamily = "Consolas",
                            FontStyle = FontStyle.Normal,
                            FontWeight = FontWeight.Bold,
                            FontStretch = FontStretch.Normal,
                            Size = 12,
                            Color = CairoEx.ColorBlack
                        }
                    },
                    new StyleModifier
                    {
                        Name = "BackgroundStyle",
                        Value = new BackgroundStyle
                        {
                            Color = activeBgColor,
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
                    },
                    new StyleModifier
                    {
                        Name = "TextStyle",
                        Value = new TextStyle
                        {
                            TextAlignment = TextAlignment.Center
                        }
                    }
                };
                Toggle["Normal"] = Style.Make(normalModifiers);
                Toggle["Normal"].ExtraStyles["TickColor"] = CairoEx.ColorWhite;
                Toggle["Normal"].ExtraStyles["FillColor"] = CairoEx.ColorDarkBlue;
                Toggle["Hover"] = Style.Make(Toggle["Normal"]);
                Toggle["Active"] = Style.Make(Toggle["Normal"]);
            }
            #endregion

            #region ComboBox
            {
                StyleModifier[] normalModifiers =
                {
                    new StyleModifier{Name = "BorderTop", Value = 2},
                    new StyleModifier{Name = "BorderRight", Value = 2},
                    new StyleModifier{Name = "BorderBottom", Value = 2},
                    new StyleModifier{Name = "BorderLeft", Value = 2},

                    new StyleModifier{Name = "BorderTopColor", Value = CairoEx.ColorRgb(225,225,225)},
                    new StyleModifier{Name = "BorderRightColor", Value = CairoEx.ColorRgb(225,225,225)},
                    new StyleModifier{Name = "BorderBottomColor", Value = CairoEx.ColorRgb(225,225,225)},
                    new StyleModifier{Name = "BorderLeftColor", Value = CairoEx.ColorRgb(225,225,225)},
                    
                    new StyleModifier{Name = "PaddingTop", Value = 2},
                    new StyleModifier{Name = "PaddingRight", Value = 2},
                    new StyleModifier{Name = "PaddingBottom", Value = 2},
                    new StyleModifier{Name = "PaddingLeft", Value = 2},

                    new StyleModifier
                    {
                        Name = "TextStyle",
                        Value = new TextStyle
                        {
                            TextAlignment = TextAlignment.Center,
                            LineSpacing = 0,
                            TabSize = 4
                        }
                    },
                    
                    new StyleModifier
                    {
                        Name = "BackgroundStyle",
                        Value = new BackgroundStyle
                        {
                            Color = CairoEx.ColorRgb(193,193,193),
                            Image = null,
                            Pattern = null
                        }
                    },

                    new StyleModifier
                    {
                        Name = "LineColor",
                        Value = CairoEx.ColorRgb(225,225,225)
                    }
                };
                ComboBox["Normal"] = Style.Make(normalModifiers);


                StyleModifier[] hoverModifiers =
                {
                    new StyleModifier{Name = "BorderTopColor", Value = CairoEx.ColorRgb(115,115,115)},
                    new StyleModifier{Name = "BorderRightColor", Value = CairoEx.ColorRgb(115,115,115)},
                    new StyleModifier{Name = "BorderBottomColor", Value = CairoEx.ColorRgb(115,115,115)},
                    new StyleModifier{Name = "BorderLeftColor", Value = CairoEx.ColorRgb(115,115,115)},

                    new StyleModifier
                    {
                        Name = "LineColor",
                        Value = CairoEx.ColorRgb(115,115,115)
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
                            Color = CairoEx.ColorLightBlue,
                            Image = null,
                            Pattern = null
                        }
                    },
                };
                ComboBox["Active"] = Style.Make(ComboBox["Normal"], activeModifiers);
            }
            #endregion

            #region Image
            {
#if ImageBorder
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
#else
                Image["Normal"] = Style.Make();
#endif
            }


            #endregion

            #region Radio
            {
                Radio["Normal"] = Style.Make();
                //TODO build ExtraStyles into modifier
                Radio["Normal"].ExtraStyles["CircleColor.Selected"] = CairoEx.ColorDarkBlue;

                Radio["Hover"] = Style.Make();
                Radio["Active"] = Style.Make();

            }
            #endregion

            #region TextBox
            {
                StyleModifier[] normalModifiers =
                {
                    new StyleModifier{Name = "BorderTop", Value = 1},
                    new StyleModifier{Name = "BorderRight", Value = 1},
                    new StyleModifier{Name = "BorderBottom", Value = 1},
                    new StyleModifier{Name = "BorderLeft", Value = 1},

                    new StyleModifier{Name = "BorderTopColor", Value = CairoEx.ColorBlack},
                    new StyleModifier{Name = "BorderRightColor", Value = CairoEx.ColorBlack},
                    new StyleModifier{Name = "BorderBottomColor", Value = CairoEx.ColorBlack},
                    new StyleModifier{Name = "BorderLeftColor", Value = CairoEx.ColorBlack},
                    
                    new StyleModifier{Name="PaddingTop", Value = 2},
                    new StyleModifier{Name="PaddingRight", Value = 2},
                    new StyleModifier{Name="PaddingBottom", Value = 2},
                    new StyleModifier{Name="PaddingLeft", Value = 2},

                    new StyleModifier
                    {
                        Name = "TextStyle",
                        Value = new TextStyle
                        {
                            TextAlignment = TextAlignment.Leading,
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
                TextBox["Normal"] = Style.Make(normalModifiers);

                StyleModifier[] hoverModifiers =
                {
                    new StyleModifier{Name = "BorderTopColor", Value = CairoEx.ColorLightBlue},
                    new StyleModifier{Name = "BorderRightColor", Value = CairoEx.ColorLightBlue},
                    new StyleModifier{Name = "BorderBottomColor", Value = CairoEx.ColorLightBlue},
                    new StyleModifier{Name = "BorderLeftColor", Value = CairoEx.ColorLightBlue},
                };
                TextBox["Hover"] = Style.Make(TextBox["Normal"], hoverModifiers);

                StyleModifier[] activeModifiers =
                {
                    new StyleModifier{Name = "BorderTopColor", Value = CairoEx.ColorDarkBlue},
                    new StyleModifier{Name = "BorderRightColor", Value = CairoEx.ColorDarkBlue},
                    new StyleModifier{Name = "BorderBottomColor", Value = CairoEx.ColorDarkBlue},
                    new StyleModifier{Name = "BorderLeftColor", Value = CairoEx.ColorDarkBlue},

                    new StyleModifier{Name = "Cursor", Value = Cursor.Text}
                };
                TextBox["Active"] = Style.Make(TextBox["Normal"], activeModifiers);
            }
            #endregion

            #region Slider
            {
                Slider["Normal"] = Style.Make();

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
                Slider["Hover"] = Style.Make(hoverModifiers);

                StyleModifier[] activeModifiers =
                {
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
                Slider["Active"] = Style.Make(activeModifiers);

                Slider["Line:Normal"] = Style.Make(Slider["Normal"]);
                Slider["Normal"].ExtraStyles["Line:Unused"] = CairoEx.ColorBlack;
                Slider["Normal"].ExtraStyles["Line:Used"] = CairoEx.ColorDarkBlue;
            }
            #endregion

            #region PolygonButton
            {
                PolygonButton["Normal"] = Style.Make();

                StyleModifier[] hoverModifiers =
                {
                    new StyleModifier
                    {
                        Name = "LineColor",
                        Value = new Color(0,0,1)
                    },
                    new StyleModifier
                    {
                        Name = "FillColor",
                        Value = new Color(0,0,1)
                    }
                };
                PolygonButton["Hover"] = Style.Make(hoverModifiers);

                StyleModifier[] activeModifiers =
                {
                    new StyleModifier
                    {
                        Name = "LineColor",
                        Value = new Color(1,0,0)
                    },
                    new StyleModifier
                    {
                        Name = "FillColor",
                        Value = new Color(1,0,0)
                    }
                };
                PolygonButton["Active"] = Style.Make(activeModifiers);
            }
            #endregion

            #region ToolTip
            {
                ToolTip = Style.Make();
                ToolTip.ExtraStyles.Add("FixedSize", new Size(100, 40));
            }
            #endregion

            #region Box
            {
                var borderColor = CairoEx.ColorRgb(24, 131, 215);
                var bgColor = CairoEx.ColorRgb(242, 242, 242);
                StyleModifier[] normalModifiers =
                {
                    new StyleModifier{Name = "BorderTop", Value = 2},
                    new StyleModifier{Name = "BorderRight", Value = 2},
                    new StyleModifier{Name = "BorderBottom", Value = 2},
                    new StyleModifier{Name = "BorderLeft", Value = 2},

                    new StyleModifier{Name = "BorderTopColor", Value = borderColor},
                    new StyleModifier{Name = "BorderRightColor", Value = borderColor},
                    new StyleModifier{Name = "BorderBottomColor", Value = borderColor},
                    new StyleModifier{Name = "BorderLeftColor", Value = borderColor},

                    new StyleModifier{Name = "PaddingTop", Value = 0},
                    new StyleModifier{Name = "PaddingRight", Value = 0},
                    new StyleModifier{Name = "PaddingBottom", Value = 0},
                    new StyleModifier{Name = "PaddingLeft", Value = 0},

                    new StyleModifier{Name = "CellingSpacingHorizontal", Value = 0},
                    new StyleModifier{Name = "CellingSpacingVertical", Value = 15},

                    new StyleModifier
                    {
                        Name = "BackgroundStyle",
                        Value = new BackgroundStyle
                        {
                            Color = bgColor,
                            Image = null,
                            Pattern = null
                        }
                    },
                };
                Box = Style.Make(normalModifiers);
            }
            #endregion

            #region Space
            {
                Space = Style.Make();
            }
            #endregion
             
        }

        //internal Style GetStyle(string str)
        //{
        //    Style result;
        //    if (this.StyleMap.TryGetValue(str, out result))
        //    {
        //        return result;
        //    }
        //    return null;
        //}
    }
}
