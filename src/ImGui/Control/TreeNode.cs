using System;
using ImGui.Common.Primitive;

namespace ImGui
{
    public partial class GUILayout
    {
        public static bool TreeNode(string label, ref bool open)
        {
            Window window = GetCurrentWindow();
            if (window.SkipItems)
                return false;

            int id = window.GetID(label);

            PushHStretchFactor(1);
            BeginVertical(label + "_Tree");
            PopStyleVar(1);
                open = (CollapsingHeader(label, ref open));
                BeginHorizontal("#Content");
                    Space("Space", 20);
                    PushHStretchFactor(1);
                    BeginVertical("#Items");
                    PopStyleVar(1);

            return open;
        }

        public static void TreePop()
        {
                    EndVertical();
                EndHorizontal();
            EndVertical();
        }
    }
}
