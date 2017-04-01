using System;
using System.Collections.Generic;
using System.Text;
using ImGui;

public class TestUI
{
    bool widgetsOn = true;
    bool toggleOn = false;
    bool a = false;
    string active_id;

    public void OnGUI()
    {
        GUILayout.BeginVertical(Skin.current.Box, GUILayout.ExpandHeight(true));
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
        GUILayout.EndVertical();

    }
}
