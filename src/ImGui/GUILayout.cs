using System;
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

        internal static Window GetCurrentWindow()
        {
            return Utility.GetCurrentWindow();
        }

        #region stack-layout

        public static void BeginHorizontal(string str_id, GUIStyle style = null)
        {
            BeginHorizontal(str_id, Size.Zero, style);
        }

        public static void BeginHorizontal(string str_id, Size size, GUIStyle style = null)
        {
            Window window = GetCurrentWindow();

            int id = window.GetID(str_id);
            PushHStretchFactor(1);
            PushID(id);
            window.StackLayout.BeginLayoutGroup(id, false, size, style);
            PopHStretchFactor();
        }

        public static void EndHorizontal()
        {
            Window window = GetCurrentWindow();

            window.StackLayout.EndLayoutGroup();
            PopID();
        }

        public static void BeginVertical(string str_id, GUIStyle style = null)
        {
            BeginVertical(str_id, Size.Zero, style);
        }

        public static void BeginVertical(string str_id, Size size, GUIStyle style = null)
        {
            Window window = GetCurrentWindow();

            int id = window.GetID(str_id);
            PushHStretchFactor(1);
            PushID(id);
            window.StackLayout.BeginLayoutGroup(id, true, size, style);
            PopHStretchFactor();
        }

        public static void EndVertical()
        {
            Window window = GetCurrentWindow();

            window.StackLayout.EndLayoutGroup();
            PopID();
        }

        public static Rect GetWindowClientRect()
        {
            Window window = GetCurrentWindow();
            return window.ClientRect;
        }

        // test only

        #endregion

        #region options

        /// <summary>
        /// Set the width of a control.
        /// </summary>
        /// <param name="width">width value</param>
        /// <returns>A <see cref="LayoutOption"/> that will set the width of a control/group.</returns>
        public static LayoutOption Width(double width)
        {
            return new LayoutOption(LayoutOptionType.FixedWidth, width);
        }

        /// <summary>
        /// Set the height of a control.
        /// </summary>
        /// <param name="height">height value</param>
        /// <returns>A <see cref="LayoutOption"/> that will set the height of a control/group.</returns>
        public static LayoutOption Height(double height)
        {
            return new LayoutOption(LayoutOptionType.FixedHeight, height);
        }

        /// <summary>
        /// Set whether the width of a control should be expanded to occupy as much space as possible.
        /// </summary>
        /// <param name="expand">expanded?</param>
        /// <returns>A <see cref="LayoutOption"/> that will expand the width of a control/group.</returns>
        public static LayoutOption ExpandWidth(bool expand)
        {
            return new LayoutOption(LayoutOptionType.StretchWidth, (!expand) ? 0 : 1);
        }

        /// <summary>
        /// Set whether the height of a control should be expanded to occupy as much space as possible.
        /// </summary>
        /// <param name="expand">expanded?</param>
        /// <returns>A <see cref="LayoutOption"/> that will expand the height of a control/group.</returns>
        public static LayoutOption ExpandHeight(bool expand)
        {
            return new LayoutOption(LayoutOptionType.StretchHeight, (!expand) ? 0 : 1);
        }

        /// <summary>
        /// Set the factor when expanding the width of a control.
        /// </summary>
        /// <param name="factor">the value of the factor</param>
        /// <returns>A <see cref="LayoutOption"/> that will set the factor when expanding the width of a control/group.</returns>
        public static LayoutOption StretchWidth(int factor)
        {
            if (factor <= 0) throw new ArgumentOutOfRangeException(nameof(factor), "The stretch factor must be positive.");
            return new LayoutOption(LayoutOptionType.StretchWidth, factor);
        }

        /// <summary>
        /// Set the factor when expanding the height of a control.
        /// </summary>
        /// <param name="factor">the value of the factor</param>
        /// <returns>A <see cref="LayoutOption"/> that will set the factor when expanding the height of a control/group.</returns>
        public static LayoutOption StretchHeight(int factor)
        {
            if (factor <= 0) throw new ArgumentOutOfRangeException(nameof(factor), "The stretch factor must be positive.");
            return new LayoutOption(LayoutOptionType.StretchHeight, factor);
        }

        #endregion

    }
}