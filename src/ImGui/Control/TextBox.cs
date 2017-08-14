//#define INSPECT_STATE
using System;
using System.Diagnostics;
using ImGui.Common.Primitive;
using ImGui.Input;

namespace ImGui
{
    public partial class GUILayout
    {
        /// <summary>
        /// multi-line text box
        /// </summary>
        /// <param name="label"></param>
        /// <param name="size"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Textbox(string label, Size size, string text)
        {
            var g = GetCurrentContext();
            Window window = GetCurrentWindow();

            //apply skin and stack style modifiers
            var s = g.StyleStack;
            s.Apply(GUISkin.Instance[GUIControlName.Label]);

            int id = window.GetID(label);
            Rect rect = window.GetRect(id, size);
            var result = GUI.DoTextbox(rect, label, text);

            s.Restore();

            return result;
        }

        /// <summary>
        /// single-line text box
        /// </summary>
        /// <param name="label"></param>
        /// <param name="width"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Textbox(string label, double width, string text)
        {
            var g = GetCurrentContext();
            Window window = GetCurrentWindow();

            //apply skin and stack style modifiers
            var s = g.StyleStack;
            s.Apply(GUISkin.Instance[GUIControlName.TextBox]);

            int id = window.GetID(label);
            var height = s.Style.FontSize;
            var size = new Size(width, height);
            Rect rect = window.GetRect(id, size);
            var result = GUI.DoTextbox(rect, label, text);

            s.Restore();

            return result;
        }
    }

    public partial class GUI
    {
        public static string Textbox(Rect rect, string label, string text)
        {
            var g = GetCurrentContext();
            var window = GetCurrentWindow();

            //apply skin and stack style modifiers
            var s = g.StyleStack;
            s.Apply(GUISkin.Instance[GUIControlName.TextBox]);

            rect = window.GetRect(rect);
            var result = DoTextbox(rect, label, text);

            s.Restore();

            return result;
        }

        internal static string DoTextbox(Rect rect, string label, string text)
        {
            var g = Form.current.uiContext;
            var window = GetCurrentWindow();
            var id = window.GetID(label);

            var hovered = g.IsHovered(rect, id);
            // control logic
            var style = g.StyleStack.Style;
            var uiState = Form.current.uiContext;
            if (hovered)
            {
                uiState.SetHoverID(id);

                if (Mouse.Instance.LeftButtonPressed)
                {
                    uiState.SetActiveID(id);
                }
            }
            else
            {
                if (Mouse.Instance.LeftButtonPressed)
                {
                    if (g.ActiveId == id)
                    {
                        uiState.SetActiveID(0);
                    }
                }
            }

            if (g.ActiveId == id && g.InputTextState.inputTextContext.Id != id)//editing text box changed to TextBox<id>
            {
                g.InputTextState.stateMachine.CurrentState = "Active";//reset state
                g.InputTextState.inputTextContext = new InputTextContext()//reset input text context data
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

            InputTextContext context = null;
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

                if (hovered && Mouse.Instance.LeftButtonState == KeyState.Down && stateMachine.CurrentState != TextBoxState.ActiveSelecting)
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

            // ui painting
            {
                var d = window.DrawList;
                var contentRect = Utility.GetContentRect(rect, g.StyleStack.Style);
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

                    // Check if the caret is outside the rect. If so, move the text so the caret is always shown.
                    var caretX = caretTopPoint.X;
                    if (caretX < contentRect.X || caretX > contentRect.Right)
                    {
                        var offsetX = -(caretX - contentRect.Width - rect.X);
                        contentRect.Offset(offsetX, 0);
                        caretTopPoint.Offset(offsetX, 0);
                        caretBottomPoint.Offset(offsetX, 0);
                    }

                    //Draw the box //TODO draw selection line blocks
                    d.AddRect(rect.Min, rect.Max, Color.White);

                    //Draw text
                    d.DrawText(contentRect, context.Text, style, GUIState.Normal);

                    //Draw selection rect
                    if (context.SelectIndex != context.CaretIndex)
                    {
                        float selectPointX, selectPointY, dummyHeight;
                        textContext.IndexToXY(context.SelectIndex, false, out selectPointX, out selectPointY, out dummyHeight);
                        var selectionRect = new Rect(
                            new Point(pointX, pointY),
                            new Point(selectPointX, selectPointY + caretHeight));
                        selectionRect.Offset(offsetOfTextRect.X, offsetOfTextRect.Y);
                        d.AddRectFilled(selectionRect.Min, selectionRect.Max, Color.Argb(100, 10, 102, 214));
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

            return text;
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

    partial class GUISkin
    {
        void InitTextBoxStyles()
        {
            double fontSize = CurrentOS.IsAndroid ? 32.0 : 13.0;
            var textBoxStyles = new StyleModifier[] {
                new StyleModifier(GUIStyleName.PaddingLeft, StyleType.@double, 5.0, GUIState.Normal),
                new StyleModifier(GUIStyleName.PaddingLeft, StyleType.@double, 5.0, GUIState.Hover),
                new StyleModifier(GUIStyleName.PaddingLeft, StyleType.@double, 5.0, GUIState.Active),
                new StyleModifier(GUIStyleName.PaddingTop, StyleType.@double, 5.0, GUIState.Normal),
                new StyleModifier(GUIStyleName.PaddingTop, StyleType.@double, 5.0, GUIState.Hover),
                new StyleModifier(GUIStyleName.PaddingTop, StyleType.@double, 5.0, GUIState.Active),
                new StyleModifier(GUIStyleName.PaddingRight, StyleType.@double, 5.0, GUIState.Normal),
                new StyleModifier(GUIStyleName.PaddingRight, StyleType.@double, 5.0, GUIState.Hover),
                new StyleModifier(GUIStyleName.PaddingRight, StyleType.@double, 5.0, GUIState.Active),
                new StyleModifier(GUIStyleName.PaddingBottom, StyleType.@double, 5.0, GUIState.Normal),
                new StyleModifier(GUIStyleName.PaddingBottom, StyleType.@double, 5.0, GUIState.Hover),
                new StyleModifier(GUIStyleName.PaddingBottom, StyleType.@double, 5.0, GUIState.Active),
                new StyleModifier(GUIStyleName.FontSize, StyleType.@double, fontSize, GUIState.Normal),
                new StyleModifier(GUIStyleName.FontSize, StyleType.@double, fontSize, GUIState.Hover),
                new StyleModifier(GUIStyleName.FontSize, StyleType.@double,  fontSize, GUIState.Active),
            };
            this.styles.Add(GUIControlName.TextBox, textBoxStyles);
        }
    }
}
