//#define INSPECT_STATE
using System;
using System.Diagnostics;
using ImGui.Common.Primitive;
using ImGui.Input;

namespace ImGui
{
    public partial class GUI
    {
        public static string Textbox(Rect rect, string label, string text)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return text;

            var id = window.GetID(label);

            // style apply
            var s = g.StyleStack;
            var modifiers = GUISkin.Instance[GUIControlName.TextBox];
            s.PushRange(modifiers);

            // rect
            rect = window.GetRect(rect);

            // interact
            InputTextContext context;
            text = GUIBehavior.TextBoxBehavior(id, rect, text, out context);

            // render
            GUIAppearance.DrawTextBox(rect, id, text, context);

            // style restore
            s.PopStyle(modifiers.Length);

            return text;
        }
    }

    public partial class GUILayout
    {
        /// <summary>
        /// Create a multi-line text box.
        /// </summary>
        /// <param name="label">label</param>
        /// <param name="size">size</param>
        /// <param name="text">text</param>
        /// <returns>(modified) text</returns>
        public static string TextBox(string label, Size size, string text)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return text;

            int id = window.GetID(label);

            // style apply
            var s = g.StyleStack;
            var modifiers = GUISkin.Instance[GUIControlName.TextBox];
            s.PushRange(modifiers);

            // rect
            Rect rect = window.GetRect(id, size);

            // interact
            InputTextContext context;
            text = GUIBehavior.TextBoxBehavior(id, rect, text, out context);

            // render
            GUIAppearance.DrawTextBox(rect, id, text, context);

            // style restore
            s.PopStyle(modifiers.Length);

            return text;
        }

        /// <summary>
        /// Create a single-line text box.
        /// </summary>
        /// <param name="label">label</param>
        /// <param name="width">width</param>
        /// <param name="text">text</param>
        /// <returns>(modified) text</returns>
        public static string TextBox(string label, double width, string text)
        {
            GUIContext g = GetCurrentContext();
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return text;

            int id = window.GetID(label);

            // style apply
            var s = g.StyleStack;
            var style = s.Style;
            var modifiers = GUISkin.Instance[GUIControlName.Label];
            s.PushRange(modifiers);

            // rect
            var height = style.CalcSize(text, GUIState.Normal).Height;
            var size = new Size(width, height);
            Rect rect = window.GetRect(id, size);

            // interact
            InputTextContext context;
            text = GUIBehavior.TextBoxBehavior(id, rect, text, out context);

            // render
            GUIAppearance.DrawTextBox(rect, id, text, context);

            // style restore
            s.PopStyle(modifiers.Length);

            return text;
        }
    }

    internal partial class GUIBehavior
    {
        public static string TextBoxBehavior(int id, Rect rect, string text, out InputTextContext context)
        {
            GUIContext g = Form.current.uiContext;

            var hovered = g.IsHovered(rect, id);
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
                    Rect = rect,
                    Text = text
                };
            }

            context = null;
            if (g.ActiveId == id)
            {
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
                text = context.Text;
            }
            return text;
        }
    }

    internal partial class GUIAppearance
    {
        public static void DrawTextBox(Rect rect, int id, string text, InputTextContext context)
        {
            GUIContext g = Form.current.uiContext;
            WindowManager w = g.WindowManager;
            Window window = w.CurrentWindow;

            var d = window.DrawList;
            var style = g.StyleStack.Style;
            var contentRect = Utility.GetContentRect(rect, style);
            d.PushClipRect(rect, true);
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

                //Draw the box
                {
                    d.AddRect(rect.Min, rect.Max, Color.White);
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
                d.PathStroke(Color.Argb(caretAlpha, 255, 255, 255), false, 2);
            }
            else
            {
                d.DrawText(contentRect, text, style, GUIState.Normal);
                d.AddRect(rect.Min, rect.Max, Color.White);
            }
            d.PopClipRect();
        }
    }

    partial class GUISkin
    {
        void InitTextBoxStyles()
        {
            double fontSize = CurrentOS.IsAndroid ? 32.0 : 13.0;
            var textBoxStyles = new [] {
                new StyleModifier(GUIStyleName.PaddingLeft, StyleType.@double, 10.0, GUIState.Normal),
                new StyleModifier(GUIStyleName.PaddingLeft, StyleType.@double, 10.0, GUIState.Hover),
                new StyleModifier(GUIStyleName.PaddingLeft, StyleType.@double, 10.0, GUIState.Active),
                new StyleModifier(GUIStyleName.PaddingTop, StyleType.@double, 10.0, GUIState.Normal),
                new StyleModifier(GUIStyleName.PaddingTop, StyleType.@double, 10.0, GUIState.Hover),
                new StyleModifier(GUIStyleName.PaddingTop, StyleType.@double, 10.0, GUIState.Active),
                new StyleModifier(GUIStyleName.PaddingRight, StyleType.@double, 10.0, GUIState.Normal),
                new StyleModifier(GUIStyleName.PaddingRight, StyleType.@double, 10.0, GUIState.Hover),
                new StyleModifier(GUIStyleName.PaddingRight, StyleType.@double, 10.0, GUIState.Active),
                new StyleModifier(GUIStyleName.PaddingBottom, StyleType.@double, 10.0, GUIState.Normal),
                new StyleModifier(GUIStyleName.PaddingBottom, StyleType.@double, 10.0, GUIState.Hover),
                new StyleModifier(GUIStyleName.PaddingBottom, StyleType.@double, 10.0, GUIState.Active),
                new StyleModifier(GUIStyleName.FontSize, StyleType.@double, fontSize, GUIState.Normal),
                new StyleModifier(GUIStyleName.FontSize, StyleType.@double, fontSize, GUIState.Hover),
                new StyleModifier(GUIStyleName.FontSize, StyleType.@double,  fontSize, GUIState.Active),
            };
            this.styles.Add(GUIControlName.TextBox, textBoxStyles);
        }
    }

    internal static class TextBoxState
    {
        public const string Normal = "Normal";
        public const string Hover = "Hover";
        public const string Active = "Active";
        public const string ActiveSelecting = "ActiveSelecting";
    }

    internal static class TextBoxCommand
    {
        public const string MoveIn = "MoveIn";
        public const string MoveOut = "MoveOut";
        public const string EnterEdit = "EnterEdit";
        public const string ExitEditIn = "ExitEditIn";
        public const string ExitEditOut = "ExitEditOut";
        public const string BeginSelect = "BeginSelect";
        public const string EndSelect = "EndSelect";

        public const string MoveCaret = "MoveCaret";
        public const string MoveCaretKeyboard = "MoveCaretKeyboard";
        public const string DoSelect = "DoSelect";
        public const string DoEdit = "DoEdit";
    }
}
