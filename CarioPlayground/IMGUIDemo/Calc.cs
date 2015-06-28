using System;
using System.Globalization;
using System.Text;

namespace IMGUIDemo
{
    public class Calc
    {
        private string _operand0 = "0";
        private string _operand1 = "0";

        /// <summary>
        /// States
        /// </summary>
        internal bool EnteringOperand0 = true;
        internal bool EnteringOperand1 = false;
        internal bool EnteringOperator = false;
        internal bool ShowingResult = false;

        public OpType Op { get; set; }

        public string Operand0
        {
            get { return _operand0; }
            set { _operand0 = value; }
        }

        public string Operand1
        {
            get { return _operand1; }
            set { _operand1 = value; }
        }

        public double Operand0Number
        {
            get { return double.Parse(Operand0); }
        }

        public double Operand1Number
        {
            get { return double.Parse(Operand1); }
        }

        public string Expression
        {
            get
            {
                string value = "";
                if(ShowingResult)
                {
                    if(Op.IsUnaryOperator())
                        value = Operand0;
                    else
                        value = Operand0 + Op.ToCustomString() + Operand1;
                }
                else
                {
                    if (EnteringOperand0)
                        value = Operand0;
                    if (EnteringOperator)
                        value = Operand0 + Op.ToCustomString();
                    if (EnteringOperand1)
                        value = Operand0 + Op.ToCustomString() + Operand1;
                }
                return value;
            }
        }

        public double ResultNumber { get; set; }

        public string Result
        {
            get
            {
                return ShowingResult ? ResultNumber.ToString() : "0";
            }
        }

        internal void DoCalc()
        {
            ResultNumber = _doCalc();
        }

        private double _doCalc()
        {
            if (Op.IsUnaryOperator())
            {
                var operand0 = Operand0Number;
                switch (Op)
                {
                    case OpType.PlusMinus:
                        return -operand0;
                    case OpType.Sqrt:
                        return Math.Sqrt(operand0);
                    case OpType.Percent:
                        return operand0 * 0.01;
                    case OpType.Inverse:
                        return 1.0/operand0;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (Op.IsBinaryOperator())
            {
                var operand0 = Operand0Number;
                var operand1 = Operand1Number;
                switch (Op)
                {
                    case OpType.Divide:
                        return operand0/operand1;
                    case OpType.Multiply:
                        return operand0*operand1;
                    case OpType.Minus:
                        return operand0 - operand1;
                    case OpType.Plus:
                        return operand0 + operand1;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            throw new InvalidOperationException("Can not calc!");
        }

        internal void Clear()
        {
            Operand0 = Operand1 = "0";
            Op = OpType.None;
        }

        internal void Backspace()
        {
            if(EnteringOperand0)
            {
                if(Operand0 != "0")
                {
                    Operand0 = Operand0.Remove(Operand0.Length - 1);
                }
                if(Operand0.Length == 0)
                {
                    Operand0 = "0";
                }
            }
            else if (EnteringOperand0)
            {
                if (Operand1 != "0")
                {
                    Operand1 = Operand1.Remove(Operand1.Length - 1);
                }
                if (Operand1.Length == 0)
                {
                    Operand1 = "0";
                }
            }
        }
    }
}
