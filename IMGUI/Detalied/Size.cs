using System;

namespace ImGui
{
    /// <summary>
    /// Size - A value type which defined a size in terms of non-negative width and height
    /// </summary>
    public partial struct Size
    {
        /// <summary>
        /// Constructor which sets the size's initial values.  Width and Height must be non-negative
        /// </summary>
        /// <param name="width"> double - The initial Width </param>
        /// <param name="height"> double - THe initial Height </param>
        public Size(double width, double height)
        {
            if (width < 0 || height < 0)
            {
                throw new System.ArgumentException(Error.Get(ErrorId.Size_WidthAndHeightCannotBeNegative));
            }

            _width = width;
            _height = height;
        }

        /// <summary>
        /// Empty - a static property which provides an Empty size.  Width and Height are 
        /// negative-infinity.  This is the only situation
        /// where size can be negative.
        /// </summary>
        public static Size Empty
        {
            get
            {
                return s_empty;
            }
        }

        /// <summary>
        /// Empty - a static property which provides an Zero size.  Width and Height are 0.
        /// </summary>
        public static Size Zero
        {
            get
            {
                return s_zero;
            }
        }

        /// <summary>
        /// IsEmpty - this returns true if this size is the Empty size.
        /// Note: If size is 0 this Size still contains a 0 or 1 dimensional set
        /// of points, so this method should not be used to check for 0 area.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return _width < 0;
            }
        }

        /// <summary>
        /// Width - Default is 0, must be non-negative
        /// </summary>
        public double Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (IsEmpty)
                {
                    throw new System.InvalidOperationException(Error.Get(ErrorId.Size_CannotModifyEmptySize));
                }
                    
                if (value < 0)
                {
                    throw new System.ArgumentException(Error.Get(ErrorId.Size_WidthCannotBeNegative));
                }

                _width = value;
            }
        }

        /// <summary>
        /// Height - Default is 0, must be non-negative.
        /// </summary>
        public double Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (IsEmpty)
                {
                    throw new System.InvalidOperationException(Error.Get(ErrorId.Size_CannotModifyEmptySize));
                }
                    
                if (value < 0)
                {
                    throw new System.ArgumentException(Error.Get(ErrorId.Size_HeightCannotBeNegative));
                }

                _height = value;
            }
        }

        /// <summary>
        /// Explicit conversion to Vector.
        /// </summary>
        /// <returns>
        /// Vector - A Vector equal to this Size
        /// </returns>
        /// <param name="size"> Size - the Size to convert to a Vector </param>
        public static explicit operator Vector(Size size)
        {
            return new Vector(size._width, size._height);
        }

        /// <summary>
        /// Explicit conversion to Point
        /// </summary>
        /// <returns>
        /// Point - A Point equal to this Size
        /// </returns>
        /// <param name="size"> Size - the Size to convert to a Point </param>
        public static explicit operator Point(Size size)
        {
            return new Point(size._width, size._height);
        }

        static private Size CreateEmptySize()
        {
            Size size = new Size();
            // We can't set these via the property setters because negatives widths
            // are rejected in those APIs.
            size._width = double.NegativeInfinity;
            size._height = double.NegativeInfinity;
            return size;
        }

        private readonly static Size s_empty = CreateEmptySize();
        private readonly static Size s_zero = new Size(0, 0);
    }
}
