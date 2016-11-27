using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace ImGui
{
    public partial class Texture
    {
        public static readonly byte[] PngHeaderEightBytes =
        {
            0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A
        };

        internal static Dictionary<string, Texture> _presets;
        
        internal ImageSharp.Color[] _data;

        internal IntPtr Pointer { get { return Marshal.UnsafeAddrOfPinnedArrayElement(_data, 0); } }

        public Texture(string filePath)
        {
            using (var fileStream = File.Open(filePath, FileMode.Open))
            using (var binaryReader = new BinaryReader(fileStream))
            {
                var headEightBytes = binaryReader.ReadBytes(8);
                var isPngFile = headEightBytes.SequenceEqual(PngHeaderEightBytes);
                if (!isPngFile)
                {
                    throw new NotSupportedException("Only PNG format is supported.");
                }
                var imgae = new ImageSharp.Image(fileStream);
                _data = imgae.Pixels;
            }
        }

        public Texture(byte[] pngFileData)
        {
            throw new NotImplementedException();
            byte[] headEightBytes = new byte[8];
            Array.Copy(pngFileData, headEightBytes, 8);
            var isPngFile = headEightBytes.SequenceEqual(PngHeaderEightBytes);
            if (!isPngFile)
            {
                throw new NotSupportedException("Only PNG format is supported.");
            }
        }

    }
}
