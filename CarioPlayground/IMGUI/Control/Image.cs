using Cairo;
using System.Diagnostics;

namespace IMGUI
{
    internal class Image : Control
    {
        public Texture Texture { get; private set; }

        public Image(string name, BaseForm form, Texture texture)
            : base(name, form)
        {
            Texture = texture;
        }

        static internal void DoControl(BaseForm form, Rect rect, Texture texture, string name)
        {
            if (!form.Controls.ContainsKey(name))
            {
                var image = new Image(name, form, texture);
                image.Rect = rect;
            }

            var control = form.Controls[name] as Image;
            Debug.Assert(control != null);
            control.Active = true;
        }

        #region Overrides of Control

        public override void OnUpdate()
        {
        }

        public override void OnRender(Context g)
        {
            g.DrawBoxModel(Rect, new Content(Texture), Skin.current.Image["Normal"]);
        }

        public override void Dispose()
        {
            
        }

        public override void OnClear(Context g)
        {
            g.FillRectangle(Rect, CairoEx.ColorWhite);
        }

        #endregion
    }
}
