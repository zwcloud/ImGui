using System.Diagnostics;

namespace ImGui
{
    /// <summary>
    /// Box
    /// </summary>
    /// <remarks>
    /// The box is a simple control, which only contains a Content.
    /// </remarks>
    internal class Box : SimpleControl
    {
        private readonly string name;

        private Rect rect;
        private Content content;

        public Box(string name, Form form, Content content)
        {
        }

        public static void DoControl(Rect rect, Content content, string name)
        {
            if (Event.current.type == EventType.Repaint)
            {
                GUIPrimitive.DrawBoxModel(rect, content, Skin.current.Box);
            }
        }

        public override string Name
        {
            get { return name; }
        }

        #region Overrides of SimpleControl

        public override Rect Rect
        {
            get { return rect; }
            set
            {
                //TODO need re-layout
                rect = value;
            }
        }

        public override Content Content
        {
            get
            {
                return content;
            }
        }

        public override Style Style
        {
            get
            {
                return Skin.current.Box;
            }
        }

        #endregion
    }
}