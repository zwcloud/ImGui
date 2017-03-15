using System;
using System.Runtime.InteropServices;
using System.Linq;
using DWriteSharp.Internal;

namespace DWriteSharp
{
    public static class DWrite
    {
        private static DWriteFactory factory;
        private static DWriteFactory Factory
        {
            get
            {
                if(factory == null)
                {
                    factory = DWriteFactory.Create();
                }
                return factory;
            }
        }

        private static DirectWriteMeshTextRenderer render;
        private static DirectWriteMeshTextRenderer Render
        {
            get
            {
                if (render == null)
                {
                    render = DirectWriteMeshTextRenderer.Create();
                }
                return render;
            }
        }

        /// <summary>
        /// Create a text format object used for text layout.
        /// </summary>
        /// <param name="fontFamilyName">Name of the font family</param>
        /// <param name="fontWeight">Font weight</param>
        /// <param name="fontStyle">Font style</param>
        /// <param name="fontStretch">Font stretch</param>
        /// <param name="fontSizeInDip">Logical size of the font in DIP units. A DIP ("device-independent pixel") equals 1/96 inch.</param>
        /// <param name="localeName">Locale name(optional)</param>
        /// TODO understand the meaning of Locale name
        /// <returns> newly created text format object </returns>
        public static TextFormat CreateTextFormat(
            string fontFamilyName,
            FontWeight fontWeight, FontStyle fontStyle, FontStretch fontStretch,
            float fontSizeInDip,
            string localeName = "en-us")
        {
            var ptr = Factory.CreateTextFormat(fontFamilyName, IntPtr.Zero,
                fontWeight,
                fontStyle, fontStretch, fontSizeInDip,
                localeName);
            var dWriteTextFormat = new TextFormat(ptr);
            return dWriteTextFormat;
        }

        /// <summary>
        /// CreateTextLayout takes a string, format, and associated constraints
        /// and produces an object representing the fully analyzed
        /// and formatted result.
        /// </summary>
        /// <param name="text">The text to layout.</param>
        /// <param name="textFormat">The format to apply to the string.</param>
        /// <param name="maxWidth">Width of the layout box.</param>
        /// <param name="maxHeight">Height of the layout box.</param>
        /// <returns>
        /// The resultant object.
        /// </returns>
        public static TextLayout CreateTextLayout(string text, TextFormat textFormat, int maxWidth, int maxHeight)
        {
            var ptr = Factory.CreateTextLayout(text,
            text.Length, textFormat.Handle, maxWidth, maxHeight);
            var dWriteTextLayout = new TextLayout(ptr);
            return dWriteTextLayout;
        }

        public static void RenderLayoutToMesh(TextLayout textLayout, float offsetX, float offsetY,
            PointAdder pointAdder, BezierAdder bezierAdder, PathCloser pathCloser, FigureBeginner figureBeginner, FigureEnder figureEnder)
        {
            Context context = new Context { PointAdder = pointAdder, BezierAdder = bezierAdder, PathCloser = pathCloser,
                FigureBeginner = figureBeginner,
                FigureEnder = figureEnder
            };
            textLayout.RenderToMesh(ref context, Render, offsetX, offsetY);
            Render.ClearBuffer();
        }
    }
}