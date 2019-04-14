using ImGui;

namespace Calculator
{
    partial class Form1
    {
        public ButtonType Last { get; set; }
        public ButtonType Current { get; private set; }
        public Phase Phase { get; set; }

        private readonly Calc calc = new Calc();

        public bool open = true;

        protected override void OnGUI()
        {
            GUI.Begin("Simple Calculator", ref open, (10, 10), (240, 270));

            if(Current != ButtonType.Idle)
                Last = Current;
            Current = ButtonType.Idle;

            const int o = -20;

            GUI.Label(new Rect(14, 22 + o, new Size(190, 20)), calc.Expression+"##Expression");//ExpressionLabel
            GUI.Label(new Rect(14, 42+ o, new Size(190, 24)), calc.Result+"##Result");//ResultLabel

            var backspace = GUI.Button(new Rect(14, 68+ o, new Size(34, 27)), "←");//backspaceButton
            if (backspace) Current = ButtonType.Backspace;
            var clearInput = GUI.Button(new Rect(53, 68+ o, new Size(34, 27)), "CE");//CEButton
            if (clearInput) Current = ButtonType.ClearInput;
            var clear = GUI.Button(new Rect(92, 68+ o, new Size(34, 27)), "C");//CButton
            if (clear) Current = ButtonType.Clear;
            var plusMinus = GUI.Button(new Rect(131, 68+ o, new Size(34, 27)), "±");//SignButton
            if (plusMinus) Current = ButtonType.PlusMinus;
            var sqrt = GUI.Button(new Rect(170, 68+ o, new Size(34, 27)), "√");//SqrtButton
            if (sqrt) Current = ButtonType.Sqrt;

            bool[] number = new bool[10];

            number[7] = GUI.Button(new Rect(14, 100+ o, new Size(34, 27)), "7");//_7Button
            if (number[7]) Current = ButtonType.Number7;
            number[8] = GUI.Button(new Rect(53, 100+ o, new Size(34, 27)), "8");//_8Button
            if (number[8]) Current = ButtonType.Number8;
            number[9] = GUI.Button(new Rect(92, 100+ o, new Size(34, 27)), "9");//_9Button
            if (number[9]) Current = ButtonType.Number9;
            var divide = GUI.Button(new Rect(131, 100+ o, new Size(34, 27)), "/");//DivideButton
            if (divide) Current = ButtonType.Divide;
            var percent = GUI.Button(new Rect(170, 100+ o, new Size(34, 27)), "%");//PercentButton
            if (percent) Current = ButtonType.Percent;

            number[4] = GUI.Button(new Rect(14, 132+ o, new Size(34, 27)), "4");//_4Button
            if (number[4]) Current = ButtonType.Number4;
            number[5] = GUI.Button(new Rect(53, 132+ o, new Size(34, 27)), "5");//_5Button
            if (number[5]) Current = ButtonType.Number5;
            number[6] = GUI.Button(new Rect(92, 132+ o, new Size(34, 27)), "6");//_6Button
            if (number[6]) Current = ButtonType.Number6;
            var multiply = GUI.Button(new Rect(131, 132+ o, new Size(34, 27)), "*");//MultiplyButton
            if (multiply) Current = ButtonType.Multiply;
            var inverse = GUI.Button(new Rect(170, 132+ o, new Size(34, 27)), "1/x");//InverseButton
            if (inverse) Current = ButtonType.Inverse;

            number[1] = GUI.Button(new Rect(14, 164+ o, new Size(34, 27)), "1");//_1Button
            if (number[1]) Current = ButtonType.Number1;
            number[2] = GUI.Button(new Rect(53, 164+ o, new Size(34, 27)), "2");//_2Button
            if (number[2]) Current = ButtonType.Number2;
            number[3] = GUI.Button(new Rect(92, 164+ o, new Size(34, 27)), "3");//_3Button
            if (number[3]) Current = ButtonType.Number3;
            var minus = GUI.Button(new Rect(131, 164+ o, new Size(34, 27)), "-");//MinusButton
            if (minus) Current = ButtonType.Minus;

            number[0] = GUI.Button(new Rect(14, 196+ o, new Size(73, 27)), "0");//_0Button
            if (number[0]) Current = ButtonType.Number0;
            var dot = GUI.Button(new Rect(92, 196+ o, new Size(34, 27)), ".");//PointButton
            if (dot) Current = ButtonType.Dot;
            var plus = GUI.Button(new Rect(131, 196+ o, new Size(34, 27)), "+");//_plusButton
            if (plus) Current = ButtonType.Plus;

            var equal = GUI.Button(new Rect(170, 164+ o, new Size(34, 59)), "=");//_EqualButton
            if (equal) Current = ButtonType.Equal;

            GUI.End();

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
