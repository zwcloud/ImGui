using System;

namespace ImGui
{
    [System.Diagnostics.DebuggerDisplay("({X}, {Y})")]
    [System.Diagnostics.DebuggerStepThrough]
    [Serializable]
    public struct Point
    {
        #region Constructors

        /// <summary>
        /// Constructor which accepts the X and Y values
        /// </summary>
        /// <param name="x">The value for the X coordinate of the new Point</param>
        /// <param name="y">The value for the Y coordinate of the new Point</param>
        public Point(double x, double y)
        {
            _x = (float)x;
            _y = (float)y;
        }

        public Point(float x, float y)
        {
            _x = x;
            _y = y;
        }

        #endregion Constructors

        /// <summary>
        /// Offset - update the location by adding offsetX to X and offsetY to Y
        /// </summary>
        /// <param name="offsetX"> The offset in the x dimension </param>
        /// <param name="offsetY"> The offset in the y dimension </param>
        public void Offset(double offsetX, double offsetY)
        {
            _x += (float)offsetX;
            _y += (float)offsetY;
        }

        /// <summary>
        /// Operator Point + Vector
        /// </summary>
        /// <returns>
        /// Point - The result of the addition
        /// </returns>
        /// <param name="point"> The Point to be added to the Vector </param>
        /// <param name="vector"> The Vectr to be added to the Point </param>
        public static Point operator +(Point point, Vector vector)
        {
            return new Point(point._x + vector._x, point._y + vector._y);
        }

        /// <summary>
        /// Add: Point + Vector
        /// </summary>
        /// <returns>
        /// Point - The result of the addition
        /// </returns>
        /// <param name="point"> The Point to be added to the Vector </param>
        /// <param name="vector"> The Vector to be added to the Point </param>
        public static Point Add(Point point, Vector vector)
        {
            return new Point(point._x + vector._x, point._y + vector._y);
        }

        /// <summary>
        /// Operator Point - Vector
        /// </summary>
        /// <returns>
        /// Point - The result of the subtraction
        /// </returns>
        /// <param name="point"> The Point from which the Vector is subtracted </param>
        /// <param name="vector"> The Vector which is subtracted from the Point </param>
        public static Point operator -(Point point, Vector vector)
        {
            return new Point(point._x - vector._x, point._y - vector._y);
        }

        /// <summary>
        /// Subtract: Point - Vector
        /// </summary>
        /// <returns>
        /// Point - The result of the subtraction
        /// </returns>
        /// <param name="point"> The Point from which the Vector is subtracted </param>
        /// <param name="vector"> The Vector which is subtracted from the Point </param>
        public static Point Subtract(Point point, Vector vector)
        {
            return new Point(point._x - vector._x, point._y - vector._y);
        }

        /// <summary>
        /// Operator Point - Point
        /// </summary>
        /// <returns>
        /// Vector - The result of the subtraction
        /// </returns>
        /// <param name="point1"> The Point from which point2 is subtracted </param>
        /// <param name="point2"> The Point subtracted from point1 </param>
        public static Vector operator -(Point point1, Point point2)
        {
            return new Vector(point1._x - point2._x, point1._y - point2._y);
        }

        /// <summary>
        /// Subtract: Point - Point
        /// </summary>
        /// <returns>
        /// Vector - The result of the subtraction
        /// </returns>
        /// <param name="point1"> The Point from which point2 is subtracted </param>
        /// <param name="point2"> The Point subtracted from point1 </param>
        public static Vector Subtract(Point point1, Point point2)
        {
            return new Vector(point1._x - point2._x, point1._y - point2._y);
        }

        /// <summary>
        /// Operator Point * double
        /// </summary>
        /// <returns>
        /// Point - The result of scaling the point with specific scalar
        /// </returns>
        /// <param name="point"> The Point </param>
        /// <param name="scalar"> The scale scalar  </param>
        public static Point operator *(Point point, double scalar)
        {
            return new Point(point._x* scalar, point._y * scalar);
        }

        public static Point Multiply(Point point, double scalar)
        {
            return new Point(point._x * scalar, point._y * scalar);
        }

#if Matrix
        /// <summary>
        /// Operator Point * Matrix
        /// </summary>
        public static Point operator * (Point point, Matrix matrix)
        {
            return matrix.Transform(point);
        }

        /// <summary>
        /// Multiply: Point * Matrix
        /// </summary>
        public static Point Multiply(Point point, Matrix matrix)
        {
            return matrix.Transform(point);
        }
#endif

        /// <summary>
        /// Explicit conversion to Size.  Note that since Size cannot contain negative values,
        /// the resulting size will contains the absolute values of X and Y
        /// </summary>
        /// <returns>
        /// Size - A Size equal to this Point
        /// </returns>
        /// <param name="point"> Point - the Point to convert to a Size </param>
        public static explicit operator Size(Point point)
        {
            return new Size(Math.Abs(point._x), Math.Abs(point._y));
        }

        /// <summary>
        /// Explicit conversion to Vector
        /// </summary>
        /// <returns>
        /// Vector - A Vector equal to this Point
        /// </returns>
        /// <param name="point"> Point - the Point to convert to a Vector </param>
        public static explicit operator Vector(Point point)
        {
            return new Vector(point._x, point._y);
        }

        /// <summary>
        /// Implicit conversion from ValueTuple(double, double)
        /// </summary>
        public static implicit operator Point((double, double) p)
        {
            return new Point(p.Item1, p.Item2);
        }

        /// <summary>
        /// Creates a string representation of this object based on the current culture.
        /// </summary>
        /// <returns>
        /// A string representation of this object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0:0.00}, {1:0.00}", X, Y);
        }

        /// <summary>
        /// Compares two Point instances for exact equality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which are logically equal may fail.
        /// Furthermore, using this equality operator, double.NaN is not equal to itself.
        /// </summary>
        /// <returns>
        /// bool - true if the two Point instances are exactly equal, false otherwise
        /// </returns>
        /// <param name='point1'>The first Point to compare</param>
        /// <param name='point2'>The second Point to compare</param>
        public static bool operator ==(Point point1, Point point2)
        {
            return point1.X == point2.X &&
                   point1.Y == point2.Y;
        }

        /// <summary>
        /// Compares two Point instances for exact inequality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which are logically equal may fail.
        /// Furthermore, using this equality operator, double.NaN is not equal to itself.
        /// </summary>
        /// <returns>
        /// bool - true if the two Point instances are exactly unequal, false otherwise
        /// </returns>
        /// <param name='point1'>The first Point to compare</param>
        /// <param name='point2'>The second Point to compare</param>
        public static bool operator !=(Point point1, Point point2)
        {
            return !(point1 == point2);
        }

        /// <summary>
        /// Compares two Point instances for object equality.  In this equality
        /// double.NaN is equal to itself, unlike in numeric equality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if the two Point instances are exactly equal, false otherwise
        /// </returns>
        /// <param name='point1'>The first Point to compare</param>
        /// <param name='point2'>The second Point to compare</param>
        public static bool Equals(Point point1, Point point2)
        {
            return point1.X.Equals(point2.X) &&
                   point1.Y.Equals(point2.Y);
        }

        /// <summary>
        /// Equals - compares this Point with the passed in object.  In this equality
        /// double.NaN is equal to itself, unlike in numeric equality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if the object is an instance of Point and if it's equal to "this".
        /// </returns>
        /// <param name='o'>The object to compare to "this"</param>
        public override bool Equals(object o)
        {
            if((null == o) || !(o is Point))
            {
                return false;
            }

            var value = (Point) o;
            return Equals(this, value);
        }

        /// <summary>
        /// Equals - compares this Point with the passed in object.  In this equality
        /// double.NaN is equal to itself, unlike in numeric equality.
        /// Note that double values can acquire error when operated upon, such that
        /// an exact comparison between two values which
        /// are logically equal may fail.
        /// </summary>
        /// <returns>
        /// bool - true if "value" is equal to "this".
        /// </returns>
        /// <param name='value'>The Point to compare to "this"</param>
        public bool Equals(Point value)
        {
            return Equals(this, value);
        }

        /// <summary>
        /// Returns the HashCode for this Point
        /// </summary>
        /// <returns>
        /// int - the HashCode for this Point
        /// </returns>
        public override int GetHashCode()
        {
            // Perform field-by-field XOR of HashCodes
            return X.GetHashCode() ^
                   Y.GetHashCode();
        }

        /// <summary>
        /// Parse - returns an instance converted from the provided string using
        /// the culture "en-US"
        /// <param name="source"> string with Point data </param>
        /// </summary>
        public static Point Parse(string source)
        {
            throw new NotImplementedException();
        }

        public static bool AlmostEqual(Point a, Point b)
        {
            return MathEx.AmostEqual(a._x, b._x) && MathEx.AmostEqual(a._y, b._y);
        }

        /// <summary>
        ///     X - double.  Default value is 0.
        /// </summary>
        public double X
        {
            get { return _x; }

            set { _x = (float)value; }
        }

        /// <summary>
        ///     Y - double.  Default value is 0.
        /// </summary>
        public double Y
        {
            get { return _y; }

            set { _y = (float)value; }
        }

        public float x
        {
            get { return _x; }
            set { _x = value; }
        }

        public float y
        {
            get { return _y; }
            set { _y = value; }
        }

        internal float _x;
        internal float _y;

        public static readonly Point Zero = new Point(0, 0);
        public static readonly Point One = new Point(1, 1);
    }
}