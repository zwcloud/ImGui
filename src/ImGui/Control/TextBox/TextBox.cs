using System;
using ImGui.Input;
using ImGui.Rendering;
using ImGui.Style;

namespace ImGui
{
    public partial class GUI
    {
        //TODO multiple/single line TextBox
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
            var state = active ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;

            // last item state
            window.TempData.LastItemState = state;

            // render
            GUIAppearance.DrawTextBox(node, id, text, context, state);

            return text;
        }
    }

    public partial class GUILayout
    {
        /// <summary>
        /// Create a multi-line text box of fixed size.
        /// </summary>
        /// <param name="str_id">id</param>
        /// <param name="size">fixed size</param>
        /// <param name="text">text</param>
        /// <returns>(modified) text</returns>
        public static string TextBox(string str_id, Size size, string text, LayoutOptions? options)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return text;
            
            var container = window.RenderTree.CurrentContainer;

            var id = window.GetID(str_id);
            Node node = container.GetNodeById(id);
            if (node == null)
            {
                //create node
                node = new Node(id, $"{nameof(TextBox)}<{str_id}>");
                node.UseBoxModel = true;
                node.RuleSet.Replace(GUISkin.Current[GUIControlName.TextBox]);
                node.AttachLayoutGroup(true);
                node.RuleSet.ApplyOptions(GUILayout.Width(size.Width).Height(size.Height));
            }

            var textNodeId = window.GetID($"{nameof(TextBox)}<{str_id}>_Text");
            var textNode = node.GetDirectNodeById(textNodeId);
            if (textNode == null)
            {
                textNode = new Node(textNodeId);
            }
            var textSize = node.RuleSet.CalcContentBoxSize(text, node.State);
            textNode.RuleSet.Replace(GUISkin.Current[GUIControlName.TextBox]);
            textNode.ContentSize = textSize;
            textNode.ActiveSelf = true;
            node.AppendChild(textNode);

            container.AppendChild(node);
            node.RuleSet.ApplyOptions(options);
            node.ActiveSelf = true;

            // rect
            node.Rect = window.GetRect(node.Id);
            textNode.Rect = window.GetRect(textNode.Id);

            // interact
            text = GUIBehavior.TextBoxBehavior(textNode.Id, textNode.Rect, text,
                out bool hovered, out bool active, out var context);
            var state = active ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;

            // last item state
            window.TempData.LastItemState = state;
            
            // render
            GUIAppearance.DrawTextBox(textNode, textNode.Id, text, context, state);

            // draw the box
            var dc = node.RenderOpen();
            dc.DrawBoxModel(node.RuleSet, node.Rect);
            dc.Close();
            
            // do GUI logic for possible scroll-bars
            node.OnGUI();

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
                var size = node.RuleSet.CalcContentBoxSize(text, GUIState.Normal);
                node.AttachLayoutEntry(size);
            }
            container.AppendChild(node);
            node.RuleSet.ApplyOptions(options);
            node.ActiveSelf = true;

            // rect
            Rect rect = window.GetRect(id);

            // interact
            InputTextContext context;
            text = GUIBehavior.TextBoxBehavior(id, rect, text, out bool hovered, out bool active, out context, flags, checker);
            var state = active ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;

            // last item state
            window.TempData.LastItemState = state;

            // render
            if (flags.HaveFlag(InputTextFlags.Password))
            {
                var dotText = new string('*', text.Length);//FIXME bad performance
                GUIAppearance.DrawTextBox(node, id, dotText, context, state);
            }
            else
            {
                GUIAppearance.DrawTextBox(node, id, text, context, state);
            }

            return text;
        }

        public static string TextBox(string label, double width, string text, InputTextFlags flags, Func<char, bool> checker)
        {
            return TextBox(label, width, text, flags, checker, null);
        }
        
        public static string TextBox(string label, double width, string text)
        {
            return TextBox(label, width, text, InputTextFlags.Default, null, null);
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
                result = TextBox(text, StyleRuleSet.Global.Get<double>("FieldWidth"), text, flags, checker, GUILayout.ExpandWidth(true));
                Label(label, GUILayout.Width((int)StyleRuleSet.Global.Get<double>("LabelWidth")));
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
            return text;
        }
    }

    internal partial class GUIAppearance
    {
        public static void DrawTextBox(Node node, int id, string text, InputTextContext context, GUIState state)
        {
            GUIContext g = Form.current.uiContext;

            Rect rect = node.Rect;
            var d = node.RenderOpen();
            var style = node.RuleSet;
            // draw text, selection and caret
            var contentRect = node.ContentRect;
            // clip text rendering to content-box
            //d.PushClip(contentRect);
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
                d.DrawText(style, context.Text, contentRect);

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
                        d.DrawRectangle(new Brush(Color.Argb(100, 10, 102, 214)), null, selectionRect);
                    }
                    else//multiple lines
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
                        d.DrawRectangle(new Brush(Color.Argb(100, 10, 102, 214)), null, rect1);
                        d.DrawRectangle(null, new Pen(Color.White, 2), rect1);

                        // Line 2
                        var rect2 = new Rect(new Point(l, B.Y), new Point(r, C.Y));
                        if (rect2.Height > 0.5 * caretHeight)//TODO There should more a more reasonable way to detect this: If it only has two lines, we don't draw the inner rectangle.
                        {
                            d.DrawRectangle(new Brush(Color.Argb(100, 10, 102, 214)), null, rect2);
                            d.DrawRectangle(null, new Pen(Color.White, 2), rect2);
                        }

                        // Line 3
                        var rect3 = new Rect(new Point(l, C.Y), D);
                        d.DrawRectangle(new Brush(Color.Argb(100, 10, 102, 214)), null, rect3);
                        d.DrawRectangle(null, new Pen(Color.White, 2), rect3);
                    }
                }

                //Draw caret
                d.DrawLine(new Pen(Color.Argb(caretAlpha, 0, 0, 0), 2), caretTopPoint, caretBottomPoint);
            }
            else
            {
                d.DrawText(style, text, contentRect);
            }
            //d.Pop();
            d.Close();
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
            ruleSet.OverflowY = OverflowPolicy.Scroll;
        }
    }

}
