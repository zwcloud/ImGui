namespace ImGui
{
    struct PointF
    {
        public float x, y;

        public float X
        {
            get { return x; }
        }

        public float Y
        {
            get { return y; }
        }

        public PointF(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public PointF(double x, double y)
        {
            this.x = (float)x;
            this.y = (float)y;
        }

        public static PointF Zero = new PointF(0f, 0f);

        public static explicit operator PointF(Point p)
        {
            return new PointF((float)p.X, (float)p.Y);
        }

        public static implicit operator PointF((float, float) p)
        {
            return new PointF(p.Item1, p.Item2);
        }

        public static implicit operator PointF((double, double) p)
        {
            return new PointF(p.Item1, p.Item2);
        }

        public override string ToString()
        {
            return string.Format("({0:0.00},{1:0.00})", this.x, this.y);
        }
    }
}
