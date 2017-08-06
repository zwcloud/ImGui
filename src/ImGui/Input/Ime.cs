using System.Collections.Generic;

namespace ImGui.Input
{
    internal static class Ime
    {
        /// <summary>
        /// Character buffer for IME
        /// </summary>
        internal static Queue<char> ImeBuffer { get; set; } = new Queue<char>(16);
    }
}