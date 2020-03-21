﻿using ImGui;
using ImGui.Rendering;
using ImGui.Style;
using ImGui.OSAbstraction.Text;
using ImGui.Development;
using System;
using System.Diagnostics;
using Xunit;

namespace TextRenderingTest
{
    public class Form1 : Form
    {
        private Action onGUI;

        public Form1(Action onGUI) : base(new Point(400, 300), new Size(800, 600)) { this.onGUI = onGUI; Form.current = this; }

        protected override void OnGUI()
        {
            if (this.onGUI != null)
                this.onGUI();
        }
    }
    
    public class TextRenderingTest
    {
        public TextRenderingTest()
        {
            Application.InitSysDependencies();
        }

        private const string ModelViewerPath = @"C:\Program Files\Autodesk\FBX Review\fbxreview.exe";
        //"E:\Program Files (green)\open3mod_1_1_standalone\open3mod.exe";

        /// <summary>
        /// This should render a filled cubic bezier curve that commonly used in font
        /// </summary>
        /// <remarks>
        /// The cubic curve used by ttf font is very special that can be perfectly split into two quadratic curves, which is easy to be filled in fragment shader.
        /// See the following links for more information.
        /// https://developer.apple.com/fonts/TrueType-Reference-Manual/RM01/Chap1.html#points
        /// http://stackoverflow.com/questions/20733790/truetype-fonts-glyph-are-made-of-quadratic-bezier-why-do-more-than-one-consecu/20772557?noredirect=1#comment68243476_20772557
        /// </remarks>
        [Fact]
        public void ShouldRenderAFilledCubicBezierCurve()
        {
            // 76, 410,   93, 312,   119, 188,   193, 190,
            // 193, 190,  267, 190,  292, 366,   311, 521,
            var p0 = new Point(76,  410);// start point
            var c0 = new Point(115, 190);// control point 0
            var c1 = new Point(273, 190);// control point 1
            var p1 = new Point(311, 521);// end point
            
            var p = new Point((c0.X + c1.X) / 2, (c0.Y + c1.Y) / 2);

            Application.Run(new Form1(() => {
                var d = new PathGeometryBuilder();
                d.BeginPath();
                d.MoveTo(p0);
                d.QuadraticCurveTo(c0, p);
                d.QuadraticCurveTo(c1, p1);
                d.Fill();

                d.MoveTo(p0);
                d.LineTo(p);
                d.LineTo(p1);
                d.Fill();
                Form.current.ForegroundDrawingContext.DrawGeometry(new Brush(Color.Black), null, d.ToGeometry());
            }));
        }

        [Fact]
        public void ShouldRenderAFilledCubicBezierCurve2()
        {
            // (625,1549) (1040,1508) (1444, 1168) (1442,794)
            // * 0.1
            var p0 = new Point(62.5, 154.9);// start point
            var c0 = new Point(104.0, 150.8);// control point 0
            var c1 = new Point(144.4, 116.8);// control point 1
            var p1 = new Point(144.2, 79.4);// end point

            var p = new Point((c0.X + c1.X) / 2, (c0.Y + c1.Y) / 2);

            Application.Run(new Form1(() => {
                var d = new PathGeometryBuilder();
                d.BeginPath();
                d.MoveTo(p0);
                d.QuadraticCurveTo(c0, p);
                d.QuadraticCurveTo(c1, p1);
                d.Fill();

                d.MoveTo(p0);
                d.LineTo(p);
                d.LineTo(p1);
                d.Fill();
                Form.current.ForegroundDrawingContext.DrawGeometry(new Brush(Color.Black), null, d.ToGeometry());
            }));
        }


        [Fact]
        public void ShouldGenerateARightTexMesh()
        {
            var text = "ABC";
            var fontFamily = Utility.FontDir + "msjh.ttf";
            var fontSize = 36;
            GlyphRun glyphRun = new GlyphRun(text, fontFamily, fontSize);

            var geometryRenderer = new ImGui.GraphicsImplementation.BuiltinGeometryRenderer();
            var textMesh = new TextMesh();
            geometryRenderer.SetTextMesh(textMesh);
            geometryRenderer.DrawGlyphRun(new Brush(Color.Black), glyphRun);
            geometryRenderer.SetTextMesh(null);

            var objFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/TextRenderingTest/texMesh.obj";
            Graphics.SaveTextMeshToObjFile(objFilePath, textMesh);
            Process.Start(ModelViewerPath, objFilePath);
        }

        [Fact]
        public void ShouldGetARightMeshAfterAppendingATextMesh()
        {
            var textMeshA = new TextMesh();
            {
                var text = "ij = I::oO(0xB81l);";
                var fontFamily = Utility.FontDir + "msjh.ttf";
                var fontSize = 12;
                GlyphRun glyphRun = new GlyphRun(text, fontFamily, fontSize);

                var geometryRenderer = new ImGui.GraphicsImplementation.BuiltinGeometryRenderer();
                geometryRenderer.SetTextMesh(textMeshA);
                geometryRenderer.DrawGlyphRun(new Brush(Color.Black), glyphRun);
                geometryRenderer.SetTextMesh(null);
            }

            var textMeshB = new TextMesh();
            {
                var text = "auto-sized";
                var fontFamily = Utility.FontDir + "msjh.ttf";
                var fontSize = 12;
                GlyphRun glyphRun = new GlyphRun(new Point(50, 100), text, fontFamily, fontSize);

                var geometryRenderer = new ImGui.GraphicsImplementation.BuiltinGeometryRenderer();
                geometryRenderer.SetTextMesh(textMeshB);
                geometryRenderer.DrawGlyphRun(new Brush(Color.Black), glyphRun);
                geometryRenderer.SetTextMesh(null);
            }

            TextMesh textMesh = new TextMesh();
            textMesh.Append(textMeshA, Vector.Zero);
            textMesh.Append(textMeshB, Vector.Zero);

            var objFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                + "/TextRenderingTest/ShouldGetARightMeshAfterAppendingATextMesh.obj";
            Graphics.SaveTextMeshToObjFile(objFilePath, textMesh);
            Process.Start(ModelViewerPath, objFilePath);
        }

        [Fact]
        public void ShouldRenderABigGlyph()
        {
            var labelStyle = GUISkin.Current[GUIControlName.Label];
            labelStyle.FontSize = 400;

            Application.Run(new Form1(()=> {
                GUILayout.Label("D", GUILayout.Height(410), GUILayout.Width(410));
            }));
        }

        [Fact]
        public void ShouldRenderAMidiumGlyph()
        {
            var labelStyle = GUISkin.Current[GUIControlName.Label];
            labelStyle.FontSize = 32;

            Application.Run(new Form1(() => {
                GUILayout.Label("D", GUILayout.Height(410), GUILayout.Width(410));
            }));
        }
        
        [Fact]
        public void ShouldRenderASmallGlyph()
        {
            var labelStyle = GUISkin.Current[GUIControlName.Label];
            labelStyle.FontSize = 12;

            Application.Run(new Form1(() => {
                GUILayout.Label("D", GUILayout.Height(410), GUILayout.Width(410));
            }));
        }

        [Fact]
        public void ShouldRenderAStringInMeasuredRectangle()
        {
            string text = "Hello ImGui!你好";

            var ruleSet = GUISkin.Current[GUIControlName.Label];

            var size = ruleSet.CalcSize(text, GUIState.Normal);
            var rect = new Rect(10, 100, size);

            Application.Run(new Form1(() => {
                var d = Form.current.ForegroundDrawingContext;
                d.DrawRectangle(null, new Pen(Color.Red, 1), rect);
                d.DrawGlyphRun(ruleSet, text, rect.TopLeft);
            }));
        }
    }
}
