using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using IMGUI;

//TODO 1. Auto-size menu window
//TODO 2. Select window item by hovering on it
//TODO 3. Open or close main menu window by click on main menu items
//TODO 4. Sub-menu

namespace IMGUIDemo_Menu
{
    partial class Form1
    {
        protected override void OnGUI(GUI gui)
        {
            gui.BeginV();
                gui.Label(new Rect(this.Size.Width, 20), "Demo project - Menu", "caption");

                //TODO implement radio-like button to implement menu
                gui.BeginH();
                {
                    if (gui.RadioButton(new Rect(80, 26), "文件", "MainMenu", false, "MenuItem0"))
                    {
                        var rect = gui.GetControlRect("MenuItem0");
                        gui.Window(new Rect(rect.Left, rect.Bottom, 100, 120), MenuFileWindowFunc, "Menu.File");
                    }
                    if (gui.RadioButton(new Rect(80, 26), "编辑", "MainMenu", false, "MenuItem1"))
                    {
                        var rect = gui.GetControlRect("MenuItem1");
                        gui.Window(new Rect(rect.Left, rect.Bottom, 100, 140), MenuEditWindowFunc, "Menu.Edit");
                    }
                    gui.RadioButton(new Rect(80, 26), "视图", "MainMenu", false, "MenuItem2");
                    gui.RadioButton(new Rect(80, 26), "调试", "MainMenu", false, "MenuItem3");
                    gui.RadioButton(new Rect(80, 26), "团队", "MainMenu", false, "MenuItem4");
                    gui.RadioButton(new Rect(80, 26), "工具", "MainMenu", false, "MenuItem5");
                }
                gui.EndH();
            gui.EndV();

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

        private void MenuFileWindowFunc(GUI gui)
        {
            gui.BeginV();
            {
                gui.Button(new Rect(100, 26), "新建", "File.New");
                gui.Button(new Rect(100, 26), "打开", "File.Open");
                gui.Button(new Rect(100, 26), "关闭", "File.Close");
                gui.Button(new Rect(100, 26), "退出", "File.Quit");
            }
            gui.EndV();
        }

        private void MenuEditWindowFunc(GUI gui)
        {
            gui.BeginV();
            {
                gui.Button(new Rect(100, 26), "撤销", "Edit.Undo");
                gui.Button(new Rect(100, 26), "重做", "Edit.Redo");
                gui.Button(new Rect(100, 26), "剪切", "Edit.Cut");
                gui.Button(new Rect(100, 26), "复制", "Edit.Copy");
                gui.Button(new Rect(100, 26), "粘贴", "Edit.Paste");
            }
            gui.EndV();
        }
    }

}
