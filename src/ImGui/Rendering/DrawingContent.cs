using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ImGui.OSAbstraction.Graphics;
using ImGui.OSAbstraction.Text;
using ImGui.Rendering.Composition;

namespace ImGui.Rendering
{
    /// <summary>
    /// Drawing content contains drawing instructions.
    /// </summary>
    /// <remarks>
    /// Drawing content contains intermediate rendering data from two sources:
    /// 1. <see cref="Geometry.GetPathGeometryData"/>,
    /// 2. <see cref="Node.content"/>
    /// Further, DrawingContent will be converted to Mesh or TextMesh for rendering with GPU in <see cref="ImGui.OSAbstraction.Graphics.IRenderer"/> .
    /// </remarks>
    internal partial class DrawingContent
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

        public void ReadAllRecords(GeometryRenderer ctx)
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
                                        (Brush)DependentLookup(data->BrushIndex),
                                        (Pen)DependentLookup(data->PenIndex),
                                        data->Rectangle
                                        );
                                }
                                break;
                                case RecordType.DrawRoundedRectangle:
                                {
                                    DrawRoundedRectangleCommand* data = (DrawRoundedRectangleCommand*)(pCur + sizeof(RecordHeader));
                                    ctx.DrawRoundedRectangle(
                                        (Brush)DependentLookup(data->BrushIndex),
                                        (Pen)DependentLookup(data->PenIndex),
                                        data->Rectangle,
                                        data->radiusX,
                                        data->radiusY
                                    );
                                }
                                    break;
                                case RecordType.DrawEllipse:
                                {
                                    DrawEllipseCommand* data = (DrawEllipseCommand*)(pCur + sizeof(RecordHeader));
                                    ctx.DrawEllipse(
                                        (Brush)DependentLookup(data->BrushIndex),
                                        (Pen)DependentLookup(data->PenIndex),
                                        data->Center,
                                        data->RadiusX,
                                        data->RadiusY
                                    );
                                }
                                    break;
                                case RecordType.DrawGlyphRun:
                                {
                                    DrawGlyphRunCommand* data = (DrawGlyphRunCommand*)(pCur + sizeof(RecordHeader));
                                    ctx.DrawGlyphRun(
                                        (Brush)DependentLookup(data->BrushIndex),
                                        (GlyphRun)DependentLookup(data->GlyphRunIndex)
                                    );
                                }
                                    break;
                                case RecordType.DrawText:
                                {
                                    DrawTextCommand* data = (DrawTextCommand*)(pCur + sizeof(RecordHeader));
                                    ctx.DrawText(
                                        (Brush)DependentLookup(data->BrushIndex),
                                        (FormattedText)DependentLookup(data->FormattedTextIndex)
                                    );
                                }
                                    break;
                                case RecordType.DrawGeometry:
                                {
                                    DrawGeometryCommand* data = (DrawGeometryCommand*)(pCur + sizeof(RecordHeader));
                                    ctx.DrawGeometry(
                                        (Brush)DependentLookup(data->BrushIndex),
                                        (Pen)DependentLookup(data->PenIndex),
                                        (Geometry)DependentLookup(data->GeometryIndex)
                                    );
                                }
                                    break;
                                case RecordType.DrawImage:
                                {
                                    DrawImageCommand* data = (DrawImageCommand*)(pCur + sizeof(RecordHeader));
                                    ctx.DrawImage(
                                        (ITexture)DependentLookup(data->ImageSourceIndex),
                                        data->rectangle,
                                        data->UVMin,
                                        data->UVMax
                                    );
                                }
                                    break;
                                case RecordType.DrawSlicedImage:
                                {
                                    DrawSlicedImageCommand* data = (DrawSlicedImageCommand*)(pCur + sizeof(RecordHeader));
                                    ctx.DrawImage(
                                        (ITexture)DependentLookup(data->ImageSourceIndex),
                                        data->Rectangle,
                                        (data->sliceLeft, data->sliceTop, data->sliceRight, data->sliceBottom)
                                    );
                                }
                                break;
                                case RecordType.PushClip:
                                {
                                    PushClipCommand* data = (PushClipCommand*)(pCur + sizeof(RecordHeader));
                                    ctx.PushClip(
                                        (Geometry)DependentLookup(data->ClipGeometryIndex)
                                    );
                                }
                                    break;
                                case RecordType.Pop:
                                {
                                    ctx.Pop();
                                }
                                break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                                break;
                            }

                            pCur += pCurRecord->Size;
                        }
                    }
                }
            }
        }

        public void AppendRecords(DrawingContent newContent)
        {
            var curOffset = newContent.curOffset;
            var dependentResources = newContent.dependentResources;
            var buffer = newContent.buffer;

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
                    fixed (byte* pByte = buffer)
                    {
                        // This pointer points to the current read point in the
                        // instruction stream.
                        byte* pCur = pByte;

                        // This points to the first byte past the end of the
                        // instruction stream (i.e. when to stop)
                        byte* pEndOfInstructions = pByte + curOffset;

                        // Iterate across the entire list of instructions, stopping at the
                        // end or when it has signalled a stop.
                        while (pCur < pEndOfInstructions)
                        {
                            RecordHeader* pCurRecord = (RecordHeader*)pCur;

                            switch (pCurRecord->Type)
                            {
                                case RecordType.DrawLine:
                                {
                                    DrawLineCommand* data = (DrawLineCommand*)(pCur + sizeof(RecordHeader));
                                    var pen = (Pen)newContent.DependentLookup(data->PenIndex);
                                    int penIndexUint = this.dependentResources.IndexOf(pen);
                                    if (penIndexUint < 0)
                                    {
                                        data->PenIndex = this.AddDependentResource(pen);
                                    }
                                    else
                                    {
                                        data->PenIndex = (uint)penIndexUint;
                                    }
                                    this.WriteRecord(RecordType.DrawLine, (byte*)data, sizeof(DrawLineCommand));
                                }
                                break;
                                case RecordType.DrawRectangle:
                                {
                                    DrawRectangleCommand* data = (DrawRectangleCommand*)(pCur + sizeof(RecordHeader));

                                    var brush = (Brush)newContent.DependentLookup(data->BrushIndex);
                                    int brushIndexUint = this.dependentResources.IndexOf(brush);
                                    if (brushIndexUint < 0)
                                    {
                                        data->BrushIndex = this.AddDependentResource(brush);
                                    }
                                    else
                                    {
                                        data->BrushIndex = (uint)brushIndexUint;
                                    }

                                    var pen = (Pen)newContent.DependentLookup(data->PenIndex);
                                    int penIndexUint = this.dependentResources.IndexOf(pen);
                                    if (penIndexUint < 0)
                                    {
                                        data->PenIndex = this.AddDependentResource(pen);
                                    }
                                    else
                                    {
                                        data->PenIndex = (uint)penIndexUint;
                                    }

                                    this.WriteRecord(RecordType.DrawRectangle, (byte*)data, sizeof(DrawRectangleCommand));
                                }
                                break;
                                case RecordType.DrawRoundedRectangle:
                                {
                                    DrawRoundedRectangleCommand* data = (DrawRoundedRectangleCommand*)(pCur + sizeof(RecordHeader));

                                    var brush = (Brush)newContent.DependentLookup(data->BrushIndex);
                                    int brushIndexUint = this.dependentResources.IndexOf(brush);
                                    if (brushIndexUint < 0)
                                    {
                                        data->BrushIndex = this.AddDependentResource(brush);
                                    }
                                    else
                                    {
                                        data->BrushIndex = (uint)brushIndexUint;
                                    }

                                    var pen = (Pen)newContent.DependentLookup(data->PenIndex);
                                    int penIndexUint = this.dependentResources.IndexOf(pen);
                                    if (penIndexUint < 0)
                                    {
                                        data->PenIndex = this.AddDependentResource(pen);
                                    }
                                    else
                                    {
                                        data->PenIndex = (uint)penIndexUint;
                                    }

                                    this.WriteRecord(RecordType.DrawRoundedRectangle, (byte*)data,
                                        sizeof(DrawRoundedRectangleCommand));
                                }
                                break;
                                case RecordType.DrawEllipse:
                                {
                                    DrawEllipseCommand* data = (DrawEllipseCommand*)(pCur + sizeof(RecordHeader));

                                    var brush = (Brush)newContent.DependentLookup(data->BrushIndex);
                                    int brushIndexUint = this.dependentResources.IndexOf(brush);
                                    if (brushIndexUint < 0)
                                    {
                                        data->BrushIndex = this.AddDependentResource(brush);
                                    }
                                    else
                                    {
                                        data->BrushIndex = (uint)brushIndexUint;
                                    }

                                    var pen = (Pen)newContent.DependentLookup(data->PenIndex);
                                    int penIndexUint = this.dependentResources.IndexOf(pen);
                                    if (penIndexUint < 0)
                                    {
                                        data->PenIndex = this.AddDependentResource(pen);
                                    }
                                    else
                                    {
                                        data->PenIndex = (uint)penIndexUint;
                                    }

                                    this.WriteRecord(RecordType.DrawEllipse, (byte*)data, sizeof(DrawEllipseCommand));
                                }
                                break;
                                case RecordType.DrawGlyphRun:
                                {
                                    DrawGlyphRunCommand* data = (DrawGlyphRunCommand*)(pCur + sizeof(RecordHeader));

                                    var brush = (Brush)newContent.DependentLookup(data->BrushIndex);
                                    int brushIndexUint = this.dependentResources.IndexOf(brush);
                                    if (brushIndexUint < 0)
                                    {
                                        data->BrushIndex = this.AddDependentResource(brush);
                                    }
                                    else
                                    {
                                        data->BrushIndex = (uint)brushIndexUint;
                                    }

                                    var glyphRun = (GlyphRun)newContent.DependentLookup(data->GlyphRunIndex);
                                    int glyphRunIndexUint = this.dependentResources.IndexOf(glyphRun);
                                    if (glyphRunIndexUint < 0)
                                    {
                                        data->GlyphRunIndex = this.AddDependentResource(glyphRun);
                                    }
                                    else
                                    {
                                        data->GlyphRunIndex = (uint)glyphRunIndexUint;
                                    }

                                    this.WriteRecord(RecordType.DrawGlyphRun, (byte*)data, sizeof(DrawGlyphRunCommand));
                                }
                                break;
                                case RecordType.DrawText:
                                {
                                    DrawTextCommand* data = (DrawTextCommand*)(pCur + sizeof(RecordHeader));

                                    var brush = (Brush)newContent.DependentLookup(data->BrushIndex);
                                    int brushIndexUint = this.dependentResources.IndexOf(brush);
                                    if (brushIndexUint < 0)
                                    {
                                        data->BrushIndex = this.AddDependentResource(brush);
                                    }
                                    else
                                    {
                                        data->BrushIndex = (uint)brushIndexUint;
                                    }

                                    var formattedText = (FormattedText)newContent.DependentLookup(data->FormattedTextIndex);
                                    int formattedTextIndexUint = this.dependentResources.IndexOf(formattedText);
                                    if (formattedTextIndexUint < 0)
                                    {
                                        data->FormattedTextIndex = this.AddDependentResource(formattedText);
                                    }
                                    else
                                    {
                                        data->FormattedTextIndex = (uint)formattedTextIndexUint;
                                    }

                                    this.WriteRecord(RecordType.DrawText, (byte*)data, sizeof(DrawTextCommand));
                                }
                                break;
                                case RecordType.DrawGeometry:
                                {
                                    DrawGeometryCommand* data = (DrawGeometryCommand*)(pCur + sizeof(RecordHeader));

                                    var brush = (Brush)newContent.DependentLookup(data->BrushIndex);
                                    int brushIndexUint = this.dependentResources.IndexOf(brush);
                                    if (brushIndexUint < 0)
                                    {
                                        data->BrushIndex = this.AddDependentResource(brush);
                                    }
                                    else
                                    {
                                        data->BrushIndex = (uint)brushIndexUint;
                                    }

                                    var pen = (Pen)newContent.DependentLookup(data->PenIndex);
                                    int penIndexUint = this.dependentResources.IndexOf(pen);
                                    if (penIndexUint < 0)
                                    {
                                        data->PenIndex = this.AddDependentResource(pen);
                                    }
                                    else
                                    {
                                        data->PenIndex = (uint)penIndexUint;
                                    }

                                    var geometry = (Geometry)newContent.DependentLookup(data->GeometryIndex);
                                    int geometryIndexUint = this.dependentResources.IndexOf(geometry);
                                    if (geometryIndexUint < 0)
                                    {
                                        data->GeometryIndex = this.AddDependentResource(geometry);
                                    }
                                    else
                                    {
                                        data->GeometryIndex = (uint)geometryIndexUint;
                                    }

                                    this.WriteRecord(RecordType.DrawGeometry, (byte*)data, sizeof(DrawGeometryCommand));
                                }
                                break;
                                case RecordType.DrawImage:
                                {
                                    DrawImageCommand* data = (DrawImageCommand*)(pCur + sizeof(RecordHeader));

                                    var imageSource = (Brush)newContent.DependentLookup(data->ImageSourceIndex);
                                    int imageSourceIndexUint = this.dependentResources.IndexOf(imageSource);
                                    if (imageSourceIndexUint < 0)
                                    {
                                        data->ImageSourceIndex = this.AddDependentResource(imageSource);
                                    }
                                    else
                                    {
                                        data->ImageSourceIndex = (uint)imageSourceIndexUint;
                                    }

                                    this.WriteRecord(RecordType.DrawImage, (byte*)data, sizeof(DrawImageCommand));
                                }
                                break;
                                case RecordType.DrawSlicedImage:
                                {
                                    DrawSlicedImageCommand* data = (DrawSlicedImageCommand*)(pCur + sizeof(RecordHeader));

                                    var imageSource = (Brush)newContent.DependentLookup(data->ImageSourceIndex);
                                    int imageSourceIndexUint = this.dependentResources.IndexOf(imageSource);
                                    if (imageSourceIndexUint < 0)
                                    {
                                        data->ImageSourceIndex = this.AddDependentResource(imageSource);
                                    }
                                    else
                                    {
                                        data->ImageSourceIndex = (uint)imageSourceIndexUint;
                                    }

                                    this.WriteRecord(RecordType.DrawSlicedImage, (byte*)data, sizeof(DrawSlicedImageCommand));
                                }
                                break;
                                case RecordType.PushClip:
                                {
                                    PushClipCommand* data = (PushClipCommand*)(pCur + sizeof(RecordHeader));

                                    var geometry = (Geometry)newContent.DependentLookup(data->ClipGeometryIndex);
                                    int geometryIndexUint = this.dependentResources.IndexOf(geometry);
                                    if (geometryIndexUint < 0)
                                    {
                                        data->ClipGeometryIndex = this.AddDependentResource(geometry);
                                    }
                                    else
                                    {
                                        data->ClipGeometryIndex = (uint)geometryIndexUint;
                                    }

                                    this.WriteRecord(RecordType.PushClip, (byte*)data, sizeof(PushClipCommand));
                                }
                                break;
                                case RecordType.Pop:
                                {
                                    PopCommand* data = (PopCommand*)(pCur + sizeof(RecordHeader));
                                    this.WriteRecord(RecordType.Pop, (byte*)data, 0);
                                }
                                break;
                                default:
                                {
                                    throw new ArgumentOutOfRangeException();
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