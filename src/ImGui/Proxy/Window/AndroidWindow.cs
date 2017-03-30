using System;

namespace ImGui
{
    class AndroidWindow : IWindow
    {
        private static AndroidWindow instance;

        public static AndroidWindow Instance
        {
            get
            {
                return instance;
            }
        }

        private readonly IntPtr nativeWindow;

        private AndroidWindow(IntPtr nativeWindow)
        {
            this.nativeWindow = nativeWindow;
        }

        public static AndroidWindow CreateAndroidWindow(IntPtr nativeWindow)
        {
            instance = new AndroidWindow(nativeWindow);
            return instance;
        }

        #region Implementation of IWindow

        public object Handle
        {
            get { return nativeWindow; }
        }

        public IntPtr Pointer
        {
            get { return nativeWindow; }
        }

        public Point Position
        {
            get
            {
                return Point.Zero;
            }

            set
            {
                //dummy
            }
        }

        public Size Size
        {
            get
            {
                return this.size;
            }

            set
            {
                this.size = value;
            }
        }

        public string Title
        {
            get
            {
                return Application.windowContext.GetWindowTitle(this);
            }

            set
            {
                Application.windowContext.SetWindowTitle(this, value);
            }
        }

        public Point ClientPosition
        {
            get => Application.windowContext.GetClientPosition(this);
            set => Application.windowContext.SetClientPosition(this, value);
        }

        public Size ClientSize
        {
            get => Application.windowContext.GetClientSize(this);
            set => Application.windowContext.SetClientSize(this, value);
        }


        public void Close()
        {
            Application.windowContext.CloseWindow(this);
        }

        public void Hide()
        {
            Application.windowContext.HideWindow(this);
        }

        public void Maximize()
        {
            Application.windowContext.MaximizeWindow(this);
        }

        public void Minimize()
        {
            Application.windowContext.MinimizeWindow(this);
        }

        public void Normalize()
        {
            Application.windowContext.NormalizeWindow(this);
        }

        public void Show()
        {
            Application.windowContext.ShowWindow(this);
        }

        public Point ScreenToClient(Point point)
        {
            return Application.windowContext.ScreenToClient(this, point);
        }

        public Point ClientToScreen(Point point)
        {
            return Application.windowContext.ClientToScreen(this, point);
        }

        #endregion

        private Size size;
    }
}