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
        public static bool Button(Rect rect, string text)
        {
            return ImGui.Button.DoControl(rect, text);
        }

        #endregion

        #region Label

        /// <summary>
        /// Create a label.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="text">text to display on the label</param>
        /// <param name="id">the unique id of this control</param>
        public static void Label(Rect rect, string text)
        {
            ImGui.Label.DoControl(rect, text);
        }

        #endregion

        #region Box

        internal static void Box(Rect rect, string text)
        {
            ImGui.Box.DoControl(rect, text);
        }

        #endregion

        #region Toggle

        /// <summary>
        /// Create a toggle (check-box) with a label.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="value">Is this toggle checked or unchecked?</param>
        /// <param name="id">the unique id of this control</param>
        /// <returns>new value of the toggle</returns>
        public static bool Toggle(Rect rect, string label, bool value)
        {
            return DoToggle(rect, label, value);
        }

        private static bool DoToggle(Rect rect, string label, bool value)
        {
            return ImGui.Toggle.DoControl(rect, label, value);
        }

        #endregion

        #region HoverButton

        /// <summary>
        /// Create a button that will be actived when the mouse is over it.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="text">text to display on the control</param>
        /// <returns>whether it is activated (the mouse is over it)</returns>
        public static bool HoverButton(Rect rect, string text)
        {
            return ImGui.HoverButton.DoControl(rect, text);
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
        /// <returns>The value set by the user.</returns>
        /// <remarks>minValue &lt;= value &lt;= maxValue</remarks>
        public static double Slider(Rect rect, string label, double value, double minValue, double maxValue)
        {
            return Slider(rect, label, value, minValue, maxValue, true);
        }

        /// <summary>
        /// Create a vertical slider that user can drag to select a value.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="value">The value the slider shows.</param>
        /// <param name="minValue">The value at the top end of the slider.</param>
        /// <param name="maxValue">The value at the bottom end of the slider.</param>
        /// <returns>The value set by the user.</returns>
        /// <remarks>minValue &lt;= value &lt;= maxValue</remarks>
        public static double VSlider(Rect rect, string label, double value, double minValue, double maxValue)
        {
            return Slider(rect, label, value, minValue, maxValue, false);
        }

        internal static double Slider(Rect rect, string label, double value, double minValue, double maxValue, bool isHorizontal)
        {
            return ImGui.Slider.DoControl(rect, label, value, minValue, maxValue, isHorizontal);
        }

        #endregion

        #region ToggleButton

        /// <summary>
        /// Create a button that acts like a toggle.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="text">text to display on the button</param>
        /// <param name="value">Is this toggle checked or unchecked?</param>
        /// <returns>new value of the toggle-button</returns>
        public static bool ToggleButton(Rect rect, string text, bool value)
        {
            return ImGui.ToggleButton.DoControl(rect, text, value);
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
        /// <returns>true when the users clicks the button.</returns>
        public static bool PolygonButton(Rect rect, IReadOnlyList<Point> points, Rect textRect, string text)
        {
            return ImGui.PolygonButton.DoControl(rect, points, textRect, text);
        }

        #endregion

        #region Image

        /// <summary>
        /// Create an image.
        /// </summary>
        /// <param name="rect">position and size of the control</param>
        /// <param name="filePath">file path of the image to display. The path should be relative to current dir or absolute.</param>
        public static void Image(Rect rect, string filePath)
        {
            Image(rect, filePath, GUISkin.Instance[GUIControlName.Image]);
        }

        public static void Image(Rect rect, string filePath, GUIStyle style)
        {
            ImGui.Image.DoControl(rect, filePath, style);
        }

        public static void Image(Rect rect, ITexture texture)
        {
            Image(rect, texture, GUISkin.Instance[GUIControlName.Image]);
        }

        public static void Image(Rect rect, ITexture texture, GUIStyle style)
        {
            ImGui.Image.DoControl(rect, texture, style);
        }

        #endregion

        #region Constant

        public const GUIState Normal = GUIState.Normal;
        public const GUIState Hover = GUIState.Hover;
        public const GUIState Active = GUIState.Active;

        #endregion

        #region Helper

        public static ITexture CreateTexture(string filePath)
        {
            var texture = Application.platformContext.CreateTexture();
            texture.LoadImage(filePath);
            return texture;
        }

        #endregion
    }

}
