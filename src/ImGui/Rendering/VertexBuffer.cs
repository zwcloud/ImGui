using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ImGui
{
    internal class VertexBuffer
    {
        private DrawVertex[] data;

        private int size;

        private int capacity;

        public int Capacity => this.capacity;

        private GCHandle handle;//TODO free this handle finally
        private unsafe DrawVertex* ptr;

        public VertexBuffer(int capacity)
        {
            this.data = new DrawVertex[capacity];
            this.capacity = capacity;
            this.size = 0;
            this.UpdatePointer();
        }

        public IntPtr Pointer
        {
            get;
            set;
        }

        public DrawVertex[] Data => this.data;

        void UpdatePointer()
        {
            if (this.handle.IsAllocated)
            {
                this.handle.Free();
            }
            this.handle = GCHandle.Alloc(this.data, GCHandleType.Pinned);
            this.Pointer = this.handle.AddrOfPinnedObject();
            unsafe
            {
                this.ptr = (DrawVertex*)this.Pointer.ToPointer();
            }
        }

        public unsafe DrawVertex this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return *(this.ptr + index);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                *(this.ptr + index) = value;
            }
        }

        public int Count
        {
            get { return this.size; }
        }

        public void Clear()
        {
            this.size = 0;
        }

        public void Resize(int newSize)
        {
            if (newSize > this.data.Length)
            {
                this.Reserve(this._grow_capacity(newSize));
            }
            this.size = newSize;
        }

        public void Append(VertexBuffer vertexBuffer)
        {
            var original_size = this.size;
            this.Resize(this.Count + vertexBuffer.Count);
            Array.Copy(vertexBuffer.data, 0, this.data, original_size, vertexBuffer.size);
        }

        private void Reserve(int new_capacity)
        {
            if (new_capacity <= this.capacity) return;
            DrawVertex[] new_data = new DrawVertex[new_capacity];
            if (this.data != null)
            {
                unsafe
                {
                    var dest_byte_size = new_capacity * Marshal.SizeOf<DrawVertex>();
                    var src_byte_size = this.size * Marshal.SizeOf<DrawVertex>();
                    fixed (DrawVertex* data_ptr = this.data)
                    fixed (DrawVertex* new_data_ptr = new_data)
                    {
                        Buffer.MemoryCopy(data_ptr, new_data_ptr, dest_byte_size, src_byte_size);
                    }
                }
            }
            this.data = null;
            this.data = new_data;
            this.capacity = new_capacity;
            this.UpdatePointer();
        }

        private int _grow_capacity(int newSize)
        {
            int new_capacity = (this.capacity != 0) ? (this.Capacity + this.Capacity / 2) : 8;
            return new_capacity > newSize ? new_capacity : newSize;
        }
    }
}
