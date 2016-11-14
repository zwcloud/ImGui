namespace Calculator
{
    public enum ButtonType
    {
        Number0 = 0,
        Number1 = 1,
        Number2 = 2,
        Number3 = 3,
        Number4 = 4,
        Number5 = 5,
        Number6 = 6,
        Number7 = 7,
        Number8 = 8,
        Number9 = 9,
        Dot = 10,

        Backspace = 11,
        ClearInput = 12,
        Clear = 13,

        PlusMinus = 14,
        Sqrt = 15,
        Percent = 16,
        Inverse = 17,

        Plus = 18,
        Minus = 19,
        Multiply = 20,
        Divide = 21,

        Equal = 22,

        Idle = 99
    }

    public static class ButtonTypeExtensions
    {
        public static bool IsNumber(this ButtonType enumValue)
        {
            return ButtonType.Number0 <= enumValue && enumValue <= ButtonType.Number9;
        }

        public static bool IsBinaryOperator(this ButtonType enumValue)
        {
            return ButtonType.Plus <= enumValue && enumValue <= ButtonType.Divide;
        }

        public static bool IsUnaryOperator(this ButtonType enumValue)
        {
            return ButtonType.PlusMinus <= enumValue && enumValue <= ButtonType.Inverse;
        }

        public static bool IsOperator(this ButtonType enumValue)
        {
            return enumValue.IsBinaryOperator() || enumValue.IsUnaryOperator();
        }

        public static bool IsEqualOpeartor(this ButtonType enumValue)
        {
            return enumValue == ButtonType.Equal;
        }



    }
}