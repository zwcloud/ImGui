using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using ImGui;
using ImGui.Common.Primitive;

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

    //TODO make these tests standalone without Application and Form.

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
                    labelStyle.Set<Color>(GUIStyleName.FontColor, Color.Rgb(253, 79, 10));
                    GUILayout.Label(text, GUILayout.Height(410), GUILayout.Width(410));
                }));
            }

            [Theory]
            [InlineData("msjh.ttf", "文本渲染", 100)]
            [InlineData("msjh.ttf", "文本渲染", 60)]
            [InlineData("msjh.ttf", "文本渲染", 20)]
            [InlineData("msjh.ttf", "文本渲染", 12)]
            public void RenderText(string fontName, string text, int fontSize)
            {
                Application.Run(new Form1(() => {
                    GUIStyle labelStyle = "Label";
                    labelStyle.Set<double>(GUIStyleName.FontSize, fontSize);
                    labelStyle.Set<string>(GUIStyleName.FontFamily, Utility.FontDir + fontName);
                    labelStyle.Set<Color>(GUIStyleName.FontColor, Color.Rgb(253, 79, 10));
                    GUILayout.Label(text, GUILayout.Height(410), GUILayout.Width(410));
                }));
            }

            [Theory]
            [InlineData("msjh.ttf", new[] {"D1","D2"}, 100)]
            public void RenderTextSegments(string fontName, string[] textSegments, int fontSize)
            {
                Application.Run(new Form1(() => {
                    GUIStyle labelStyle = "Label";
                    labelStyle.Set<double>(GUIStyleName.FontSize, fontSize);
                    labelStyle.Set<string>(GUIStyleName.FontFamily, Utility.FontDir + fontName);
                    for (int i = 0; i < textSegments.Length; i++)
                    {
                        GUILayout.Label(textSegments[i], GUILayout.Height(60), GUILayout.Width(410));
                    }
                }));
            }

        }
    }
}
