//#define INSPECT_STATE
using System;
using System.Diagnostics;
using Key = ImGui.Key;

namespace ImGui
{
    public partial class GUI
    {
        public static string Textbox(Rect rect, string label, string text)
        {
            var window = GetCurrentWindow();
            rect = window.GetRect(rect);
            return DoTextbox(rect, label, text);
        }

        private static string DoTextbox(Rect rect, string label, string text)
        {
            var g = Form.current.uiContext;
            var window = GetCurrentWindow();
            var id = window.GetID(label);

            // control logic
            var style = GUISkin.Instance[GUIControlName.TextBox];
            //if (id != g.ActiveId)//not editing
            //{
            //    var d = window.DrawList;
            //    d.DrawText(rect, text, style, GUIState.Normal);
            //    d.AddRect(rect.Min, rect.Max, Color.White);
            //    return text;
            //}

            //editing
            if (g.InputTextState.inputTextContext.Id != id)//editing text box changed
            {
                g.InputTextState.stateMachine.CurrentState = "Normal";//reset state
                g.InputTextState.inputTextContext = new InputTextContext()//set input text context data
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

            var stateMachine = g.InputTextState.stateMachine;
            var context = g.InputTextState.inputTextContext;
            {
#if INSPECT_STATE
            var A = stateMachine.CurrentState;
#endif
                bool insideRectLast = rect.Contains(Input.Mouse.LastMousePos);
                bool insideRectCurrent = rect.Contains(Input.Mouse.MousePos);
                bool insideRect = insideRectCurrent;

                //Execute state commands
                if (!insideRectLast && insideRectCurrent)
                {
                    stateMachine.MoveNext(TextBoxCommand.MoveIn, context);
                }
                if (insideRectLast && !insideRectCurrent)
                {
                    stateMachine.MoveNext(TextBoxCommand.MoveOut, context);
                }
                if (insideRectCurrent)
                {
                    if (Input.Mouse.LeftButtonPressed)
                    {
                        stateMachine.MoveNext(TextBoxCommand.EnterEdit, context);
                        stateMachine.MoveNext(TextBoxCommand.MoveCaret, context);
                    }
                    else
                    {
                        if (Input.Mouse.LeftButtonState == InputState.Down && stateMachine.CurrentState != TextBoxState.ActiveSelecting)
                        {
                            stateMachine.MoveNext(TextBoxCommand.MoveCaret, context);
                            stateMachine.MoveNext(TextBoxCommand.BeginSelect, context);
                        }
                        if (Input.Mouse.LeftButtonState == InputState.Up)
                        {
                            stateMachine.MoveNext(TextBoxCommand.EndSelect, context);
                        }
                    }
                }
                else
                {
                    if (Input.Mouse.LeftButtonPressed)
                    {
                        stateMachine.MoveNext(TextBoxCommand.ExitEditOut, context);
                    }
                }

#if INSPECT_STATE
            var B = stateMachine.CurrentState;
            Debug.WriteLineIf(A != B,
                string.Format("TextBox<{0}> {1}=>{2} CaretIndex: {3}, SelectIndex: {4}",
                    id, A, B, context.CaretIndex, context.SelectIndex));
#endif

                Input.Mouse.Cursor = insideRect ? Cursor.Text : Cursor.Default;
                if (stateMachine.CurrentState == TextBoxState.Active)
                {
                    stateMachine.MoveNext(TextBoxCommand.DoEdit, context);
                }
                if (stateMachine.CurrentState == TextBoxState.ActiveSelecting)
                {
                    stateMachine.MoveNext(TextBoxCommand.DoSelect, context);
                }
                stateMachine.MoveNext(TextBoxCommand.MoveCaretKeyboard, context);
            }
            bool active = stateMachine.CurrentState.StartsWith("Active");
            if (active)
            {
                g.SetActiveID(id);
            }

            // ui painting
            {
                var d = window.DrawList;
                var contentRect = Utility.GetContentRect(rect, style);
                if (active)
                {
                    //Calculate positions and sizes
                    var textContext = TextMeshUtil.GetTextContext(text, rect.Size, style, GUIState.Normal);
                    var offsetOfTextRect = contentRect.TopLeft;
                    float pointX, pointY;
                    float caretHeight;
                    textContext.IndexToXY(context.CaretIndex, false, out pointX, out pointY, out caretHeight);
                    var caretTopPoint = new Point(pointX, pointY);
                    var caretBottomPoint = new Point(pointX, pointY + caretHeight);
                    caretTopPoint.Offset(offsetOfTextRect.X, offsetOfTextRect.Y);
                    caretBottomPoint.Offset(offsetOfTextRect.X, offsetOfTextRect.Y);

                    var caretAlpha = (byte)(Application.Time % 1060 / 1060.0f * 255);
                    caretAlpha = (byte)(caretAlpha < 100 ? 0 : 255);

                    //FIXME: This is not working! Check if the caret is outside the rect. If so, move the text rect so the caret is shown.
                    var textRect = contentRect;
                    var caretX = caretTopPoint.X;
                    if (caretX < textRect.X || caretX > textRect.Right)
                    {
                        var offsetX = -(caretX - textRect.Width - rect.X);
                        textRect.Offset(offsetX, 0);
                        caretTopPoint.Offset(offsetX, 0);
                        caretBottomPoint.Offset(offsetX, 0);
                    }

                    //Draw the box
                    d.AddRect(rect.Min, rect.Max, Color.White);

                    //TODO Clip the text
                    //d.PathMoveTo(rect.TopLeft);
                    //d.PathLineTo(rect.TopRight);
                    //d.PathLineTo(rect.BottomRight);
                    //d.PathLineTo(rect.BottomLeft);
                    //d.PathLineTo(rect.TopLeft);
                    //d.Clip();
                    //d.ResetClip();

                    //Draw text
                    d.DrawText(textRect, text, style, GUIState.Normal);

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
            }

            return context.Text;
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
