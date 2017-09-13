using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ImGui
{
    internal class IndexBuffer
    {
        private DrawIndex[] data;

        private int size;

        private int capacity;

        public int Capacity => capacity;

        public IndexBuffer(int capacity)
        {
            this.data = new DrawIndex[capacity];
            this.capacity = capacity;
            this.size = 0;
        }

        public IntPtr Pointer
        {
            get { return Marshal.UnsafeAddrOfPinnedArrayElement(data, 0); }
        }

        public unsafe DrawIndex this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                fixed (DrawIndex* ptr = data)
                {
                    return *(ptr + index);
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                fixed (DrawIndex* ptr = data)
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
            DrawIndex[] new_data = new DrawIndex[new_capacity];
            if (this.data != null)
            {
                unsafe
                {
                    var dest_byte_size = new_capacity * Marshal.SizeOf<DrawIndex>();
                    var src_byte_size = size * Marshal.SizeOf<DrawIndex>();
                    fixed (DrawIndex* data_ptr = this.data)
                    fixed (DrawIndex* new_data_ptr = new_data)
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
