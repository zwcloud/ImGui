//#define INSPECT_STATE
using System;
using System.Diagnostics;
using Key = ImGui.Key;

namespace ImGui
{
    public partial class GUILayout
    {
        public static string Textbox(string label, Size size, string text, params LayoutOption[] options)
        {
            return Textbox(label, size, text, GUISkin.Instance[GUIControlName.TextBox], options);
        }
        public static string Textbox(string label, Size size, string text, GUIStyle style, params LayoutOption[] options)
        {
            Window window = GetCurrentWindow();

            int id = window.GetID(label);
            Rect rect = window.GetRect(id, size, style, options);
            return GUI.DoTextbox(rect, label, text);
        }
    }

    public partial class GUI
    {
        public static string Textbox(Rect rect, string label, string text)
        {
            var window = GetCurrentWindow();
            rect = window.GetRect(rect);
            return DoTextbox(rect, label, text);
        }

        internal static string DoTextbox(Rect rect, string label, string text)
        {
            var g = Form.current.uiContext;
            var window = GetCurrentWindow();
            var id = window.GetID(label);

            var hovered = g.IsHovered(rect, id);
            // control logic
            var style = GUISkin.Instance[GUIControlName.TextBox];
            var uiState = Form.current.uiContext;
            if (hovered)
            {
                uiState.SetHoverID(id);

                if (Input.Mouse.LeftButtonPressed)
                {
                    uiState.SetActiveID(id);
                }
            }
            else
            {
                if (Input.Mouse.LeftButtonPressed)
                {
                    if (g.ActiveId == id)
                    {
                        uiState.SetActiveID(0);
                    }
                }
            }

            if (g.ActiveId == id && g.InputTextState.inputTextContext.Id != id)//editing text box changed
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
                    Style = style,
                    Text = text
                };
            }

            StateMachineEx stateMachine = null;
            InputTextContext context = null;
            if (g.ActiveId == id)
            {
                stateMachine = g.InputTextState.stateMachine;
                context = g.InputTextState.inputTextContext;
                context.Rect = rect;
                context.Style = style;

                // Although we are active we don't prevent mouse from hovering other elements unless we are interacting right now with the widget.
                // Down the line we should have a cleaner library-wide concept of Selected vs Active.
                g.ActiveIdAllowOverlap = !Input.Mouse.LeftButtonPressed;

#if INSPECT_STATE
            var A = stateMachine.CurrentState;
#endif
                if (Input.Mouse.LeftButtonPressed)
                {
                    stateMachine.MoveNext(TextBoxCommand.MoveCaret, context);
                }

                if (hovered && Input.Mouse.LeftButtonState == InputState.Down && stateMachine.CurrentState != TextBoxState.ActiveSelecting)
                {
                    stateMachine.MoveNext(TextBoxCommand.MoveCaret, context);
                    stateMachine.MoveNext(TextBoxCommand.BeginSelect, context);
                }

                if (hovered && Input.Mouse.LeftButtonState == InputState.Up)
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

            if (g.ActiveId == id)
            {
                TextBoxDebug.CaretIndex = context.CaretIndex;
                //Debug.WriteLine(stateMachine.CurrentState);
            }

            return text;
        }
    }

    public static class TextBoxDebug
    {
        public static uint CaretIndex;
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
