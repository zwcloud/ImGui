using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMGUI;

namespace IMGUIDemo
{
    partial class Form1
    {
        #region paramters
        private bool _opened;


        public ButtonType Last { get; set; }
        public ButtonType Current { get; private set; }
        public Phase Phase { get; set; }

        private readonly Calc calc = new Calc();



        private readonly string[] comboBoxItems = new[] { "item0", "item1", "item2", "item3" };
        private int selectedindex = 0;

        private Texture myImage = new Texture(@"W:\VS2013\CarioPlayground\IMGUIDemo\empowered-by-gnu.svg");

        private bool radio0Selected = false;
        private bool radio1Selected = false;
        private bool radio2Selected = false;

        #endregion

        protected override void OnGUI(GUI gui)
        {
#if true
            if(gui.Button(new Rect(new Point(20, 20), new Point(120, 40)), "button 0!", "Button0"))
            {
                Debug.WriteLine("button 0 clicked!");
            }

            if(gui.Button(new Rect(new Point(20, 42), new Point(120, 62)), "button 1!", "Button1"))
            {
                Debug.WriteLine("button 1 clicked!");
            }

            if(gui.Button(new Rect(new Point(20, 64), new Point(120, 84)), "button 2!", "Button2"))
            {
                Debug.WriteLine("button 2 clicked!");
            }

            var oldValueOfTaggle = _opened;
            _opened = gui.Toggle(new Rect(new Point(20, 86), new Point(120, 106)), "Opened?", _opened, "Toggle0");
            if(_opened ^ oldValueOfTaggle)
                Debug.WriteLine("Toggle 0 {0}", _opened ? "on!" : "off!", null);

            var oldValueOfComboBox = selectedindex;
            selectedindex = gui.CombolBox(new Rect(new Point(20, 108), new Point(120, 128)), comboBoxItems, selectedindex,
                "MyCombo");
            if(selectedindex != oldValueOfComboBox)
                Debug.WriteLine("ComboBox item changed to {0}:{1}", selectedindex, comboBoxItems[selectedindex]);

            var oldValueOfradio0 = radio0Selected;
            radio0Selected = gui.Radio(new Rect(new Point(20, 132), new Point(120, 152)), "RadioItem0", "G0", radio0Selected, "Radio0");
            if(radio0Selected && radio0Selected != oldValueOfradio0)
                Debug.WriteLine("Radio0 selected");

            var oldValueOfradio1 = radio1Selected;
            radio1Selected = gui.Radio(new Rect(new Point(20, 154), new Point(120, 174)), "RadioItem1", "G0", radio1Selected, "Radio1");
            if(radio1Selected && radio1Selected != oldValueOfradio1)
                Debug.WriteLine("Radio1 selected");

            var oldValueOfradio2 = radio2Selected;
            radio2Selected = gui.Radio(new Rect(new Point(20, 178), new Point(120, 198)), "RadioItem2", "G0", radio2Selected, "Radio2");
            if(radio2Selected && radio2Selected != oldValueOfradio2)
                Debug.WriteLine("Radio2 selected");

            gui.Image(new Rect(new Point(130, 20), new Point(240, 200)), myImage, "MyImage");

#else
            if(Current != ButtonType.Idle)
                Last = Current;
            Current = ButtonType.Idle;

            gui.Label(new Rect(new Point(14, 14), new Size(190, 20)), calc.Expression);
            gui.Label(new Rect(new Point(14, 33), new Size(190, 30)), calc.Result);

            var backspace = gui.Button(new Rect(new Point(14, 68), new Size(34, 27)), "←—");
            if(backspace) Current = ButtonType.Backspace;
            var clearInput = gui.Button(new Rect(new Point(53, 68), new Size(34, 27)), "CE");
            if (clearInput) Current = ButtonType.ClearInput;
            var clear = gui.Button(new Rect(new Point(92, 68), new Size(34, 27)), "C");
            if (clear) Current = ButtonType.Clear;
            var plusMinus = gui.Button(new Rect(new Point(131, 68), new Size(34, 27)), "±");
            if (plusMinus) Current = ButtonType.PlusMinus;
            var sqrt = gui.Button(new Rect(new Point(170, 68), new Size(34, 27)), "√");
            if (sqrt) Current = ButtonType.Sqrt;

            bool[] number = new bool[10];

            number[7] = gui.Button(new Rect(new Point(14, 100), new Size(34, 27)), "7");
            if (number[7]) Current = ButtonType.Number7;
            number[8] = gui.Button(new Rect(new Point(53, 100), new Size(34, 27)), "8");
            if (number[8]) Current = ButtonType.Number8;
            number[9] = gui.Button(new Rect(new Point(92, 100), new Size(34, 27)), "9");
            if (number[9]) Current = ButtonType.Number9;
            var divide = gui.Button(new Rect(new Point(131, 100), new Size(34, 27)), "/");
            if (divide) Current = ButtonType.Divide;
            var percent = gui.Button(new Rect(new Point(170, 100), new Size(34, 27)), "%");
            if (percent) Current = ButtonType.Percent;

            number[4] = gui.Button(new Rect(new Point(14, 132), new Size(34, 27)), "4");
            if (number[4]) Current = ButtonType.Number4;
            number[5] = gui.Button(new Rect(new Point(53, 132), new Size(34, 27)), "5");
            if (number[5]) Current = ButtonType.Number5;
            number[6] = gui.Button(new Rect(new Point(92, 132), new Size(34, 27)), "6");
            if (number[6]) Current = ButtonType.Number6;
            var multiply = gui.Button(new Rect(new Point(131, 132), new Size(34, 27)), "*");
            if (multiply) Current = ButtonType.Multiply;
            var inverse = gui.Button(new Rect(new Point(170, 132), new Size(34, 27)), "1/x");
            if (inverse) Current = ButtonType.Inverse;

            number[1] = gui.Button(new Rect(new Point(14, 164), new Size(34, 27)), "1");
            if (number[1]) Current = ButtonType.Number1;
            number[2] = gui.Button(new Rect(new Point(53, 164), new Size(34, 27)), "2");
            if (number[2]) Current = ButtonType.Number2;
            number[3] = gui.Button(new Rect(new Point(92, 164), new Size(34, 27)), "3");
            if (number[3]) Current = ButtonType.Number3;
            var minus = gui.Button(new Rect(new Point(131, 164), new Size(34, 27)), "-");
            if (minus) Current = ButtonType.Minus;

            number[0] = gui.Button(new Rect(new Point(14, 196), new Size(73, 27)), "0");
            if (number[0]) Current = ButtonType.Number0;
            var dot = gui.Button(new Rect(new Point(92, 196), new Size(34, 27)), ".");
            if (dot) Current = ButtonType.Dot;
            var plus = gui.Button(new Rect(new Point(131, 196), new Size(34, 27)), "+");
            if (plus) Current = ButtonType.Plus;

            var equal = gui.Button(new Rect(new Point(170, 164), new Size(34, 59)), "=");
            if (equal) Current = ButtonType.Equal;

            if (Current == ButtonType.Idle)
                return;

            if(Current == ButtonType.Clear)
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

            if(Last.IsBinaryOperator() && Current.IsNumber())
            {
                calc.EnteringOperand0 = false;
                calc.EnteringOperator = false;
                calc.EnteringOperand1 = true;

                calc.ShowingResult = false;
            }

            if (calc.EnteringOperand1 && Last.IsNumber() && Current == ButtonType.Equal)
            {
                calc.DoCalc();
                calc.EnteringOperand0 = false;
                calc.EnteringOperator = false;
                calc.EnteringOperand1 = false;

                calc.ShowingResult = true;
            }
#endif
        }
    }

}
