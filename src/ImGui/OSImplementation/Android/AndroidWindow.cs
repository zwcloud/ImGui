using System;
using ImGui.OSAbstraction.Window;

namespace ImGui.OSImplementation.Android
{
    internal class AndroidWindow : IWindow
    {
        public void Init()
        {
            this.Position = Point.Zero;
            //dummy
        }

        #region Implementation of IWindow

        public object Handle => IntPtr.Zero;//dummy

        public IntPtr Pointer => IntPtr.Zero;//dummy

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
            get; set;//dummy
        }

        public string Title
        {
            get
            {
                return "dummy";
            }

            set
            {
                ;//dummy
            }
        }

        public Point ClientPosition
        {
            get => Point.Zero;
            set { throw new NotSupportedException("Cannot change client area position on Android. It is fixed to (0,0)."); }
        }

        public Size ClientSize
        {
            get => this.Size;
            set { throw new NotSupportedException("Cannot change client area size on Android. It is fixed to screen size."); }
        }


        public void Close()
        {
            //dummy
        }

        public void Hide()
        {
            //dummy
        }

        public void Show()
        {
            //dummy
        }

        public Point ScreenToClient(Point point)
        {
            return point;
        }

        public Point ClientToScreen(Point point)
        {
            return point;
        }

        public void MainLoop(Action guiMethod)
        {
            guiMethod();
        }

        #endregion
    }
}