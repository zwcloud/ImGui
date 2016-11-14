namespace ImGui
{
    internal class DummyBox : IRenderBox
    {
        #region Implementation of IRenderBox

        public bool NeedRepaint
        {
            get { return false; }
            set {  }
        }

        public Rect Rect
        {
            get { return Rect.Empty; }
            set { }
        }

        public Content Content
        {
            get { return Content.None; }
            set { }
        }

        public Style Style
        {
            get { return Style.None; }
            set { }
        }

        public RenderBoxType Type { get { return RenderBoxType.Dummy; } }

        public bool Active { get; set; }

        #endregion
    }
}