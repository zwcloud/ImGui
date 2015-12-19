using System;
using System.Collections.Generic;

namespace ImGuiDemo
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
        private static Dictionary<OpType, string> s_opTypeStrings = new Dictionary<OpType, string>
        {
            {OpType.None, ""},
            {OpType.PlusMinus, "±"},
            {OpType.Sqrt, "√"},
            {OpType.Percent,"%"},
            {OpType.Inverse,"-"},
            {OpType.Divide,"/"},
            {OpType.Multiply,"*"},
            {OpType.Minus,"-"},
            {OpType.Plus,"+"}
        };

        public static string ToCustomString(this OpType opTypetype)
        {
            return s_opTypeStrings[opTypetype];
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