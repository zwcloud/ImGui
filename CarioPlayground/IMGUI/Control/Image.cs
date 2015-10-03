using Cairo;

//BUG image location is not right!

namespace IMGUI
{
    internal class Image : Control
    {
        public Texture Texture { get; private set; }

        public Rect Rect { get; private set; }

        public Image(string name, Texture texture)
        {
            Name = name;
            State = "Normal";
            Controls[Name] = this;

            Texture = texture;
        }

        static internal void DoControl(Context g, Rect rect, Texture texture, string name)
        {
            //The control hasn't been created, create it.
            if (!Controls.ContainsKey(name))
            {
                var image = new Image(name, texture);
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

        #endregion
    }
}
