namespace IMGUI
{
    internal class Border : IRenderable
    {
        #region Implementation of IRenderable

        public bool NeedRepaint
        {
            get; set;
        }

        public void OnRender(Cairo.Context g)
        {

        }

        public void OnClear(Cairo.Context g)
        {

        }

        #endregion

        public void DoControl(string name, Rect rect)
        {

            
        }
    }
}