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
            this.name = name;
            this.NeedRepaint = true;
            this.Form = form;

            this.content = content;

            form.renderBoxMap[name] = this;
            this.NeedRepaint = true;
        }

        public static void DoControl(Rect rect, Content content, string name)
        {
            var form = Form.current;
            //Create
            if (!form.renderBoxMap.ContainsKey(name))
            {
                var box = new Box(name, form, content);
            }

            //Update
            var control = form.renderBoxMap[name] as Box;
            Debug.Assert(control != null);
            if (Event.current.type == EventType.Repaint)
            {
                control.Rect = rect;
                control.content = content;
            }

            //Active
            control.Active = true;
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