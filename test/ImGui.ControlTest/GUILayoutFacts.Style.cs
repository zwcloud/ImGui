using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ImGui.ControlTest
{
    public class GUILayoutFacts
    {
        [Fact]
        public void ShowButtonsOfDifferentStyle()
        {
            Application.IsRunningInUnitTest = true;
            Application.Init();

            var form = new MainForm();
            form.OnGUIAction = () =>
            {
                GUILayout.PushStyle(StylePropertyName.BorderTop, 10.0, GUIState.Normal);
                GUILayout.PushStyle(StylePropertyName.BorderTop, 10.0, GUIState.Hover);
                GUILayout.PushStyle(StylePropertyName.BorderTop, 10.0, GUIState.Active);
                GUILayout.Button("Button1");
                GUILayout.Button("Button2");
                GUILayout.Label("Label");
                GUILayout.CheckBox("Check", true);
                GUILayout.PopStyle(3);
                GUILayout.Button("Button3");
                GUILayout.Button("Button4");
            };

            Application.Run(form);
        }

    }
}
