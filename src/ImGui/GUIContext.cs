namespace ImGui
{
    internal class GUIContext
    {
        public const string None = "None";
        public const string Unavailable = "Unavailable";

        public bool LogEnabled = true;

        // fps
        public long lastFPSUpdateTime;
        public int fps;
        public int elapsedFrameCount = 0;

        private string hoverId;
        private string activeId;
        private bool activeIdIsAlive;
        private bool activeIdIsJustActivated;

        private string hoverIdPreviousFrame;
        private string activeIdPreviousFrame;

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
    }
}