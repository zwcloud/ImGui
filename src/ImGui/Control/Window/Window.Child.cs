using ImGui.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ImGui
{
    public partial class GUILayout
    {
        public static bool BeginChild(string name, LayoutOptions? options, WindowFlags extra_flags)
        {
            bool open = true;
            var window = GetCurrentWindow();
            var id = window.GetID(name);
            var childWindowContainer = window.RenderTree.CurrentContainer.GetNodeById(id);
            if (childWindowContainer == null)
            {
                childWindowContainer = new Node(id, name);
                childWindowContainer.AttachLayoutGroup(true);
                if (options.HasValue)
                {
                    childWindowContainer.RuleSet.ApplyOptions(options);
                }
                else
                {
                    childWindowContainer.RuleSet.ApplyOptions(GUILayout.ExpandWidth(true).ExpandHeight(true));
                }
            }
            window.RenderTree.CurrentContainer.AppendChild(childWindowContainer);
            childWindowContainer.ActiveSelf = true;
            if(childWindowContainer.Rect.Width == 0 || childWindowContainer.Rect.Height == 0)
            {
                return false;
            }

            WindowFlags flags = WindowFlags.NoTitleBar | WindowFlags.NoResize | WindowFlags.ChildWindow | WindowFlags.VerticalScrollbar;
            return GUI.Begin(name, ref open, childWindowContainer.Rect.TopLeft, childWindowContainer.Rect.Size, 1.0, flags | extra_flags);
        }

        public static bool BeginChild(string name, LayoutOptions? options) => BeginChild(name, options, WindowFlags.Default);

        public static void EndChild()
        {
            var window = GetCurrentWindow();
            Debug.Assert(window.Flags.HaveFlag(WindowFlags.ChildWindow));//check mismatched BeginChild/EndChild
            GUI.End();
        }
    }


}
