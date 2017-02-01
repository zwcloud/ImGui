using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using DWriteSharp.Internal;

namespace DWriteSharp
{
    /// <summary>
    /// DirectWrite TextLayout
    /// </summary>
    public class TextLayout : IDisposable
    {
        #region API

        /// <summary>
        /// Layout maximum width
        /// </summary>
        public int MaxWidth
        {
            get { return (int)GetMaxWidth(); }
            set { SetMaxWidth(value); }
        }

        /// <summary>
        /// Layout maximum height
        /// </summary>
        public int MaxHeight
        {
            get { return (int)GetMaxHeight(); }
            set { SetMaxHeight(value); }
        }


        /// <summary>
        /// Font family name
        /// </summary>
        public string FontFamilyName
        {
            get { return GetFontFamilyName(); }
            set { SetFontFamilyName(value); }
        }

        /// <summary>
        /// Font weight
        /// </summary>
        public FontWeight FontWeight
        {
            get { return GetFontWeight(); }
            set { SetFontWeight(value); }
        }

        /// <summary>
        /// Font style
        /// </summary>
        public FontStyle FontStyle
        {
            get { return GetFontStyle(); }
            set { SetFontStyle(value); }
        }

        /// <summary>
        /// Font stretch
        /// </summary>
        public FontStretch FontStretch
        {
            get { return GetFontStretch(); }
            set { SetFontStretch(value); }
        }

        /// <summary>
        /// Get nearest character index from the point.
        /// </summary>
        /// <param name="pointX">X coordinate to hit-test, relative to the top-left location of the layout box.</param>
        /// <param name="pointY">Y coordinate to hit-test, relative to the top-left location of the layout box.</param>
        /// <param name="isInside">Output flag indicating whether the hit-test location is inside the text string.
        ///     When false, the position nearest the text's edge is returned.</param>
        public uint XyToIndex(float pointX, float pointY, out bool isInside)
        {
            bool isTrailingHit;
            HitTestMetrics hitTestMetrics;
            HitTestPoint(pointX, pointY, out isTrailingHit, out isInside, out hitTestMetrics);

            return hitTestMetrics.TextPosition;
        }

        /// <summary>
        /// Given a character index and whether the caret is on the leading or trailing
        /// edge of that position.
        /// </summary>
        /// <param name="textPosition"></param>
        /// <param name="isTrailingHit"></param>
        /// <param name="pointX"></param>
        /// <param name="pointY"></param>
        /// <param name="height"></param>
        public void IndexToXY(uint textPosition, bool isTrailingHit,
            out float pointX, out float pointY, out float height)
        {
            HitTestMetrics hitTestMetrics;
            HitTestTextPosition(textPosition, isTrailingHit, out pointX, out pointY, out hitTestMetrics);
            height = hitTestMetrics.Height;
        }

        public void GetRect(out float left, out float top, out float width, out float height)
        {
            TextMetrics textMetrics;
            GetMetrics(out textMetrics);
            left = textMetrics.Left;
            top = textMetrics.Top;
            width = textMetrics.WidthIncludingTrailingWhitespace;
            height = textMetrics.Height;
        }

        #endregion

        #region COM internals

        [ComImport]
        [Guid("53737037-6d14-410b-9bfe-0b182bb70961")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        internal interface IDWriteTextLayout { }//contains 25 method(inherited method not included)

        #region COM Method
        #region signature delegates of COM method

        [ComMethod(Index = 28)]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int SetMaxWidthSignature(
            IDWriteTextLayout layout,
            float maxWidth);
        private SetMaxWidthSignature setMaxWidth;

        [ComMethod(Index = 29)]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int SetMaxHeightSignature(
            IDWriteTextLayout layout,
            float maxHeight);
        private SetMaxHeightSignature setMaxHeight;
        
        [ComMethod(Index = 31)]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int SetFontFamilyNameSignature(
            IDWriteTextLayout layout,
            [MarshalAs(UnmanagedType.LPStr)]
            string fontFamilyName,
            TextRange textRange);
        private SetFontFamilyNameSignature setFontFamilyName;
        
        [ComMethod(Index = 32)]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int SetFontWeightSignature(
            IDWriteTextLayout layout,
            FontWeight fontWeight,
            TextRange textRange);
        private SetFontWeightSignature setFontWeight;

        [ComMethod(Index = 33)]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int SetFontStyleSignature(
            IDWriteTextLayout layout,
            FontStyle fontStyle,
            TextRange textRange);
        private SetFontStyleSignature setFontStyle;

        [ComMethod(Index = 34)]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int SetFontStretchSignature(
            IDWriteTextLayout layout,
            FontStretch fontStretch,
            TextRange textRange);
        private SetFontStretchSignature setFontStretch;

        [ComMethod(Index = 35)]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int SetFontSizeSignature(
            IDWriteTextLayout layout,
            float fontSize,
            TextRange textRange);
        private SetFontSizeSignature setFontSize;

        [ComMethod(Index = 42)]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate float GetMaxWidthSignature(
            IDWriteTextLayout layout);
        private GetMaxWidthSignature getMaxWidth;

        [ComMethod(Index = 43)]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate float GetMaxHeightSignature(
            IDWriteTextLayout layout);
        private GetMaxHeightSignature getMaxHeight;
        
        [ComMethod(Index = 45)]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int GetFontFamilyNameLengthSignature(
            IDWriteTextLayout layout,
            uint currentPosition,
            out uint nameLength,
            out TextRange textRange);
        private GetFontFamilyNameLengthSignature getFontFamilyNameLength;

        [ComMethod(Index = 46)]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int GetFontFamilyNameSignature(
            IDWriteTextLayout layout,
            uint currentPosition,
            [Out]
            char[] fontFamilyName,
            uint nameSize,
            out TextRange textRange);
        private GetFontFamilyNameSignature getFontFamilyName;

        [ComMethod(Index = 47)]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int GetFontWeightSignature(
            IDWriteTextLayout layout,
            uint currentPosition,
            out FontWeight fontWeight,
            out TextRange textRange);
        private GetFontWeightSignature getFontWeight;

        [ComMethod(Index = 48)]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int GetFontStyleSignature(
            IDWriteTextLayout layout,
            uint currentPosition,
            out FontStyle fontStyle,
            out TextRange textRange);
        private GetFontStyleSignature getFontStyle;
        
        [ComMethod(Index = 49)]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int GetFontStretchSignature(
            IDWriteTextLayout layout,
            uint currentPosition,
            out FontStretch fontStretch,
            out TextRange textRange);
        private GetFontStretchSignature getFontStretch;
        
        [ComMethod(Index = 50)]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int GetFontSizeSignature(
            IDWriteTextLayout layout,
            out float fontSize,
            out TextRange textRange);
        private GetFontSizeSignature getFontSize;

        [ComMethod(Index = 58)]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DrawSignature(
            IDWriteTextLayout layout,
            ref Context clientDrawingContext,
            IDWriteTextRenderer renderer,
            float originX, float originY);
        private DrawSignature draw;

        [ComMethod(Index = 60)]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int GetMetricsSignature(
            IDWriteTextLayout layout,
            out TextMetrics textMetrics);
        private GetMetricsSignature getMetrics;

        [ComMethod(Index = 64)]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int HitTestPointSignature(
            IDWriteTextLayout layout,
            float pointX,
            float pointY,
            out bool isTrailingHit,
            out bool isInside,
            out HitTestMetrics hitTestMetrics);
        private HitTestPointSignature hitTestPoint;
        
        [ComMethod(Index = 65)]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int HitTestTextPositionSignature(
            IDWriteTextLayout layout,
            uint textPosition,
            bool isTrailingHit,
            out float pointX,
            out float pointY,
            out HitTestMetrics hitTestMetrics);
        private HitTestTextPositionSignature hitTestTextPosition;

        #endregion

        internal IntPtr Handle { get; private set; }
        private IDWriteTextLayout comObject;

        internal TextLayout(IntPtr objPtr)
        {
            Handle = objPtr;
            comObject =
                (IDWriteTextLayout) Marshal.GetObjectForIUnknown(objPtr);

            InitComMethods();
        }
        internal void InitComMethods()
        {
            bool result;

            result = ComHelper.GetMethod(comObject, 28, out setMaxWidth);
            if (!result) Debug.WriteLine("Fail to get COM method at index {0}", 28);

            result = ComHelper.GetMethod(comObject, 29, out setMaxHeight);
            if (!result) Debug.WriteLine("Fail to get COM method at index {0}", 29);

            result = ComHelper.GetMethod(comObject, 31, out setFontFamilyName);
            if (!result) Debug.WriteLine("Fail to get COM method at index {0}", 31);

            result = ComHelper.GetMethod(comObject, 32, out setFontWeight);
            if (!result) Debug.WriteLine("Fail to get COM method at index {0}", 32);

            result = ComHelper.GetMethod(comObject, 33, out setFontStyle);
            if (!result) Debug.WriteLine("Fail to get COM method at index {0}", 33);

            result = ComHelper.GetMethod(comObject, 34, out setFontStretch);
            if (!result) Debug.WriteLine("Fail to get COM method at index {0}", 34);

            result = ComHelper.GetMethod(comObject, 35, out setFontSize);
            if (!result) Debug.WriteLine("Fail to get COM method at index {0}", 35);
            
            result = ComHelper.GetMethod(comObject, 42, out getMaxWidth);
            if (!result) Debug.WriteLine("Fail to get COM method at index {0}", 42);

            result = ComHelper.GetMethod(comObject, 43, out getMaxHeight);
            if (!result) Debug.WriteLine("Fail to get COM method at index {0}", 43);
            
            result = ComHelper.GetMethod(comObject, 45, out getFontFamilyNameLength);
            if (!result) Debug.WriteLine("Fail to get COM method at index {0}", 45);
            
            result = ComHelper.GetMethod(comObject, 46, out getFontFamilyName);
            if (!result) Debug.WriteLine("Fail to get COM method at index {0}", 46);
            
            result = ComHelper.GetMethod(comObject, 47, out getFontWeight);
            if (!result) Debug.WriteLine("Fail to get COM method at index {0}", 47);
            
            result = ComHelper.GetMethod(comObject, 48, out getFontStyle);
            if (!result) Debug.WriteLine("Fail to get COM method at index {0}", 48);
            
            result = ComHelper.GetMethod(comObject, 49, out getFontStretch);
            if (!result) Debug.WriteLine("Fail to get COM method at index {0}", 49);
            
            result = ComHelper.GetMethod(comObject, 50, out getFontSize);
            if (!result) Debug.WriteLine("Fail to get COM method at index {0}", 50);

            result = ComHelper.GetMethod(comObject, 58, out draw);
            if (!result) Debug.WriteLine("Fail to get COM method at index {0}", 58);

            result = ComHelper.GetMethod(comObject, 60, out getMetrics);
            if (!result) Debug.WriteLine("Fail to get COM method at index {0}", 60);

            result = ComHelper.GetMethod(comObject, 64, out hitTestPoint);
            if (!result) Debug.WriteLine("Fail to get COM method at index {0}", 64);

            result = ComHelper.GetMethod(comObject, 65, out hitTestTextPosition);
            if (!result) Debug.WriteLine("Fail to get COM method at index {0}", 65);
            
        }

        ~TextLayout()
        {
            Release();
        }

        private void Release()
        {
            if (comObject != null)
            {
                Marshal.ReleaseComObject(comObject);
                comObject = null;

                setMaxWidth = null;
                setMaxHeight = null;
                getMaxWidth = null;
                getMaxHeight = null;
                draw = null;
            }
        }

        #endregion

        #region COM Method wrappers

        /// <summary>
        /// Initiate drawing of the text.
        /// (In fact it just build a cairo path of the text. It DO NOT draw to cairo surface.)
        /// </summary>
        /// <param name="clientDrawingContext">An application defined value
        /// included in rendering callbacks.</param>
        /// <param name="renderer">The set of application-defined callbacks that do
        /// the actual rendering.</param>
        /// <param name="originX">X-coordinate of the layout's left side.</param>
        /// <param name="originY">Y-coordinate of the layout's top side.</param>
        /// <remarks>
        /// Native signature of the COM method
        /// <code>
        /// STDMETHOD(Draw)(
        ///     _In_opt_ void* clientDrawingContext,
        ///     _In_ IDWriteTextRenderer* renderer,
        ///     FLOAT originX,
        ///     FLOAT originY
        ///     ) PURE;
        /// </code>
        /// </remarks>
        private void Draw(
            ref Context clientDrawingContext, IDWriteTextRenderer renderer,
            float originX, float originY)
        {
            var hr = draw(comObject,
                ref clientDrawingContext, renderer, originX, originY);
            Marshal.ThrowExceptionForHR(hr);
        }
        
        /// <summary>
        /// Set layout maximum width
        /// </summary>
        /// <param name="maxWidth">Layout maximum width</param>
        /// <returns>
        /// Standard HRESULT error code.
        /// </returns>
        /// <remarks>
        /// <code>
        /// STDMETHOD(SetMaxWidth)(
        ///     FLOAT maxWidth
        ///     ) PURE;
        /// </code>
        /// </remarks>
        private void SetMaxWidth(float maxWidth)
        {
            Marshal.ThrowExceptionForHR(setMaxWidth(comObject,
                maxWidth));
        }

        /// <summary>
        /// Set layout maximum height
        /// </summary>
        /// <param name="maxHeight">Layout maximum height</param>
        /// <returns>
        /// Standard HRESULT error code.
        /// </returns>
        /// <remarks>
        /// <code>
        /// STDMETHOD(SetMaxHeight)(
        ///     FLOAT maxHeight
        ///     ) PURE;
        /// </code>
        /// </remarks>
        private void SetMaxHeight(float maxHeight)
        {
            Marshal.ThrowExceptionForHR(setMaxHeight(comObject,
                maxHeight));
        }

        /// <summary>
        /// Get layout maximum width
        /// </summary>
        /// STDMETHOD_(FLOAT, GetMaxWidth)() PURE;
        private float GetMaxWidth()
        {
            return getMaxWidth(comObject);
        }
        
        /// <summary>
        /// Get layout maximum height
        /// </summary>
        /// <remarks>
        /// <code>
        /// STDMETHOD_(FLOAT, GetMaxHeight)() PURE;
        /// </code>
        /// </remarks>
        private float GetMaxHeight()
        {
            return getMaxHeight(comObject);
        }

        private FontWeight GetFontWeight()
        {
            FontWeight fontWeight;
            TextRange textRangDummy;//Not used
            var hr = getFontWeight(comObject, 0, out fontWeight, out textRangDummy);
            Marshal.ThrowExceptionForHR(hr);
            return fontWeight;
        }

        private void SetFontWeight(FontWeight fontWeight)
        {
            var hr = setFontWeight(comObject, fontWeight, new TextRange {Length = 65535}
                //Affact all text
                //TODO Make sure this hack do not harm performance
                );
            Marshal.ThrowExceptionForHR(hr);
        }

        private FontStyle GetFontStyle()
        {
            FontStyle fontStyle;
            TextRange textRangDummy;//Not used
            var hr = getFontStyle(comObject, 0, out fontStyle, out textRangDummy);
            Marshal.ThrowExceptionForHR(hr);
            return fontStyle;
        }

        private void SetFontStyle(FontStyle fontStyle)
        {
            var hr = setFontStyle(comObject, fontStyle, new TextRange { Length = 65535 }
                //Affact all text
                //TODO Make sure this hack do not harm performance
                );
            Marshal.ThrowExceptionForHR(hr);
        }

        private FontStretch GetFontStretch()
        {
            FontStretch fontStretch;
            TextRange textRangDummy;//Not used
            var hr = getFontStretch(comObject, 0, out fontStretch, out textRangDummy);
            Marshal.ThrowExceptionForHR(hr);
            return fontStretch;
        }

        private void SetFontStretch(FontStretch fontStretch)
        {
            var hr = setFontStretch(comObject, fontStretch, new TextRange { Length = 65535 }
                //Affact all text
                //TODO Make sure this hack do not harm performance
                );
            Marshal.ThrowExceptionForHR(hr);
        }

        private string GetFontFamilyName()
        {
            uint length;
            TextRange textRange;
            var hr = getFontFamilyNameLength(comObject, 0, out length, out textRange);
            Marshal.ThrowExceptionForHR(hr);
            length += 1;
            char[] buffer = new char[2*length];
            hr = getFontFamilyName(comObject, 0, buffer, length, out textRange);
            Marshal.ThrowExceptionForHR(hr);
            var result = new string(buffer).TrimEnd('\0');
            return result;
        }

        private void SetFontFamilyName(string fontFamilyName)
        {
            var hr = setFontFamilyName(comObject, fontFamilyName, new TextRange { Length = 65535 });
            Marshal.ThrowExceptionForHR(hr);
        }

        /// <summary>
        /// GetMetrics retrieves overall metrics for the formatted string.
        /// </summary>
        /// <param name="textMetrics">The returned metrics.</param>
        /// <returns>
        /// Standard HRESULT error code.
        /// </returns>
        /// <remarks>
        /// <code>
        /// STDMETHOD(GetMetrics)(
        ///     _Out_ DWRITE_INLINE_OBJECT_METRICS* metrics
        ///     ) PURE;
        /// </code>
        /// Drawing effects like underline and strikethrough do not contribute
        /// to the text size, which is essentially the sum of advance widths and
        /// line heights. Additionally, visible swashes and other graphic
        /// adornments may extend outside the returned width and height.
        /// </remarks>
        private void GetMetrics(out TextMetrics textMetrics)
        {
            var hr = getMetrics(comObject, out textMetrics);
            Marshal.ThrowExceptionForHR(hr);
        }

        /// <summary>
        /// Given a coordinate (in DIPs) relative to the top-left of the layout box,
        /// this returns the corresponding hit-test metrics of the text string where
        /// the hit-test has occurred. This is useful for mapping mouse clicks to caret
        /// positions. When the given coordinate is outside the text string, the function
        /// sets the output value *isInside to false but returns the nearest character
        /// position.
        /// </summary>
        /// <param name="pointX">X coordinate to hit-test, relative to the top-left location of the layout box.</param>
        /// <param name="pointY">Y coordinate to hit-test, relative to the top-left location of the layout box.</param>
        /// <param name="isTrailingHit">Output flag indicating whether the hit-test location is at the leading or the trailing
        ///     side of the character. When the output *isInside value is set to false, this value is set according to the output
        ///     *position value to represent the edge closest to the hit-test location. </param>
        /// <param name="isInside">Output flag indicating whether the hit-test location is inside the text string.
        ///     When false, the position nearest the text's edge is returned.</param>
        /// <param name="hitTestMetrics">Output geometry fully enclosing the hit-test location. When the output *isInside value
        ///     is set to false, this structure represents the geometry enclosing the edge closest to the hit-test location.</param>
        /// <returns>
        /// Standard HRESULT error code.
        /// </returns>
        /// <remarks>
        /// <code>
        /// STDMETHOD(HitTestPoint)(
        ///    FLOAT pointX,
        ///    FLOAT pointY,
        ///    _Out_ BOOL* isTrailingHit,
        ///    _Out_ BOOL* isInside,
        ///    _Out_ DWRITE_HIT_TEST_METRICS* hitTestMetrics
        ///    ) PURE;
        /// </code>
        /// </remarks>
        private void HitTestPoint(float pointX, float pointY,
            out bool isTrailingHit, out bool isInside, out HitTestMetrics hitTestMetrics)
        {
            var hr = hitTestPoint(comObject, pointX, pointY, out isTrailingHit, out isInside, out hitTestMetrics);
            Marshal.ThrowExceptionForHR(hr);
        }

        /// <summary>
        /// Given a text position and whether the caret is on the leading or trailing
        /// edge of that position, this returns the corresponding coordinate (in DIPs)
        /// relative to the top-left of the layout box. This is most useful for drawing
        /// the caret's current position, but it could also be used to anchor an IME to the
        /// typed text or attach a floating menu near the point of interest. It may also be
        /// used to programmatically obtain the geometry of a particular text position
        /// for UI automation.
        /// </summary>
        /// <param name="textPosition">Text position to get the coordinate of.</param>
        /// <param name="isTrailingHit">Flag indicating whether the location is of the leading or the trailing side of the specified text position. </param>
        /// <param name="pointX">Output caret X, relative to the top-left of the layout box.</param>
        /// <param name="pointY">Output caret Y, relative to the top-left of the layout box.</param>
        /// <param name="hitTestMetrics">Output geometry fully enclosing the specified text position.</param>
        /// <returns>
        /// Standard HRESULT error code.
        /// </returns>
        /// <remarks>
        /// <code>
        /// STDMETHOD(HitTestTextPosition)(
        ///     UINT32 textPosition,
        ///     BOOL isTrailingHit,
        ///     _Out_ FLOAT* pointX,
        ///     _Out_ FLOAT* pointY,
        ///     _Out_ DWRITE_HIT_TEST_METRICS* hitTestMetrics
        ///     ) PURE;
        /// </code>
        /// When drawing a caret at the returned X,Y, it should be centered on X
        /// and drawn from the Y coordinate down. The height will be the size of the
        /// hit-tested text (which can vary in size within a line).
        /// Reading direction also affects which side of the character the caret is drawn.
        /// However, the returned X coordinate will be correct for either case.
        /// You can get a text length back that is larger than a single character.
        /// This happens for complex scripts when multiple characters form a single cluster,
        /// when diacritics join their base character, or when you test a surrogate pair.
        /// </remarks>
        private void HitTestTextPosition(uint textPosition, bool isTrailingHit,
            out float pointX, out float pointY, out HitTestMetrics hitTestMetrics)
        {
            var hr = hitTestTextPosition(comObject, textPosition, isTrailingHit, out pointX, out pointY,
                out hitTestMetrics);
            Marshal.ThrowExceptionForHR(hr);
        }

        #endregion

        #region Implementation of IDisposable

        public void Dispose()
        {
            Release();
            GC.SuppressFinalize(this);
        }

        #endregion

        #endregion
        
        internal void RenderToMesh(ref Context context, DirectWriteMeshTextRenderer renderer, float offsetX, float offsetY)
        {
            var dwriterender = (IDWriteTextRenderer)renderer.comObject;
            Draw(ref context, dwriterender, offsetX, offsetY);
        }
    }


    /// <summary>
    /// Specifies a range of text positions where format is applied.
    /// </summary>
    internal struct TextRange
    {
        /// <summary>
        /// The start text position of the range.
        /// </summary>
        public uint StartPosition;

        /// <summary>
        /// The number of text positions in the range.
        /// </summary>
        public uint Length;
    };

    /// <summary>
    /// Overall metrics associated with text after layout.
    /// All coordinates are in device independent pixels (DIPs).
    /// </summary>
    internal struct TextMetrics
    {
        /// <summary>
        /// Left-most point of formatted text relative to layout box
        /// (excluding any glyph overhang).
        /// </summary>
        public float Left;

        /// <summary>
        /// Top-most point of formatted text relative to layout box
        /// (excluding any glyph overhang).
        /// </summary>
        public float Top;

        /// <summary>
        /// The width of the formatted text ignoring trailing whitespace
        /// at the end of each line.
        /// </summary>
        public float Width;

        /// <summary>
        /// The width of the formatted text taking into account the
        /// trailing whitespace at the end of each line.
        /// </summary>
        public float WidthIncludingTrailingWhitespace;

        /// <summary>
        /// The height of the formatted text. The height of an empty string
        /// is determined by the size of the default font's line height.
        /// </summary>
        public float Height;

        /// <summary>
        /// Initial width given to the layout. Depending on whether the text
        /// was wrapped or not, it can be either larger or smaller than the
        /// text content width.
        /// </summary>
        public float LayoutWidth;

        /// <summary>
        /// Initial height given to the layout. Depending on the length of the
        /// text, it may be larger or smaller than the text content height.
        /// </summary>
        public float LayoutHeight;

        /// <summary>
        /// The maximum reordering count of any line of text, used
        /// to calculate the most number of hit-testing boxes needed.
        /// If the layout has no bidirectional text or no text at all,
        /// the minimum level is 1.
        /// </summary>
        public uint MaxBidiReorderingDepth;

        /// <summary>
        /// Total number of lines.
        /// </summary>
        public uint LineCount;
    };

    /// <summary>
    /// Geometry enclosing of text positions.
    /// </summary>
    internal struct HitTestMetrics
    {
        /// <summary>
        /// First text position within the geometry.
        /// </summary>
        public uint TextPosition;

        /// <summary>
        /// Number of text positions within the geometry.
        /// </summary>
        public uint Length;

        /// <summary>
        /// Left position of the top-left coordinate of the geometry.
        /// </summary>
        public float Left;

        /// <summary>
        /// Top position of the top-left coordinate of the geometry.
        /// </summary>
        public float Top;

        /// <summary>
        /// Geometry's width.
        /// </summary>
        public float Width;

        /// <summary>
        /// Geometry's height.
        /// </summary>
        public float Height;

        /// <summary>
        /// Bidi level of text positions enclosed within the geometry.
        /// </summary>
        public uint BidiLevel;

        /// <summary>
        /// Geometry encloses text?
        /// </summary>
        public bool IsText;

        /// <summary>
        /// Range is trimmed.
        /// </summary>
        public bool IsTrimmed;
    };
}