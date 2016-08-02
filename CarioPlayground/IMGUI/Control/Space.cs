namespace ImGui
{
    internal class Space : IRenderBox
    {
        #region Implementation of IRenderBox

        public bool NeedRepaint
        {
            get { return false; }
            set { }
        }

        public bool NeedLayout { get; set; }

        public Rect Rect { get; set; }

        public Content Content
        {
            get { return null; }
        }

        public Style Style
        {
            get { return null; }
        }

        public RenderBoxType Type
        {
            get { return RenderBoxType.Space; }
        }

        #endregion
    }
}