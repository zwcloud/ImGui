using System;

namespace ImGui
{
    [System.Diagnostics.DebuggerDisplay("({R}, {G}, {B}, {A})")]
    [System.Diagnostics.DebuggerStepThrough]
    public struct Color
    {
        public Color(double r, double g, double b) : this(r, g, b, 1.0)
        {
        }

        public Color(double r, double g, double b, double a)
        {
            this.r = (float)r;
            this.g = (float)g;
            this.b = (float)b;
            this.a = (float)a;
        }

        private float r, g, b, a;

        public double R
        {
            get { return r; }
            set { r = (float)value; }
        }

        public double G
        {
            get { return g; }
            set { g = (float)value; }
        }

        public double B
        {
            get { return b; }
            set { b = (float)value; }
        }

        public double A
        {
            get { return a; }
            set { a = (float)value; }
        }

        public static bool operator ==(Color a, Color b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Color a, Color b)
        {
            return !(a == b);
        }

        #region Overrides of ValueType

        public override bool Equals(object obj)
        {
            Color other = (Color) obj;
            return System.Math.Abs(this.r - other.r) < double.Epsilon
                   && System.Math.Abs(this.g - other.g) < double.Epsilon
                   && System.Math.Abs(this.b - other.b) < double.Epsilon
                   && System.Math.Abs(this.a - other.a) < double.Epsilon;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return $"R:{(int)(this.r*255)} G:{(int)(this.g*255)} B:{(int)(this.b*255)} A:{(int)(this.a*255)}";
        }
        #endregion

        #region Pre-defined colors

        public static readonly Color Clear = Argb(0, 0, 0, 0);

        #region Pink colors
        public static readonly Color Pink = Rgb(255, 192, 203);
        public static readonly Color LightPink = Rgb(255, 182, 193);
        public static readonly Color HotPink = Rgb(255, 105, 180);
        public static readonly Color DeepPink = Rgb(255, 20, 147);
        public static readonly Color PaleVioletRed = Rgb(219, 112, 147);
        public static readonly Color MediumVioletRed = Rgb(199, 21, 133);
        #endregion

        #region Red colors
        public static readonly Color LightSalmon = Rgb(255, 160, 122);
        public static readonly Color Salmon = Rgb(250, 128, 114);
        public static readonly Color DarkSalmon = Rgb(233, 150, 122);
        public static readonly Color LightCoral = Rgb(240, 128, 128);
        public static readonly Color IndianRed = Rgb(205, 92, 92);
        public static readonly Color Crimson = Rgb(220, 20, 60);
        public static readonly Color FireBrick = Rgb(178, 34, 34);
        public static readonly Color DarkRed = Rgb(139, 0, 0);
        public static readonly Color Red = Rgb(255, 0, 0);
        #endregion

        #region Orange colors
        public static readonly Color OrangeRed = Rgb(255, 69, 0);
        public static readonly Color Tomato = Rgb(255, 99, 71);
        public static readonly Color Coral = Rgb(255, 127, 80);
        public static readonly Color DarkOrange = Rgb(255, 140, 0);
        public static readonly Color Orange = Rgb(255, 165, 0);
        #endregion

        #region Yellow colors
        public static readonly Color Yellow = Rgb(255, 255, 0);
        public static readonly Color LightYellow = Rgb(255, 255, 224);
        public static readonly Color LemonChiffon = Rgb(255, 250, 205);
        public static readonly Color LightGoldenrodYellow = Rgb(250, 250, 210);
        public static readonly Color PapayaWhip = Rgb(255, 239, 213);
        public static readonly Color Moccasin = Rgb(255, 228, 181);
        public static readonly Color PeachPuff = Rgb(255, 218, 185);
        public static readonly Color PaleGoldenrod = Rgb(238, 232, 170);
        public static readonly Color Khaki = Rgb(240, 230, 140);
        public static readonly Color DarkKhaki = Rgb(189, 183, 107);
        public static readonly Color Gold = Rgb(255, 215, 0);
        #endregion

        #region Brown colors
        public static readonly Color Cornsilk = Rgb(255, 248, 220);
        public static readonly Color BlanchedAlmond = Rgb(255, 235, 205);
        public static readonly Color Bisque = Rgb(255, 228, 196);
        public static readonly Color NavajoWhite = Rgb(255, 222, 173);
        public static readonly Color Wheat = Rgb(245, 222, 179);
        public static readonly Color BurlyWood = Rgb(222, 184, 135);
        public static readonly Color Tan = Rgb(210, 180, 140);
        public static readonly Color RosyBrown = Rgb(188, 143, 143);
        public static readonly Color SandyBrown = Rgb(244, 164, 96);
        public static readonly Color Goldenrod = Rgb(218, 165, 32);
        public static readonly Color DarkGoldenrod = Rgb(184, 134, 11);
        public static readonly Color Peru = Rgb(205, 133, 63);
        public static readonly Color Chocolate = Rgb(210, 105, 30);
        public static readonly Color SaddleBrown = Rgb(139, 69, 19);
        public static readonly Color Sienna = Rgb(160, 82, 45);
        public static readonly Color Brown = Rgb(165, 42, 42);
        public static readonly Color Maroon = Rgb(128, 0, 0);
        #endregion

        #region Green colors
        public static readonly Color DarkOliveGreen = Rgb(85, 107, 47);
        public static readonly Color Olive = Rgb(128, 128, 0);
        public static readonly Color OliveDrab = Rgb(107, 142, 35);
        public static readonly Color YellowGreen = Rgb(154, 205, 50);
        public static readonly Color LimeGreen = Rgb(50, 205, 50);
        public static readonly Color Lime = Rgb(0, 255, 0);
        public static readonly Color LawnGreen = Rgb(124, 252, 0);
        public static readonly Color Chartreuse = Rgb(127, 255, 0);
        public static readonly Color GreenYellow = Rgb(173, 255, 47);
        public static readonly Color SpringGreen = Rgb(0, 255, 127);
        public static readonly Color MediumSpringGreen = Rgb(0, 250, 154);
        public static readonly Color LightGreen = Rgb(144, 238, 144);
        public static readonly Color PaleGreen = Rgb(152, 251, 152);
        public static readonly Color DarkSeaGreen = Rgb(143, 188, 143);
        public static readonly Color MediumAquamarine = Rgb(102, 205, 170);
        public static readonly Color MediumSeaGreen = Rgb(60, 179, 113);
        public static readonly Color SeaGreen = Rgb(46, 139, 87);
        public static readonly Color ForestGreen = Rgb(34, 139, 34);
        public static readonly Color Green = Rgb(0, 128, 0);
        public static readonly Color DarkGreen = Rgb(0, 100, 0);
        #endregion

        #region Cyan colors
        public static readonly Color Aqua = Rgb(0, 255, 255);
        public static readonly Color Cyan = Rgb(0, 255, 255);
        public static readonly Color LightCyan = Rgb(224, 255, 255);
        public static readonly Color PaleTurquoise = Rgb(175, 238, 238);
        public static readonly Color Aquamarine = Rgb(127, 255, 212);
        public static readonly Color Turquoise = Rgb(64, 224, 208);
        public static readonly Color MediumTurquoise = Rgb(72, 209, 204);
        public static readonly Color DarkTurquoise = Rgb(0, 206, 209);
        public static readonly Color LightSeaGreen = Rgb(32, 178, 170);
        public static readonly Color CadetBlue = Rgb(95, 158, 160);
        public static readonly Color DarkCyan = Rgb(0, 139, 139);
        public static readonly Color Teal = Rgb(0, 128, 128);
        #endregion

        #region Blue colors
        public static readonly Color LightSteelBlue = Rgb(176, 196, 222);
        public static readonly Color PowderBlue = Rgb(176, 224, 230);
        public static readonly Color LightBlue = Rgb(173, 216, 230);
        public static readonly Color SkyBlue = Rgb(135, 206, 235);
        public static readonly Color LightSkyBlue = Rgb(135, 206, 250);
        public static readonly Color DeepSkyBlue = Rgb(0, 191, 255);
        public static readonly Color DodgerBlue = Rgb(30, 144, 255);
        public static readonly Color CornflowerBlue = Rgb(100, 149, 237);
        public static readonly Color SteelBlue = Rgb(70, 130, 180);
        public static readonly Color RoyalBlue = Rgb(65, 105, 225);
        public static readonly Color Blue = Rgb(0, 0, 255);
        public static readonly Color MediumBlue = Rgb(0, 0, 205);
        public static readonly Color DarkBlue = Rgb(0, 0, 139);
        public static readonly Color Navy = Rgb(0, 0, 128);
        public static readonly Color MidnightBlue = Rgb(25, 25, 112);
        #endregion

        #region Purple, violet, and magenta colors
        public static readonly Color Lavender = Rgb(230, 230, 250);
        public static readonly Color Thistle = Rgb(216, 191, 216);
        public static readonly Color Plum = Rgb(221, 160, 221);
        public static readonly Color Violet = Rgb(238, 130, 238);
        public static readonly Color Orchid = Rgb(218, 112, 214);
        public static readonly Color Fuchsia = Rgb(255, 0, 255);
        public static readonly Color Magenta = Rgb(255, 0, 255);
        public static readonly Color MediumOrchid = Rgb(186, 85, 211);
        public static readonly Color MediumPurple = Rgb(147, 112, 219);
        public static readonly Color BlueViolet = Rgb(138, 43, 226);
        public static readonly Color DarkViolet = Rgb(148, 0, 211);
        public static readonly Color DarkOrchid = Rgb(153, 50, 204);
        public static readonly Color DarkMagenta = Rgb(139, 0, 139);
        public static readonly Color Purple = Rgb(128, 0, 128);
        public static readonly Color Indigo = Rgb(75, 0, 130);
        public static readonly Color DarkSlateBlue = Rgb(72, 61, 139);
        public static readonly Color SlateBlue = Rgb(106, 90, 205);
        public static readonly Color MediumSlateBlue = Rgb(123, 104, 238);
        #endregion

        #region White colors
        public static readonly Color White = Rgb(255, 255, 255);
        public static readonly Color Snow = Rgb(255, 250, 250);
        public static readonly Color Honeydew = Rgb(240, 255, 240);
        public static readonly Color MintCream = Rgb(245, 255, 250);
        public static readonly Color Azure = Rgb(240, 255, 255);
        public static readonly Color AliceBlue = Rgb(240, 248, 255);
        public static readonly Color GhostWhite = Rgb(248, 248, 255);
        public static readonly Color WhiteSmoke = Rgb(245, 245, 245);
        public static readonly Color Seashell = Rgb(255, 245, 238);
        public static readonly Color Beige = Rgb(245, 245, 220);
        public static readonly Color OldLace = Rgb(253, 245, 230);
        public static readonly Color FloralWhite = Rgb(255, 250, 240);
        public static readonly Color Ivory = Rgb(255, 255, 240);
        public static readonly Color AntiqueWhite = Rgb(250, 235, 215);
        public static readonly Color Linen = Rgb(250, 240, 230);
        public static readonly Color LavenderBlush = Rgb(255, 240, 245);
        public static readonly Color MistyRose = Rgb(255, 228, 225);
        #endregion

        #region Gray and black colors
        public static readonly Color Gainsboro = Rgb(220, 220, 220);
        public static readonly Color LightGray = Rgb(211, 211, 211);
        public static readonly Color LightGrey = LightGray;
        public static readonly Color Silver = Rgb(192, 192, 192);
        public static readonly Color DarkGray = Rgb(169, 169, 169);
        public static readonly Color DarkGrey = DarkGray;
        public static readonly Color Gray = Rgb(128, 128, 128);
        public static readonly Color Grey = Gray;
        public static readonly Color DimGray = Rgb(105, 105, 105);
        public static readonly Color DimGrey = DimGray;
        public static readonly Color LightSlateGray = Rgb(119, 136, 153);
        public static readonly Color LightSlateGrey = LightSlateGray;
        public static readonly Color SlateGray = Rgb(112, 128, 144);
        public static readonly Color SlateGrey = SlateGray;
        public static readonly Color DarkSlateGray = Rgb(47, 79, 79);
        public static readonly Color DarkSlateGrey = DarkSlateGray;
        public static readonly Color Black = Rgb(0, 0, 0);
        #endregion


        #region "System" colors
        public static readonly Color TextDisabled = new Color(0.60f, 0.60f, 0.60f, 1.00f);
        public static readonly Color FrameBg =        new Color(0.43f, 0.43f, 0.43f, 0.39f);
        public static readonly Color FrameBgHovered = new Color(0.47f, 0.47f, 0.69f, 0.40f);
        public static readonly Color FrameBgActive =  new Color(0.42f, 0.41f, 0.64f, 0.69f);
        public static readonly Color CheckMark = new Color(0.90f, 0.90f, 0.90f, 0.50f);
        #endregion

        #endregion

        public static Color Rgb(byte r, byte g, byte b)
        {
            return new Color(r / 255.0, g / 255.0, b / 255.0, 1.0);
        }

        public static Color Rgb(byte all)
        {
            var grey = all / 255.0;
            return new Color(grey, grey, grey, 1.0);
        }
        
        public static Color Rgb(uint colorValue)
        {
            return Rgb(
                (byte)((colorValue >> 16) & 0xff),
                (byte)((colorValue >> 8) & 0xff),
                (byte)(colorValue & 0xff)
            );
        }

        public static Color Argb(byte a, byte r, byte g, byte b)
        {
            return new Color(r / 255.0, g / 255.0, b / 255.0, a / 255.0);
        }

        public static Color Argb(uint colorValue)
        {
            return Argb(
                (byte)((colorValue >> 24) & 0xff),
                (byte)((colorValue >> 16) & 0xff),
                (byte)((colorValue >> 8) & 0xff),
                (byte)(colorValue & 0xff)
                );
        }
        
        /// <summary>
        /// Create a Color from HSV color
        /// </summary>
        /// <param name="hue">hue ∈[0, 360]</param>
        /// <param name="saturation">saturation ∈[0, 1]</param>
        /// <param name="value">value ∈[0, 1]</param>
        /// <returns>RGB color</returns>
        public static Color HSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            byte v = Convert.ToByte(value);
            byte p = Convert.ToByte(value * (1 - saturation));
            byte q = Convert.ToByte(value * (1 - f * saturation));
            byte t = Convert.ToByte(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Argb(255, v, t, p);
            if (hi == 1)
                return Argb(255, q, v, p);
            if (hi == 2)
                return Argb(255, p, v, t);
            if (hi == 3)
                return Argb(255, p, q, v);
            if (hi == 4)
                return Color.Argb(255, t, p, v);

            return Argb(255, v, p, q);
        }

        public void Darken(double amount)
        {
            var red = this.R;
            var green = this.G;
            var blue = this.B;

            if (amount < 0)
            {
                amount = 1 + amount;
                red *= amount;
                green *= amount;
                blue *= amount;
            }
            else
            {
                red = (255 - red) * amount + red;
                green = (255 - green) * amount + green;
                blue = (255 - blue) * amount + blue;
            }

            this.R = red;
            this.G = green;
            this.B = blue;
        }

        public static Color Darken(Color color, double amount)
        {
            var red = color.R;
            var green = color.G;
            var blue = color.B;

            if (amount < 0)
            {
                amount = 1 + amount;
                red *= amount;
                green *= amount;
                blue *= amount;
            }
            else
            {
                red = (255 - red) * amount + red;
                green = (255 - green) * amount + green;
                blue = (255 - blue) * amount + blue;
            }

            return new Color(red, green, blue, color.a);
        }

        public static Color Lerp(Color a, Color b, double k)
        {
            k = MathEx.Clamp01(k);
            return new Color(a.r + (b.r - a.r) * k, a.g + (b.g - a.g) * k, a.b + (b.b - a.b) * k, a.a + (b.a - a.a) * k);
        }
    }
}
