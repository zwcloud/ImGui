using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace ImGui
{
    using xcb_window_t = System.UInt32;//typedef uint32_t xcb_window_t;
    using xcb_colormap_t = System.UInt32;//typedef uint32_t xcb_colormap_t;
    using xcb_visualid_t = System.UInt32;//typedef uint32_t xcb_visualid_t;
    using xcb_keycode_t = System.Byte;//typedef uint8_t xcb_keycode_t;

    class LinuxWindowContext : IWindowContext
    {
        #region Native

        const string libXcb = "libxcb";

        [DllImport(libXcb, CallingConvention=CallingConvention.Cdecl)]
        static extern IntPtr xcb_connect(IntPtr displayname, IntPtr screenp);

        //int xcb_connection_has_error(xcb_connection_t* c);
        [DllImport(libXcb, CallingConvention = CallingConvention.Cdecl)]
        static extern int xcb_connection_has_error(IntPtr connection);

        enum XCB_CONN
        {
            XCB_CONN_NO_ERROR = 0,
            XCB_CONN_ERROR = 1,
            XCB_CONN_CLOSED_EXT_NOTSUPPORTED = 2,
            XCB_CONN_CLOSED_MEM_INSUFFICIENT = 3,
            XCB_CONN_CLOSED_REQ_LEN_EXCEED = 4,
            XCB_CONN_CLOSED_PARSE_ERR = 5,
            XCB_CONN_CLOSED_INVALID_SCREEN = 6,
        }

        static readonly string[] XCB_CONN_Text = new string[]
        {
            "No error.",
            "xcb connection errors because of socket, pipe and other stream errors.",
            "xcb connection shutdown because of extension not supported",
            "malloc(), calloc() and realloc() error upon failure, for eg ENOMEM",
            "Connection closed, exceeding request length that server accepts.",
            "Connection closed, error during parsing display string.",
            "Connection closed because the server does not have a screen matching the display.",
        };

        [DllImport(libXcb, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr xcb_disconnect(IntPtr connection);

        struct xcb_screen_t
        {
            public xcb_window_t root;
            xcb_colormap_t default_colormap;//default color map
            uint white_pixel;//white pixel
            uint black_pixel;//black pixel
            uint current_input_masks;
            uint width_in_pixels;//width (px)
            uint height_in_pixels;//height (px)
            uint width_in_millimeters;//height (mm)
            uint height_in_millimeters;//height (mm)
            uint min_installed_maps;
            uint max_installed_maps;
            public xcb_visualid_t root_visual;
            byte backing_stores;
            byte save_unders;
            byte root_depth;
            byte allowed_depths_len;
        }

        //xcb_screen_iterator_t xcb_setup_roots_iterator(xcb_setup_t* R);
        [DllImport(libXcb, CallingConvention = CallingConvention.Cdecl)]
        static extern xcb_screen_iterator_t xcb_setup_roots_iterator(IntPtr R);

        struct xcb_screen_iterator_t
        {
            public IntPtr data;//xcb_screen_t*
            int rem;
            int index;
        }

        //const xcb_setup_t* xcb_get_setup(xcb_connection_t * c);
        [DllImport(libXcb, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr xcb_get_setup(IntPtr connection);

        struct xcb_setup_t
        {
            byte status;
            byte pad0;
            UInt16 protocol_major_version;
            UInt16 protocol_minor_version;
            UInt16 length;
            UInt32 release_number;
            UInt32 resource_id_base;
            UInt32 resource_id_mask;
            UInt32 motion_buffer_size;
            UInt16 vendor_len;
            UInt16 maximum_request_length;
            byte roots_len;
            byte pixmap_formats_len;
            byte image_byte_order;
            byte bitmap_format_bit_order;
            byte bitmap_format_scanline_unit;
            byte bitmap_format_scanline_pad;
            xcb_keycode_t min_keycode;
            xcb_keycode_t max_keycode;
            byte pad1_0;
            byte pad1_1;
            byte pad1_2;
            byte pad1_3;
        };

        //uint32_t xcb_generate_id(xcb_connection_t* c);
        [DllImport(libXcb, CallingConvention = CallingConvention.Cdecl)]
        static extern System.UInt32 xcb_generate_id(IntPtr connection);

        /*
        xcb_void_cookie_t
        xcb_create_window (xcb_connection_t *c,
                           uint8_t depth,
                           xcb_window_t      wid,
                           xcb_window_t parent,
                           int16_t           x,
                           int16_t y,
                           uint16_t          width,
                           uint16_t height,
                           uint16_t          border_width,
                           uint16_t _class,
                           xcb_visualid_t    visual,
                           uint32_t value_mask,
                           const uint32_t* value_list  );
        */
        [DllImport(libXcb, CallingConvention = CallingConvention.Cdecl)]
        static extern xcb_void_cookie_t xcb_create_window(IntPtr c,
            byte depth,
            xcb_window_t wid,
            xcb_window_t parent,
            Int16 x,
            Int16 y,
            UInt16 width,
            UInt16 height,
            UInt16 border_width,
            UInt16 _class,
            xcb_visualid_t visual,
            UInt32 value_mask,
            IntPtr value_list);

        struct xcb_void_cookie_t
        {
            uint sequence;  //Sequence number
        };

        const byte XCB_COPY_FROM_PARENT = 0;

        enum xcb_window_class_t
        {
            XCB_WINDOW_CLASS_COPY_FROM_PARENT = 0,
            XCB_WINDOW_CLASS_INPUT_OUTPUT = 1,
            XCB_WINDOW_CLASS_INPUT_ONLY = 2
        }

        //xcb_void_cookie_t xcb_map_window(xcb_connection_t* c, xcb_window_t window);
        [DllImport(libXcb, CallingConvention = CallingConvention.Cdecl)]
        static extern xcb_void_cookie_t xcb_map_window(IntPtr connection, xcb_window_t window);

        //int xcb_flush(xcb_connection_t* c);
        [DllImport(libXcb, CallingConvention = CallingConvention.Cdecl)]
        static extern int xcb_flush(IntPtr connection);

        #endregion

        public IWindow CreateWindow(Point position, Size size, WindowTypes windowType)
        {
            IntPtr/*xcb_connection_t* */ c;
            unsafe
            {
                xcb_screen_t* screen;
                xcb_window_t win;

                //connect to X server
                Console.WriteLine("xcb_connect..");
                c = xcb_connect(IntPtr.Zero, IntPtr.Zero);
                var error = (XCB_CONN)xcb_connection_has_error(c);
                if (error != XCB_CONN.XCB_CONN_NO_ERROR)
                {
                    Console.WriteLine("xcb_connect error: {0}", XCB_CONN_Text[(int)error]);
                }

                //get first screen
                Console.WriteLine("xcb_setup_roots_iterator..");
                screen = (xcb_screen_t*)xcb_setup_roots_iterator(xcb_get_setup(c)).data.ToPointer();
                if (new IntPtr(screen) == IntPtr.Zero)
                {
                    Console.WriteLine("xcb_setup_roots_iterator failed.");
                }

                //generate window id
                Console.WriteLine("xcb_generate_id..");
                win = xcb_generate_id(c);

                Console.WriteLine("xcb_create_window..");
                xcb_create_window(c,
                    XCB_COPY_FROM_PARENT,//depth: same as parent
                    win,//window id
                    screen->root,//parent window
                    (short)position.X, (short)position.Y,//x,y
                    (ushort)size.Width, (ushort)size.Height,//width, height
                    10,//border width
                    (UInt16)xcb_window_class_t.XCB_WINDOW_CLASS_INPUT_OUTPUT,//windos class
                    screen->root_visual,//visual
                    0, IntPtr.Zero);//masks(not used)

                // show it
                xcb_map_window(c, win);
                Console.WriteLine("xcb_map_window..");

                // flush pending requests
                xcb_flush(c);
                Console.WriteLine("xcb_flush..");
            }

            return new LinuxWindow(c, size);
        }

        public Size GetWindowSize(IWindow window)
        {
            //dummy
            return Size.Zero;
        }

        public Point GetWindowPosition(IWindow window)
        {
            //dummy
            return Point.Zero;
        }

        public void SetWindowSize(IWindow window, Size size)
        {
            //dummy
        }

        public void SetWindowPosition(IWindow window, Point position)
        {
            //dummy
        }

        public string GetWindowTitle(IWindow window)
        {
            //dummy
            return "dummy";
        }

        public void SetWindowTitle(IWindow window, string title)
        {
            //dummy
        }

        public void ShowWindow(IWindow window)
        {
            //dummy
        }

        public void HideWindow(IWindow window)
        {
            //dummy
        }

        public void CloseWindow(IWindow window)
        {
            //dummy
        }

        public void MinimizeWindow(IWindow window)
        {
            //dummy
        }

        public void MaximizeWindow(IWindow window)
        {
            //dummy
        }

        public void NormalizeWindow(IWindow window)
        {
            //dummy
        }

        public Point ScreenToClient(IWindow window, Point point)
        {
            return point;
        }

        public Point ClientToScreen(IWindow window, Point point)
        {
            return point;
        }

        public Size GetClientSize(IWindow window)
        {
            //dummy
            return GetWindowSize(window);
        }

        public void SetClientSize(IWindow window, Size size)
        {
            //dummy
        }

        public Point GetClientPosition(IWindow window)
        {
            return Point.Zero;
        }

        public void SetClientPosition(IWindow window, Point position)
        {
            //dummy
        }

        public void MainLoop(Action guiMethod)
        {
            guiMethod();
        }
    }
}
