using System;
using System.Runtime.InteropServices;
using ImGui.Input;

namespace ImGui.OSImplementation.Web
{
    internal class WebCursor
    {
        private static string ToWebCursorName(Cursor cursor)
        {
            switch (cursor)
            {
                case Cursor.Default:
                    return "default";
                case Cursor.Text:
                    return "text";
                case Cursor.EwResize:
                    return "ew-resize";
                case Cursor.NsResize:
                    return "ns-resize";
                case Cursor.NeswResize:
                    return "nesw-resize";
                default:
                    throw new ArgumentOutOfRangeException(nameof(cursor), cursor, null);
            }
        }

        [DllImport("*")]
        private static extern void emscripten_run_script(string script);

        public static void ChangeCursor(Cursor cursor)
        {
            emscripten_run_script($"document.body.style.cursor = {ToWebCursorName(cursor)}");
        }
    }
}