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
        string active_id;

        protected override void OnGUI()
        {
            if (widgetsOn = GUILayout.ToggleButton("Widgets", widgetsOn, "Widgets"))
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(30);
                    GUILayout.BeginVertical();
                    {
                        GUILayout.BeginHorizontal();
                        {
                            if (GUILayout.Button("Button", "Button"))
                            {
                                a ^= true;
                                Event.current.type = EventType.Layout;
                            }
                            if (a)
                            {
                                GUILayout.Label("Thanks for clicking me!", "ThanksForClickingMe");
                            }
                        }
                        GUILayout.EndHorizontal();

                        toggleOn = GUILayout.Toggle("CheckBox", toggleOn, "Toggle");

                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Radio("Radio 0", ref active_id, "radio_b_0");
                            GUILayout.Radio("Radio 1", ref active_id, "radio_b_1");
                            GUILayout.Radio("Radio 2", ref active_id, "radio_b_2");
                        }
                        GUILayout.EndHorizontal();


                    }

                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
            }

        }
    }
}
