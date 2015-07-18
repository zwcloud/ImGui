using System.Collections.Generic;
//using Ivony.Html;

namespace IMGUI
{
    /// <summary>
    /// Render object, containing all the info needed for rendering with cairo
    /// </summary>
    public partial class RenderObject
    {
#if false
        #region Meta
        public string Name;
        #endregion

        #region Hierchary
        public string Path;
        public RenderObject Parent;
        public RenderObject Previous;
        public RenderObject Next;
        public List<RenderObject> vChild;
        #endregion

        /// <summary>
        /// Corresponding dom object
        /// </summary>
        /// <remarks>From the HTML file for layout</remarks>
        public IHtmlNode DOMObject;

        /// <summary>
        /// Corresponding style object
        /// </summary>
        /// <remarks>From the css file for style</remarks>
        public Style StyleObject;

#endif
    }

}
