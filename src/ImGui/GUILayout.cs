using System;
using ImGui.Common.Primitive;
using ImGui.Layout;

namespace ImGui
{
    public struct GUILayoutScope : IDisposable
    {
        public void Dispose()
        {
            GUILayout.EndHorizontal();
        }
    }

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

        internal static GUIContext GetCurrentContext()
        {
            return Form.current.uiContext;
        }

        internal static Window GetCurrentWindow()
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;
            Window window = g.WindowManager.CurrentWindow;
            window.Accessed = true;
            return window;
        }

        #region stack-layout

        public static GUILayoutScope HScope(string str_id)
        {
            BeginHorizontal(str_id);
            return new GUILayoutScope();
        }

        public static GUILayoutScope VScope(string str_id)
        {
            BeginVertical(str_id);
            return new GUILayoutScope();
        }

        public static void BeginHorizontal(string str_id)
        {
            BeginHorizontal(str_id, Size.Zero, ExpandWidth(true));
        }

        public static void BeginHorizontal(string str_id, LayoutOptions? options = null)
        {
            BeginHorizontal(str_id, Size.Zero, options);
        }

        public static void BeginHorizontal(string str_id, Size size, LayoutOptions? options = null)
        {
            Window window = GetCurrentWindow();
            var styleStack = Form.current.uiContext.StyleStack;

            int id = window.GetID(str_id);
            PushID(id);

            window.StackLayout.BeginLayoutGroup(id, false, size, options);
        }

        public static void EndHorizontal()
        {
            Window window = GetCurrentWindow();

            window.StackLayout.EndLayoutGroup();
            PopID();
        }

        public static void BeginVertical(string str_id)
        {
            BeginVertical(str_id, Size.Zero, ExpandWidth(true));
        }

        public static void BeginVertical(string str_id, LayoutOptions? options = null)
        {
            BeginVertical(str_id, Size.Zero, options);
        }

        public static void BeginVertical(string str_id, Size size, LayoutOptions? options)
        {
            Window window = GetCurrentWindow();
            var styleStack = Form.current.uiContext.StyleStack;

            int id = window.GetID(str_id);
            PushID(id);

            window.StackLayout.BeginLayoutGroup(id, true, size, options);
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

        #endregion

        #region layout option short-cuts

        /// <summary>
        /// Set the width of a control.
        /// </summary>
        /// <param name="width">width value</param>
        /// <returns>A <see cref="LayoutOption"/> that will set the width of a control/group.</returns>
        public static LayoutOptions Width(int width) => new LayoutOptions().Width(width);

        /// <summary>
        /// Set the height of a control.
        /// </summary>
        /// <param name="height">height value</param>
        /// <returns>A <see cref="LayoutOption"/> that will set the height of a control/group.</returns>
        public static LayoutOptions Height(int height) => new LayoutOptions().Height(height);

        /// <summary>
        /// Set whether the width of a control should be expanded to occupy as much space as possible.
        /// </summary>
        /// <param name="expand">expanded?</param>
        /// <returns>A <see cref="LayoutOption"/> that will expand the width of a control/group.</returns>
        public static LayoutOptions ExpandWidth(bool expand) => new LayoutOptions().ExpandWidth(expand);

        /// <summary>
        /// Set whether the height of a control should be expanded to occupy as much space as possible.
        /// </summary>
        /// <param name="expand">expanded?</param>
        /// <returns>A <see cref="LayoutOption"/> that will expand the height of a control/group.</returns>
        public static LayoutOptions ExpandHeight(bool expand) => new LayoutOptions().ExpandHeight(expand);

        /// <summary>
        /// Set the factor when expanding the width of a control.
        /// </summary>
        /// <param name="factor">the value of the factor</param>
        /// <returns>A <see cref="LayoutOption"/> that will set the factor when expanding the width of a control/group.</returns>
        public static LayoutOptions StretchWidth(int factor) => new LayoutOptions().StretchWidth(factor);
        /// <summary>
        /// Set the factor when expanding the height of a control.
        /// </summary>
        /// <param name="factor">the value of the factor</param>
        /// <returns>A <see cref="LayoutOption"/> that will set the factor when expanding the height of a control/group.</returns>
        public static LayoutOptions StretchHeight(int factor) => new LayoutOptions().StretchHeight(factor);

        #endregion
    }
}