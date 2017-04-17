namespace ImGui
{
    internal class Label
    {
        internal static void DoControl(Rect rect, Content content, string name)
        {
            GUIPrimitive.DrawBoxModel(rect, content, GUISkin.Instance[GUIControlName.Label]);
        }
    }
}