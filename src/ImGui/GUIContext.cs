using System.Collections.Generic;

namespace ImGui
{
    internal class GUIContext
    {

        public bool LogEnabled = true;

        // fps
        public long lastFPSUpdateTime;
        public int fps;
        public int elapsedFrameCount = 0;

        public readonly List<Window> Windows = new List<Window>();
        public Window CurrentWindow;
        public readonly Stack<Window> CurrentWindowStack = new Stack<Window>();

        private string hoverId;
        private string activeId;
        private bool activeIdIsAlive;
        private bool activeIdIsJustActivated;

        private string hoverIdPreviousFrame;
        private string activeIdPreviousFrame;


        //-----

        public const string None = "None";
        public const string Unavailable = "Unavailable";

        public string HoverId
        {
            get { return hoverId; }
            set { hoverId = value; }
        }

        public string ActiveId
        {
            get { return activeId; }
            set { activeId = value; }
        }

        public bool ActiveIdIsAlive
        {
            get { return activeIdIsAlive; }
            set { activeIdIsAlive = value; }
        }

        public string ActiveIdPreviousFrame
        {
            get { return activeIdPreviousFrame; }
            set { activeIdPreviousFrame = value; }
        }

        public bool ActiveIdIsJustActivated
        {
            get { return activeIdIsJustActivated; }
            set { activeIdIsJustActivated = value; }
        }

        public string HoverIdPreviousFrame
        {
            get { return hoverIdPreviousFrame; }
            set { hoverIdPreviousFrame = value; }
        }

        public Window HoveredWindow { get; internal set; }
        public Window MovedWindow { get; internal set; }
        public Window HoveredRootWindow { get; internal set; }

        public void SetHoverId(string id)
        {
            HoverId = id;
        }

        public void SetActiveId(string id)
        {
            ActiveId = id;
            ActiveIdIsJustActivated = true;

        }

        public void KeepAliveId(string id)
        {
            if (ActiveId == id)
                ActiveIdIsAlive = true;
        }


        public bool IsMouseHoveringRect(Rect rect, bool clip = true)
        {
            var g = this;
            Window window = CurrentWindow;

            // Clip
            Rect rect_clipped = rect;
            if (clip)
                rect_clipped.Intersect(window.ClipRect);

            return rect_clipped.Contains(Input.Mouse.MousePos);
        }

        public bool IsMouseHoveringRect(Point r_min, Point r_max, bool clip = true)
        {
            Rect rect = new Rect(r_min, r_max);
            return IsMouseHoveringRect(rect, clip);
        }

        public Window FindWindowByName(string name)
        {
            var g = this;
            for (int i = 0; i < g.Windows.Count; i++)
            {
                if (g.Windows[i].ID == name.GetHashCode())
                {
                    return g.Windows[i];
                }
            }
            return null;
        }

        public Window FindHoveredWindow(Point pos, bool excluding_childs)
        {
            var g = this;
            for (int i = g.Windows.Count - 1; i >= 0; i--)
            {
                Window window = g.Windows[i];
                if (!window.Active)
                    continue;
                if (excluding_childs)
                    continue;

                if (window.Rect.Contains(pos))
                    return window;
            }
            return null;
        }
    }
}