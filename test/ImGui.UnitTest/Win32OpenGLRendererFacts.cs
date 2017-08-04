using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using ImGui;

namespace ImGui.UnitTest
{
    public class Form1 : Form
    {
        private Action onGUI;

        public Form1(Action onGUI) : base(new Point(400, 300), new Size(800, 600)) { this.onGUI = onGUI; Form.current = this; }

        protected override void OnGUI()
        {
            this.onGUI?.Invoke();
        }
    }

    public class Win32OpenGLRendererFacts
    {
        public class DrawTextMeshFacts
        {
            public DrawTextMeshFacts()
            {
                Application.InitSysDependencies();
            }

            [Theory]
            [InlineData("msjh.ttf", "A", 400)]
            [InlineData("Helvetica.ttf", "A", 400)]
            [InlineData("unifont-9.0.06.ttf", "A", 400)]
            public void RenderAGlyph(string fontName, string text, int fontSize)
            {
                Application.Run(new Form1(() => {
                    GUIStyle labelStyle = "Label";
                    labelStyle.Set<double>(GUIStyleName.FontSize, fontSize);
                    labelStyle.Set<string>(GUIStyleName.FontFamily, Utility.FontDir + fontName);
                    GUILayout.Label(text, GUILayout.Height(410), GUILayout.Width(410));
                }));
            }

            [Theory]
            [InlineData("msjh.ttf", "ImGui", 100)]
            [InlineData("msjh.ttf", "立即模式GUI", 40)]
            [InlineData("msjh.ttf", "Debug your test", 20)]
            public void RenderText(string fontName, string text, int fontSize)
            {
                Application.Run(new Form1(() => {
                    GUIStyle labelStyle = "Label";
                    labelStyle.Set<double>(GUIStyleName.FontSize, fontSize);
                    labelStyle.Set<string>(GUIStyleName.FontFamily, Utility.FontDir + fontName);
                    GUILayout.Label(text, GUILayout.Height(410), GUILayout.Width(410));
                }));
            }

            [Theory]
            [InlineData("msjh.ttf", "D", 60)]
            public void RenderTexts(string fontName, string text, int fontSize)
            {
                Application.Run(new Form1(() => {
                    GUIStyle labelStyle = "Label";
                    labelStyle.Set<double>(GUIStyleName.FontSize, fontSize);
                    labelStyle.Set<string>(GUIStyleName.FontFamily, Utility.FontDir + fontName);
                    GUILayout.Label("D0", GUILayout.Height(60), GUILayout.Width(410));
                    GUILayout.Label("D1", GUILayout.Height(60), GUILayout.Width(410));
                    //GUI.Label(new Rect(0, 41, 200, 40), "D1");
                }));
            }

        }
    }
}
