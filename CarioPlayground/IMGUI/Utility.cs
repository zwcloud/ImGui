using System;
using System.Runtime.InteropServices;
using System.Text;
using Win32;

namespace IMGUI
{
    static class Utility
    {
        public static bool IsApplicationIdle()
        {
            NativeMessage result;
            return Native.PeekMessage(out result, IntPtr.Zero, (uint)0, (uint)0, (uint)0) == 0;
        }

        //http://stackoverflow.com/a/15051945/3427520
        private static readonly DateTime Jan1St1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        /// <summary>Get extra long current timestamp</summary>
        public static long Millis { get { return (long)((DateTime.UtcNow - Jan1St1970).TotalMilliseconds); } }

        //TODO use this in all logic instead of Millis
        /// <summary>
        /// Beginning time of this frame
        /// </summary>
        public static long MillisFrameBegin { internal set; get; }

        /// <summary>
        /// Get rect of the context box
        /// </summary>
        /// <param name="rect">rect of the entire box</param>
        /// <param name="style">style</param>
        /// <returns>rect of the context box</returns>
        public static Rect GetContentRect(Rect rect, Style style)
        {
            //Widths of border
            var bt = style.BorderTop;
            var br = style.BorderRight;
            var bb = style.BorderBottom;
            var bl = style.BorderLeft;

            //Widths of padding
            var pt = style.PaddingTop;
            var pr = style.PaddingRight;
            var pb = style.PaddingBottom;
            var pl = style.PaddingLeft;

            //4 corner of the border-box
            var btl = new Point(rect.Left, rect.Top);
            var bbr = new Point(rect.Right, rect.Bottom);

            //4 corner of the padding-box
            var ptl = new Point(btl.X + bl, btl.Y + bt);
            var pbr = new Point(bbr.X - br, bbr.Y - bb);

            //4 corner of the content-box
            var ctl = new Point(ptl.X + pl, ptl.Y + pt);
            var cbr = new Point(pbr.X - pr, pbr.Y - pb);
            var contentBoxRect = new Rect(ctl, cbr);
            return contentBoxRect;
        }

        internal static System.Windows.Forms.Cursor GetFormCursor(Cursor cursor)
        {
            switch (cursor)
            {
                case Cursor.Default:
                    return System.Windows.Forms.Cursors.Default;
                case Cursor.Text:
                    return System.Windows.Forms.Cursors.IBeam;
                default:
                    throw new ArgumentOutOfRangeException("cursor", cursor, null);
            }
        }

        internal static byte[] PngHeaderEightBytes =
        {
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A
        };

        internal static string SvgFileFirstLineTextPrefix = "<?xml";

    }
}
