using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Cairo;
using SFML.System;
using SFML.Window;

namespace IMGUI
{
    /// <summary>
    /// The SFML Form (not used)
    /// </summary>
    public class SFMLForm : IForm
    {
        private Window form;
        private string title;
        private BorderStyle borderStyle;

        public SFMLForm(string title, BorderStyle borderStyle = BorderStyle.Default)
        {
            this.title = title;
            this.borderStyle = borderStyle;
            form = new Window(
                VideoMode.DesktopMode,
                Title,
                SFMLUtil.ConvertBorderStyle(this.borderStyle));
        }
        
        #region Implementation of IForm

        public string Title
        {
            get { return title; }
            set
            {
                if(value == null)
                {
                    throw new ArgumentNullException();
                }
                form.SetTitle(value);
                title = value;
            }
        }

        public Color TitleColor
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public BorderStyle BorderStyle
        {
            get { return borderStyle; }
            set { throw new System.NotImplementedException(); }
        }

        public Point Position
        {
            get
            {
                var p = form.Position;
                return new Point(p.X, p.Y);
            }
            set
            {
                form.Position = new Vector2i((int)value.X, (int)value.Y);
            }
        }

        public Size Size
        {
            get
            {
                var s = form.Size;
                return new Size(s.X, s.Y);
            }
            set
            {
                form.Size = new Vector2u((uint) value.Width, (uint) value.Height);
            }
        }

        public void WindowProc(object Context)
        {
            throw new System.NotImplementedException();
        }

        public bool CanEnableIme
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public Cursor Cursor
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        #endregion
    }

    internal static class SFMLUtil
    {
        public static Styles ConvertBorderStyle(BorderStyle borderStyle)
        {
            switch (borderStyle)
            {
                case BorderStyle.None:
                    return Styles.None;
                case BorderStyle.Single:
                case BorderStyle.ThreeD:
                case BorderStyle.Default:
                    return Styles.Default;
                default:
                    throw new ArgumentOutOfRangeException("borderStyle", borderStyle, null);
            }
        }

    }
}