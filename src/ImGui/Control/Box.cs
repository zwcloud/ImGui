using System.Diagnostics;

namespace ImGui
{
    /// <summary>
    /// Box
    /// </summary>
    /// <remarks>
    /// The box is a simple control containing an optinal Content.
    /// </remarks>
    internal class Box
    {
        public static void DoControl(Rect rect, Content content, string name)
        {
            GUIPrimitive.DrawBoxModel(rect, content, GUISkin.Instance[GUIControlName.Box]);
        }
    }
}