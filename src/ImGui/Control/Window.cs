using System;
using System.Collections.Generic;
using System.Text;

namespace ImGui
{
    internal class Window
    {
        public int ID;
        public Point Position;
        public Size Size;
        public Size FullSize;
        public WindowFlags Flags;
        public DrawList DrawList;
        public Rect ClipRect;
        public Point PosFloat;

        public long LastActiveFrame;

        private Window()
        {
            DrawList = new DrawList();
            MoveID = GetID("#MOVE");
            Active = WasActive = false;
            {
                var style = new GUIStyle();
                var bgColor = Color.Rgb(34, 43, 46);
                style.Set(GUIStyleName.PaddingTop, 1.0);
                style.Set(GUIStyleName.PaddingRight, 1.0);
                style.Set(GUIStyleName.PaddingBottom, 2.0);
                style.Set(GUIStyleName.PaddingLeft, 1.0);
                style.Set(GUIStyleName.BackgroundColor, new Color(0.00f, 0.00f, 0.00f, 0.70f));
                style.Set(GUIStyleName.ResizeGripColor, new Color(1.00f, 1.00f, 1.00f, 0.30f));
                style.Set(GUIStyleName.ResizeGripColor, new Color(1.00f, 1.00f, 1.00f, 0.60f), GUIState.Hover);
                style.Set(GUIStyleName.ResizeGripColor, new Color(1.00f, 1.00f, 1.00f, 0.90f), GUIState.Active);
                this.Style = style;
            }
            {
                var style = new GUIStyle();
                var bgColor = Color.Rgb(86, 90, 160);
                style.Set(GUIStyleName.BackgroundColor, new Color(0.27f, 0.27f, 0.54f, 0.83f));
                style.Set(GUIStyleName.BackgroundColor, new Color(0.32f, 0.32f, 0.63f, 0.87f), GUIState.Active);
                style.Set(GUIStyleName.BackgroundColor, new Color(0.40f, 0.40f, 0.80f, 0.20f), GUIState.Disabled);
                style.Set(GUIStyleName.FontColor, Color.White);
                this.HeaderStyle = style;
            }
        }

        public Window(string name, Point position, Size size, WindowFlags Flags) : this()
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;

            this.ID = name.GetHashCode();
            this.Flags = Flags;
            this.PosFloat = position;
            this.Position = new Point((int)PosFloat.X, (int)PosFloat.Y);
            this.Size = this.FullSize = size;
            this.HeaderContent = new Content(name);

            g.Windows.Add(this);
        }

        public int GetID(string strID)
        {
            //http://stackoverflow.com/a/263416/3427520
            int hash = 17;
            hash = hash * 23 + this.ID.GetHashCode();
            hash = hash * 23 + strID.GetHashCode();
            return hash;
        }

        public void ApplySize(Size new_size)
        {
            GUIContext g = Form.current.uiContext;
            Window window = this;
            
            window.FullSize = new_size;
        }

        public Rect Rect => new Rect(Position, Size);

        public double TitleBarHeight => HeaderStyle.PaddingVertical + HeaderStyle.PaddingVertical + HeaderStyle.FontSize*96.0/72.0;

        public Rect TitleBarRect => new Rect(Position, Size.Width, TitleBarHeight);

        public bool Collapsed { get; internal set; } = false;
        public bool Active { get; internal set; }
        public Window RootWindow { get; internal set; }

        /// <summary>
        /// == window->GetID("#MOVE")
        /// </summary>
        public int MoveID { get; internal set; }
        public Rect WindowClippedRect { get; internal set; }
        public bool WasActive { get; internal set; }

        public GUIStyle Style;
        public GUIStyle HeaderStyle;
        public Content HeaderContent;
    }

    [Flags]
    public enum WindowFlags
    {
        ChildWindow,
        NoResize,
        AlwaysAutoResize,
        Popup,
        NoTitleBar,
    }
}
