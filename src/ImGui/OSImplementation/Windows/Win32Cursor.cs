using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ImGui.Input;

namespace ImGui.OSImplementation.Windows
{
    internal class Win32Cursor
    {
        #region Native

        [DllImport("user32.dll")]
        private static extern IntPtr SetCursor(IntPtr hCursor);

        [DllImport("user32.dll")]
        private static extern IntPtr GetCursor();

        [DllImport("user32.dll")]
        private static extern IntPtr LoadCursor(IntPtr hInstance, uint lpCursorName);

        [DllImport("user32.dll")]
        private static extern Int32 SystemParametersInfo(UInt32 uiAction,
            UInt32 uiParam, String pvParam, UInt32 fWinIni);

        private enum IDC_STANDARD_CURSORS
        {
            IDC_ARROW = 32512,
            IDC_IBEAM = 32513,
            IDC_WAIT = 32514,
            IDC_CROSS = 32515,
            IDC_UPARROW = 32516,
            IDC_SIZE = 32640,
            IDC_ICON = 32641,
            IDC_SIZENWSE = 32642,
            IDC_SIZENESW = 32643,
            IDC_SIZEWE = 32644,
            IDC_SIZENS = 32645,
            IDC_SIZEALL = 32646,
            IDC_NO = 32648,
            IDC_HAND = 32649,
            IDC_APPSTARTING = 32650,
            IDC_HELP = 32651
        }

        private static IntPtr NormalCursurHandle;
        private static IntPtr IBeamCursurHandle;
        private static IntPtr HorizontalResizeCursurHandle;
        private static IntPtr VerticalResizeCursurHandle;
        private static IntPtr MoveCursurHandle;
        private static IntPtr BottomRightCornerResizeCursorHandle;

        private static void LoadCursors()//called by windowcontext
        {
            NormalCursurHandle = LoadCursor(IntPtr.Zero, (uint)IDC_STANDARD_CURSORS.IDC_ARROW);
            IBeamCursurHandle = LoadCursor(IntPtr.Zero, (uint)IDC_STANDARD_CURSORS.IDC_IBEAM);
            HorizontalResizeCursurHandle = LoadCursor(IntPtr.Zero, (uint)IDC_STANDARD_CURSORS.IDC_SIZEWE);
            VerticalResizeCursurHandle = LoadCursor(IntPtr.Zero, (uint)IDC_STANDARD_CURSORS.IDC_SIZENS);
            MoveCursurHandle = LoadCursor(IntPtr.Zero, (uint)IDC_STANDARD_CURSORS.IDC_SIZEALL);
            BottomRightCornerResizeCursorHandle = LoadCursor(IntPtr.Zero, (uint)IDC_STANDARD_CURSORS.IDC_SIZENWSE);
        }

        private static void RevertCursors()
        {
            SystemParametersInfo(0x0057, 0, null, 0);
        }
        #endregion

        static Win32Cursor()
        {
            LoadCursors();
            s_cursorDictionary = new Dictionary<Cursor, IntPtr>
            {
                [Cursor.Default] = NormalCursurHandle,
                [Cursor.Text] = IBeamCursurHandle,
                [Cursor.EwResize] = HorizontalResizeCursurHandle,
                [Cursor.NsResize] = VerticalResizeCursurHandle,
                [Cursor.NeswResize] = MoveCursurHandle,
                [Cursor.NwseResize] = BottomRightCornerResizeCursorHandle,
                [Cursor.Default] = NormalCursurHandle,
            };
        }

        private static readonly Dictionary<Cursor, IntPtr> s_cursorDictionary;

        public static void ChangeCursor(Cursor cursor)
        {
            if (s_cursorDictionary.TryGetValue(cursor, out var cursorHandle))
            {
                if (GetCursor() == cursorHandle)
                {
                    return;
                }
                SetCursor(cursorHandle);
            }
            else
            {
                RevertCursors();
            }
        }
    }
}
