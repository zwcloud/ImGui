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

        public int Capacity => capacity;

        public VertexBuffer(int capacity)
        {
            this.data = new DrawVertex[capacity];
            this.capacity = capacity;
            this.size = 0;
        }

        public IntPtr Pointer
        {
            get { return Marshal.UnsafeAddrOfPinnedArrayElement(data, 0); }
        }

        public unsafe DrawVertex this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                fixed (DrawVertex* ptr = data)
                {
                    return *(ptr + index);
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                fixed(DrawVertex* ptr = data)
                {
                    *(ptr + index) = value;
                }
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
            if (newSize > data.Length)
            {
                Reserve(_grow_capacity(newSize));
            }
            this.size = newSize;
        }

        private void Reserve(int new_capacity)
        {
            if (new_capacity <= capacity) return;
            DrawVertex[] new_data = new DrawVertex[new_capacity];
            if (this.data != null)
            {
                unsafe
                {
                    var dest_byte_size = new_capacity * Marshal.SizeOf<DrawVertex>();
                    var src_byte_size = size * Marshal.SizeOf<DrawVertex>();
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
        }

        private int _grow_capacity(int newSize)
        {
            int new_capacity = (capacity != 0) ? (Capacity + Capacity / 2) : 8;
            return new_capacity > newSize ? new_capacity : newSize;
        }
    }
}
