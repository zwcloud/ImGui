using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        #region Window

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
            public xcb_colormap_t default_colormap;//default color map
            public uint white_pixel;//white pixel
            public uint black_pixel;//black pixel
            public uint current_input_masks;
            public uint width_in_pixels;//width (px)
            public uint height_in_pixels;//height (px)
            public uint width_in_millimeters;//height (mm)
            public uint height_in_millimeters;//height (mm)
            public uint min_installed_maps;
            public uint max_installed_maps;
            public xcb_visualid_t root_visual;
            public byte backing_stores;
            public byte save_unders;
            public byte root_depth;
            public byte allowed_depths_len;
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
            UInt32[] value_list);

        struct xcb_void_cookie_t
        {
            uint sequence;  //Sequence number
        };

        [Flags]
        enum xcb_cw_t
        {
            XCB_CW_BACK_PIXMAP = 1,
            /*Overrides the default background-pixmap. The background pixmap and window must
have the same root and same depth. Any size pixmap can be used, although some
sizes may be faster than others.

If `XCB_BACK_PIXMAP_NONE` is specified, the window has no defined background.
The server may fill the contents with the previous screen contents or with
contents of its own choosing.

If `XCB_BACK_PIXMAP_PARENT_RELATIVE` is specified, the parent's background is
used, but the window must have the same depth as the parent (or a Match error
results).   The parent's background is tracked, and the current version is
used each time the window background is required. */

            XCB_CW_BACK_PIXEL = 2,
            /*Overrides `BackPixmap`. A pixmap of undefined size filled with the specified
background pixel is used for the background. Range-checking is not performed,
the background pixel is truncated to the appropriate number of bits. */

            XCB_CW_BORDER_PIXMAP = 4,
            /*Overrides the default border-pixmap. The border pixmap and window must have the
same root and the same depth. Any size pixmap can be used, although some sizes
may be faster than others.

The special value `XCB_COPY_FROM_PARENT` means the parent's border pixmap is
copied (subsequent changes to the parent's border attribute do not affect the
child), but the window must have the same depth as the parent. */

            XCB_CW_BORDER_PIXEL = 8,
            /*Overrides `BorderPixmap`. A pixmap of undefined size filled with the specified
border pixel is used for the border. Range checking is not performed on the
border-pixel value, it is truncated to the appropriate number of bits. */

            XCB_CW_BIT_GRAVITY = 16,
            /*Defines which region of the window should be retained if the window is resized. */

            XCB_CW_WIN_GRAVITY = 32,
            /*Defines how the window should be repositioned if the parent is resized (see
`ConfigureWindow`). */

            XCB_CW_BACKING_STORE = 64,
            /*A backing-store of `WhenMapped` advises the server that maintaining contents of
obscured regions when the window is mapped would be beneficial. A backing-store
of `Always` advises the server that maintaining contents even when the window
is unmapped would be beneficial. In this case, the server may generate an
exposure event when the window is created. A value of `NotUseful` advises the
server that maintaining contents is unnecessary, although a server may still
choose to maintain contents while the window is mapped. Note that if the server
maintains contents, then the server should maintain complete contents not just
the region within the parent boundaries, even if the window is larger than its
parent. While the server maintains contents, exposure events will not normally
be generated, but the server may stop maintaining contents at any time. */

            XCB_CW_BACKING_PLANES = 128,
            /*The backing-planes indicates (with bits set to 1) which bit planes of the
window hold dynamic data that must be preserved in backing-stores and during
save-unders. */

            XCB_CW_BACKING_PIXEL = 256,
            /*The backing-pixel specifies what value to use in planes not covered by
backing-planes. The server is free to save only the specified bit planes in the
backing-store or save-under and regenerate the remaining planes with the
specified pixel value. Any bits beyond the specified depth of the window in
these values are simply ignored. */

            XCB_CW_OVERRIDE_REDIRECT = 512,
            /*The override-redirect specifies whether map and configure requests on this
window should override a SubstructureRedirect on the parent, typically to
inform a window manager not to tamper with the window. */

            XCB_CW_SAVE_UNDER = 1024,
            /*If 1, the server is advised that when this window is mapped, saving the
contents of windows it obscures would be beneficial. */

            XCB_CW_EVENT_MASK = 2048,
            /*The event-mask defines which events the client is interested in for this window
(or for some event types, inferiors of the window). */

            XCB_CW_DONT_PROPAGATE = 4096,
            /*The do-not-propagate-mask defines which events should not be propagated to
ancestor windows when no client has the event type selected in this window. */

            XCB_CW_COLORMAP = 8192,
            /*The colormap specifies the colormap that best reflects the true colors of the window. Servers
capable of supporting multiple hardware colormaps may use this information, and window man-
agers may use it for InstallColormap requests. The colormap must have the same visual type
and root as the window (or a Match error results). If CopyFromParent is specified, the parent's
colormap is copied (subsequent changes to the parent's colormap attribute do not affect the child).
However, the window must have the same visual type as the parent (or a Match error results),
and the parent must not have a colormap of None (or a Match error results). For an explanation
of None, see FreeColormap request. The colormap is copied by sharing the colormap object
between the child and the parent, not by making a complete copy of the colormap contents. */

            XCB_CW_CURSOR = 16384
            /*If a cursor is specified, it will be used whenever the pointer is in the window. If None is speci-
fied, the parent's cursor will be used when the pointer is in the window, and any change in the
parent's cursor will cause an immediate change in the displayed cursor. */

        }

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

        #region Event

        [Flags]
        enum xcb_event_mask_t
        {
            XCB_EVENT_MASK_NO_EVENT = 0,
            XCB_EVENT_MASK_KEY_PRESS = 1,
            XCB_EVENT_MASK_KEY_RELEASE = 2,
            XCB_EVENT_MASK_BUTTON_PRESS = 4,
            XCB_EVENT_MASK_BUTTON_RELEASE = 8,
            XCB_EVENT_MASK_ENTER_WINDOW = 16,
            XCB_EVENT_MASK_LEAVE_WINDOW = 32,
            XCB_EVENT_MASK_POINTER_MOTION = 64,
            XCB_EVENT_MASK_POINTER_MOTION_HINT = 128,
            XCB_EVENT_MASK_BUTTON_1_MOTION = 256,
            XCB_EVENT_MASK_BUTTON_2_MOTION = 512,
            XCB_EVENT_MASK_BUTTON_3_MOTION = 1024,
            XCB_EVENT_MASK_BUTTON_4_MOTION = 2048,
            XCB_EVENT_MASK_BUTTON_5_MOTION = 4096,
            XCB_EVENT_MASK_BUTTON_MOTION = 8192,
            XCB_EVENT_MASK_KEYMAP_STATE = 16384,
            XCB_EVENT_MASK_EXPOSURE = 32768,
            XCB_EVENT_MASK_VISIBILITY_CHANGE = 65536,
            XCB_EVENT_MASK_STRUCTURE_NOTIFY = 131072,
            XCB_EVENT_MASK_RESIZE_REDIRECT = 262144,
            XCB_EVENT_MASK_SUBSTRUCTURE_NOTIFY = 524288,
            XCB_EVENT_MASK_SUBSTRUCTURE_REDIRECT = 1048576,
            XCB_EVENT_MASK_FOCUS_CHANGE = 2097152,
            XCB_EVENT_MASK_PROPERTY_CHANGE = 4194304,
            XCB_EVENT_MASK_COLOR_MAP_CHANGE = 8388608,
            XCB_EVENT_MASK_OWNER_GRAB_BUTTON = 16777216
        }

        struct xcb_generic_event_t
        {
            public byte response_type;  //Type of the response
            public byte pad0;           //Padding
            public UInt16 sequence;       //Sequence number */
            public unsafe fixed UInt32 pad[7]; //Padding
            public UInt32 full_sequence;  //full sequence
        }


        /**
         * @brief Returns the next event or error from the server.
         * @param c: The connection to the X server.
         * @return The next event from the server.
         *
         * Returns the next event or error from the server, or returns null in
         * the event of an I/O error. Blocks until either an event or error
         * arrive, or an I/O error occurs.
         */
        //xcb_generic_event_t* xcb_wait_for_event(xcb_connection_t* c);
        [DllImport(libXcb, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr xcb_wait_for_event(IntPtr connection);//not used


        /**
         * @brief Returns the next event or error from the server.
         * @param c: The connection to the X server.
         * @return The next event from the server.
         *
         * Returns the next event or error from the server, if one is
         * available, or returns @c NULL otherwise. If no event is available, that
         * might be because an I/O error like connection close occurred while
         * attempting to read the next event, in which case the connection is
         * shut down when this function returns.
         */
        //xcb_generic_event_t* xcb_poll_for_event(xcb_connection_t* c);
        [DllImport(libXcb, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr xcb_poll_for_event(IntPtr connection);

        struct xcb_expose_event_t
        {
            public byte response_type;
            public byte pad0;
            public UInt16 sequence;
            public xcb_window_t window;
            public UInt16 x;
            public UInt16 y;
            public UInt16 width;
            public UInt16 height;
            public UInt16 count;
            public unsafe fixed byte pad1[2];
        }

        struct xcb_button_press_event_t
        {
            public byte response_type;
            public byte detail;/*typedef uint8_t xcb_button_t;*/
            public UInt16 sequence;
            public UInt32 time; /*typedef uint32_t xcb_timestamp_t;*/
            public xcb_window_t root;
            public xcb_window_t _event;
            public xcb_window_t child;
            public Int16 root_x;
            public Int16 root_y;
            public Int16 event_x;
            public Int16 event_y;
            public UInt16 state;
            public byte same_screen;
            public byte pad0;
        }

        struct xcb_motion_notify_event_t
        {
            public byte response_type;
            public byte detail;
            public UInt16 sequence;
            public UInt32 time; /*typedef uint32_t xcb_timestamp_t;*/
            public xcb_window_t root;
            public xcb_window_t _event;
            public xcb_window_t child;
            public Int16 root_x;
            public Int16 root_y;
            public Int16 event_x;
            public Int16 event_y;
            public UInt16 state;
            public byte same_screen;
            public byte pad0;
        }

        struct xcb_enter_notify_event_t
        {
            public byte response_type;
            public byte detail;
            public UInt16 sequence;
            public UInt32 time; /*typedef uint32_t xcb_timestamp_t;*/
            public xcb_window_t root;
            public xcb_window_t _event;
            public xcb_window_t child;
            public Int16 root_x;
            public Int16 root_y;
            public Int16 event_x;
            public Int16 event_y;
            public UInt16 state;
            public byte mode;
            public byte same_screen_focus;
        }

        struct xcb_key_press_event_t
        {
            public byte response_type;
            public xcb_keycode_t detail;
            public UInt16 sequence;
            public UInt32 time; /*typedef uint32_t xcb_timestamp_t;*/
            public xcb_window_t root;
            public xcb_window_t _event;
            public xcb_window_t child;
            public Int16 root_x;
            public Int16 root_y;
            public Int16 event_x;
            public Int16 event_y;
            public UInt16 state;
            public byte same_screen;
            public byte pad0;
        }

        enum xcb_button_mask_t
        {
            XCB_BUTTON_MASK_1 = 256,
            XCB_BUTTON_MASK_2 = 512,
            XCB_BUTTON_MASK_3 = 1024,
            XCB_BUTTON_MASK_4 = 2048,
            XCB_BUTTON_MASK_5 = 4096,
            XCB_BUTTON_MASK_ANY = 32768
        }

        #endregion

        #endregion

        IntPtr/*xcb_connection_t* */ c;//Temp HACK, maybe this should be a per-window data

        public IWindow CreateWindow(Point position, Size size, WindowTypes windowType)
        {
            xcb_window_t win;
            unsafe
            {
                xcb_screen_t* screen;

                //connect to X server
                c = xcb_connect(IntPtr.Zero, IntPtr.Zero);
                var error = (XCB_CONN)xcb_connection_has_error(c);
                if (error != XCB_CONN.XCB_CONN_NO_ERROR)
                {
                    Console.WriteLine("xcb_connect error: {0}", XCB_CONN_Text[(int)error]);
                }

                //get first screen
                screen = (xcb_screen_t*)xcb_setup_roots_iterator(xcb_get_setup(c)).data.ToPointer();
                if (new IntPtr(screen) == IntPtr.Zero)
                {
                    Console.WriteLine("xcb_setup_roots_iterator failed.");
                }

                //generate window id
                win = xcb_generate_id(c);
                
                var mask = xcb_cw_t.XCB_CW_BACK_PIXEL | xcb_cw_t.XCB_CW_EVENT_MASK;//background and event mask
                UInt32[] maskValues = new UInt32[2];
                maskValues[0] = screen->white_pixel;//white back ground
                maskValues[1] = (UInt32)(xcb_event_mask_t.XCB_EVENT_MASK_EXPOSURE | xcb_event_mask_t.XCB_EVENT_MASK_BUTTON_PRESS |
                    xcb_event_mask_t.XCB_EVENT_MASK_BUTTON_RELEASE | xcb_event_mask_t.XCB_EVENT_MASK_POINTER_MOTION |
                    xcb_event_mask_t.XCB_EVENT_MASK_ENTER_WINDOW | xcb_event_mask_t.XCB_EVENT_MASK_LEAVE_WINDOW |
                    xcb_event_mask_t.XCB_EVENT_MASK_KEY_PRESS | xcb_event_mask_t.XCB_EVENT_MASK_KEY_RELEASE);//events that will be received
                xcb_create_window(c,
                    XCB_COPY_FROM_PARENT,//depth: same as parent
                    win,//window id
                    screen->root,//parent window
                    (short)position.X, (short)position.Y,//x,y
                    (ushort)size.Width, (ushort)size.Height,//width, height
                    10,//border width
                    (UInt16)xcb_window_class_t.XCB_WINDOW_CLASS_INPUT_OUTPUT,//windos class
                    screen->root_visual,//visual
                    (UInt32)mask, maskValues);//masks and their values

                // show it
                xcb_map_window(c, win);

                // flush pending requests
                xcb_flush(c);
                Console.WriteLine("xcb_flush..");

                Console.WriteLine("Press key to continue. (just a temp hack for remote debugging)");
                Console.ReadKey();
            }

            return new LinuxWindow((IntPtr)win, size);
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
            unsafe
            {
                IntPtr eventPtr = xcb_poll_for_event(c);
                if (eventPtr != IntPtr.Zero)
                {
                    Debug.WriteLine("xcb_poll_for_event got an event.");
                    ProcessEvent(eventPtr);
                }
                else
                {
                    guiMethod();
                }
            }
        }

        private unsafe void ProcessEvent(IntPtr eventPtr)
        {
            xcb_generic_event_t* e = (xcb_generic_event_t*)eventPtr.ToPointer();

            switch (e->response_type & ~0x80)
            {
                case 12:/*XCB_EXPOSE*/
                {
                    xcb_expose_event_t* expose = (xcb_expose_event_t*)e;

                    //Debug.WriteLine("Window {0} exposed. Region to be redrawn at location ({1},{2}), with dimension ({3},{4})",
                    //        expose->window, expose->x, expose->y, expose->width, expose->height);
                    break;
                }
                case 4://XCB_BUTTON_PRESS
                {
                    xcb_button_press_event_t* bp = (xcb_button_press_event_t*)e;
                    //print_modifiers(bp->state);

                    switch (bp->detail)
                    {
                        case 4:
                        //Debug.WriteLine("Wheel Button up in window {0}, at coordinates ({1},{2})",
                        //        bp->_event, bp->event_x, bp->event_y);
                        //TODO
                        break;
                        case 5:
                        //Debug.WriteLine("Wheel Button down in window {0}, at coordinates ({1},{2})",
                        //        bp->_event, bp->event_x, bp->event_y);
                        //TODO
                        break;
                        default:
                        //Debug.WriteLine("Button {0} pressed in window {1}, at coordinates ({2},{3})",
                        //        bp->detail, bp->_event, bp->event_x, bp->event_y);

                        if (((ushort)xcb_button_mask_t.XCB_BUTTON_MASK_1 | bp->state) != 0)//this should be left button
                        {
                            Input.Mouse.LeftButtonState = InputState.Down;
                        }
                        //TODO process other buttons
                        break;
                    }
                    break;
                }
                case 5:/*XCB_BUTTON_RELEASE*/
                {
                    //typedef xcb_button_press_event_t xcb_button_release_event_t;
                    xcb_button_press_event_t* br = (xcb_button_press_event_t*)e;
                    //print_modifiers(br->state);

                    //Debug.WriteLine("Button {0} released in window {1}, at coordinates ({2},{3})",
                    //        br->detail, br->_event, br->event_x, br->event_y);
                    if (((ushort)xcb_button_mask_t.XCB_BUTTON_MASK_1 | br->state) != 0)//this should be left button
                    {
                        Input.Mouse.LeftButtonState = InputState.Up;
                    }
                    //TODO process other buttons
                    break;
                }
                case 6:/* XCB_MOTION_NOTIFY*/
                {
                    xcb_motion_notify_event_t* motion = (xcb_motion_notify_event_t*)e;

                    //Debug.WriteLine("Mouse moved in window {0}, at coordinates ({1},{2})",
                    //        motion->_event, motion->event_x, motion->event_y);
                    Input.Mouse.MousePos = new Point(motion->event_x, motion->event_y);
                    break;
                }
                case 7:/*XCB_ENTER_NOTIFY*/
                {
                    xcb_enter_notify_event_t* enter = (xcb_enter_notify_event_t*)e;

                    //Debug.WriteLine("Mouse entered window {0}, at coordinates ({1},{2})",
                    //        enter->_event, enter->event_x, enter->event_y);
                    break;
                }
                case 8:/*XCB_LEAVE_NOTIFY*/
                {
                    //typedef xcb_enter_notify_event_t xcb_leave_notify_event_t;
                    xcb_enter_notify_event_t* leave = (xcb_enter_notify_event_t*)e;

                    //Debug.WriteLine("Mouse left window {0}, at coordinates ({1},{2})",
                    //        leave->_event, leave->event_x, leave->event_y);
                    break;
                }
                case 2:/*XCB_KEY_PRESS*/
                {
                    xcb_key_press_event_t* kp = (xcb_key_press_event_t*)e;
                    //print_modifiers(kp->state);

                    //Debug.WriteLine("Key <{0}> pressed in window {1}", kp->detail, kp->_event);
                    var keyCode = kp->detail;
                    Input.Keyboard.lastKeyStates[keyCode] = Input.Keyboard.keyStates[keyCode];
                    Input.Keyboard.keyStates[keyCode] = InputState.Down;

                    break;
                }
                case 3:/*XCB_KEY_RELEASE*/
                {
                    //typedef xcb_key_press_event_t xcb_key_release_event_t;
                    xcb_key_press_event_t* kr = (xcb_key_press_event_t*)e;
                    //print_modifiers(kr->state);

                    //Debug.WriteLine("Key <{0}> released in window {1}", kr->detail, kr->_event);
                    var keyCode = kr->detail;
                    Input.Keyboard.lastKeyStates[keyCode] = Input.Keyboard.keyStates[keyCode];
                    Input.Keyboard.keyStates[keyCode] = InputState.Up;
                    break;
                }
                default:
                /* Unknown e type, ignore it */
                //Debug.WriteLine("Unknown event: {0}\n", e->response_type);
                break;
            }

            Marshal.FreeHGlobal(eventPtr);//free(e);
        }

        #region DEBUG only
        private readonly string[] MODIFIERS =
        {
            "Shift", "Lock", "Ctrl", "Alt",
            "Mod2", "Mod3", "Mod4", "Mod5",
            "Button1", "Button2", "Button3", "Button4", "Button5"
        };
        /* print names of modifiers present in mask */
        private void print_modifiers(UInt32 mask)
        {
            Debug.Write("Modifier mask: ");
            for (var i=0; mask!=0; mask >>= 1, ++i)
            {
                var modifier = MODIFIERS[i];
                if ((mask & 1)!=0)
                {
                    Debug.Write(modifier);
                }
            }
            Debug.WriteLine("");
        }
        #endregion

    }
}
