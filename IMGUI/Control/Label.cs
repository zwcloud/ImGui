using System.Diagnostics;
namespace ImGui
{
    internal class Label : SimpleControl
    {
        private readonly string name;
        private Rect rect;
        private Style style;
        private string text;
        private Content content;

        public string Text
        {
            get { return text; }
            private set
            {
                text = value;
                NeedRepaint = true;
            }
        }

        internal Label(string name, Form form, Content content)
        {
            this.name = name;
            this.State = "Normal";
            this.NeedRepaint = true;
            this.Form = form;

            this.style = Skin.current.Label[State];
            this.Text = content.Text;
            this.content = content;

            form.renderBoxMap[name] = this;
        }

        internal static void DoControl(Rect rect, Content content, string name)
        {
            if (Event.current.type == EventType.Repaint)
            {
                GUIPrimitive.DrawBoxModel(rect, content, Skin.current.Label["Normal"]);
            }

#if false
            //Create
            var form = Form.current;
            if (!form.renderBoxMap.ContainsKey(name))
            {
                var label = new Label(name, form, content);
            }

            //Update
            var control = form.renderBoxMap[name] as Label;
            Debug.Assert(control != null);
            if (Event.current.type == EventType.Repaint)
            {
                if (control.Content == null// first drawn
                    || control.Rect != rect// rect changed
                    || control.Text != content.Text// text changed
                    )
                {
                    control.Text = content.Text;
                    control.Rect = rect;
                    content.Build(control.ContentRect.Size, control.Style);
                }
            }

            //Active
            control.Active = true;
#endif
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
                this.style = Skin.current.Label[State];
                return this.style;
            }
        }

        #endregion
    }
}