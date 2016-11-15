namespace ImGui
{
    internal abstract class SimpleControl : IRenderBox
    {
        public Form Form { get; set; }

        public virtual string Name { get { return ""; } }

        public string State { get; set; }

        #region IRenderBox Members

        /// <summary>
        /// If need repaint, repaint
        /// </summary>
        public bool NeedRepaint { get; set; }

        public abstract Rect Rect { get; set; }

        public Rect ContentRect
        {
            get
            {
                return new Rect(
                    Rect.X + Style.PaddingLeft+Style.BorderLeft,
                    Rect.X + Style.PaddingTop+Style.BorderTop,
                    Rect.Width - Style.PaddingHorizontal - Style.BorderHorizontal,
                    Rect.Height - Style.PaddingVertical- Style.BorderVertical);
            }
        }

        public abstract Content Content { get; }

        public abstract Style Style { get; }

        public RenderBoxType Type { get { return RenderBoxType.SimpleControl; } }

        public bool Active { get; set; }

        #endregion
    }
}