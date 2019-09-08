using System;
using ImGui.Input;
using ImGui.Rendering;
using ImGui.Style;

namespace ImGui
{
    public partial class GUI
    {
        public static string TextBox(Rect rect, string label, string text, LayoutOptions? options)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return text;

            var id = window.GetID(label);
            var container = window.AbsoluteVisualList;
            Node node = (Node)container.Find(visual => visual.Id == id);
            label = Utility.FindRenderedText(label);
            if (node == null)
            {
                //create node
                node = new Node(id, $"{nameof(TextBox)}<{label}>");
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.TextBox]);
                container.Add(node);
            }
            node.RuleSet.ApplyOptions(options);
            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(rect);

            // interact
            InputTextContext context;
            text = GUIBehavior.TextBoxBehavior(id, rect, text, out bool hovered, out bool active, out context);

            // render
            var state = active ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;
            GUIAppearance.DrawTextBox(rect, id, text, context, state);

            return text;
        }
    }

    public partial class GUILayout
    {
        /// <summary>
        /// Create a multi-line text box.
        /// </summary>
        /// <param name="str_id">id</param>
        /// <param name="size">size</param>
        /// <param name="text">text</param>
        /// <returns>(modified) text</returns>
        public static string TextBox(string str_id, Size size, string text, LayoutOptions? options)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return text;

            var id = window.GetID(str_id);
            var container = window.RenderTree.CurrentContainer;
            Node node = container.GetNodeById(id);
            if (node == null)
            {
                //create node
                node = new Node(id, $"{nameof(TextBox)}<{text}>");
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.TextBox]);
                node.AttachLayoutEntry(size);
                container.AppendChild(node);
            }
            node.RuleSet.ApplyOptions(options);
            node.ActiveSelf = true;

            // rect
            Rect rect = window.GetRect(id);

            // interact
            InputTextContext context;
            text = GUIBehavior.TextBoxBehavior(id, rect, text, out bool hovered, out bool active, out context);

            // render
            var state = active ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;
            GUIAppearance.DrawTextBox(rect, id, text, context, state);

            return text;
        }

        public static string TextBox(string str_id, Size size, string text) => TextBox(str_id, size, text, null);

        /// <summary>
        /// Create a single-line text box.
        /// </summary>
        /// <param name="label">label</param>
        /// <param name="width">width</param>
        /// <param name="text">text</param>
        /// <param name="flags">filter flags</param>
        /// <param name="checker">custom checker per char</param>
        /// <returns>(modified) text</returns>
        public static string TextBox(string label, double width, string text, InputTextFlags flags, Func<char, bool> checker, LayoutOptions? options)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return text;

            var id = window.GetID(label);
            var container = window.RenderTree.CurrentContainer;
            Node node = container.GetNodeById(id);
            label = Utility.FindRenderedText(label);
            if (node == null)
            {
                //create node
                node = new Node(id, $"{nameof(TextBox)}<{label}>");
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.TextBox]);
                var size = node.RuleSet.CalcSize(text, GUIState.Normal);
                node.AttachLayoutEntry(size);
                container.AppendChild(node);
            }
            node.RuleSet.ApplyOptions(options);
            node.ActiveSelf = true;

            // rect
            Rect rect = window.GetRect(id);

            // interact
            InputTextContext context;
            text = GUIBehavior.TextBoxBehavior(id, rect, text, out bool hovered, out bool active, out context, flags, checker);

            // render
            var state = active ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;
            if (flags.HaveFlag(InputTextFlags.Password))
            {
                var dotText = new string('*', text.Length);//FIXME bad performance
                GUIAppearance.DrawTextBox(rect, id, dotText, context, state);
            }
            else
            {
                GUIAppearance.DrawTextBox(rect, id, text, context, state);
            }

            return text;
        }

        public static string TextBox(string label, double width, string text, InputTextFlags flags, Func<char, bool> checker)
        {
            return TextBox(label, width, text, flags, checker, null);
        }

        /// <summary>
        /// Create a multi-line text box.
        /// </summary>
        /// <param name="str_id">id</param>
        /// <param name="size">size</param>
        /// <param name="text">text</param>
        /// <returns>(modified) text</returns>
        public static string InputTextMultiline(string str_id, Size size, string text) => TextBox(str_id, size, text, null);

        /// <summary>
        /// Create a single-line text box.
        /// </summary>
        /// <param name="label">label</param>
        /// <param name="text">text</param>
        /// <param name="flags">filter flags</param>
        /// <param name="checker">custom checker per char</param>
        /// <returns>(modified) text</returns>
        public static string InputText(string label, string text, InputTextFlags flags = 0, Func<char, bool> checker = null)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return text;

            var id = window.GetID(label);

            string result;
            BeginHorizontal("FieldGroup~" + id);
            {
                result = TextBox(text, GUISkin.Current.FieldWidth, text, flags, checker, GUILayout.ExpandWidth(true));
                //Space("FieldSpacing", GUISkin.Current.FieldSpacing);
                Label(label, GUILayout.Width((int)GUISkin.Current.LabelWidth));
            }
            EndHorizontal();

            return result;
        }

        /// <summary>
        /// Create a float input field.
        /// </summary>
        /// <param name="label">label</param>
        /// <param name="value">value</param>
        /// <returns>modified value</returns>
        public static float InputFloat(string label, float value)
        {
            var text = value.ToString();
            text = InputText(label, text, InputTextFlags.CharsDecimal);
            float newValue = value;
            if (float.TryParse(text, out newValue))
            {
                return newValue;
            }
            return value;
        }

        /// <summary>
        /// Create a double input field.
        /// </summary>
        /// <param name="label">label</param>
        /// <param name="value">value</param>
        /// <returns>modified value</returns>
        public static double InputDouble(string label, double value)
        {
            var text = value.ToString();
            text = InputText(label, text, InputTextFlags.CharsDecimal);
            double newValue = value;
            if (double.TryParse(text, out newValue))
            {
                return newValue;
            }
            return value;
        }

        /// <summary>
        /// Create a int input field.
        /// </summary>
        /// <param name="label">label</param>
        /// <param name="value">value</param>
        /// <returns>modified value</returns>
        public static int InputInt(string label, int value)
        {
            var text = value.ToString();
            text = InputText(label, text, InputTextFlags.CharsDecimal, (c) => c != '.');
            int newValue = value;
            if (int.TryParse(text, out newValue))
            {
                return newValue;
            }
            return value;
        }
    }

    internal partial class GUIBehavior
    {
        public static string TextBoxBehavior(int id, Rect rect, string text, out bool hovered, out bool active, out InputTextContext context, InputTextFlags flags = 0, Func<char, bool> checker = null)
        {
            hovered = false;
            active = false;
            context = null;
#if false
            GUIContext g = Form.current.uiContext;

            active = false;
            hovered = g.IsHovered(rect, id);
            if (hovered)
            {
                g.SetHoverID(id);

                if (Mouse.Instance.LeftButtonPressed)
                {
                    g.SetActiveID(id);
                }
            }
            else
            {
                if (Mouse.Instance.LeftButtonPressed)
                {
                    if (g.ActiveId == id)
                    {
                        g.SetActiveID(0);
                    }
                }
            }

            if (g.ActiveId == id && g.InputTextState.inputTextContext.Id != id) //editing text box changed to TextBox<id>
            {
                g.InputTextState.stateMachine.CurrentState = "Active"; //reset state
                g.InputTextState.inputTextContext = new InputTextContext() //reset input text context data
                {
                    Id = id,
                    CaretIndex = 0,
                    SelectIndex = 0,
                    Selecting = false,
                    CaretPosition = Point.Zero,
                    Flags = flags,
                    Checker = checker,
                    Rect = rect,
                    Text = text
                };
            }

            context = null;
            if (g.ActiveId == id)
            {
                active = true;

                var stateMachine = g.InputTextState.stateMachine;
                context = g.InputTextState.inputTextContext;
                context.Rect = rect;

                // Although we are active we don't prevent mouse from hovering other elements unless we are interacting right now with the widget.
                // Down the line we should have a cleaner library-wide concept of Selected vs Active.
                g.ActiveIdAllowOverlap = !Mouse.Instance.LeftButtonPressed;

#if INSPECT_STATE
            var A = stateMachine.CurrentState;
#endif
                if (Mouse.Instance.LeftButtonPressed)
                {
                    stateMachine.MoveNext(TextBoxCommand.MoveCaret, context);
                }

                if (hovered && Mouse.Instance.LeftButtonState == KeyState.Down &&
                    stateMachine.CurrentState != TextBoxState.ActiveSelecting)
                {
                    stateMachine.MoveNext(TextBoxCommand.MoveCaret, context);
                    stateMachine.MoveNext(TextBoxCommand.BeginSelect, context);
                }

                if (hovered && Mouse.Instance.LeftButtonState == KeyState.Up)
                {
                    stateMachine.MoveNext(TextBoxCommand.EndSelect, context);
                }

                if (stateMachine.CurrentState == TextBoxState.Active)
                {
                    stateMachine.MoveNext(TextBoxCommand.DoEdit, context);
                }
                if (stateMachine.CurrentState == TextBoxState.ActiveSelecting)
                {
                    stateMachine.MoveNext(TextBoxCommand.DoSelect, context);
                }
                stateMachine.MoveNext(TextBoxCommand.MoveCaretKeyboard, context);
#if INSPECT_STATE
            var B = stateMachine.CurrentState;
            Debug.WriteLineIf(A != B,
                string.Format("TextBox<{0}> {1}=>{2} CaretIndex: {3}, SelectIndex: {4}",
                    id, A, B, context.CaretIndex, context.SelectIndex));
#endif
                return context.Text;
            }
#endif
            return text;
        }
    }

    internal partial class GUIAppearance
    {
        public static void DrawTextBox(Rect rect, int id, string text, InputTextContext context, GUIState state)
        {
#if false
            GUIContext g = Form.current.uiContext;
            WindowManager w = g.WindowManager;
            Window window = w.CurrentWindow;

            var d = window.DrawList;
            var style = GUIStyle.Basic;
            // draw text, selection and caret
            var contentRect = style.GetContentRect(rect);
            d.PushClipRect(contentRect, true);
            if (g.ActiveId == id)
            {
                //Calculate positions and sizes
                var textContext = TextMeshUtil.GetTextContext(context.Text, rect.Size, style, GUIState.Normal);
                var offsetOfTextRect = contentRect.TopLeft;
                float pointX, pointY;
                float caretHeight;
                textContext.IndexToXY(context.CaretIndex, false, out pointX, out pointY, out caretHeight);
                var caretTopPoint = new Point(pointX, pointY);
                var caretBottomPoint = new Point(pointX, pointY + caretHeight);
                caretTopPoint.Offset(offsetOfTextRect.X, offsetOfTextRect.Y);
                caretBottomPoint.Offset(offsetOfTextRect.X, offsetOfTextRect.Y);

                byte caretAlpha = context.CaretAlpha;

                // Check if the caret is outside the rect. If so, move the text so the caret is always shown. FIXME this should be done in TextBoxBehaviour
                var caretX = caretTopPoint.X;
                if (caretX < contentRect.X || caretX > contentRect.Right)
                {
                    var offsetX = -(caretX - contentRect.Width - rect.X);
                    contentRect.Offset(offsetX, 0);
                    caretTopPoint.Offset(offsetX, 0);
                    caretBottomPoint.Offset(offsetX, 0);
                }

                //Draw text
                d.DrawText(contentRect, context.Text, style, GUIState.Normal);

                //Draw selection rect
                /*
                 * Note: Design
                 * 
                 *  left bound            right bound
                 *     ↓                        ↓
                 *     |      A-----------------+
                 *     |      |CONTENT_CONTENT_C| => Line 1 => rect1
                 *     +------B                 |
                 *     |ONTENT_CONTENT_CONTENT_C| => Line 2 (represents inner lines) => rect2
                 *     |                 C------+
                 *     |ONTENT_CONTENT_CO| => Line 3 => rect3
                 *     +-----------------D
                 * 
                 * left bound = l
                 * right bound = r
                 */
                if (context.SelectIndex != context.CaretIndex)
                {
                    float selectPointX, selectPointY;
                    textContext.IndexToXY(context.SelectIndex, false, out selectPointX, out selectPointY, out float dummyHeight);
                    var selectTopPoint = new Point(selectPointX, selectPointY);
                    var selectBottomPoint = new Point(selectPointX, selectPointY + caretHeight);
                    selectTopPoint.Offset(offsetOfTextRect.X, offsetOfTextRect.Y);
                    selectBottomPoint.Offset(offsetOfTextRect.X, offsetOfTextRect.Y);
                    var delta = Math.Abs(selectTopPoint.Y - caretTopPoint.Y);
                    if (delta < caretHeight) // single line
                    {
                        var selectionRect = new Rect(
                            new Point(pointX, pointY),
                            new Point(selectPointX, selectPointY + caretHeight));
                        selectionRect.Offset(offsetOfTextRect.X, offsetOfTextRect.Y);
                        d.AddRectFilled(selectionRect.Min, selectionRect.Max, Color.Argb(100, 10, 102, 214));
                    }
                    else//mutiple line
                    {
                        var l = contentRect.Left;
                        var r = contentRect.Right;

                        Point A;
                        Point B;
                        Point C;
                        Point D;

                        if (selectTopPoint.Y > caretTopPoint.Y)
                        {
                            A = caretTopPoint;
                            B = caretBottomPoint;
                            C = selectTopPoint;
                            D = selectBottomPoint;
                        }
                        else
                        {
                            A = selectTopPoint;
                            B = selectBottomPoint;
                            C = caretTopPoint;
                            D = caretBottomPoint;
                        }


                        // Line 1
                        var rect1 = new Rect(A, r - A.X, caretHeight);
                        d.AddRectFilled(rect1, Color.Argb(100, 10, 102, 214), 12);
                        d.AddRect(rect1.Min, rect1.Max, Color.White, 12, 15, 2);

                        // Line 2
                        var rect2 = new Rect(new Point(l, B.Y), new Point(r, C.Y));
                        if (rect2.Height > 0.5 * caretHeight)//TODO There should more a more reasonable way to detect this: If it only has two lines, we don't draw the inner rectangle.
                        {
                            d.AddRectFilled(rect2, Color.Argb(100, 10, 102, 214), 12);
                            d.AddRect(rect2.Min, rect2.Max, Color.White, 12, 15, 2);
                        }

                        // Line 3
                        var rect3 = new Rect(new Point(l, C.Y), D);
                        d.AddRectFilled(rect3, Color.Argb(100, 10, 102, 214), 12);
                        d.AddRect(rect3.Min, rect3.Max, Color.White, 12, 15, 2);
                    }
                }

                //Draw caret
                d.PathMoveTo(caretTopPoint);
                d.PathLineTo(caretBottomPoint);
                d.PathStroke(Color.Argb(caretAlpha, 0, 0, 0), false, 2);
            }
            else
            {
                d.DrawText(contentRect, text, style, GUIState.Normal);
            }
            d.PopClipRect();

            // draw the box
            {
                d.AddRect(rect.Min, rect.Max, style.BorderColor);
            }
#endif
        }
    }

    internal partial class GUISkin
    {
        private void InitTextBoxStyles(StyleRuleSet ruleSet)
        {
            StyleRuleSetBuilder builder = new StyleRuleSetBuilder(ruleSet);
            builder
                .Border(1.0)
                .Padding(3.0)
                .BorderColor(Color.Rgb(112), GUIState.Normal)
                .BorderColor(Color.Rgb(23), GUIState.Hover)
                .BorderColor(Color.Rgb(0, 120, 215), GUIState.Active);
        }
    }

}
