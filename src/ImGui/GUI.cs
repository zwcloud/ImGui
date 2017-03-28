using System;
using System.Collections.Generic;

namespace ImGui
{
    /// <summary>
    /// The interface for GUI with manual positioning.
    /// </summary>
    public class GUI
    {
        internal delegate bool WindowFunction();

        #region Button

        /// <summary>
        /// Create a button. When the user click it, something will happen immediately.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="text">text to display on the button</param>
        /// <param name="id">the unique id of this control</param>
        /// <returns>true when the users clicks the button.</returns>
        public static bool Button(Rect rect, string text, string id)
        {
            return DoButton(rect, Content.Cached(text, id), id);
        }

        internal static bool Button(Rect rect, Content content, string id)
        {
            return DoButton(rect, content, id);
        }

        private static bool DoButton(Rect rect, Content content, string id)
        {
            return ImGui.Button.DoControl(rect, content, id);
        }

        #endregion

        #region Label

        /// <summary>
        /// Create a label.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="text">text to display on the label</param>
        /// <param name="id">the unique id of this control</param>
        public static void Label(Rect rect, string text, string id)
        {
            ImGui.Label.DoControl(rect, Content.Cached(text, id), id);
        }

        internal static void Label(Rect rect, Content content, string id)
        {
            ImGui.Label.DoControl(rect, content, id);
        }

        #endregion

        #region Box

        internal static void Box(Rect rect, Content content, string id)
        {
            DoBox(rect, content, id);
        }

        private static void DoBox(Rect rect, Content content, string id)
        {
            ImGui.Box.DoControl(rect, content, id);
        }

        #endregion

        #region Toggle

        /// <summary>
        /// Create a toggle (check-box).
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="value">Is this toggle checked or unchecked?</param>
        /// <param name="id">the unique id of this control</param>
        /// <returns>new value of the toggle</returns>
        public static bool Toggle(Rect rect, bool value, string id)
        {
            return DoToggle(rect, value, id);
        }

        /// <summary>
        /// Create a toggle (check-box) with a label.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="value">Is this toggle checked or unchecked?</param>
        /// <param name="id">the unique id of this control</param>
        /// <returns>new value of the toggle</returns>
        public static bool Toggle(Rect rect, string label, bool value, string id)
        {
            return DoToggle(rect, Content.Cached(label, id), value, id);
        }

        internal static bool Toggle(Rect rect, Content content, bool value, string id)
        {
            return DoToggle(rect, content, value, id);
        }

        private static bool DoToggle(Rect rect, bool value, string id)
        {
            return ImGui.Toggle.DoControl(rect, value, id);
        }

        private static bool DoToggle(Rect rect, Content content, bool value, string id)
        {
            return ImGui.Toggle.DoControl(rect, content, value, id);
        }

        #endregion

        #region HoverButton

        /// <summary>
        /// Create a button that will be actived when the mouse is over it.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="text">text to display on the control</param>
        /// <param name="id">the unique id of this control</param>
        /// <returns>whether it is activated (the mouse is over it)</returns>
        public static bool HoverButton(Rect rect, string text, string id)
        {
            return DoHoverButton(rect, Content.Cached(text, id), id);
        }

        internal static bool HoverButton(Rect rect, Content content, string id)
        {
            return DoHoverButton(rect, content, id);
        }

        private static bool DoHoverButton(Rect rect, Content content, string id)
        {
            return ImGui.HoverButton.DoControl(rect, content, id);
        }

        #endregion

        #region Slider

        /// <summary>
        /// Create a horizontal slider that user can drag to select a value.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="value">The value the slider shows.</param>
        /// <param name="minValue">The value at the top end of the slider.</param>
        /// <param name="maxValue">The value at the bottom end of the slider.</param>
        /// <param name="id">the unique id of this control</param>
        /// <returns>The value set by the user.</returns>
        /// <remarks>minValue &lt;= value &lt;= maxValue</remarks>
        public static double Slider(Rect rect, double value, double minValue, double maxValue, string id)
        {
            return Slider(rect, value, minValue, maxValue, true, id);
        }

        /// <summary>
        /// Create a vertical slider that user can drag to select a value.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="value">The value the slider shows.</param>
        /// <param name="minValue">The value at the top end of the slider.</param>
        /// <param name="maxValue">The value at the bottom end of the slider.</param>
        /// <param name="id">the unique id of this control</param>
        /// <returns>The value set by the user.</returns>
        /// <remarks>minValue &lt;= value &lt;= maxValue</remarks>
        public static double VSlider(Rect rect, double value, double minValue, double maxValue, string id)
        {
            return Slider(rect, value, minValue, maxValue, false, id);
        }

        internal static double Slider(Rect rect, double value, double minValue, double maxValue, bool isHorizontal, string id)
        {
            return ImGui.Slider.DoControl(rect, value, minValue, maxValue, isHorizontal, id);
        }

        #endregion

        #region ToggleButton

        /// <summary>
        /// Create a button that acts like a toggle.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="text">text to display on the button</param>
        /// <param name="value">Is this toggle checked or unchecked?</param>
        /// <param name="id">the unique id of this control</param>
        /// <returns>new value of the toggle-button</returns>
        public static bool ToggleButton(Rect rect, string text, bool value, string id)
        {
            return DoToggleButton(rect, Content.Cached(text, id), value, id);
        }

        internal static bool ToggleButton(Rect rect, Content content, bool value, string id)
        {
            return DoToggleButton(rect, content, value, id);
        }

        private static bool DoToggleButton(Rect rect, Content content, bool value, string id)
        {
            return ImGui.ToggleButton.DoControl(rect, content, value, id);
        }

        #endregion

        #region PolygonButton

        /// <summary>
        /// Create a polyon-button.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="points"><see cref="ImGui.Point"/> list of the polygon.</param>
        /// <param name="textRect">the rect that occupied by the text</param>
        /// <param name="text">text to display on the button</param>
        /// <param name="id">the unique id of this control</param>
        /// <returns>true when the users clicks the button.</returns>
        public static bool PolygonButton(Rect rect, IReadOnlyList<Point> points, Rect textRect, string text, string id)
        {
            return DoPolygonButton(rect, points, textRect, Content.Cached(text, id), id);
        }

        internal static bool PolygonButton(Rect rect, IReadOnlyList<Point> points, Rect textRect, Content content, string id)
        {
            return DoPolygonButton(rect, points, textRect, content, id);
        }

        internal static bool DoPolygonButton(Rect rect, IReadOnlyList<Point> points, Rect textRect, Content content, string id)
        {
            return ImGui.PolygonButton.DoControl(rect, points, textRect, content, id);
        }

        #endregion

        #region Image

        /// <summary>
        /// Create a image.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="filePath">file path of the image to display. The path should be relative to current dir or absolute.</param>
        /// <param name="id">the unique id of this control</param>
        public static void Image(Rect rect, string filePath, string id)
        {
            DoImage(rect, Content.CachedTexture(filePath, id), id);
        }

        internal static void Image(Rect rect, ITexture image, string id)
        {
            DoImage(rect, Content.Cached(image, id), id);
        }

        internal static void Image(Rect rect, Content imageContent, string id)
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

        internal static ITexture CreateTexture(string filePath)
        {
            var texture = Application._map.CreateTexture();
            texture.LoadImage(filePath);
            return texture;
        }

        #endregion
    }

}
