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

        internal static GUIContext GetCurrentContext()
        {
            return Form.current.uiContext;
        }

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
    }
}