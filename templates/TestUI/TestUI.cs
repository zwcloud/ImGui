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
    double widthScale = 0.5f;
    double heightScale = 0.5f;
    ITexture image;

    public void OnGUI()
    {
        if(image == null)
        {
            image = GUI.CreateTexture("Image/trees.jpg");
        }

        if (widgetsOn = GUILayout.CollapsingHeader("Widgets", "Widgets", widgetsOn))
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

                    widthScale = GUILayout.Slider("Width Scale", widthScale, 0, 1.0, "SliderA");
                    GUILayout.BeginHorizontal();
                    {
                        heightScale = GUILayout.VSlider("Height Scale", heightScale, 0, 1.0, "SliderB", GUILayout.ExpandWidth(false), GUILayout.Height(image.Height));
                        var rect = GUILayout.GetRect(new Size(image.Width, image.Height), "Image");
                        if(Event.current.type != EventType.Layout)
                        {
                            rect.Width = image.Width * widthScale;
                            rect.Height = image.Height * heightScale;
                        }
                        GUI.Image(rect, image, "Image0");
                    }
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();
        }

    }
}
