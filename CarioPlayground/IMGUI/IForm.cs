using Cairo;

namespace IMGUI
{
    /// <summary>
    /// Form functionaliy
    /// </summary>
    public interface IForm : IWindow
    {
        #region title
        /// <summary>
        /// Title text
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Title color
        /// </summary>
        Color TitleColor { get; set; }
        #endregion

        #region border
        /// <summary>
        /// Border style
        /// </summary>
        BorderStyle BorderStyle { get; set; }
        #endregion
    }

    /// <summary>Specifies the border style for a control.</summary>
	/// <filterpriority>2</filterpriority>
	public enum BorderStyle
	{
		/// <summary>
		/// No border.
		/// </summary>
		None,
		/// <summary>
		/// A single-line border.
		/// </summary>
		Single,
		/// <summary>
		/// A three-dimensional border.
		/// </summary>
		ThreeD,
        /// <summary>
        /// Default
        /// </summary>
        Default
	}
}