using System;
using ImGui.Input;
using WebAssembly;

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

        public static void ChangeCursor(Cursor cursor)
        {
            //document.body.style.cursor = "text";
            var document = Runtime.GetGlobalObject("document") as JSObject;
            var body = document.GetObjectProperty("body") as JSObject;
            var bodyStyle = body.GetObjectProperty("style") as JSObject;
            bodyStyle.SetObjectProperty("cursor", ToWebCursorName(cursor));
        }
    }
}