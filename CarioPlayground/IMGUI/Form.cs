using Cairo;
using System.Collections.Generic;

namespace ImGui
{
    public abstract class Form : SFMLForm
    {
        public static Form current;
        internal Dictionary<string, Control> controls;
        internal GUIRenderer guiRenderer;
        internal LayoutCache layoutCache = new LayoutCache();

        internal Point originalPosition;
        internal Size originalSize;

        protected Form(int width, int height)
            : base(width, height)
        {
            controls = new Dictionary<string, Control>();

            originalSize = new Size(this.internalForm.Size.X, this.internalForm.Size.Y);
            originalPosition = Position;

            var size = this.internalForm.Size;
            guiRenderer = new GUIRenderer();
            guiRenderer.OnLoad(new Size(size.X, size.Y));
            guiRenderer.PrintGraphicInfo();

            InitGUI();
        }

        public FormState FormState
        {
            get { return formState; }
            set { formState = value; }
        }

        internal Dictionary<string, Control> Controls
        {
            get { return controls; }
        }

        internal LayoutCache LayoutCache
        {
            get { return layoutCache; }
        }

        internal ImageSurface FrontSurface
        {
            get { return renderContext.FrontSurface; }
        }

        internal ImageSurface DebugSurface
        {
            get { return renderContext.DebugSurface; }
        }

        internal Context DebugContext
        {
            get { return renderContext.DebugContext; }
        }

        #region Overrides of SFMLForm

        public override void Show()
        {
            base.Show();
            Event.current.type = EventType.Layout;
        }

        public override void Close()
        {
            this.renderContext.Dispose();

            foreach (var pair in renderBoxMap)
            {
                var control = pair.Value;
                if (control.Type != RenderBoxType.Dummy)
                {
                    control.Content.Dispose();
                }
            }

            base.Close();
        }

        public override void Minimize()
        {
            if (this.Visible)
            {
                Event.current.type = EventType.MinimizeWindow;
            }
        }

        public override void Maximize()
        {
            if (this.Visible)
            {
                Event.current.type = EventType.MaximizeWindow;
            }
        }

        public override void Normalize()
        {
            if (this.Visible)
            {
                Event.current.type = EventType.NormalizeWindow;
            }
        }

        #endregion

        #region the GUI Loop

        internal struct GUILoopResult
        {
            public readonly Rect dirtyRect;
            public readonly bool needExit;

            public GUILoopResult(bool needExit, Rect dirtyRect)
            {
                this.dirtyRect = dirtyRect;
                this.needExit = needExit;
            }
        }

        internal static void BeginGUI(bool useGUILayout)
        {
            if (useGUILayout)
            {
                LayoutUtility.Begin();
            }
        }

        internal static void EndGUI()
        {
            if (Event.current.type == EventType.Layout)
            {
                LayoutUtility.Layout();
                Event.current.type = EventType.Repaint;
            }
        }

        /// <summary>
        /// GUI Loop
        /// </summary>
        internal GUILoopResult GUILoop()
        {
            current = this;

            Rect dirtyRect;
            handleEvent(out dirtyRect);//TODO Use event message queue. Do not change Event.current directly!

            // deactive all render-boxes
            foreach (var pair in renderBoxMap)
            {
                var renderBox = pair.Value;
                renderBox.Active = false;
            }

            return new GUILoopResult(needExit, dirtyRect);
        }

        private void handleEvent(out Rect dirtyRect)
        {
            dirtyRect = Rect.Empty;
            switch (Event.current.type)
            {
                case EventType.Layout:
                    LayoutUtility.current.Clear();
                    LoadFormGroup();
                    BeginGUI(true);
                    OnGUI();
                    EndGUI();
                    break;
                case EventType.Repaint:
                    if (this.Visible)
                    {
                        BeginGUI(true);
                        OnGUI();
                        EndGUI();
                        dirtyRect = DoRender();
                        Event.current.type = EventType.Used;
                    }
                    break;
                case EventType.MaximizeWindow:
                {
                    Rect rect;
                    Utility.MaximizeForm(this, out rect);
                    FormState = FormState.Maximized;
                    renderContext.Dispose();
                    renderContext.Build((int)rect.Width, (int)rect.Height);
                    this.guiRenderer.OnLoad(rect.Size);
                    foreach (var pair in renderBoxMap)
                    {
                        SimpleControl simpleControl = (SimpleControl)pair.Value;
                        simpleControl.NeedRepaint = true;
                        //simpleControl.State = "Normal";
                    }

                    Event.current.type = EventType.Layout;
                }
                    break;
                case EventType.MinimizeWindow:
                    BeginGUI(true);
                    OnGUI();
                    EndGUI();
                    Utility.MinimizeForm(this);
                    FormState = FormState.Minimized;
                    Event.current.type = EventType.Used;
                    break;
                case EventType.NormalizeWindow:
                    Utility.NormalizeForm(this);
                    FormState = FormState.Normal;
                    renderContext.Dispose();
                    renderContext.Build((int)originalSize.Width, (int)originalSize.Height);
                    this.guiRenderer.OnLoad(originalSize);
                    foreach (var pair in renderBoxMap)
                    {
                        SimpleControl simpleControl = (SimpleControl)pair.Value;
                        simpleControl.NeedRepaint = true;
                        //simpleControl.State = "Normal";
                    }

                    Event.current.type = EventType.Layout;
                    break;
                default:
                    BeginGUI(true);
                    OnGUI();
                    EndGUI();
                    break;
            }
        }

        private Rect DoRender()
        {
            var dirtyRect = Rect.Empty;
            foreach (var box in this.renderBoxMap.Values)
            {
                if (box.Type == RenderBoxType.Dummy) continue;
                if (!box.Active) continue;
                if (box.NeedRepaint)
                {
                    renderContext.BackContext.DrawBoxModel(box.Rect, box.Content, box.Style);
                    box.NeedRepaint = false;
                    dirtyRect.Union(box.Rect);
                }
            }

            if (dirtyRect != Rect.Empty)
            {
                renderContext.SwapSurface();
            }
            return dirtyRect;
        }

        private bool needExit;

        public void RequestClose()
        {
            needExit = true;
        }

        private RenderContext renderContext;
        internal readonly Dictionary<string, IRenderBox> renderBoxMap = new Dictionary<string, IRenderBox>();
        private FormState formState = FormState.Normal;

        /// <summary>
        /// Call this to initialize GUI
        /// </summary>
        private void InitGUI()
        {
            var clientWidth = (int) internalForm.Size.X;
            var clientHeight = (int) internalForm.Size.Y;

            // init the render context
            renderContext = new RenderContext(clientWidth, clientHeight);

            // init the layout group of this form
            LoadFormGroup();

            // init the event
            Event.current = new Event();
        }

        private void LoadFormGroup()
        {
            var formGroup = new LayoutGroup(true, Style.None, GUILayout.Width(this.Size.Width),
                GUILayout.Height(this.Size.Height));
            formGroup.isForm = true;
            layoutCache.Push(formGroup);
        }

        #endregion
    }
}