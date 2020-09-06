using ImGui.Input;
using ImGui.Rendering;

namespace ImGui
{
    internal partial class Window
    {
        public void FirstUpdate(string name, Size size, ref bool open, double backgroundAlpha,
            WindowFlags flags,
            long currentFrame, Window parentWindow)
        {
            //short names
            var form = Form.current;
            var g = form.uiContext;
            var w = g.WindowManager;

            this.Active = true;
            this.BeginCount = 0;
            this.LastActiveFrame = currentFrame;

            // determine if window is collapsed
            if (!flags.HaveFlag(WindowFlags.NoTitleBar) && !flags.HaveFlag(WindowFlags.NoCollapse))
            {
                // Collapse window by double-clicking on title bar
                if (w.HoveredWindow == this && g.IsMouseHoveringRect(this.TitleBarRect) &&
                    Mouse.Instance.LeftButtonDoubleClicked)
                {
                    this.Collapsed = !this.Collapsed;
                    w.FocusWindow(this);
                    open = !this.Collapsed; //overwrite the open state
                }
            }

            this.Collapsed = !open;

            //window container
            using (var dc = WindowContainer.RenderOpen())
            {
                dc.DrawBoxModel(WindowContainer);
            }

            //update title bar
            var windowRounding = (float) this.WindowContainer.RuleSet.Get<double>(StylePropertyName.WindowRounding);
            if (!flags.HaveFlag(WindowFlags.NoTitleBar))
            {
                // background
                using (var dc = this.titleBar.RenderOpen())
                {
                    dc.DrawBoxModel(this.titleBar.RuleSet, this.titleBar.Rect);
                }

                //icon
                using (var dc = titleIcon.RenderOpen())
                {
                    dc.DrawImage(@"assets\images\logo.png", this.titleIcon.Rect);
                }

                //title
                using (var dc = titleText.RenderOpen())
                {
                    dc.DrawGlyphRun(titleText.RuleSet, this.Name, titleText.Rect.TopLeft);
                }
            }

            this.ShowWindowClientArea(!this.Collapsed);

            if (!this.Collapsed)
            {
                //show and update window client area
                using (var dc = clientArea.RenderOpen())
                {
                    dc.DrawBoxModel(clientArea);
                }

                if (!flags.HaveFlag(WindowFlags.NoResize))
                {
                    if (this.ResizeGripNode == null)
                    {
                        var id = this.GetID("#RESIZE");
                        var node = new Node(id, "Window_ResizeGrip");
                        this.ResizeGripNode = node;
                        this.AbsoluteVisualList.Add(node);
                    }

                    //resize grip
                    this.ResizeGripNode.ActiveSelf = true;
                    var resizeGripColor = Color.Clear;
                    var br = this.Rect.BottomRight;
                    if (!flags.HaveFlag(WindowFlags.AlwaysAutoResize) && !flags.HaveFlag(WindowFlags.NoResize))
                    {
                        // Manual resize
                        var resizeRect = new Rect(
                            br - new Vector(this.WindowContainer.PaddingLeft + this.WindowContainer.BorderLeft,
                                this.WindowContainer.PaddingBottom + this.WindowContainer.BorderLeft),
                            br);
                        var resizeId = this.GetID("#RESIZE");
                        GUIBehavior.ButtonBehavior(resizeRect, resizeId, out var hovered, out var held,
                            ButtonFlags.FlattenChilds);
                        resizeGripColor =
                            held
                                ? this.WindowContainer.RuleSet.Get<Color>(StylePropertyName.ResizeGripColor, GUIState.Active)
                                : hovered
                                    ? this.WindowContainer.RuleSet.Get<Color>(StylePropertyName.ResizeGripColor, GUIState.Hover)
                                    : this.WindowContainer.RuleSet.Get<Color>(StylePropertyName.ResizeGripColor);

                        if (hovered || held)
                        {
                            Mouse.Instance.Cursor = (Cursor.NwseResize);
                        }
                        else
                        {
                            Mouse.Instance.Cursor = (Cursor.Default);
                        }

                        if (held)
                        {
                            // We don't use an incremental MouseDelta but rather compute an absolute target size based on mouse position
                            var t = Mouse.Instance.Position - g.ActiveIdClickOffset - this.Position;
                            var newSizeWidth = t.X + resizeRect.Width;
                            var newSizeHeight = t.Y + resizeRect.Height;
                            var resizeSize = new Size(newSizeWidth, newSizeHeight);
                            this.ApplySize(resizeSize);
                        }
                    }

                    // Render resize grip
                    // (after the input handling so we don't have a frame of latency)
                    var borderBottom = this.WindowContainer.RuleSet.BorderBottom;
                    var paddingBottom = this.WindowContainer.RuleSet.PaddingBottom;
                    var borderRight = this.WindowContainer.RuleSet.BorderRight;
                    var paddingRight = this.WindowContainer.RuleSet.PaddingRight;
                    using (var dc = this.ResizeGripNode.RenderOpen())
                    {
                        var path = new PathGeometry();
                        var figure = new PathFigure();
                        var A = br + new Vector(-10 - borderRight - paddingRight, 0);
                        var B = br + new Vector(0, -10 - borderBottom - paddingBottom);
                        figure.StartPoint = A;
                        figure.IsFilled = true;
                        figure.Segments.Add(new LineSegment(B, false));
                        figure.Segments.Add(new LineSegment(br, false));
                        figure.Segments.Add(new LineSegment(A, false));
                        path.Figures.Add(figure);
                        dc.DrawGeometry(new Brush(resizeGripColor), null, path);
                    }
                }

                this.ContentRect = Rect.Zero;
            }
        }
    }
}