namespace IMGUI
{
    public interface IWindow
    {


        #region position and size
        /// <summary>
        /// Position
        /// </summary>
        Point Position { get; set; }

        /// <summary>
        /// Size
        /// </summary>
        Size Size { get; set; }
        #endregion

        #region window loop
        void WindowProc(object Context);
        #endregion
    }
}