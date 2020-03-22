using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Primitives;
using SixLabors.ImageSharp.Processing;
using System;

namespace ImageSharp.Extension
{
    /// <summary>
    /// Class to perform simple image comparisons.
    /// </summary>
    public static class ImageComparer
    {
        const int DefaultScalingFactor = 16;
        const int DefaultSegmentThreshold = 3;
        const float DefaultImageThreshold = 0f;

        public static void VisualComparer<TColorA, TColorB>(Image<TColorA> expected, Image<TColorB> actual, float imageTheshold = DefaultImageThreshold, byte segmentThreshold = DefaultSegmentThreshold, int scalingFactor = DefaultScalingFactor)
           where TColorA : struct, IPixel<TColorA>
           where TColorB : struct, IPixel<TColorB>
        {
            var percentage = expected.PercentageDifference(actual, segmentThreshold, scalingFactor);

            System.Diagnostics.Debug.Assert(0 < percentage && percentage < imageTheshold);
        }

        public static float PercentageDifference<TColorA, TColorB>(this Image<TColorA> source, Image<TColorB> target, byte segmentThreshold = DefaultSegmentThreshold, int scalingFactor = DefaultScalingFactor)
            where TColorA : struct, IPixel<TColorA>
            where TColorB : struct, IPixel<TColorB>
        {
            // code adapted from https://www.codeproject.com/Articles/374386/Simple-image-comparison-in-NET
            DenseMatrix<byte> differences = GetDifferences(source, target, scalingFactor);

            int diffPixels = 0;

            foreach (byte b in differences.Data)
            {
                if (b > segmentThreshold) { diffPixels++; }
            }

            return diffPixels / 256f;
        }

        private static DenseMatrix<byte> GetDifferences<TColorA, TColorB>(Image<TColorA> source, Image<TColorB> target, int scalingFactor)
            where TColorA : struct, IPixel<TColorA>
            where TColorB : struct, IPixel<TColorB>
        {
            DenseMatrix<byte> differences = new DenseMatrix<byte>(scalingFactor, scalingFactor);
            DenseMatrix<byte> firstGray = source.GetGrayScaleValues(scalingFactor);
            DenseMatrix<byte> secondGray = target.GetGrayScaleValues(scalingFactor);

            for (int y = 0; y < scalingFactor; y++)
            {
                for (int x = 0; x < scalingFactor; x++)
                {
                    differences[x, y] = (byte)Math.Abs(firstGray[x, y] - secondGray[x, y]);
                }
            }

            return differences;
        }

        private static DenseMatrix<byte> GetGrayScaleValues<TColorA>(this Image<TColorA> source, int scalingFactor)
            where TColorA : struct, IPixel<TColorA>
        {
            Rgba32 pixel = Rgba32.Black;
            var clonedImage = source.Clone();
            clonedImage.Mutate(context => context.Resize(scalingFactor, scalingFactor));
            clonedImage.Mutate(context => context.Grayscale());
            using (clonedImage)
            {
                DenseMatrix<byte> grayScale = new DenseMatrix<byte>(scalingFactor, scalingFactor);
                for (int y = 0; y < scalingFactor; y++)
                {
                    for (int x = 0; x < scalingFactor; x++)
                    {
                        clonedImage[x, y].ToRgba32(ref pixel);
                        grayScale[x, y] = pixel.R;
                    }
                }

                return grayScale;
            }
        }
    }
}
