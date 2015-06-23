using System;

namespace IMGUIDemo
{
    public enum OpType
    {
        None = 0,

        PlusMinus = 14,
        Sqrt = 15,
        Percent = 16,
        Inverse = 17,

        Plus = 18,
        Minus = 19,
        Multiply = 20,
        Divide = 21,
    }

    public static class OpTypeExtensions
    {
        public static string ToCustomString(this OpType opTypetype)
        {
            switch (opTypetype)
            {
                case OpType.None:
                    return "";
                case OpType.PlusMinus:
                    return "±";
                case OpType.Sqrt:
                    return "√";
                case OpType.Percent:
                    return "%";
                case OpType.Inverse:
                    return "-";
                case OpType.Divide:
                    return "/";
                case OpType.Multiply:
                    return "*";
                case OpType.Minus:
                    return "-";
                case OpType.Plus:
                    return "+";
                default:
                    throw new ArgumentOutOfRangeException("opTypetype", opTypetype, null);
            }
        }

        public static bool IsUnaryOperator(this OpType enumValue)
        {
            switch (enumValue)
            {
                case OpType.PlusMinus:
                case OpType.Sqrt:
                case OpType.Percent:
                case OpType.Inverse:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsBinaryOperator(this OpType enumValue)
        {
            switch (enumValue)
            {
                case OpType.Divide:
                case OpType.Multiply:
                case OpType.Minus:
                case OpType.Plus:
                    return true;
                default:
                    return false;
            }
        }
    }

}