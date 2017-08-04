using ImGui.Common.Primitive;

namespace ImGui
{
    internal struct ColorF
    {
        public float r, g, b, a;

        public ColorF(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public static explicit operator ColorF(Color c)
        {
            return new ColorF((float)c.R, (float)c.G, (float)c.B, (float)c.A);
        }

        public override string ToString()
        {
            return string.Format("(r:{0}, g:{1}, b:{2}, a:{3})", this.r, this.g, this.b, this.a);
        }
    }
}
