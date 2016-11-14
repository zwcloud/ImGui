using System;

namespace ImGui
{
    public struct Color
    {
        public Color(double r, double g, double b) : this(r, g, b, 1.0)
        {
        }

        public Color(double r, double g, double b, double a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        private double r, g, b, a;

        public double R
        {
            get { return r; }
            set { r = value; }
        }

        public double G
        {
            get { return g; }
            set { g = value; }
        }

        public double B
        {
            get { return b; }
            set { b = value; }
        }

        public double A
        {
            get { return a; }
            set { a = value; }
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

        #endregion


        public static readonly Color Clear = ColorArgb(0, 0, 0, 0);
        public static readonly Color Black = ColorRgb(0, 0, 0);
        public static readonly Color White = ColorRgb(255, 255, 255);
        public static readonly Color Red = ColorRgb(255, 0, 0);
        public static readonly Color Green = ColorRgb(0, 255, 0);
        public static readonly Color Blue = ColorRgb(0, 0, 255);
        public static readonly Color Metal = ColorRgb(192, 192, 192);
        public static readonly Color LightBlue = ColorRgb(46, 167, 224);
        public static readonly Color DarkBlue = ColorRgb(3, 110, 184);
        public static readonly Color Pink = ColorRgb(255, 192, 203);
        public static readonly Color Orange = ColorRgb(255, 165, 0);

        public static Color ColorRgb(byte r, byte g, byte b)
        {
            return new Color(r / 255.0, g / 255.0, b / 255.0, 1.0);
        }

        public static Color ColorArgb(byte a, byte r, byte g, byte b)
        {
            return new Color(r / 255.0, g / 255.0, b / 255.0, a / 255.0);
        }

        public static Color ColorArgb(uint colorValue)
        {
            return ColorArgb(
                (byte)((colorValue >> 24) & 0xff),
                (byte)((colorValue >> 16) & 0xff),
                (byte)((colorValue >> 8) & 0xff),
                (byte)(colorValue & 0xff)
                );
        }

        public static Color ColorFromHSV(double hue, double saturation, double value)
        {
            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            byte v = Convert.ToByte(value);
            byte p = Convert.ToByte(value * (1 - saturation));
            byte q = Convert.ToByte(value * (1 - f * saturation));
            byte t = Convert.ToByte(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return CairoEx.ColorArgb(255, v, t, p);
            if (hi == 1)
                return CairoEx.ColorArgb(255, q, v, p);
            if (hi == 2)
                return CairoEx.ColorArgb(255, p, v, t);
            if (hi == 3)
                return CairoEx.ColorArgb(255, p, q, v);
            if (hi == 4)
                return CairoEx.ColorArgb(255, t, p, v);

            return CairoEx.ColorArgb(255, v, p, q);
        }

    }
}
