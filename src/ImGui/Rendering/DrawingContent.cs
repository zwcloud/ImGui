using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ImGui.Rendering.Composition;

namespace ImGui.Rendering
{
    /// <summary>
    /// Drawing content contains drawing instructions.
    /// </summary>
    /// <remarks>
    /// Drawing content contains intermediate rendering data from two sources:
    /// 1. <see cref="Geometry.GetPathGeometryData"/>,
    /// 2. <see cref="Node.drawingContent"/>
    /// Further, DrawingContent will be converted to Mesh or TextMesh for rendering with GPU in <see cref="ImGui.OSAbstraction.Graphics.IRenderer"/> .
    /// </remarks>
    internal class DrawingContent
    {
        public int AddReferenceToResource(object resource)
        {
            dependentResources.Add(resource);
            return dependentResources.Count - 1;
        }

        public unsafe void WriteRecord(RecordType type, byte* recordData, int recordSize)
        {
            Debug.Assert(recordSize >= 0);

            // The records must always be padded to be QWORD aligned.
            Debug.Assert((curOffset % 8) == 0);
            Debug.Assert((recordSize % 8) == 0);
            Debug.Assert((sizeof(RecordHeader) % 8) == 0);

            int totalSize, newOffset;
            checked
            {
                totalSize = recordSize + sizeof(RecordHeader);
                newOffset = curOffset + totalSize;
            }

            // Do we need to increase the buffer size?
            // Yes, if there's no buffer or if the buffer is too small.
            if ((buffer == null) || (newOffset > buffer.Length))
            {
                EnsureBuffer(newOffset);
            }

            // At this point, _buffer must be non-null and
            // _buffer.Length must be >= newOffset
            Debug.Assert((buffer != null) && (buffer.Length >= newOffset));

            // Also, because pinning a 0-length buffer fails, we assert this too.
            Debug.Assert(buffer.Length > 0);

            RecordHeader header;

            header.Size = totalSize;
            header.Type = type;

            Marshal.Copy((IntPtr)(&header), this.buffer, curOffset, sizeof(RecordHeader));
            Marshal.Copy((IntPtr)recordData, this.buffer, curOffset + sizeof(RecordHeader), recordSize);

            curOffset += totalSize;
        }

        /// <summary>
        /// EnsureBuffer - this method ensures that the capacity is at least equal to cbRequiredSize.
        /// </summary>
        /// <param name="cbRequiredSize"> int - the new minimum size required.  Must be >= 0. </param>
        private void EnsureBuffer(int cbRequiredSize)
        {
            Debug.Assert(cbRequiredSize >= 0);

            // If we don't have a buffer, this is easy: we simply allocate a new one of the appropriate size.
            if (buffer == null)
            {
                buffer = new byte[cbRequiredSize];
            }
            else
            {
                // For efficiency, we shouldn't have been called if there's already enough room
                Debug.Assert(buffer.Length < cbRequiredSize);

                // The new size will be 1.5 x the previous size, or the min size required (whichever is larger)
                // We perform the 1.5x math by taking 2x of the length and subtracting 0.5x the length because
                // the 2x and 0.5x can be figured via shifts.  This is ~2x faster than performing the floating
                // point math.
                int newSize = Math.Max((buffer.Length << 1) - (buffer.Length >> 1), cbRequiredSize);

                // This is a double-check against the math above - if newSize isn't at least cbRequiredSize,
                // this growth function is broken.
                Debug.Assert(newSize >= cbRequiredSize);

                byte[] newBuffer = new byte[newSize];

                buffer.CopyTo(newBuffer, 0);

                buffer = newBuffer;
            }
        }

        // The offset of the beginning of the next record
        // We ensure that the types in our instruction structs are correctly aligned wrt. their
        // size for read/write access, assuming that the instruction struct sits at an 8-byte
        // boundary.  Thus curOffset must always be at an 8-byte boundary to begin writing
        // an instruction.
        private int curOffset;
        private byte[] buffer;

        private List<object> dependentResources = new List<object>();
    }
}