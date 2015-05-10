
namespace IMGUI
{
    /// <summary>
    /// Key state
    /// </summary>
    public enum KeyState
    {
        /// <summary>Pressing</summary>
        Up,

        /// <summary>Released</summary>
        Down,

        /// <summary>Enabled</summary>
        On,

        /// <summary>Disabled</summary>
        Off,
    }

	/// <summary>Identifies a particular key on a keyboard.</summary>
	/// <param name="A">A key</param>
	/// <param name="Add">Add key</param>
	/// <param name="Apps">Applications key</param>
	/// <param name="Attn">Attn key</param>
	/// <param name="B">B key</param>
	/// <param name="Back">BACKSPACE key</param>
	/// <param name="BrowserBack">Windows 2000/XP: Browser Back key</param>
	/// <param name="BrowserFavorites">Windows 2000/XP: Browser Favorites key</param>
	/// <param name="BrowserForward">Windows 2000/XP: Browser Forward key</param>
	/// <param name="BrowserHome">Windows 2000/XP: Browser Start and Home key</param>
	/// <param name="BrowserRefresh">Windows 2000/XP: Browser Refresh key</param>
	/// <param name="BrowserSearch">Windows 2000/XP: Browser Search key</param>
	/// <param name="BrowserStop">Windows 2000/XP: Browser Stop key</param>
	/// <param name="C">C key</param>
	/// <param name="CapsLock">CAPS LOCK key</param>
	/// <param name="ChatPadGreen">Green ChatPad key</param>
	/// <param name="ChatPadOrange">Orange ChatPad key</param>
	/// <param name="Crsel">CrSel key</param>
	/// <param name="D">D key</param>
	/// <param name="D0">Used for miscellaneous characters; it can vary by keyboard.</param>
	/// <param name="D1">Used for miscellaneous characters; it can vary by keyboard.</param>
	/// <param name="D2">Used for miscellaneous characters; it can vary by keyboard.</param>
	/// <param name="D3">Used for miscellaneous characters; it can vary by keyboard.</param>
	/// <param name="D4">Used for miscellaneous characters; it can vary by keyboard.</param>
	/// <param name="D5">Used for miscellaneous characters; it can vary by keyboard.</param>
	/// <param name="D6">Used for miscellaneous characters; it can vary by keyboard.</param>
	/// <param name="D7">Used for miscellaneous characters; it can vary by keyboard.</param>
	/// <param name="D8">Used for miscellaneous characters; it can vary by keyboard.</param>
	/// <param name="D9">Used for miscellaneous characters; it can vary by keyboard.</param>
	/// <param name="Decimal">Decimal key</param>
	/// <param name="Delete">DEL key</param>
	/// <param name="Divide">Divide key</param>
	/// <param name="Down">DOWN ARROW key</param>
	/// <param name="E">E key</param>
	/// <param name="End">END key</param>
	/// <param name="Enter">ENTER key</param>
	/// <param name="EraseEof">Erase EOF key</param>
	/// <param name="Escape">ESC key</param>
	/// <param name="Execute">EXECUTE key</param>
	/// <param name="Exsel">ExSel key</param>
	/// <param name="F">F key</param>
	/// <param name="F1">F1 key</param>
	/// <param name="F10">F10 key</param>
	/// <param name="F11">F11 key</param>
	/// <param name="F12">F12 key</param>
	/// <param name="F13">F13 key</param>
	/// <param name="F14">F14 key</param>
	/// <param name="F15">F15 key</param>
	/// <param name="F16">F16 key</param>
	/// <param name="F17">F17 key</param>
	/// <param name="F18">F18 key</param>
	/// <param name="F19">F19 key</param>
	/// <param name="F2">F2 key</param>
	/// <param name="F20">F20 key</param>
	/// <param name="F21">F21 key</param>
	/// <param name="F22">F22 key</param>
	/// <param name="F23">F23 key</param>
	/// <param name="F24">F24 key</param>
	/// <param name="F3">F3 key</param>
	/// <param name="F4">F4 key</param>
	/// <param name="F5">F5 key</param>
	/// <param name="F6">F6 key</param>
	/// <param name="F7">F7 key</param>
	/// <param name="F8">F8 key</param>
	/// <param name="F9">F9 key</param>
	/// <param name="G">G key</param>
	/// <param name="H">H key</param>
	/// <param name="Help">HELP key</param>
	/// <param name="Home">HOME key</param>
	/// <param name="I">I key</param>
	/// <param name="ImeConvert">IME Convert key</param>
	/// <param name="ImeNoConvert">IME NoConvert key</param>
	/// <param name="Insert">INS key</param>
	/// <param name="J">J key</param>
	/// <param name="K">K key</param>
	/// <param name="Kana">Kana key on Japanese keyboards</param>
	/// <param name="Kanji">Kanji key on Japanese keyboards</param>
	/// <param name="L">L key</param>
	/// <param name="LaunchApplication1">Windows 2000/XP: Start Application 1 key</param>
	/// <param name="LaunchApplication2">Windows 2000/XP: Start Application 2 key</param>
	/// <param name="LaunchMail">Windows 2000/XP: Start Mail key</param>
	/// <param name="Left">LEFT ARROW key</param>
	/// <param name="LeftAlt">Left ALT key</param>
	/// <param name="LeftControl">Left CONTROL key</param>
	/// <param name="LeftShift">Left SHIFT key</param>
	/// <param name="LeftWindows">Left Windows key</param>
	/// <param name="M">M key</param>
	/// <param name="MediaNextTrack">Windows 2000/XP: Next Track key</param>
	/// <param name="MediaPlayPause">Windows 2000/XP: Play/Pause Media key</param>
	/// <param name="MediaPreviousTrack">Windows 2000/XP: Previous Track key</param>
	/// <param name="MediaStop">Windows 2000/XP: Stop Media key</param>
	/// <param name="Multiply">Multiply key</param>
	/// <param name="N">N key</param>
	/// <param name="None">Reserved</param>
	/// <param name="NumLock">NUM LOCK key</param>
	/// <param name="NumPad0">Numeric keypad 0 key</param>
	/// <param name="NumPad1">Numeric keypad 1 key</param>
	/// <param name="NumPad2">Numeric keypad 2 key</param>
	/// <param name="NumPad3">Numeric keypad 3 key</param>
	/// <param name="NumPad4">Numeric keypad 4 key</param>
	/// <param name="NumPad5">Numeric keypad 5 key</param>
	/// <param name="NumPad6">Numeric keypad 6 key</param>
	/// <param name="NumPad7">Numeric keypad 7 key</param>
	/// <param name="NumPad8">Numeric keypad 8 key</param>
	/// <param name="NumPad9">Numeric keypad 9 key</param>
	/// <param name="O">O key</param>
	/// <param name="Oem8">Used for miscellaneous characters; it can vary by keyboard.</param>
	/// <param name="OemAuto">OEM Auto key</param>
	/// <param name="OemBackslash">Windows 2000/XP: The OEM angle bracket or backslash key on the RT 102 key keyboard</param>
	/// <param name="OemClear">CLEAR key</param>
	/// <param name="OemCloseBrackets">Windows 2000/XP: The OEM close bracket key on a US standard keyboard</param>
	/// <param name="OemComma">Windows 2000/XP: For any country/region, the ',' key</param>
	/// <param name="OemCopy">OEM Copy key</param>
	/// <param name="OemEnlW">OEM Enlarge Window key</param>
	/// <param name="OemMinus">Windows 2000/XP: For any country/region, the '-' key</param>
	/// <param name="OemOpenBrackets">Windows 2000/XP: The OEM open bracket key on a US standard keyboard</param>
	/// <param name="OemPeriod">Windows 2000/XP: For any country/region, the '.' key</param>
	/// <param name="OemPipe">Windows 2000/XP: The OEM pipe key on a US standard keyboard</param>
	/// <param name="OemPlus">Windows 2000/XP: For any country/region, the '+' key</param>
	/// <param name="OemQuestion">Windows 2000/XP: The OEM question mark key on a US standard keyboard</param>
	/// <param name="OemQuotes">Windows 2000/XP: The OEM singled/double quote key on a US standard keyboard</param>
	/// <param name="OemSemicolon">Windows 2000/XP: The OEM Semicolon key on a US standard keyboard</param>
	/// <param name="OemTilde">Windows 2000/XP: The OEM tilde key on a US standard keyboard</param>
	/// <param name="P">P key</param>
	/// <param name="Pa1">PA1 key</param>
	/// <param name="PageDown">PAGE DOWN key</param>
	/// <param name="PageUp">PAGE UP key</param>
	/// <param name="Pause">PAUSE key</param>
	/// <param name="Play">Play key</param>
	/// <param name="Print">PRINT key</param>
	/// <param name="PrintScreen">PRINT SCREEN key</param>
	/// <param name="ProcessKey">Windows 95/98/Me, Windows NT 4.0, Windows 2000/XP: IME PROCESS key</param>
	/// <param name="Q">Q key</param>
	/// <param name="R">R key</param>
	/// <param name="Right">RIGHT ARROW key</param>
	/// <param name="RightAlt">Right ALT key</param>
	/// <param name="RightControl">Right CONTROL key</param>
	/// <param name="RightShift">Right SHIFT key</param>
	/// <param name="RightWindows">Right Windows key</param>
	/// <param name="S">S key</param>
	/// <param name="Scroll">SCROLL LOCK key</param>
	/// <param name="Select">SELECT key</param>
	/// <param name="SelectMedia">Windows 2000/XP: Select Media key</param>
	/// <param name="Separator">Separator key</param>
	/// <param name="Sleep">Computer Sleep key</param>
	/// <param name="Space">SPACEBAR</param>
	/// <param name="Subtract">Subtract key</param>
	/// <param name="T">T key</param>
	/// <param name="Tab">TAB key</param>
	/// <param name="U">U key</param>
	/// <param name="Up">UP ARROW key</param>
	/// <param name="V">V key</param>
	/// <param name="VolumeDown">Windows 2000/XP: Volume Down key</param>
	/// <param name="VolumeMute">Windows 2000/XP: Volume Mute key</param>
	/// <param name="VolumeUp">Windows 2000/XP: Volume Up key</param>
	/// <param name="W">W key</param>
	/// <param name="X">X key</param>
	/// <param name="Y">Y key</param>
	/// <param name="Z">Z key</param>
	/// <param name="Zoom">Zoom key</param>
    public enum Key
    {
        A = 65,
        Add = 107,
        Apps = 93,
        Attn = 246,
        B = 66,
        Back = 8,
        BrowserBack = 166,
        BrowserFavorites = 171,
        BrowserForward = 167,
        BrowserHome = 172,
        BrowserRefresh = 168,
        BrowserSearch = 170,
        BrowserStop = 169,
        C = 67,
        CapsLock = 20,
        Crsel = 247,
        D = 68,
        D0 = 48,
        D1,
        D2,
        D3,
        D4,
        D5,
        D6,
        D7,
        D8,
        D9,
        Decimal = 110,
        Delete = 46,
        Divide = 111,
        Down = 40,
        E = 69,
        End = 35,
        Enter = 13,
        EraseEof = 249,
        Escape = 27,
        Execute = 43,
        Exsel = 248,
        F = 70,
        F1 = 112,
        F10 = 121,
        F11,
        F12,
        F13,
        F14,
        F15,
        F16,
        F17,
        F18,
        F19,
        F2 = 113,
        F20 = 131,
        F21,
        F22,
        F23,
        F24,
        F3 = 114,
        F4,
        F5,
        F6,
        F7,
        F8,
        F9,
        G = 71,
        H,
        Help = 47,
        Home = 36,
        I = 73,
        ImeConvert = 28,
        ImeNoConvert,
        Insert = 45,
        J = 74,
        K,
        Kana = 21,
        Kanji = 25,
        L = 76,
        LaunchApplication1 = 182,
        LaunchApplication2,
        LaunchMail = 180,
        LeftControl = 162,
        Left = 37,
        LeftAlt = 164,
        LeftShift = 160,
        LeftWindows = 91,
        M = 77,
        MediaNextTrack = 176,
        MediaPlayPause = 179,
        MediaPreviousTrack = 177,
        MediaStop,
        Multiply = 106,
        N = 78,
        None = 0,
        NumLock = 144,
        NumPad0 = 96,
        NumPad1,
        NumPad2,
        NumPad3,
        NumPad4,
        NumPad5,
        NumPad6,
        NumPad7,
        NumPad8,
        NumPad9,
        O = 79,
        OemAuto = 243,
        OemCopy = 242,
        OemEnlW = 244,
        OemSemicolon = 186,
        OemBackslash = 226,
        OemQuestion = 191,
        OemTilde,
        OemOpenBrackets = 219,
        OemPipe,
        OemCloseBrackets,
        OemQuotes,
        Oem8,
        OemClear = 254,
        OemComma = 188,
        OemMinus,
        OemPeriod,
        OemPlus = 187,
        P = 80,
        Pa1 = 253,
        PageDown = 34,
        PageUp = 33,
        Pause = 19,
        Play = 250,
        Print = 42,
        PrintScreen = 44,
        ProcessKey = 229,
        Q = 81,
        R,
        RightControl = 163,
        Right = 39,
        RightAlt = 165,
        RightShift = 161,
        RightWindows = 92,
        S = 83,
        Scroll = 145,
        Select = 41,
        SelectMedia = 181,
        Separator = 108,
        Sleep = 95,
        Space = 32,
        Subtract = 109,
        T = 84,
        Tab = 9,
        U = 85,
        Up = 38,
        V = 86,
        VolumeDown = 174,
        VolumeMute = 173,
        VolumeUp = 175,
        W = 87,
        X,
        Y,
        Z,
        Zoom = 251,
        ChatPadGreen = 202,
        ChatPadOrange
    }
}