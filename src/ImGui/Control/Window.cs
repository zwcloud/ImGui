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
        public DrawList DrawList;
        public Rect ClipRect;

        public int LastActiveFrame;

        private Window()
        {
            DrawList = new DrawList();
            {
                var style = new GUIStyle();
                var bgColor = Color.Rgb(34, 43, 46);
                style.Set(GUIStyleName.PaddingTop, 1.0);
                style.Set(GUIStyleName.PaddingRight, 1.0);
                style.Set(GUIStyleName.PaddingBottom, 2.0);
                style.Set(GUIStyleName.PaddingLeft, 1.0);
                style.Set(GUIStyleName.BackgroundColor, bgColor);
                this.Style = style;
            }
            {
                var style = new GUIStyle();
                var bgColor = Color.Rgb(86, 90, 160);
                style.Set(GUIStyleName.BackgroundColor, bgColor);
                style.Set(GUIStyleName.FontColor, Color.White);
                this.HeaderStyle = style;
            }
        }

        public Window(string name, Point position, Size size) : this()
        {
            Form form = Form.current;
            GUIContext g = form.uiContext;

            this.ID = name.GetHashCode();
            this.Position = position;
            this.Size = this.FullSize = size;
            this.HeaderContent = new Content(name);

            g.Windows.Add(this);
        }

        public Rect Rect => new Rect(Position, Size);

        public double TitleBarHeight => HeaderStyle.PaddingVertical + HeaderStyle.PaddingVertical + HeaderStyle.FontSize*96.0/72.0;

        public Rect TitleBarRect => new Rect(Position, Size.Width, TitleBarHeight);

        public bool Collapsed { get; internal set; } = false;
        public bool Active { get; internal set; }
        public Window RootWindow { get; internal set; }

        public GUIStyle Style;
        public GUIStyle HeaderStyle;
        public Content HeaderContent;
    }
}
