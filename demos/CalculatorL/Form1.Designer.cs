using ImGui;

namespace Calculator
{
    partial class Form1
    {
        public ButtonType Last { get; set; }
        public ButtonType Current { get; private set; }
        public Phase Phase { get; set; }

        private readonly Calc calc = new Calc();

        protected override void OnGUI()
        {
            GUILayout.Label("Simple Calculator");//Title

            if (Current != ButtonType.Idle)
                Last = Current;
            Current = ButtonType.Idle;

            GUILayout.Label(calc.Expression);//ExpressionLabel
            GUILayout.Label(calc.Result);//ResultLabel

            GUILayout.BeginHorizontal("Line0");
            {
                var backspace = GUILayout.Button("←");//backspaceButton
                if (backspace) Current = ButtonType.Backspace;
                var clearInput = GUILayout.Button("CE");//CEButton
                if (clearInput) Current = ButtonType.ClearInput;
                var clear = GUILayout.Button("C");//CButton
                if (clear) Current = ButtonType.Clear;
                var plusMinus = GUILayout.Button("±");//SignButton
                if (plusMinus) Current = ButtonType.PlusMinus;
                var sqrt = GUILayout.Button("√");//SqrtButton
                if (sqrt) Current = ButtonType.Sqrt;
            }
            GUILayout.EndHorizontal();

            bool[] number = new bool[10];

            GUILayout.BeginHorizontal("Line1");
            {
                number[7] = GUILayout.Button("7");//_7Button
                if (number[7]) Current = ButtonType.Number7;
                number[8] = GUILayout.Button("8");//_8Button
                if (number[8]) Current = ButtonType.Number8;
                number[9] = GUILayout.Button("9");//_9Button
                if (number[9]) Current = ButtonType.Number9;
                var divide = GUILayout.Button("/");//DivideButton
                if (divide) Current = ButtonType.Divide;
                var percent = GUILayout.Button("%");//PercentButton
                if (percent) Current = ButtonType.Percent;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Line2");
            {
                number[4] = GUILayout.Button("4");//_4Button
                if (number[4]) Current = ButtonType.Number4;
                number[5] = GUILayout.Button("5");//_5Button
                if (number[5]) Current = ButtonType.Number5;
                number[6] = GUILayout.Button("6");//_6Button
                if (number[6]) Current = ButtonType.Number6;
                var multiply = GUILayout.Button("*");//MultiplyButton
                if (multiply) Current = ButtonType.Multiply;
                var inverse = GUILayout.Button("1/x");//InverseButton
                if (inverse) Current = ButtonType.Inverse;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal("Line3");
            {
                GUILayout.BeginVertical("VGroup~", GUILayout.StretchWidth(4));
                {
                    GUILayout.BeginHorizontal("HGroup~0");
                    {
                        number[1] = GUILayout.Button("1");//_1Button
                        if (number[1]) Current = ButtonType.Number1;
                        number[2] = GUILayout.Button("2");//_2Button
                        if (number[2]) Current = ButtonType.Number2;
                        number[3] = GUILayout.Button("3");//_3Button
                        if (number[3]) Current = ButtonType.Number3;
                        var minus = GUILayout.Button("-");//MinusButton
                        if (minus) Current = ButtonType.Minus;
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal("HGroup~1");
                    {
                        number[0] = GUILayout.Button("0", GUILayout.StretchWidth(2));//_0Button
                        if (number[0]) Current = ButtonType.Number0;
                        var dot = GUILayout.Button(".");//PointButton
                        if (dot) Current = ButtonType.Dot;
                        var plus = GUILayout.Button("+");//_plusButton
                        if (plus) Current = ButtonType.Plus;
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();

                var equal = GUILayout.Button("=", GUILayout.StretchWidth(1));//_EqualButton
                if (equal) Current = ButtonType.Equal;
            }
            GUILayout.EndHorizontal();

            if (Current == ButtonType.Idle)
                return;

            if (Current == ButtonType.ClearInput)
            {
                calc.ClearInput();
                calc.ShowingResult = false;
                return;
            }

            if (Current == ButtonType.Clear)
            {
                calc.Clear();
                calc.EnteringOperand0 = true;
                calc.EnteringOperator = false;
                calc.EnteringOperand1 = false;

                calc.ShowingResult = false;
                return;
            }

            if(Current == ButtonType.Backspace)
            {
                calc.Backspace();
                return;
            }

            if (Current.IsNumber())
            {
                if (calc.ShowingResult)
                {
                    calc.Clear();
                }
                if(calc.EnteringOperand0)
                {
                    if(calc.Operand0 == "0")
                        calc.Operand0 = ((int)Current).ToString();
                    else
                        calc.Operand0 += (int)Current;
                }
                else if (calc.EnteringOperand1)
                {
                    if (calc.Operand1 == "0")
                        calc.Operand1 = ((int)Current).ToString();
                    else
                        calc.Operand1 += (int)Current;
                }

                calc.ShowingResult = false;
            }

            if (Last.IsNumber() && Current.IsUnaryOperator())
            {
                calc.Op = (OpType)Current;
                calc.DoCalc();
                calc.EnteringOperand0 = true;
                calc.EnteringOperator = false;
                calc.EnteringOperand1 = false;

                calc.ShowingResult = true;
            }

            if (Last.IsNumber() && Current.IsBinaryOperator())
            {
                calc.Op = (OpType)Current;
                calc.EnteringOperand0 = false;
                calc.EnteringOperator = true;
                calc.EnteringOperand1 = true;

                calc.ShowingResult = false;
            }

            if (!calc.ShowingResult && Current.IsBinaryOperator())
            {
                calc.Op = (OpType)Current;
                calc.EnteringOperand0 = false;
                calc.EnteringOperator = true;
                calc.EnteringOperand1 = true;

                calc.ShowingResult = false;
            }

            if(Last.IsBinaryOperator() && Current.IsNumber())
            {
                calc.EnteringOperand0 = false;
                calc.EnteringOperator = false;
                calc.EnteringOperand1 = true;

                calc.ShowingResult = false;
            }

            if (calc.EnteringOperand1 && Current == ButtonType.Equal)
            {
                calc.DoCalc();
                calc.EnteringOperand0 = false;
                calc.EnteringOperator = false;
                calc.EnteringOperand1 = false;

                calc.ShowingResult = true;
            }

            if (Last == ButtonType.Equal && Current.IsNumber())
            {
                calc.Clear();
                calc.EnteringOperand0 = true;
                calc.EnteringOperator = false;
                calc.EnteringOperand1 = false;
                calc.ShowingResult = false;
                calc.Operand0 = ((int)Current).ToString();
            }
        }
    }

}
