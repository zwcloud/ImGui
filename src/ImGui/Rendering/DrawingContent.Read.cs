using System;
using ImGui;
using ImGui.Rendering.Composition;

namespace ImGui.Rendering
{
    internal partial class DrawingContent
    {
        public bool ReadRecord(out DrawLineCommand record)
        {
            record = default;
            unsafe
            {
                fixed (byte* pByte = this.buffer)
                {
                    // This pointer points to the current read point in the
                    // instruction stream.
                    byte* pCur = pByte;

                    // This points to the first byte past the end of the
                    // instruction stream (i.e. when to stop)
                    byte* pEndOfInstructions = pByte + this.currentReadOffset;

                    if ((pCur >= pEndOfInstructions)) //reach end
                    {
                        return false;
                    }

                    RecordHeader* pCurRecord = (RecordHeader*) pCur;
                    if (pCurRecord->Type != RecordType.DrawLine)
                    {
                        return false;
                    }

                    DrawLineCommand* data = (DrawLineCommand*) (pCur + sizeof(RecordHeader));
                    record = *data;
                    this.currentReadOffset += pCurRecord->Size;
                    return true;
                }
            }
        }

        public bool ReadRecord(out DrawRectangleCommand record)
        {
            record = default;
            unsafe
            {
                fixed (byte* pByte = this.buffer)
                {
                    // This pointer points to the current read point in the
                    // instruction stream.
                    byte* pCur = pByte;

                    // This points to the first byte past the end of the
                    // instruction stream (i.e. when to stop)
                    byte* pEndOfInstructions = pByte + this.currentReadOffset;

                    if ((pCur >= pEndOfInstructions)) //reach end
                    {
                        return false;
                    }

                    RecordHeader* pCurRecord = (RecordHeader*) pCur;
                    if (pCurRecord->Type != RecordType.DrawRectangle)
                    {
                        return false;
                    }

                    DrawRectangleCommand* data = (DrawRectangleCommand*) (pCur + sizeof(RecordHeader));
                    record = *data;
                    this.currentReadOffset += pCurRecord->Size;
                    return true;
                }
            }
        }

        public bool ReadRecord(out DrawRoundedRectangleCommand record)
        {
            record = default;
            unsafe
            {
                fixed (byte* pByte = this.buffer)
                {
                    // This pointer points to the current read point in the
                    // instruction stream.
                    byte* pCur = pByte;

                    // This points to the first byte past the end of the
                    // instruction stream (i.e. when to stop)
                    byte* pEndOfInstructions = pByte + this.currentReadOffset;

                    if ((pCur >= pEndOfInstructions)) //reach end
                    {
                        return false;
                    }

                    RecordHeader* pCurRecord = (RecordHeader*) pCur;
                    if (pCurRecord->Type != RecordType.DrawRoundedRectangle)
                    {
                        return false;
                    }

                    DrawRoundedRectangleCommand* data = (DrawRoundedRectangleCommand*) (pCur + sizeof(RecordHeader));
                    record = *data;
                    this.currentReadOffset += pCurRecord->Size;
                    return true;
                }
            }
        }

        public bool ReadRecord(out DrawEllipseCommand record)
        {
            record = default;
            unsafe
            {
                fixed (byte* pByte = this.buffer)
                {
                    // This pointer points to the current read point in the
                    // instruction stream.
                    byte* pCur = pByte;

                    // This points to the first byte past the end of the
                    // instruction stream (i.e. when to stop)
                    byte* pEndOfInstructions = pByte + this.currentReadOffset;

                    if ((pCur >= pEndOfInstructions)) //reach end
                    {
                        return false;
                    }

                    RecordHeader* pCurRecord = (RecordHeader*)pCur;
                    if (pCurRecord->Type != RecordType.DrawRoundedRectangle)
                    {
                        return false;
                    }

                    DrawEllipseCommand* data = (DrawEllipseCommand*)(pCur + sizeof(RecordHeader));
                    record = *data;
                    this.currentReadOffset += pCurRecord->Size;
                    return true;
                }
            }
        }

        public bool ReadRecord(out DrawGeometryCommand record)
        {
            record = default;
            unsafe
            {
                fixed (byte* pByte = this.buffer)
                {
                    // This pointer points to the current read point in the
                    // instruction stream.
                    byte* pCur = pByte;

                    // This points to the first byte past the end of the
                    // instruction stream (i.e. when to stop)
                    byte* pEndOfInstructions = pByte + this.currentReadOffset;

                    if ((pCur >= pEndOfInstructions)) //reach end
                    {
                        return false;
                    }

                    RecordHeader* pCurRecord = (RecordHeader*) pCur;
                    if (pCurRecord->Type != RecordType.DrawGeometry)
                    {
                        return false;
                    }

                    DrawGeometryCommand* data = (DrawGeometryCommand*) (pCur + sizeof(RecordHeader));
                    record = *data;
                    this.currentReadOffset += pCurRecord->Size;
                    return true;
                }
            }
        }

        public bool ReadRecord(out DrawImageCommand record)
        {
            record = default;
            unsafe
            {
                fixed (byte* pByte = this.buffer)
                {
                    // This pointer points to the current read point in the
                    // instruction stream.
                    byte* pCur = pByte;

                    // This points to the first byte past the end of the
                    // instruction stream (i.e. when to stop)
                    byte* pEndOfInstructions = pByte + this.currentReadOffset;

                    if ((pCur >= pEndOfInstructions)) //reach end
                    {
                        return false;
                    }

                    RecordHeader* pCurRecord = (RecordHeader*) pCur;
                    if (pCurRecord->Type != RecordType.DrawImage)
                    {
                        return false;
                    }

                    DrawImageCommand* data = (DrawImageCommand*) (pCur + sizeof(RecordHeader));
                    record = *data;
                    this.currentReadOffset += pCurRecord->Size;
                    return true;
                }
            }
        }

        public bool ReadRecord(out DrawSlicedImageCommand record)
        {
            record = default;
            unsafe
            {
                fixed (byte* pByte = this.buffer)
                {
                    // This pointer points to the current read point in the
                    // instruction stream.
                    byte* pCur = pByte;

                    // This points to the first byte past the end of the
                    // instruction stream (i.e. when to stop)
                    byte* pEndOfInstructions = pByte + this.currentReadOffset;

                    if ((pCur >= pEndOfInstructions)) //reach end
                    {
                        return false;
                    }

                    RecordHeader* pCurRecord = (RecordHeader*) pCur;
                    if (pCurRecord->Type != RecordType.DrawSlicedImage)
                    {
                        return false;
                    }

                    DrawSlicedImageCommand* data = (DrawSlicedImageCommand*) (pCur + sizeof(RecordHeader));
                    record = *data;
                    this.currentReadOffset += pCurRecord->Size;
                    return true;
                }
            }
        }

        public bool ReadRecord(out DrawGlyphRunCommand record)
        {
            record = default;
            unsafe
            {
                fixed (byte* pByte = this.buffer)
                {
                    // This pointer points to the current read point in the
                    // instruction stream.
                    byte* pCur = pByte;

                    // This points to the first byte past the end of the
                    // instruction stream (i.e. when to stop)
                    byte* pEndOfInstructions = pByte + this.currentReadOffset;

                    if ((pCur >= pEndOfInstructions)) //reach end
                    {
                        return false;
                    }

                    RecordHeader* pCurRecord = (RecordHeader*) pCur;
                    if (pCurRecord->Type != RecordType.DrawGlyphRun)
                    {
                        return false;
                    }

                    DrawGlyphRunCommand* data = (DrawGlyphRunCommand*) (pCur + sizeof(RecordHeader));
                    record = *data;
                    this.currentReadOffset += pCurRecord->Size;
                    return true;
                }
            }
        }

        public bool ReadRecord(out DrawTextCommand record)
        {
            record = default;
            unsafe
            {
                fixed (byte* pByte = this.buffer)
                {
                    // This pointer points to the current read point in the
                    // instruction stream.
                    byte* pCur = pByte;

                    // This points to the first byte past the end of the
                    // instruction stream (i.e. when to stop)
                    byte* pEndOfInstructions = pByte + this.currentReadOffset;

                    if ((pCur >= pEndOfInstructions)) //reach end
                    {
                        return false;
                    }

                    RecordHeader* pCurRecord = (RecordHeader*) pCur;
                    if (pCurRecord->Type != RecordType.DrawText)
                    {
                        return false;
                    }

                    DrawTextCommand* data = (DrawTextCommand*) (pCur + sizeof(RecordHeader));
                    record = *data;
                    this.currentReadOffset += pCurRecord->Size;
                    return true;
                }
            }
        }

        public bool ReadRecord(out PushClipCommand record)
        {
            record = default;
            unsafe
            {
                fixed (byte* pByte = this.buffer)
                {
                    // This pointer points to the current read point in the
                    // instruction stream.
                    byte* pCur = pByte;

                    // This points to the first byte past the end of the
                    // instruction stream (i.e. when to stop)
                    byte* pEndOfInstructions = pByte + this.currentReadOffset;

                    if ((pCur >= pEndOfInstructions)) //reach end
                    {
                        return false;
                    }

                    RecordHeader* pCurRecord = (RecordHeader*)pCur;
                    if (pCurRecord->Type != RecordType.DrawGlyphRun)
                    {
                        return false;
                    }

                    PushClipCommand* data = (PushClipCommand*)(pCur + sizeof(RecordHeader));
                    record = *data;
                    this.currentReadOffset += pCurRecord->Size;
                    return true;
                }
            }
        }

        public bool ReadRecord(out PopCommand record)
        {
            record = default;
            unsafe
            {
                fixed (byte* pByte = this.buffer)
                {
                    // This pointer points to the current read point in the
                    // instruction stream.
                    byte* pCur = pByte;

                    // This points to the first byte past the end of the
                    // instruction stream (i.e. when to stop)
                    byte* pEndOfInstructions = pByte + this.currentReadOffset;

                    if ((pCur >= pEndOfInstructions)) //reach end
                    {
                        return false;
                    }

                    RecordHeader* pCurRecord = (RecordHeader*)pCur;
                    if (pCurRecord->Type != RecordType.Pop)
                    {
                        return false;
                    }

                    //pop command consumes no space

                    this.currentReadOffset += pCurRecord->Size;
                    return true;
                }
            }
        }

        public bool ReadDependentResource<T>(uint index, out T resource) where T:class
        {
            if (index >= this.dependentResources.Count)
            {
                resource = null;
                return false;
            }

            resource = (T)this.dependentResources[(int)index-1];
            return true;
        }

        private int currentReadOffset;
    }
}