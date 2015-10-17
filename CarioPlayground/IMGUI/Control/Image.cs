using Cairo;

//BUG image location is not right!

namespace IMGUI
{
    internal class Image : Control
    {
        public Texture Texture { get; private set; }

        public Rect Rect { get; private set; }

        public Image(string name, BaseForm form, Texture texture)
            : base(name, form)
        {
            Texture = texture;
        }

        static internal void DoControl(Context g, BaseForm form, Rect rect, Texture texture, string name)
        {
            if (!form.Controls.ContainsKey(name))
            {
                var image = new Image(name, form, texture);
                image.Rect = rect;
                image.OnUpdate();
                image.OnRender(g);
            }
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

        #endregion
    }
}
