namespace ImGui.Rendering
{
    internal partial class Node
    {
        public void CalcWidth(double unitPartWidth = -1d)
        {
            if (this.IsGroup)
            {
                CalcWidth_Group(unitPartWidth);
            }
            else
            {
                CalcWidth_Entry(unitPartWidth);
            }
        }

        public void CalcHeight(double unitPartHeight = -1d)
        {
            if (this.IsGroup)
            {
                CalcHeight_Group(unitPartHeight);
            }
            else
            {
                CalcHeight_Entry(unitPartHeight);
            }
        }

        public void SetX(double x)
        {
            if (this.IsGroup)
            {
                SetX_Group(x);
            }
            else
            {
                SetX_Entry(x);
            }
        }

        public void SetY(double y)
        {
            if (this.IsGroup)
            {
                SetY_Group(y);
            }
            else
            {
                SetY_Entry(y);
            }
        }
    }
}
