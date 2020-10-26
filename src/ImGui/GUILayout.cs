using System;

namespace ImGui
{
    public struct GUILayoutScope : IDisposable
    {
        private readonly bool isVertical;

        public GUILayoutScope(bool isVertical)
        {
            this.isVertical = isVertical;
        }

        public void Dispose()
        {
            if (this.isVertical)
            {
                GUILayout.EndVertical();
            }
            else
            {
                GUILayout.EndHorizontal();
            }
        }
    }

    /// <summary>
    /// The interface for GUI with automatic layout.
    /// </summary>
    public partial class GUILayout
    {

        #region ID

        public static void PushID(int id)
        {
            Window window = GetCurrentWindow();
            window.IDStack.Push(id);//we don't re-hash pushed raw id
        }

        public static void PushID(string str_id)
        {
            Window window = GetCurrentWindow();
            window.IDStack.Push(window.GetID(str_id));
        }

        public static int PopID()
        {
            Window window = GetCurrentWindow();
            return window.IDStack.Pop();
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

        /// <summary>
        /// Is last simple control being hovered?
        /// Note this won't work for composite controls like ListBox.
        /// </summary>
        internal static bool IsItemHovered()
        {
            Window window = GetCurrentWindow();
            if (window.IDStack.Count == 0)
            {
                return false;
            }

            return window.TempData.LastItemState == GUIState.Hover;
        }

        #region stack-layout

        public static GUILayoutScope HScope(string str_id)
        {
            BeginHorizontal(str_id);
            return new GUILayoutScope(false);
        }

        public static GUILayoutScope HScope(string str_id, LayoutOptions? options)
        {
            BeginHorizontal(str_id, options);
            return new GUILayoutScope(false);
        }

        public static GUILayoutScope VScope(string str_id)
        {
            BeginVertical(str_id);
            return new GUILayoutScope(true);
        }

        public static GUILayoutScope VScope(string str_id, LayoutOptions? options)
        {
            BeginVertical(str_id, options);
            return new GUILayoutScope(true);
        }

        public static void BeginHorizontal(string str_id)
        {
            BeginHorizontal(str_id, ExpandWidth(true));
        }

        public static void BeginHorizontal(string str_id, LayoutOptions? options)
        {
            Window window = GetCurrentWindow();
            int id = window.GetID(str_id);
            PushID(id);
            window.RenderTree.BeginLayoutGroup(id, false, options, str_id);
        }

        public static void EndHorizontal()
        {
            Window window = GetCurrentWindow();
            window.RenderTree.EndLayoutGroup();
            PopID();
        }

        public static void BeginVertical(string str_id)
        {
            BeginVertical(str_id, ExpandWidth(true));
        }

        public static void BeginVertical(string str_id, LayoutOptions? options)
        {
            Window window = GetCurrentWindow();

            int id = window.GetID(str_id);
            PushID(id);

            window.RenderTree.BeginLayoutGroup(id, true, options, str_id);
        }

        public static void EndVertical()
        {
            Window window = GetCurrentWindow();

            window.RenderTree.EndLayoutGroup();
            PopID();
        }

        #endregion

        #region layout option short-cuts

        /// <summary>
        /// Set the width of a control.
        /// </summary>
        /// <param name="width">width value</param>
        /// <returns>A <see cref="LayoutOptions"/> that will set the width of a control/group.</returns>
        public static LayoutOptions Width(double width) => new LayoutOptions().Width(width);

        /// <summary>
        /// Set the height of a control.
        /// </summary>
        /// <param name="height">height value</param>
        /// <returns>A <see cref="LayoutOptions"/> that will set the height of a control/group.</returns>
        public static LayoutOptions Height(double height) => new LayoutOptions().Height(height);

        /// <summary>
        /// Set whether the width of a control should be expanded to occupy as much space as possible.
        /// </summary>
        /// <param name="expand">expanded?</param>
        /// <returns>A <see cref="LayoutOptions"/> that will expand the width of a control/group.</returns>
        public static LayoutOptions ExpandWidth(bool expand) => new LayoutOptions().ExpandWidth(expand);

        /// <summary>
        /// Set whether the height of a control should be expanded to occupy as much space as possible.
        /// </summary>
        /// <param name="expand">expanded?</param>
        /// <returns>A <see cref="LayoutOptions"/> that will expand the height of a control/group.</returns>
        public static LayoutOptions ExpandHeight(bool expand) => new LayoutOptions().ExpandHeight(expand);

        /// <summary>
        /// Set the factor when expanding the width of a control.
        /// </summary>
        /// <param name="factor">the value of the factor</param>
        /// <returns>A <see cref="LayoutOptions"/> that will set the factor when expanding the width of a control/group.</returns>
        public static LayoutOptions StretchWidth(int factor) => new LayoutOptions().StretchWidth(factor);
        /// <summary>
        /// Set the factor when expanding the height of a control.
        /// </summary>
        /// <param name="factor">the value of the factor</param>
        /// <returns>A <see cref="LayoutOptions"/> that will set the factor when expanding the height of a control/group.</returns>
        public static LayoutOptions StretchHeight(int factor) => new LayoutOptions().StretchHeight(factor);

        #endregion
    }
}