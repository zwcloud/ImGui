using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IMGUI;

namespace IMGUIDemo_Menu
{
    partial class Form1
    {
        protected override void OnGUI(GUI gui)
        {
            gui.Label(new Rect(0, 0, this.Size.Width, 20), "Demo project - Menu", "caption");

            //TODO implement radio-like button to implement menu
            gui.BeginHorizontal(new Rect(0,20, Size.Width, 30));
            {
                if (gui.RadioButton(new Rect(60, 26), "文件", "MainMenu", false, "MenuItem0"))
                {
                    gui.Window(new Rect(100, 120), DoWindow, "FileMenu");
                }
                gui.RadioButton(new Rect(60, 26), "编辑", "MainMenu", false, "MenuItem1");
                gui.RadioButton(new Rect(60, 26), "视图", "MainMenu", false, "MenuItem2");
                gui.RadioButton(new Rect(60, 26), "调试", "MainMenu", false, "MenuItem3");
                gui.RadioButton(new Rect(60, 26), "团队", "MainMenu", false, "MenuItem4");
                gui.RadioButton(new Rect(60, 26), "工具", "MainMenu", false, "MenuItem5");
            }
            gui.EndHorizontal();

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

        private void DoWindow(GUI gui)
        {
            gui.BeginVertical(new Rect(100, 120));
            {
                gui.ToggleButton(new Rect(60, 26), "新建", false,"File.New");
                //gui.ToggleButton(new Rect(60, 26), "打开", true, "File.Open");
                //gui.ToggleButton(new Rect(60, 26), "关闭", false, "File.Close");
                //gui.ToggleButton(new Rect(60, 26), "退出", false, "File.Quit");
            }
            gui.EndVertical();
        }
    }

}
