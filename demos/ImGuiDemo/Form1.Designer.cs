using System;
using System.Diagnostics;
using ImGui;

namespace Calculator
{
    partial class Form1
    {
        bool widgetsOn = true;
        bool toggleOn = false;
        bool a = false;

        protected override void OnGUI()
        {
            if (GUILayout.Button("Button", "Button"))
            {
                Console.WriteLine("Clicked\n");
                a ^= true;
            }
                GUILayout.BeginHorizontal();
            if (a)
            {
                GUILayout.Label("Thanks for clicking me!", "ThanksForClickingMe");
            }

                GUILayout.EndHorizontal();
            if (widgetsOn = GUILayout.ToggleButton("Widgets", widgetsOn, "Widgets"))
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(30);
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("CheckBox", "CheckBoxLabel");
                    toggleOn = GUILayout.Toggle(toggleOn, "Toggle");
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }

        }
    }
}
