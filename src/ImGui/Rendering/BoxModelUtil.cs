using System.Diagnostics;

namespace ImGui.Rendering
{
    internal static class BoxModelUtil
    {
        public static void GetBoxes(Rect rect, StyleRuleSet style, out Rect borderBoxRect, out Rect paddingBoxRect,
            out Rect contentBoxRect)
        {
            //Widths of border
            var bt = style.Get<double>(StylePropertyName.BorderTop);
            var br = style.Get<double>(StylePropertyName.BorderRight);
            var bb = style.Get<double>(StylePropertyName.BorderBottom);
            var bl = style.Get<double>(StylePropertyName.BorderLeft);

            //Widths of padding
            var pt = style.Get<double>(StylePropertyName.PaddingTop);
            var pr = style.Get<double>(StylePropertyName.PaddingRight);
            var pb = style.Get<double>(StylePropertyName.PaddingBottom);
            var pl = style.Get<double>(StylePropertyName.PaddingLeft);

            //4 corner of the border-box
            var btl = new Point(rect.Left, rect.Top);
            var btr = new Point(rect.Right, rect.Top);
            var bbr = new Point(rect.Right, rect.Bottom);
            var bbl = new Point(rect.Left, rect.Bottom);
            borderBoxRect = new Rect(btl, bbr);

            //4 corner of the padding-box
            var ptl = new Point(btl.X + bl, btl.Y + bt);
            var ptr = new Point(btr.X - br, btr.Y + bt);
            var pbr = new Point(bbr.X - br, bbr.Y - bb);
            var pbl = new Point(bbl.X + bl, bbl.Y - bb);
            Debug.Assert(ptl.X < ptr.X);//TODO what if (ptl.X > ptr.X) happens?
            paddingBoxRect = new Rect(ptl, pbr);

            //4 corner of the content-box
            var ctl = new Point(ptl.X + pl, ptl.Y + pt);
            var ctr = new Point(ptr.X - pr, ptr.Y + pr);
            var cbr = new Point(pbr.X - pr, pbr.Y - pb);
            var cbl = new Point(pbl.X + pl, pbl.Y - pb);
            if (ctl.X >= ctr.X)
            {
                Log.Warning("Content box is zero-sized.");
                contentBoxRect = new Rect(ctl, Size.Zero);
            }
            else
            {
                contentBoxRect = new Rect(ctl, cbr);
            }
        }
    }
}