using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ImGui.Common.Primitive;

namespace ImGui
{
    public partial class GUILayout
    {
        public static bool BeginChild(string str_id, Size size, WindowFlags extra_flags, LayoutOptions? options)
        {
            var window = GetCurrentWindow();
            WindowFlags flags = WindowFlags.NoTitleBar | WindowFlags.NoResize | WindowFlags.ChildWindow | WindowFlags.VerticalScrollbar;

            string name = string.Format("{0}.{1}", window.Name, str_id);
            int id = window.GetID(name);

            GUIStyle style = GUIStyle.Basic;
            style.Save();
            style.ApplyOption(options);
            var rect = window.GetRect(id, size, null, null, true);
            style.Restore();
            if (rect == Layout.StackLayout.DummyRect)
            {
                return false;
            }

            bool open = true;
            return GUI.Begin(name, ref open, rect.TopLeft, rect.Size, 1.0, flags | extra_flags);
        }

        public static void EndChild()
        {
            var window = GetCurrentWindow();
            Debug.Assert(window.Flags.HaveFlag(WindowFlags.ChildWindow));//check mismatched BeginChild/EndChild
            GUI.End();
        }
    }


}
