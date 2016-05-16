namespace ImGui
{
    internal class MenuItem : Button
    {
        public MenuItem(string name, Form form, string text, Rect rect)
            : base(name, form, text, rect)
        {
        }

        public static bool DoControl(Rect rect, string text, string name)
        {

            return false;
        }
    }
}
