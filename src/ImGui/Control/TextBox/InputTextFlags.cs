namespace ImGui
{
    public enum InputTextFlags
    {
        // Default: 0
        Default = 0,
        CharsDecimal = 1 << 0,   // Allow 0123456789.+-*/
        CharsHexadecimal = 1 << 1,   // Allow 0123456789ABCDEFabcdef
        CharsUppercase = 1 << 2,   // Turn a..z into A..Z
        CharsNoBlank = 1 << 3,   // Filter out spaces, tabs
        AutoSelectAll = 1 << 4,   // Select entire text when first taking mouse focus
        EnterReturnsTrue = 1 << 5,   // Return 'true' when Enter is pressed (as opposed to when the value was modified)
        CallbackCompletion = 1 << 6,   // Call user function on pressing TAB (for completion handling)
        CallbackHistory = 1 << 7,   // Call user function on pressing Up/Down arrows (for history handling)
        CallbackAlways = 1 << 8,   // Call user function every time. User code may query cursor position, modify text buffer.
        CallbackCharFilter = 1 << 9,   // Call user function to filter character. Modify data->EventChar to replace/filter input, or return 1 to discard character.
        AllowTabInput = 1 << 10,  // Pressing TAB input a '\t' character into the text field
        CtrlEnterForNewLine = 1 << 11,  // In multi-line mode, unfocus with Enter, add new line with Ctrl+Enter (default is opposite: unfocus with Ctrl+Enter, add line with Enter).
        NoHorizontalScroll = 1 << 12,  // Disable following the cursor horizontally
        AlwaysInsertMode = 1 << 13,  // Insert mode
        ReadOnly = 1 << 14,  // Read-only mode
        Password = 1 << 15,  // Password mode, display all characters as '*'

        // [Internal]
        Multiline = 1 << 20   // For internal use by InputTextMultiline()
    };

    internal static class InputTextFlagsExtension
    {
        public static bool HaveFlag(this InputTextFlags value, InputTextFlags flag)
        {
            return (value & flag) != 0;
        }
    }
}
