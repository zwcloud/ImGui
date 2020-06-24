using System;
using System.Diagnostics;

namespace ImGui.Style
{
    internal static class StyleRuleSetEx
    {
        public static Size CalcContentBoxSize(this StyleRuleSet ruleSet, string text, GUIState state)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            //Note text height shouldn't be set to zero but to the line height,
            //since it determines the height of inline controls.

            // apply font and text styles
            var measureContext = TextMeshUtil.GetTextContext(text, new Size(4096, 4096), ruleSet, state);
            var actualSize = measureContext.Measure();
            double width = actualSize.Width;
            double height = actualSize.Height;
            if (width < 0)
            {
                width = 0;
            }
            if (height < 0)
            {
                height = 0;
            }

            var size = new Size(Math.Ceiling(width), Math.Ceiling(height));
            return size;
        }
        
        public static Rect CalcContentBoxRect(this StyleRuleSet style, Rect borderBox)
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
            var btl = new Point(borderBox.Left, borderBox.Top);
            var btr = new Point(borderBox.Right, borderBox.Top);
            var bbr = new Point(borderBox.Right, borderBox.Bottom);
            var bbl = new Point(borderBox.Left, borderBox.Bottom);

            //4 corner of the padding-box
            var ptl = new Point(btl.X + bl, btl.Y + bt);
            var ptr = new Point(btr.X - br, btr.Y + bt);
            var pbr = new Point(bbr.X - br, bbr.Y - bb);
            var pbl = new Point(bbl.X + bl, bbl.Y - bb);
            Debug.Assert(ptl.X < ptr.X);//TODO what if (ptl.X > ptr.X) happens?

            //4 corner of the content-box
            var ctl = new Point(ptl.X + pl, ptl.Y + pt);
            var ctr = new Point(ptr.X - pr, ptr.Y + pr);
            var cbr = new Point(pbr.X - pr, pbr.Y - pb);
            var cbl = new Point(pbl.X + pl, pbl.Y - pb);
            if (ctl.X >= ctr.X)
            {
                Log.Warning("Content box is zero-sized.");
                return new Rect(ctl, Size.Zero);
            }

            return new Rect(ctl, cbr);
        }

        public static Size CalcContentBoxSize(this StyleRuleSet style, Size borderBoxSize)
        {
            //Widths of border
            var bt = style.Get<double>(StylePropertyName.BorderTop);
            var br = style.Get<double>(StylePropertyName.BorderRight);
            var bb = style.Get<double>(StylePropertyName.BorderBottom);
            var bl = style.Get<double>(StylePropertyName.BorderLeft);
            var borderWidth = br + bl;
            var borderHeight = bb + bt;

            //Widths of padding
            var pt = style.Get<double>(StylePropertyName.PaddingTop);
            var pr = style.Get<double>(StylePropertyName.PaddingRight);
            var pb = style.Get<double>(StylePropertyName.PaddingBottom);
            var pl = style.Get<double>(StylePropertyName.PaddingLeft);
            var paddingWidth = pr + pl;
            var paddingHeight = pb + pt;

            Size contentBoxSize = new Size(
                borderBoxSize.Width - borderWidth - paddingWidth,
                borderBoxSize.Height - borderHeight - paddingHeight);

            return contentBoxSize;
        }
    }
    
}
