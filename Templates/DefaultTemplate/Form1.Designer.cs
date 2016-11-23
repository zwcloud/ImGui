using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ImGui;

namespace DefaultTemplate
{
    partial class Form1
    {
        private bool toggleValue = false;

        protected override void OnGUI()
        {
            GUILayout.Button("Test Button");
            GUILayout.FlexibleSpace();
            toggleValue = GUILayout.Toggle("123 Toggle GO!", toggleValue, "A_toggle");
        }

    }
}
