using Cairo;
using System.Collections.Generic;

namespace ImGui
{
    public abstract class Form : SFMLForm
    {
        internal static Form current;

        internal Dictionary<string, SimpleControl> simpleControls;
        internal Dictionary<string, SimpleControl> SimpleControls
        {
            get { return simpleControls; }
        }

        internal Dictionary<string, Control> controls;
        internal Dictionary<string, Control> Controls
        {
            get { return controls; }
        }

        internal LayoutCache layoutCache = new LayoutCache();
        internal LayoutCache LayoutCache { get{return layoutCache;}}

        protected Form(int width, int height)
            : base(width, height)
        {
            controls = new Dictionary<string, Control>();
            simpleControls = new Dictionary<string, SimpleControl>();
            var size = this.internalForm.Size;
            guiRenderer = new GUIRenderer(new Size(size.X, size.Y));
            guiRenderer.OnLoad();

            InitGUI();

        }

        internal GUIRenderer guiRenderer;

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
            Event.current = new Event();
            Event.current.type = EventType.Layout;
        }

        #region Overrides of SFMLForm

        public override void Close()
        {
            this.renderContext.Dispose();

            var controls = SimpleControls.Values;
            foreach (var control in controls)
            {
                if (control.Type != RenderBoxType.Dummy)
                {
                    control.Content.Dispose();
                }
            }

            base.Close();
        }

        #endregion

        #endregion

        #region the GUI Loop

        internal struct GUILoopResult
        {
            public readonly bool needExit;
            public readonly Rect dirtyRect;

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
            Form.current = this;
            var needExit = false;//dummy

            Rect dirtyRect = Rect.Empty;
            if (this.Visible && Event.current.type == EventType.Repaint)
            {
                BeginGUI(true);
                OnGUI();
                EndGUI();
                dirtyRect = DoRender();
                Event.current.type = EventType.Used;
            }
            else
            {
                BeginGUI(true);
                OnGUI();
                EndGUI();
            }

            this.renderBoxMap.Clear();
            return new GUILoopResult(needExit, dirtyRect);
        }

        private Rect DoRender()
        {
            Rect dirtyRect = Rect.Empty;
            foreach (var box in this.renderBoxMap.Values)
            {
                if(box.Type == RenderBoxType.Dummy) continue;
                if(box.NeedRepaint)
                {
                    renderContext.BackContext.DrawBoxModel(box.Rect, box.Content, box.Style);
                    box.NeedRepaint = false;
                    dirtyRect.Union(box.Rect);
                }
            }

            if (dirtyRect!=Rect.Empty)
            {
                renderContext.SwapSurface();
            }
            return dirtyRect;
        }

        private RenderContext renderContext;
        internal readonly Dictionary<string, IRenderBox> renderBoxMap = new Dictionary<string, IRenderBox>();

        /// <summary>
        /// Call this to initialize GUI
        /// </summary>
        private void InitGUI()
        {
            var clientWidth = (int)internalForm.Size.X;
            var clientHeight = (int)internalForm.Size.Y;

            //build all surface
            renderContext = new RenderContext();
            renderContext.BackSurface = CairoEx.BuildSurface(clientWidth, clientHeight,
                CairoEx.ColorWhite, Format.Argb32);
            renderContext.BackContext = new Context(renderContext.BackSurface);
            renderContext.FrontSurface = CairoEx.BuildSurface(clientWidth, clientHeight,
                CairoEx.ColorWhite, Format.Argb32);
            renderContext.FrontContext = new Context(renderContext.FrontSurface);

            renderContext.DebugSurface = CairoEx.BuildSurface(clientWidth, clientHeight,
                CairoEx.ColorClear, Format.Argb32);
            renderContext.DebugContext = new Context(renderContext.DebugSurface);

            // init layout
            LayoutGroup formGroup = new LayoutGroup();
            formGroup.isForm = true;
            formGroup.isVertical = true;
            formGroup.minHeight = formGroup.maxHeight = this.Size.Width;
            formGroup.minWidth = formGroup.maxWidth = this.Size.Height;
            layoutCache.Push(formGroup);
        }

        #endregion
    }
}