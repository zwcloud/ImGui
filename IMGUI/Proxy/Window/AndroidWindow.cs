using System;

namespace ImGui
{
    class AndroidWindow : IWindow
    {
        #region Implementation of IWindow

        public object Handle
        {
            get { throw new NotImplementedException(); }
        }

        public IntPtr Pointer
        {
            get { throw new NotImplementedException(); }
        }

        public Point Position
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public Size Size
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public string Title
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public void Show()
        {
            throw new NotImplementedException();
        }

        public void Hide()
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Minimize()
        {
            throw new NotImplementedException();
        }

        public void Maximize()
        {
            throw new NotImplementedException();
        }

        public void Normalize()
        {
            throw new NotImplementedException();
        }

        public Point ScreenToClient(Point point)
        {
            throw new NotImplementedException();
        }

        public Point ClientToScreen(Point point)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}