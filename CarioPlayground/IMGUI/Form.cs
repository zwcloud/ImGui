using Cairo;
using System.Collections.Generic;
using System.Diagnostics;

namespace ImGui
{
    public abstract class Form : SFMLForm
    {
        internal Dictionary<string, Control> controls;
        internal Dictionary<string, Control> Controls
        {
            get { return controls; }
        }

        protected Form(int width, int height)
            : base(width, height)
        {
            controls = new Dictionary<string, Control>();
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
            if(!Visible)
            {
                foreach (var control in Controls.Values)
                {
                    control.NeedRepaint = true;
                }
            }
        }

        #region Overrides of SFMLForm

        public override void Close()
        {
            this.renderContext.Dispose();
            foreach (var control in this.Controls.Values)
            {
                control.Dispose();
            }
            base.Close();
        }

        #endregion

        #endregion

        #region the GUI Loop

        internal struct GUILoopResult
        {
            public readonly bool needExit;
            public readonly bool isRepainted;

            public GUILoopResult(bool needExit, bool isRepainted)
            {
                this.isRepainted = isRepainted;
                this.needExit = needExit;
            }
        }

        /// <summary>
        /// GUI Loop
        /// </summary>
        /// <returns>true:need repaint / false: not repaint</returns>
        internal GUILoopResult GUILoop()
        {
            OnGUI(GUI);

            bool needExit, isRepainted = false;
            needExit = Update();
            if (this.Visible)
            {
                isRepainted = Render();
                isRepainted = isRepainted || Clear();
            }
            return new GUILoopResult(needExit, isRepainted);
        }

        private GUI GUI { get; set; }
        private RenderContext renderContext;

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

            //create GUI
            GUI = new GUI(renderContext.BackContext, this);
        }

        private readonly List<string> removeList = new List<string>();

        private bool Update()
        {
            foreach (var controlName in removeList)
            {
                var result = Controls.Remove(controlName);
                Debug.WriteLineIf(!result, "Remove failed");
            }
            removeList.Clear();
            foreach (var control in Controls.Values)
            {
                if (control.Active)
                {
                    control.OnUpdate();
                    control.Active = false;
                }
                else
                {
                    removeList.Add(control.Name);
                }
            }
            if (Input.Keyboard.KeyDown(SFML.Window.Keyboard.Key.Escape))
            {
                return true;
            }
            return false;
        }

        private bool Render()
        {
            var renderHappend = false;
            foreach (var control in Controls.Values)
            {
                if (control.NeedRepaint)
                {
                    renderHappend = true;
                    control.OnRender(renderContext.BackContext);
                    control.NeedRepaint = false;
                }
            }

            if (renderHappend)
            {
                renderContext.SwapSurface();
            }

            return renderHappend;
        }

        private bool Clear()
        {
            bool cleared = removeList.Count != 0;
            foreach (var controlName in removeList)
            {
                var control = Controls[controlName];
                control.OnClear(renderContext.BackContext);
                control.Dispose();
            }
            if (cleared)
            {
                renderContext.SwapSurface();
            }
            return cleared;
        }

        #endregion
    }
}