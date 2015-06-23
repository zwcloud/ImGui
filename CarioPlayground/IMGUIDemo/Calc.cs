using System;
using System.Globalization;
using System.Text;

namespace IMGUIDemo
{
    public class Calc
    {
        private string _operand0 = String.Empty;
        private string _operand1 = String.Empty;
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
                if (Operand0 == String.Empty)
                    value = "";
                else
                {
                    value = Operand0;
                    if (Op >= OpType.Divide)
                        value += Op.ToCustomString();
                }
                return value;
            }
        }

        public double ResultNumber { get; set; }

        public string Result
        {
            get
            {
                if (Operand0 == string.Empty || Op == OpType.None)
                {
                    return "0";
                }
                return ResultNumber.ToString();
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
    }
}
