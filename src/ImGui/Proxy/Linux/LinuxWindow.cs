using System;
using System.Collections.Generic;
using System.Text;

namespace ImGui
{
    class LinuxWindow : IWindow
    {
        IntPtr connection;

        public object Handle
        {
            get
            {
                return connection;
            }
        }

        public IntPtr Pointer
        {
            get
            {
                return connection;
            }
        }

        public LinuxWindow(IntPtr connection, Size size)
        {
            this.connection = connection;
            this.size = size;
        }

        public Point Position
        {
            get
            {
                return Application.windowContext.GetWindowPosition(this);
            }

            set
            {
                Application.windowContext.SetWindowPosition(this, value);
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
                if(value!= this.size)
                {
                    Application.windowContext.SetWindowSize(this, value);
                    this.size = value;
                }
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
            get => this.Position;
            set => this.Position = value;
        }

        public Size ClientSize
        {
            get => this.size;
            set => this.size = value; //dummy
        }


        public void Close()
        {
            Application.windowContext.CloseWindow(this);
        }

        public void Hide()
        {
            Application.windowContext.HideWindow(this);
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
        
        private Size size;
    }
}
