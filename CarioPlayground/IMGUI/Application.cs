using System.Collections.Generic;

namespace IMGUI
{
    public delegate void OnGUIDelegate(GUI gui);

    /// <summary>
    /// A single window IMGUI application
    /// </summary>
    public sealed class Application
    {
        /// <summary>
        /// The character buffer for input from IME
        /// </summary>
        internal static Queue<char> imeBuffer = new Queue<char>();

        /// <summary>
        /// The character buffer for input from IME
        /// </summary>
        internal static Queue<char> ImeBuffer
        {
            get { return imeBuffer; }
            set { imeBuffer = value; }
        }

        public static void Run(Form form)
        {
            System.Windows.Forms.Application.Run(form);
        }
    }
}