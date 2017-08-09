using ImGui.Common.Primitive;

namespace ImGui
{
    internal partial class DrawList
    {
        /// <summary>
        /// Append a text mesh to this drawlist
        /// </summary>
        /// <param name="textMesh"></param>
        /// <param name="offset"></param>
        public void AddText(TextMesh textMesh, Vector offset)
        {
            AddTextDrawCommand();
            this.TextMesh.Append(textMesh, offset);
        }
    }
}
