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
        public uint AddDependentResource(object resource)
        {
            if (resource == null)
            {
                return 0;
            }

            dependentResources.Add(resource);
            return (uint)dependentResources.Count;
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

        public void ReadAllRecords(RecordReader ctx)
        {
            // We shouldn't have any dependent resources if _curOffset is 0
            // (curOffset == 0) -> (renderData.dependentResources.Count == 0)
            Debug.Assert((curOffset > 0) || (dependentResources.Count == 0));

            // The buffer being null implies that curOffset must be 0.
            // (buffer == null) -> (curOffset == 0)
            Debug.Assert((buffer != null) || (curOffset == 0));

            // The _curOffset must be less than the length, if there is a buffer.
            Debug.Assert((buffer == null) || (curOffset <= buffer.Length));

            if (curOffset > 0)
            {
                unsafe
                {
                    fixed (byte* pByte = this.buffer)
                    {
                        // This pointer points to the current read point in the
                        // instruction stream.
                        byte* pCur = pByte;

                        // This points to the first byte past the end of the
                        // instruction stream (i.e. when to stop)
                        byte* pEndOfInstructions = pByte + curOffset;

                        // Iterate across the entire list of instructions, stopping at the
                        // end or when it has signalled a stop.
                        while ((pCur < pEndOfInstructions) && !ctx.ShouldStopWalking)
                        {
                            RecordHeader* pCurRecord = (RecordHeader*)pCur;

                            switch (pCurRecord->Type)
                            {
                                case RecordType.DrawLine:
                                {
                                    DrawLineCommand* data = (DrawLineCommand*)(pCur + sizeof(RecordHeader));
                                    ctx.DrawLine(
                                        (Pen)DependentLookup(data->PenIndex),
                                        data->StartPoint,
                                        data->EndPoint
                                        );
                                }
                                break;
                                case RecordType.DrawRectangle:
                                {
                                    DrawRectangleCommand* data = (DrawRectangleCommand*)(pCur + sizeof(RecordHeader));
                                    ctx.DrawRectangle(
                                        (Brush)DependentLookup(data->BrushHandle),
                                        (Pen)DependentLookup(data->PenHandle),
                                        data->Rectangle
                                        );
                                }
                                break;
                                case RecordType.DrawGlyphRun:
                                    throw new NotImplementedException();
                                    break;
                                case RecordType.DrawGeometry:
                                {
                                    DrawGeometryCommand* data = (DrawGeometryCommand*)(pCur + sizeof(RecordHeader));
                                    ctx.DrawGeometry(
                                        (Brush)DependentLookup(data->hBrush),
                                        (Pen)DependentLookup(data->hPen),
                                        (Geometry)DependentLookup(data->hGeometry)
                                    );
                                }
                                    break;
                                default:
                                {
                                    Debug.Assert(false);
                                }
                                break;
                            }

                            pCur += pCurRecord->Size;
                        }
                    }
                }
            }
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

        /// <summary>
        /// DependentLookup - given an index into the dependent resource array,
        /// we return null if the index is 0, else we return the dependent at index - 1.
        /// </summary>
        /// <param name="index"> uint - 1-based index into the dependent array, 0 means "no lookup". </param>
        private object DependentLookup(uint index)
        {
            Debug.Assert(index <= (uint)Int32.MaxValue);

            if (index == 0)
            {
                return null;
            }

            Debug.Assert(dependentResources.Count >= index);

            return dependentResources[(int)index - 1];
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