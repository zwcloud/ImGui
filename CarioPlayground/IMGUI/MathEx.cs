using System.Runtime.CompilerServices;
namespace ImGui
{
    /// <summary>
    /// Extended class for System.Math
    /// </summary>
    public class MathEx
    {
        /// <summary>
        /// Clamps a value between a minimum float and maximum double value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static double Clamp(double value, double min, double max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }
            return value;
        }

        /// <summary>
        /// Clamps a value between a minimum float and maximum float value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }
            return value;
        }

        /// <summary>
        /// Clamps value between min and max and returns value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }
            return value;
        }

        /// <summary>
        ///   <para>Linearly interpolates between a and b by t.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static double Lerp(double a, double b, double t)
        {
            return a + (b - a) * MathEx.Clamp01(t);
        }

        /// <summary>
        ///   <para>Linearly interpolates between a and b by t.</para>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * MathEx.Clamp01(t);
        }

        /// <summary>
        /// Clamps value between 0 and 1 and returns value.
        /// </summary>
        public static double Clamp01(double value)
        {
            if (value < 0)
            {
                return 0;
            }
            if (value > 1)
            {
                return 1;
            }
            return value;
        }

        /// <summary>
        /// Clamps value between 0 and 1 and returns value.
        /// </summary>
        public static float Clamp01(float value)
        {
            if (value < 0f)
            {
                return 0f;
            }
            if (value > 1f)
            {
                return 1f;
            }
            return value;
        }

        /// <summary>
        /// Check if number is zero, the error is 0.001
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AmostZero(double value)
        {
            return (int)(1000 * value) == 0;
        }

        /// <summary>
        /// Check if number is zero, the error is 0.001
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AmostZero(float value)
        {
            return (int)(1000 * value) == 0;
        }

        /// <summary>
        /// Check if two number is equal, the error is 0.001
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AmostEqual(double a, double b)
        {
            return AmostZero(a - b);
        }

        /// <summary>
        /// Check if two number is equal, the error is 0.001
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool AmostEqual(float a, float b)
        {
            return AmostZero(a - b);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double InverseLength(Vector lhs, double fail_value)
        {
            var d = lhs.X * lhs.X + lhs.Y * lhs.Y;
            if (d > 0.0f) return 1.0 / System.Math.Sqrt(d);
            return fail_value;
        }

    }
}