using System;
using Cairo;
using ImGui;
using System.Diagnostics;
using Xunit;

namespace Test
{
    public class ContentTest
    {
        internal void DrawContent(Rect rect, string text, GUIStyle style)
        {
            var surface = CairoEx.BuildSurface((int)rect.Width, (int)rect.Height, CairoEx.ColorMetal, Format.Rgb24);
            var context = new Context(surface);

            context.DrawBoxModel(rect, text, style);

            string outputPath = "D:\\ContentTest";
            if (!System.IO.Directory.Exists(outputPath))
            {
                System.IO.Directory.CreateDirectory(outputPath);
            }

            string filePath = outputPath + "\\" + DateTime.UtcNow.ToString("yyyy-MM-dd_HH-mm-ss-fff_") + surface.GetHashCode() + ".png";
            surface.WriteToPng(filePath);
            surface.Dispose();
            context.Dispose();

            Process.Start("rundll32.exe", @"""C:\Program Files\Windows Photo Viewer\PhotoViewer.dll"",ImageView_Fullscreen " + filePath);
        }
        
        public ContentTest()
        {
            Application.InitSysDependencies();
        }


        [Fact]
        public void ShowAnEmptyBox()
        {
            DrawContent(new Rect(400, 300), "", GUIStyle.Default);
        }

        [Fact]
        public void ShowATextLeadingAligned()
        {
            Rect rect = new Rect(400, 300);
            string text = "New Text";
            GUIStyle style = "Label";
            style.Set<int>(GUIStyleName.TextAlignment, (int)TextAlignment.Leading);
            TextMeshUtil.GetTextMesh(text, rect, style, GUIState.Normal);
            DrawContent(rect, text, style);
        }

        [Fact]
        public void ShowATextCenterAligned()
        {
            Rect rect = new Rect(400, 300);
            string text = "New Text";
            GUIStyle style = "Label";
            style.Set<int>(GUIStyleName.TextAlignment, (int)TextAlignment.Center);
            TextMeshUtil.GetTextMesh(text, rect, style, GUIState.Normal);
            DrawContent(rect, text, style);
        }

        [Fact]
        public void ShowATextTrailingAligned()
        {
            Rect rect = new Rect(400, 300);
            string text = "New Text";
            GUIStyle style = "Label";
            style.Set<int>(GUIStyleName.TextAlignment, (int)TextAlignment.Trailing);
            TextMeshUtil.GetTextMesh(text, rect, style, GUIState.Normal);
            DrawContent(rect, text, style);
        }

        [Fact]
        public void ShowATextAutoSized()
        {
            string text = "New Text";

            GUIStyle style = new GUIStyle();
            style.Set<double>(GUIStyleName.BorderTop, 10);
            style.Set<double>(GUIStyleName.BorderRight, 10);
            style.Set<double>(GUIStyleName.BorderBottom, 10);
            style.Set<double>(GUIStyleName.BorderLeft, 10);

            style.Set<double>(GUIStyleName.PaddingTop, 10);
            style.Set<double>(GUIStyleName.PaddingRight, 10);
            style.Set<double>(GUIStyleName.PaddingBottom, 10);
            style.Set<double>(GUIStyleName.PaddingLeft, 10);
            Size size = style.CalcSize(text, GUIState.Normal, new[] { GUILayout.Height(100) });
            Rect rect = new Rect(size);
            TextMeshUtil.GetTextMesh(text, rect, style, GUIState.Normal);
            DrawContent(rect, text, style);
        }

        [Fact]
        public void ShowATextWidthAutoSizedHeightFixed()
        {
            string text = "New Text";
            GUIStyle style = "Label";
            style.Set<int>(GUIStyleName.TextAlignment, (int)TextAlignment.Leading);
            Size size = style.CalcSize(text, GUIState.Normal, new []{GUILayout.Height(100)});
            Rect rect = new Rect(size);
            TextMeshUtil.GetTextMesh(text, rect, style, GUIState.Normal);
            DrawContent(rect, text, style);
        }

        [Fact]
        public void ShowATextWidthFixedHeightAutoSized()
        {
            string text = "New Text";
            GUIStyle style = "Label";
            style.Set<int>(GUIStyleName.TextAlignment, (int)TextAlignment.Leading);
            Size size = style.CalcSize(text, GUIState.Normal, new[] { GUILayout.Height(100) });
            Rect rect = new Rect(size);
            TextMeshUtil.GetTextMesh(text, rect, style, GUIState.Normal);
            DrawContent(rect, text, style);
        }


    }
}
