using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using DWriteSharp.Internal;

namespace DWriteSharp
{
    /// <summary>
    /// DirectWrite TextFormat
    /// </summary>
    public sealed class TextFormat : IDisposable
    {
        #region API

        /// <summary>
        /// Alignment option of text relative to layout box's leading and trailing edge.
        /// </summary>
        public TextAlignment TextAlignment
        {
            get { return GetTextAlignment(); }
            set { SetTextAlignment(value); }
        }

        /// <summary>
        /// Get the font em height.
        /// </summary>
        public float FontSize
        {
            get { return GetFontSize(); }
        }

        /// <summary>
        /// Get a copy of the font family name.
        /// </summary>
        public string FontFamilyName
        {
            get { return GetFontFamilyName(); }
        }

        /// <summary>
        /// Get the font weight.
        /// </summary>
        public FontWeight FontWeight
        {
            get { return GetFontWeight(); }
        }

        /// <summary>
        /// Get the font style.
        /// </summary>
        public FontStyle FontStyle
        {
            get { return GetFontStyle(); }
        }


        /// <summary>
        /// Get the font stretch.
        /// </summary>
        public FontStretch FontStretch
        {
            get { return GetFontStretch(); }
        }

        #endregion

        #region COM internals

        [ComImport]
        [Guid("9c906818-31d7-4fd3-a151-7c5e225db55a")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IDWriteTextFormat
        {
        } //contains 25 method(inherited method not included)

        private IDWriteTextFormat comObject;

        // BUG? Marshal.GetIUnknownForObject(this.comObject) will offset the pointer by 4 bytes
        internal IntPtr Handle { get; private set; }

        #region COM methods

        #region COM method signatures

        /// <summary>
        /// Set alignment option of text relative to layout box's leading and trailing edge.
        /// </summary>
        /// <param name="textAlignment">Text alignment option</param>
        /// <returns>
        /// Standard HRESULT error code.
        /// </returns>
        [ComMethod(Index = 3)]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int SetTextAlignmentSignature(IDWriteTextFormat textformat,
            TextAlignment textAlignment);

        private SetTextAlignmentSignature setTextAlignment;

        /// <summary>
        /// Get alignment option of text relative to layout box's leading and trailing edge.
        /// </summary>
        [ComMethod(Index = 11)]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate TextAlignment GetTextAlignmentSignature(IDWriteTextFormat textformat);

        private GetTextAlignmentSignature getTextAlignment;

        /// <summary>
        /// Get the length of the font family name, in characters, not including the terminating NULL character.
        /// </summary>
        /// <param name="textformat">the IDWriteTextFormat interface itself</param>
        /// <returns>length of the font family name</returns>
        /// <remarks>
        /// <code>
        /// STDMETHOD_(UINT32, GetFontFamilyNameLength)() PURE;
        /// </code>
        /// </remarks>
        [ComMethod(Index = 20)]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate uint GetFontFamilyNameLengthSignature(IDWriteTextFormat textformat);

        private GetFontFamilyNameLengthSignature getFontFamilyNameLength;

        /// <summary>
        /// Get a copy of the font family name.
        /// </summary>
        /// <param name="fontFamilyName">Character array that receives the current font family name</param>
        /// <param name="nameSize">Size of the character array in character count including the terminated NULL character.</param>
        /// <returns>
        /// Standard HRESULT error code.
        /// </returns>
        /// <remarks>
        /// <code>
        /// STDMETHOD(GetFontFamilyName)(
        ///     _Out_writes_z_(nameSize) WCHAR* fontFamilyName,
        ///     UINT32 nameSize
        ///     ) PURE;
        /// </code>
        /// </remarks>
        [ComMethod(Index = 21)]
        [UnmanagedFunctionPointer(CallingConvention.StdCall,CharSet = CharSet.Unicode)]
        private delegate int GetFontFamilyNameSignature(IDWriteTextFormat textformat,
            char[] fontFamilyName, uint nameSize);

        private GetFontFamilyNameSignature getFontFamilyName;

        /// <summary>
        /// Get the font weight.
        /// </summary>
        /// <remarks>
        /// <code>
        /// STDMETHOD_(DWRITE_FONT_WEIGHT, GetFontWeight)() PURE;
        /// </code>
        /// </remarks>
        [ComMethod(Index = 22)]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FontWeight GetFontWeightSignature(IDWriteTextFormat textformat);

        private GetFontWeightSignature getFontWeight;

        /// <summary>
        /// Get the font style.
        /// </summary>
        /// <remarks>
        /// <code>
        /// STDMETHOD_(DWRITE_FONT_STYLE, GetFontStyle)() PURE;
        /// </code>
        /// </remarks>
        [ComMethod(Index = 23)]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FontStyle GetFontStyleSignature(IDWriteTextFormat textformat);

        private GetFontStyleSignature getFontStyle;

        /// <summary>
        /// Get the font stretch.
        /// </summary>
        /// <remarks>
        /// <code>
        /// STDMETHOD_(DWRITE_FONT_STRETCH, GetFontStretch)() PURE;
        /// </code>
        /// </remarks>
        [ComMethod(Index = 24)]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate FontStretch GetFontStretchSignature(IDWriteTextFormat textformat);

        private GetFontStretchSignature getFontStretch;

        /// <summary>
        /// Get the font em height.
        /// </summary>
        /// <remarks>
        /// <code>
        /// STDMETHOD_(FLOAT, GetFontSize)() PURE;
        /// </code>
        /// </remarks>
        [ComMethod(Index = 25)]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate float GetFontSizeSignature(IDWriteTextFormat textformat);

        private GetFontSizeSignature getFontSize;

        #endregion

        internal TextFormat(IntPtr objPtr)
        {
            comObject =
                (IDWriteTextFormat) Marshal.GetObjectForIUnknown(objPtr);
            Handle = objPtr;
            InitComMethods();
        }

        internal void InitComMethods()
        {
            bool result;

            result = ComHelper.GetMethod(comObject, 3, out setTextAlignment);
            Debug.Assert(result, "Fail to get COM method");

            result = ComHelper.GetMethod(comObject, 11, out getTextAlignment);
            Debug.Assert(result, "Fail to get COM method");

            result = ComHelper.GetMethod(comObject, 20, out getFontFamilyNameLength);
            Debug.Assert(result, "Fail to get COM method");

            result = ComHelper.GetMethod(comObject, 21, out getFontFamilyName);
            Debug.Assert(result, "Fail to get COM method");

            result = ComHelper.GetMethod(comObject, 22, out getFontWeight);
            Debug.Assert(result, "Fail to get COM method");
            
            result = ComHelper.GetMethod(comObject, 23, out getFontStyle);
            Debug.Assert(result, "Fail to get COM method");

            result = ComHelper.GetMethod(comObject, 24, out getFontStretch);
            Debug.Assert(result, "Fail to get COM method");

            result = ComHelper.GetMethod(comObject, 25, out getFontSize);
            Debug.Assert(result, "Fail to get COM method");
        }

        /// <summary>
        /// Finalizer, called only if the user forget to call Dispose
        /// </summary>
        ~TextFormat()
        {
            Debug.WriteLine("TextFormat dispose not called.");
            Release();
        }

        private void Release()
        {
            if(comObject == null)
            {
                throw new InvalidOperationException("The com interface instance has been released!");
            }
            Marshal.ReleaseComObject(comObject);
            comObject = null;
        }

        #endregion

        #region COM Method wrappers

        private void SetTextAlignment(TextAlignment textAlignment)
        {
            var hr = this.setTextAlignment(comObject, textAlignment);
            Marshal.ThrowExceptionForHR(hr);
        }

        private TextAlignment GetTextAlignment()
        {
            return this.getTextAlignment(comObject);
        }

        //TODO

        private float GetFontSize()
        {
            return getFontSize(comObject);
        }

        private string GetFontFamilyName()
        {
            var length = getFontFamilyNameLength(comObject) + 1;
            char[] buffer = new char[length];
            var hr = getFontFamilyName(comObject, buffer, length);
            Marshal.ThrowExceptionForHR(hr);
            var result = new string(buffer).TrimEnd('\0');
            return result;
        }

        private FontWeight GetFontWeight()
        {
            return getFontWeight(comObject);
        }

        private FontStyle GetFontStyle()
        {
            return getFontStyle(comObject);
        }

        private FontStretch GetFontStretch()
        {
            return getFontStretch(comObject);
        }

        #endregion

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            Release();
            //GC don't need to call finalizer because the com interface has been released.
            GC.SuppressFinalize(this);
        }

        #endregion
    }

    /// <summary>
    /// Alignment of paragraph text along the reading direction axis relative to 
    /// the leading and trailing edge of the layout box.
    /// </summary>
    public enum TextAlignment
    {
        /// <summary>
        /// The leading edge of the paragraph text is aligned to the layout box's leading edge.
        /// </summary>
        Leading,

        /// <summary>
        /// The trailing edge of the paragraph text is aligned to the layout box's trailing edge.
        /// </summary>
        Trailing,

        /// <summary>
        /// The center of the paragraph text is aligned to the center of the layout box.
        /// </summary>
        Center,

        /// <summary>
        /// Align text to the leading side, and also justify text to fill the lines.
        /// </summary>
        Justified
    };

    /// <summary>
    /// The font weight enumeration describes common values for degree of blackness or thickness of strokes of characters in a font.
    /// Font weight values less than 1 or greater than 999 are considered to be invalid, and they are rejected by font API functions.
    /// </summary>
    public enum FontWeight
    {
        /// <summary>
        /// Predefined font weight : Thin (100).
        /// </summary>
        Thin = 100,

        /// <summary>
        /// Predefined font weight : Extra-light (200).
        /// </summary>
        ExtraLight = 200,

        /// <summary>
        /// Predefined font weight : Ultra-light (200).
        /// </summary>
        UltraLight = 200,

        /// <summary>
        /// Predefined font weight : Light (300).
        /// </summary>
        Light = 300,

        /// <summary>
        /// Predefined font weight : Semi-light (350).
        /// </summary>
        SemiLight = 350,

        /// <summary>
        /// Predefined font weight : Normal (400).
        /// </summary>
        Normal = 400,

        /// <summary>
        /// Predefined font weight : Regular (400).
        /// </summary>
        Regular = 400,

        /// <summary>
        /// Predefined font weight : Medium (500).
        /// </summary>
        Medium = 500,

        /// <summary>
        /// Predefined font weight : Demi-bold (600).
        /// </summary>
        DemiBold = 600,

        /// <summary>
        /// Predefined font weight : Semi-bold (600).
        /// </summary>
        SemiBold = 600,

        /// <summary>
        /// Predefined font weight : Bold (700).
        /// </summary>
        Bold = 700,

        /// <summary>
        /// Predefined font weight : Extra-bold (800).
        /// </summary>
        ExtraBold = 800,

        /// <summary>
        /// Predefined font weight : Ultra-bold (800).
        /// </summary>
        UltraBold = 800,

        /// <summary>
        /// Predefined font weight : Black (900).
        /// </summary>
        Black = 900,

        /// <summary>
        /// Predefined font weight : Heavy (900).
        /// </summary>
        Heavy = 900,

        /// <summary>
        /// Predefined font weight : Extra-black (950).
        /// </summary>
        ExtraBlack = 950,

        /// <summary>
        /// Predefined font weight : Ultra-black (950).
        /// </summary>
        UltraBlack = 950
    }

    /// <summary>
    /// The font stretch enumeration describes relative change from the normal aspect ratio
    /// as specified by a font designer for the glyphs in a font.
    /// Values less than 1 or greater than 9 are considered to be invalid, and they are rejected by font API functions.
    /// </summary>
    public enum FontStretch
    {
        /// <summary>
        /// Predefined font stretch : Not known (0).
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Predefined font stretch : Ultra-condensed (1).
        /// </summary>
        UltraCondensed = 1,

        /// <summary>
        /// Predefined font stretch : Extra-condensed (2).
        /// </summary>
        ExtraCondensed = 2,

        /// <summary>
        /// Predefined font stretch : Condensed (3).
        /// </summary>
        Condensed = 3,

        /// <summary>
        /// Predefined font stretch : Semi-condensed (4).
        /// </summary>
        SemiCondensed = 4,

        /// <summary>
        /// Predefined font stretch : Normal (5).
        /// </summary>
        Normal = 5,

        /// <summary>
        /// Predefined font stretch : Medium (5).
        /// </summary>
        Medium = 5,

        /// <summary>
        /// Predefined font stretch : Semi-expanded (6).
        /// </summary>
        SemiExpanded = 6,

        /// <summary>
        /// Predefined font stretch : Expanded (7).
        /// </summary>
        Expanded = 7,

        /// <summary>
        /// Predefined font stretch : Extra-expanded (8).
        /// </summary>
        ExtraExpanded = 8,

        /// <summary>
        /// Predefined font stretch : Ultra-expanded (9).
        /// </summary>
        UltraExpanded = 9
    };

    /// <summary>
    /// The font style enumeration describes the slope style of a font face, such as Normal, Italic or Oblique.
    /// Values other than the ones defined in the enumeration are considered to be invalid, and they are rejected by font API functions.
    /// </summary>
    public enum FontStyle
    {
        /// <summary>
        /// Font slope style : Normal.
        /// </summary>
        Normal,

        /// <summary>
        /// Font slope style : Oblique.
        /// </summary>
        Oblique,

        /// <summary>
        /// Font slope style : Italic.
        /// </summary>
        Italic
    };
}