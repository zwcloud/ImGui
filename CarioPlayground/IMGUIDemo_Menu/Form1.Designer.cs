using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ImGui;

//TODO 1. Auto-size menu window
//TODO 2. Select window item by hovering on it
//TODO 3. Open or close main menu window by click on main menu items
//TODO 4. Sub-menu

namespace ImGuiDemo_Menu
{
    partial class Form1
    {
        protected override void OnGUI()
        {
            //gui.BeginV();
            //    gui.Label(new Rect(this.Size.Width, 20), "Demo project - Menu", "caption");
            //
            //    //TODO implement radio-like button to implement menu
            //    gui.BeginH();
            //    {
            //        if (gui.ToggleButton(new Rect(80, 26), "文件", false, "MenuItem0"))
            //        {
            //            var rect = gui.GetControlRect("MenuItem0");
            //            gui.Window(new Rect(rect.Left, rect.Bottom, 100, 120), MenuFileWindowFunc, "Menu.File");
            //        }
            //        if (gui.ToggleButton(new Rect(80, 26), "编辑", false, "MenuItem1"))
            //        {
            //            var rect = gui.GetControlRect("MenuItem1");
            //            gui.Window(new Rect(rect.Left, rect.Bottom, 100, 140), MenuEditWindowFunc, "Menu.Edit");
            //        }
            //        gui.ToggleButton(new Rect(80, 26), "视图", false, "MenuItem2");
            //        gui.ToggleButton(new Rect(80, 26), "调试", false, "MenuItem3");
            //        gui.ToggleButton(new Rect(80, 26), "团队", false, "MenuItem4");
            //        gui.ToggleButton(new Rect(80, 26), "工具", false, "MenuItem5");
            //    }
            //    gui.EndH();
            //gui.EndV();

            //if (ImGui::BeginMenuBar())
            //{
            //    if (ImGui::BeginMenu("Menu"))
            //    {
            //        ShowExampleMenuFile();
            //        ImGui::EndMenu();
            //    }
            //    if (ImGui::BeginMenu("Examples"))
            //    {
            //        ImGui::MenuItem("Main menu bar", NULL, &show_app_main_menu_bar);
            //        ImGui::MenuItem("Console", NULL, &show_app_console);
            //        ImGui::MenuItem("Log", NULL, &show_app_log);
            //        ImGui::MenuItem("Simple layout", NULL, &show_app_layout);
            //        ImGui::MenuItem("Long text display", NULL, &show_app_long_text);
            //        ImGui::MenuItem("Auto-resizing window", NULL, &show_app_auto_resize);
            //        ImGui::MenuItem("Simple overlay", NULL, &show_app_fixed_overlay);
            //        ImGui::MenuItem("Manipulating window title", NULL, &show_app_manipulating_window_title);
            //        ImGui::MenuItem("Custom rendering", NULL, &show_app_custom_rendering);
            //        ImGui::EndMenu();
            //    }
            //    if (ImGui::BeginMenu("Help"))
            //    {
            //        ImGui::MenuItem("Metrics", NULL, &show_app_metrics);
            //        ImGui::MenuItem("About ImGui", NULL, &show_app_about);
            //        ImGui::EndMenu();
            //    }
            //    ImGui::EndMenuBar();
            //}
        }
#if false
        private bool MenuFileWindowFunc(GUI gui)
        {
            bool clicked1 = false;
            bool clicked2 = false;
            bool clicked3 = false;
            bool clicked4 = false;
            gui.BeginV();
            {
                clicked1 = gui.Button(new Rect(100, 26), "新建", "File.New");
                clicked2 = gui.Button(new Rect(100, 26), "打开", "File.Open");
                clicked3 = gui.Button(new Rect(100, 26), "关闭", "File.Close");
                clicked4 = gui.Button(new Rect(100, 26), "退出", "File.Quit");
            }
            gui.EndV();
            return clicked1 || clicked2 || clicked3 || clicked4;
        }

        private bool MenuEditWindowFunc(GUI gui)
        {
            bool clicked1 = false;
            bool clicked2 = false;
            bool clicked3 = false;
            bool clicked4 = false;
            bool clicked5 = false;
            gui.BeginV();
            {
                clicked1 = gui.Button(new Rect(100, 26), "撤销", "Edit.Undo");
                clicked2 = gui.Button(new Rect(100, 26), "重做", "Edit.Redo");
                clicked3 = gui.Button(new Rect(100, 26), "剪切", "Edit.Cut");
                clicked4 = gui.Button(new Rect(100, 26), "复制", "Edit.Copy");
                clicked5 = gui.Button(new Rect(100, 26), "粘贴", "Edit.Paste");
            }
            gui.EndV();
            return clicked1 || clicked2 || clicked3 || clicked4 || clicked5;
        }
#endif
    }

}
