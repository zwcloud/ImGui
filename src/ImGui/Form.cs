#define UseLog
using System;
using System.Collections.Generic;
using Ivony.Logs;

namespace ImGui
{
    /// <summary>
    /// Represents a window that makes up an application's user interface.
    /// </summary>
    public abstract partial class Form
    {
        public static Form current;

        private readonly IWindow window;
        private bool needRender = false;

        internal DrawList DrawList = new DrawList();
        internal IRenderer renderer;
        internal LayoutCache layoutCache = new LayoutCache();
        internal UIState uiState = new UIState();
        //internal Skin skin;

        /// <summary>
        /// Create form for android.
        /// </summary>
        /// <param name="nativeWindow"></param>
        /// <param name="position"></param>
        /// <param name="size"></param>
        protected Form(IntPtr nativeWindow, Point position, Size size)
        {
            this.window = Application.windowContext.CreateWindow(position, size, WindowTypes.Regular);
            renderer = Application.platformContext.CreateRenderer();
            renderer.Init(IntPtr.Zero);//dummy paramters

            this.DrawList.DrawBuffer.CommandBuffer.Add(
                new DrawCommand
                {
                    ClipRect = new Rect(this.ClientSize)
                });
            this.DrawList.BezierBuffer.CommandBuffer.Add(
                new DrawCommand
                {
                    ClipRect = new Rect(this.ClientSize)
                });

            InitGUI();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Form"/> class at specific rectangle.
        /// </summary>
        /// <param name="rect">initial rectangle of the form</param>
        protected Form(Rect rect):this(rect.TopLeft, rect.Size)
        {
        }

        internal Form(Point position, Size size, string Title = "<unnamed>")
        {
            this.window = Application.windowContext.CreateWindow(position, size, WindowTypes.Regular);
            this.window.Title = Title;

            renderer = Application.platformContext.CreateRenderer();
            renderer.Init(this.Pointer);
            
            this.DrawList.DrawBuffer.CommandBuffer.Add(
                new DrawCommand
                {
                    //ClipRect = new Rect(this.ClientSize)
                });
            this.DrawList.BezierBuffer.CommandBuffer.Add(
                new DrawCommand
                {
                    //ClipRect = new Rect(this.ClientSize)
                });

            Input.Mouse.Cursor = Cursor.Default;

            InitGUI();
        }

        internal FormState FormState
        {
            get { return formState; }
            set { formState = value; }
        }
        
        internal LayoutCache LayoutCache
        {
            get { return layoutCache; }
        }

        #region window management

        internal bool Closed { get; private set; }

        internal IntPtr Pointer { get { return window.Pointer; } }

        internal Size Size
        {
            get { return window.Size; }
            set { window.Size = value; }
        }

        internal Point ClientPosition
        {
            get => window.ClientPosition;
            set => window.ClientPosition = value;
        }

        internal Size ClientSize
        {
            get => window.ClientSize;
            set => window.ClientSize = value;
        }

        internal Point Position
        {
            get { return window.Position; }
            set { window.Position = value; }
        }

        internal Rect Rect
        {
            get { return new Rect(Position, Size); }
        }

        internal bool Focused { get { throw new NotImplementedException(); } }

        public bool NeedRender
        {
            get { return needRender; }
            set { needRender = value; }
        }

        internal void Show()
        {
            window.Show();
            Event.current.type = EventType.Layout;
        }

        internal void Hide()
        {
            window.Hide();
        }

        internal void Close()
        {
            this.renderer.ShutDown();
            window.Close();
            this.Closed = true;
        }

        internal void Minimize()
        {
            Event.current.type = EventType.MinimizeWindow;
        }

        internal void Maximize()
        {
            Event.current.type = EventType.MaximizeWindow;
        }

        internal void Normalize()
        {
            Event.current.type = EventType.NormalizeWindow;

#if implementation
            var originalWindowRect = new Rect(window.NormalPosition, window.NormalSize);
#endif
        }

        #endregion

        #region the GUI Loop

        internal void BeginGUI(bool useGUILayout)
        {
            // update input states
            Input.Mouse.LeftButtonPressed = Input.Mouse.LeftButtonState == InputState.Down && Input.Mouse.LeftButtonDownDuration < 0;
            Input.Mouse.LeftButtonReleased = Input.Mouse.LeftButtonState == InputState.Up && Input.Mouse.LeftButtonDownDuration >= 0;
            Input.Mouse.LeftButtonDownDuration = Input.Mouse.LeftButtonState == InputState.Down ? (Input.Mouse.LeftButtonDownDuration < 0 ? 0 : Input.Mouse.LeftButtonDownDuration + Application.DetlaTime) : -1;

            Input.Mouse.RightButtonPressed = Input.Mouse.RightButtonState == InputState.Down && Input.Mouse.RightButtonDownDuration < 0;
            Input.Mouse.RightButtonReleased = Input.Mouse.RightButtonState == InputState.Up && Input.Mouse.RightButtonDownDuration >= 0;
            Input.Mouse.RightButtonDownDuration = Input.Mouse.RightButtonState == InputState.Down ? (Input.Mouse.RightButtonDownDuration < 0 ? 0 : Input.Mouse.RightButtonDownDuration + Application.DetlaTime) : -1;

            // layout if needed
            if (useGUILayout)
            {
                LayoutUtility.Begin();
            }

            // Clear reference to active widget if the widget isn't alive anymore
            var g = this.uiState;
            g.HoverIdPreviousFrame = g.HoverId;
            g.HoverId = UIState.None;
            if (!g.ActiveIdIsAlive && g.ActiveIdPreviousFrame == g.ActiveId && g.ActiveId != UIState.None)
                g.ActiveId = UIState.None;
            g.ActiveIdPreviousFrame = g.ActiveId;
            g.ActiveIdIsAlive = false;
            g.ActiveIdIsJustActivated = false;
        }

        /// <summary>
        /// GUI Logic. This will be implemented by the user.
        /// </summary>
        protected abstract void OnGUI();

        internal void EndGUI()
        {
            if (Event.current.type == EventType.Layout)
            {
                LayoutUtility.Layout();
            }
        }

        private long lastFPSUpdateTime;
        private int fps;
        private int elapsedFrameCount = 0;

        private int LeftButtonPressedTimes = 0;
        private int RightButtonPressedTimes = 0;
        private int LeftButtonReleasedTimes = 0;
        private int RightButtonReleasedTimes = 0;

        /// <summary>
        /// GUI Loop
        /// </summary>
        internal void GUILoop()
        {
            current = this;

            handleEvent();

            elapsedFrameCount++;
            var detlaTime = Application.Time - lastFPSUpdateTime;
            if (detlaTime > 1000)
            {
                fps = elapsedFrameCount;
                elapsedFrameCount = 0;
                lastFPSUpdateTime = Application.Time;
            }

#if UseLog
            Application.logger.Clear();
            Application.logger.Msg("fps:{0,5:0.0}, mouse pos: {1}, detlaTime: {2}ms", fps, GetMousePos().ToString(), Application.DetlaTime);
            Application.logger.Msg("Input");
            Application.logger.Msg("    LeftButtonState {0}", Input.Mouse.LeftButtonState);
            Application.logger.Msg("    LeftButtonDownDuration {0}ms", Input.Mouse.LeftButtonDownDuration);
            Application.logger.Msg("    LeftButtonPressed {0}, {1} times", Input.Mouse.LeftButtonPressed, Input.Mouse.LeftButtonPressed ? ++LeftButtonPressedTimes : LeftButtonPressedTimes);
            Application.logger.Msg("    LeftButtonReleased {0}, {1} times", Input.Mouse.LeftButtonReleased, Input.Mouse.LeftButtonReleased ? ++LeftButtonReleasedTimes : LeftButtonReleasedTimes);

            Application.logger.Msg("    RightButtonState {0}", Input.Mouse.RightButtonState);
            Application.logger.Msg("    RightButtonDownDuration {0}ms", Input.Mouse.RightButtonDownDuration);
            Application.logger.Msg("    RightButtonPressed {0}, {1} times", Input.Mouse.RightButtonPressed, Input.Mouse.RightButtonPressed ? ++RightButtonPressedTimes : RightButtonPressedTimes);
            Application.logger.Msg("    RightButtonReleased {0}, {1} times", Input.Mouse.RightButtonReleased, Input.Mouse.RightButtonReleased ? ++RightButtonReleasedTimes : RightButtonReleasedTimes);

            Application.logger.Msg("ActiveId: {0}, ActiveIdIsAlive: {1}", this.uiState.ActiveId, this.uiState.ActiveIdIsAlive);
            Application.logger.Msg("HoverId: {0}", this.uiState.HoverId);

            Application.logger.Msg("Cursor: {0}", Input.Mouse.Cursor);
            
#endif
        }

        private void handleEvent()
        {
            switch (Event.current.type)
            {
                case EventType.Layout:
                    LayoutUtility.current.Clear();
                    LoadFormGroup();
                    BeginGUI(true);
                    OnGUI();
                    EndGUI();
                    Event.current.type = EventType.Repaint;
                    break;
                case EventType.Repaint:
                    if (!this.Closed)
                    {
                        this.DrawList.Clear();
                        BeginGUI(true);
                        OnGUI();
                        EndGUI();
                        this.NeedRender = true;
                        //Event.current.type = EventType.Used;
                    }
                    break;
                case EventType.MaximizeWindow:
                    {
                        window.Maximize();
                        FormState = FormState.Maximized;

                        Event.current.type = EventType.Layout;
                    }
                    break;
                case EventType.MinimizeWindow:
                    BeginGUI(true);
                    OnGUI();
                    EndGUI();
                    window.Minimize();
                    FormState = FormState.Minimized;
                    Event.current.type = EventType.Used;
                    break;
                case EventType.NormalizeWindow:
                    {
                        window.Normalize();
                        FormState = FormState.Normal;

                        Event.current.type = EventType.Layout;
                    }
                    break;
                default:
                    BeginGUI(true);
                    OnGUI();
                    EndGUI();
                    break;
            }

            if(this.NeedRender && !this.Closed)
            {
                Render();
                this.NeedRender = false;
            }
        }

        internal void Render()
        {
            this.renderer.Clear();
            this.renderer.RenderDrawList(this.DrawList, (int)this.ClientSize.Width, (int)this.ClientSize.Height);
            this.renderer.SwapBuffers();
        }

        private FormState formState = FormState.Normal;

        /// <summary>
        /// Call this to initialize GUI
        /// </summary>
        private void InitGUI()
        {
            // init the layout group of this form
            LoadFormGroup();

            //skin = new Skin();

            // init the event
            Event.current = new Event();

            this.NeedRender = true;
        }

        private void LoadFormGroup()
        {
            var formGroup = new LayoutGroup(true, GUIStyle.Default,
                GUILayout.Width(this.ClientSize.Width),
                GUILayout.Height(this.ClientSize.Height)
                );
            formGroup.isForm = true;
            layoutCache.Push(formGroup);
        }

#endregion

        /// <summary>
        /// Get the mouse position relative to this form
        /// </summary>
        internal Point GetMousePos()
        {
            return ScreenToClient(Input.Mouse.MousePos);
        }

        internal Point ScreenToClient(Point point)
        {
            return window.ScreenToClient(point);
        }

        internal Point ClientToScreen(Point point)
        {
            return window.ClientToScreen(point);
        }
    }
}