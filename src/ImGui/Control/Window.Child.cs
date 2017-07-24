using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ImGui
{
    public partial class GUILayout
    {
        public static bool BeginChild(string str_id, Size size, bool border, WindowFlags extra_flags)
        {
            var window = GetCurrentWindow();
            WindowFlags flags = WindowFlags.NoTitleBar | WindowFlags.NoResize | WindowFlags.ChildWindow | WindowFlags.VerticalScrollbar;

            string name = string.Format("{0}.{1}", window.Name, str_id);
            int id = window.GetID(name);
            var rect = window.GetRect(id, size, GUIStyle.Default, null);
            bool open = true;//dummy
            return GUI.Begin(name, ref open, rect.TopLeft, rect.Size, 1.0, flags | extra_flags);
        }

        public static bool BeginChild(int id, Size size, bool border, WindowFlags extra_flags)
        {
            string str_id = string.Format("child_{0:0x8}", id);
            bool ret = BeginChild(str_id, size, border, extra_flags);
            return ret;
        }

        public static void EndChild()
        {
            var window = GetCurrentWindow();
            Debug.Assert(window.Flags.HaveFlag(WindowFlags.ChildWindow));//check mismatched BeginChild/EndChild
            GUI.End();
        }
    }


}
