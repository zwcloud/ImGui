namespace IMGUI
{
    public abstract class Form : SFMLForm
    {
        protected Form(int width, int height)
            : base(width, height, SFML.Window.Styles.Default)
        {
        }

    }
}