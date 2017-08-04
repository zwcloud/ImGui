using System;
using System.Collections.Generic;
using System.Diagnostics;
using ImGui.Common.Primitive;

namespace ImGui
{
    /// <summary>
    /// The interface for GUI with automatic layout.
    /// </summary>
    public partial class GUILayout
    {
        #region ID

        public static void PushID(int int_id)
        {
            Window window = GetCurrentWindow();
            window.IDStack.Push(window.GetID(int_id));
        }

        public static void PushID(string str_id)
        {
            Window window = GetCurrentWindow();
            window.IDStack.Push(window.GetID(str_id));
        }

        public static void PopID()
        {
            Window window = GetCurrentWindow();
            window.IDStack.Pop();
        }

        #endregion

        private static Window GetCurrentWindow()
        {
            return Utility.GetCurrentWindow();
        }

        #region stack-layout

        public static void BeginHorizontal(string str_id, GUIStyle style = null, params LayoutOption[] options)
        {
            Window window = GetCurrentWindow();

            int id = window.GetID(str_id);
            window.StackLayout.BeginLayoutGroup(id, false, style, options);
        }

        public static void EndHorizontal()
        {
            Window window = GetCurrentWindow();

            window.StackLayout.EndLayoutGroup();
        }

        public static void BeginVertical(string str_id, GUIStyle style = null, params LayoutOption[] options)
        {
            Window window = GetCurrentWindow();

            int id = window.GetID(str_id);
            window.StackLayout.BeginLayoutGroup(id, true, style, null);
        }

        public static void EndVertical()
        {
            Window window = GetCurrentWindow();

            window.StackLayout.EndLayoutGroup();
        }

        public static Rect GetWindowClientRect()
        {
            Window window = GetCurrentWindow();
            return window.ClientRect;
        }

        public static Rect GetRect(Size size, string str_id, GUIStyle style = null, params LayoutOption[] options)
        {
            Window window = GetCurrentWindow();
            var id = window.GetID(str_id);
            var rect = window.GetRect(id, size, style, options);
            return rect;
        }

        #endregion

        #region options

        /// <summary>
        /// Set the width of a control.
        /// </summary>
        /// <param name="width">width value</param>
        /// <returns>A <see cref="LayoutOption"/> that will set the width of a control/group.</returns>
        public static LayoutOption Width(double width)
        {
            return new LayoutOption(LayoutOption.Type.fixedWidth, width);
        }

        /// <summary>
        /// Set the height of a control.
        /// </summary>
        /// <param name="height">height value</param>
        /// <returns>A <see cref="LayoutOption"/> that will set the height of a control/group.</returns>
        public static LayoutOption Height(double height)
        {
            return new LayoutOption(LayoutOption.Type.fixedHeight, height);
        }

        /// <summary>
        /// Set whether the width of a control should be expanded to occupy as much space as possible.
        /// </summary>
        /// <param name="expand">expanded?</param>
        /// <returns>A <see cref="LayoutOption"/> that will expand the width of a control/group.</returns>
        public static LayoutOption ExpandWidth(bool expand)
        {
            return new LayoutOption(LayoutOption.Type.stretchWidth, (!expand) ? 0 : 1);
        }

        /// <summary>
        /// Set whether the height of a control should be expanded to occupy as much space as possible.
        /// </summary>
        /// <param name="expand">expanded?</param>
        /// <returns>A <see cref="LayoutOption"/> that will expand the height of a control/group.</returns>
        public static LayoutOption ExpandHeight(bool expand)
        {
            return new LayoutOption(LayoutOption.Type.stretchHeight, (!expand) ? 0 : 1);
        }

        /// <summary>
        /// Set the factor when expanding the width of a control.
        /// </summary>
        /// <param name="factor">the value of the factor</param>
        /// <returns>A <see cref="LayoutOption"/> that will set the factor when expanding the width of a control/group.</returns>
        public static LayoutOption StretchWidth(int factor)
        {
            if (factor <= 0) throw new ArgumentOutOfRangeException(nameof(factor), "The stretch factor must be positive.");
            return new LayoutOption(LayoutOption.Type.stretchWidth, factor);
        }

        /// <summary>
        /// Set the factor when expanding the height of a control.
        /// </summary>
        /// <param name="factor">the value of the factor</param>
        /// <returns>A <see cref="LayoutOption"/> that will set the factor when expanding the height of a control/group.</returns>
        public static LayoutOption StretchHeight(int factor)
        {
            if (factor <= 0) throw new ArgumentOutOfRangeException(nameof(factor), "The stretch factor must be positive.");
            return new LayoutOption(LayoutOption.Type.stretchHeight, factor);
        }

        #endregion

        #region controls

        public static bool CollapsingHeader(string text, ref bool open)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            var height = GUIStyle.Default.FontSize;
            var id = window.GetID(text);
            GUIStyle style = GUISkin.Instance[GUIControlName.Button];
            var rect = GetRect(new Size(0, height), text, style, GUILayout.ExpandWidth(true));

            bool hovered, held;
            bool pressed = GUIBehavior.ButtonBehavior(rect, id, out hovered, out held, ButtonFlags.PressedOnClick);
            if (pressed)
            {
                open = !open;
            }

            // Render
            DrawList d = window.DrawList;
            var state = (hovered && held) ? GUIState.Active : hovered ? GUIState.Hover : GUIState.Normal;
            Color col = style.Get<Color>(GUIStyleName.BackgroundColor, state);
            d.RenderFrame(rect.Min, rect.Max, col, false, 0);
            d.DrawText(rect, text, style, state);

            return open;
        }

        public static bool Radio(string label, ref string active_id, string id)
        {
            return false;
            throw new NotImplementedException();//TODO implement this with separate logic from Toggle.
        }

        #endregion

    }
}