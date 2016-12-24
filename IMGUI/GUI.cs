using System;
using System.Collections.Generic;

namespace ImGui
{
    public partial class GUI
    {
        public delegate bool WindowFunction();

        #region Button

        public static bool Button(Rect rect, string text, string id)
        {
            return DoButton(rect, Content.Cached(text, id), id);
        }

        public static bool Button(Rect rect, Content content, string id)
        {
            return DoButton(rect, content, id);
        }

        private static bool DoButton(Rect rect, Content content, string id)
        {
            return ImGui.Button.DoControl(rect, content, id);
        }

        #endregion

        #region Label

        public static void Label(Rect rect, string text, string id)
        {
            ImGui.Label.DoControl(rect, Content.Cached(text, id), id);
        }

        public static void Label(Rect rect, Content content, string id)
        {
            ImGui.Label.DoControl(rect, content, id);
        }

        #endregion

        #region Box

        public static void Box(Rect rect, Content content, string id)
        {
            DoBox(rect, content, id);
        }

        private static void DoBox(Rect rect, Content content, string id)
        {
            ImGui.Box.DoControl(rect, content, id);
        }

        #endregion

        #region Toggle

        public static bool Toggle(Rect rect, bool value, string id)
        {
            return DoToggle(rect, value, id);
        }

        private static bool DoToggle(Rect rect, bool value, string id)
        {
            return ImGui.Toggle.DoControl(rect, value, id);
        }

        #endregion

        #region HoverButton

        public static bool HoverButton(Rect rect, string text, string id)
        {
            return DoHoverButton(rect, Content.Cached(text, id), id);
        }

        public static bool HoverButton(Rect rect, Content content, string id)
        {
            return DoHoverButton(rect, content, id);
        }

        private static bool DoHoverButton(Rect rect, Content content, string id)
        {
            return ImGui.HoverButton.DoControl(rect, content, id);
        }

        #endregion

        #region Slider

        public static double Slider(Rect rect, double value, double minValue, double maxValue, string id)
        {
            return Slider(rect, value, minValue, maxValue, true, id);
        }

        public static double VSlider(Rect rect, double value, double minValue, double maxValue, string id)
        {
            return Slider(rect, value, minValue, maxValue, false, id);
        }

        public static double Slider(Rect rect, double value, double minValue, double maxValue, bool isHorizontal, string id)
        {
            return ImGui.Slider.DoControl(rect, value, minValue, maxValue, isHorizontal, id);
        }

        #endregion

        #region ToggleButton

        public static bool ToggleButton(Rect rect, string text, bool value, string id)
        {
            return DoToggleButton(rect, Content.Cached(text, id), value, id);
        }

        public static bool ToggleButton(Rect rect, Content content, bool value, string id)
        {
            return DoToggleButton(rect, content, value, id);
        }

        private static bool DoToggleButton(Rect rect, Content content, bool value, string id)
        {
            return ImGui.ToggleButton.DoControl(rect, content, value, id);
        }

        #endregion

        #region PolygonButton

        public static bool PolygonButton(Rect rect, IReadOnlyList<Point> points, Rect textRect, string text, string id)
        {
            return DoPolygonButton(rect, points, textRect, Content.Cached(text, id), id);
        }

        public static bool PolygonButton(Rect rect, IReadOnlyList<Point> points, Rect textRect, Content content, string id)
        {
            return DoPolygonButton(rect, points, textRect, content, id);
        }

        public static bool DoPolygonButton(Rect rect, IReadOnlyList<Point> points, Rect textRect, Content content, string id)
        {
            return ImGui.PolygonButton.DoControl(rect, points, textRect, content, id);
        }

        #endregion

        #region Image

        //public static void Image(Rect rect, string imageFilePath, string id)
        //{
        //    DoImage(rect, Content.Cached(new te, id), id);
        //}

        public static void Image(Rect rect, ITexture image, string id)
        {
            DoImage(rect, Content.Cached(image, id), id);
        }

        public static void Image(Rect rect, string filePath, string id)
        {
            throw new NotImplementedException();
        }

        public static void Image(Rect rect, Content imageContent, string id)
        {
            DoImage(rect, imageContent, id);
        }

        private static void DoImage(Rect rect, Content content, string id)
        {
            ImGui.Image.DoControl(rect, content, id);
        }

        #endregion

        /*

        public int CombolBox(Rect rect, string[] text, int selectedIndex, string name)
        {
            rect = DoLayout(rect);
            return ComboBox.DoControl(form, rect, text, selectedIndex, name);
        }

        public string TextBox(Rect rect, string text, string name)
        {
            rect = DoLayout(rect);
            return ImGui.TextBox.DoControl(form, rect, text, name);
        }

        public void Window(Rect rect, WindowFunction func, string name)
        {
            ImGui.Window.DoControl(form, rect, func, name);
        }

        public bool MenuItem(Rect rect, string text, string name)
        {
            rect = DoLayout(rect);
            return ImGui.MenuItem.DoControl(rect, text, name);
        }

        #region group methods

        public void BeginClipArea(Rect rect)
        {
            g.Rectangle(rect.TopLeft.ToPointD(), rect.Width, rect.Height);
            g.Clip();
        }

        public void EndClipArea()
        {
            g.ResetClip();
        }
        #endregion

        #region layout methods

        public void BeginScrollView(Rect occupiedRect, Point scrollPosition, Rect viewRect)
        {

        }

        public void EndScrollView()
        {
        }

        #endregion
        */


        #region Constant

        public const string Normal = "Normal";
        public const string Hover = "Hover";
        public const string Active = "Active";

        #endregion

        #region Helper

        public static ITexture CreateTexture(string filePath)
        {
            var texture = Application._map.CreateTexture();
            texture.LoadImage(filePath);
            return texture;
        }

        #endregion
    }

}
