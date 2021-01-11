using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using ImGui.Development;
using ImGui.Input;
using ImGui.OSAbstraction.Graphics;

namespace ImGui
{
    static partial class Application
    {
        internal static void NewFrame()
        {
            GUIContext g = Application.ImGuiContext;
            WindowManager w = g.WindowManager;

            if (!g.Initialized)
            {
                // Initialize on first frame
                g.Initialized = true;
            }

            g.ConfigFlagsLastFrame = g.ConfigFlagsCurrFrame;
            g.ConfigFlagsCurrFrame = IO.ConfigFlags;

            // Time
            g.DeltaTime = Time.deltaTime;
            g.Time += g.DeltaTime;
            g.FrameCount++;
            
            Metrics.ActiveWindows = 0;

            //fps
            var detlaTime = g.Time - g.lastFPSUpdateTime;
            g.lastFrameCount++;
            if (detlaTime > 1000)
            {
                g.fps = (int)g.lastFrameCount;
                g.lastFrameCount = 0;
                g.lastFPSUpdateTime = g.Time;
            }
            
            UpdateViewportsNewFrame();

            #region Input
            //TODO move to Mouse
            #region mouse position
            if (Mouse.Instance.Position.X < 0 && Mouse.Instance.Position.Y < 0)
                Mouse.Instance.Position = new Point(-9999.0f, -9999.0f);
            if ((Mouse.Instance.Position.X < 0 && Mouse.Instance.Position.Y < 0) || (Mouse.Instance.LastPosition.X < 0 && Mouse.Instance.LastPosition.Y < 0))   // if mouse just appeared or disappeared (negative coordinate) we cancel out movement in MouseDelta
                Mouse.Instance.MouseDelta = Vector.Zero;
            else
                Mouse.Instance.MouseDelta = Mouse.Instance.Position - Mouse.Instance.LastPosition;
            Mouse.Instance.LastPosition = Mouse.Instance.Position;
            #endregion

            #region mouse left button
            Mouse.Instance.LeftButtonPressed = Mouse.Instance.LeftButtonState == KeyState.Down && Mouse.Instance.LeftButtonDownDuration < 0;
            Mouse.Instance.LeftButtonReleased = Mouse.Instance.LeftButtonState == KeyState.Up && Mouse.Instance.LeftButtonDownDuration >= 0;
            Mouse.Instance.LeftButtonDownDurationPrev = Mouse.Instance.LeftButtonDownDuration;
            Mouse.Instance.LeftButtonDownDuration = Mouse.Instance.LeftButtonState == KeyState.Down ? (Mouse.Instance.LeftButtonDownDuration < 0 ? 0 : Mouse.Instance.LeftButtonDownDuration + g.DeltaTime) : -1;
            Mouse.Instance.LeftButtonDoubleClicked = false;
            if (Mouse.Instance.LeftButtonPressed)
            {
                if (g.Time - Mouse.Instance.LeftButtonClickedTime < Mouse.DoubleClickIntervalTimeSpan)
                {
                    if ((Mouse.Instance.Position - Mouse.Instance.LeftButtonPressedPosition).LengthSquared < Mouse.DoubleClickMaxDistance * Mouse.DoubleClickMaxDistance)
                    {
                        Mouse.Instance.LeftButtonDoubleClicked = true;
                    }
                    Mouse.Instance.LeftButtonClickedTime = -99999; // so the third click isn't turned into a double-click
                }
                else
                {
                    Mouse.Instance.LeftButtonClickedTime = g.Time;
                }
                Mouse.Instance.LeftButtonPressedPosition = Mouse.Instance.Position;
                Mouse.Instance.DragMaxDistanceSquared = 0;
            }
            else if (Mouse.Instance.LeftButtonState == KeyState.Down)
            {
                Mouse.Instance.DragMaxDistanceSquared = Math.Max(Mouse.Instance.DragMaxDistanceSquared, (Mouse.Instance.Position - Mouse.Instance.LeftButtonPressedPosition).LengthSquared);
            }
            if (Mouse.Instance.LeftButtonPressed) ++Mouse.Instance.LeftButtonPressedTimes;
            if (Mouse.Instance.LeftButtonReleased) ++Mouse.Instance.LeftButtonReleasedTimes;
            if (Mouse.Instance.LeftButtonDoubleClicked) ++Mouse.Instance.LeftButtonDoubleClickedTimes;
            #endregion

            #region mouse right button
            Mouse.Instance.RightButtonPressed = Mouse.Instance.RightButtonState == KeyState.Down && Mouse.Instance.RightButtonDownDuration < 0;
            Mouse.Instance.RightButtonReleased = Mouse.Instance.RightButtonState == KeyState.Up && Mouse.Instance.RightButtonDownDuration >= 0;
            Mouse.Instance.RightButtonDownDuration = Mouse.Instance.RightButtonState == KeyState.Down ? (Mouse.Instance.RightButtonDownDuration < 0 ? 0 : Mouse.Instance.RightButtonDownDuration + g.DeltaTime) : -1;

            if (Mouse.Instance.RightButtonPressed) ++Mouse.Instance.RightButtonPressedTimes;
            if (Mouse.Instance.RightButtonReleased) ++Mouse.Instance.RightButtonReleasedTimes;
            #endregion

            #endregion

            // Update HoverId data
            // 1. record data related to previous frame
            // 2. reset
            g.HoverId = 0;
            g.HoverIdAllowOverlap = false;
            g.HoveredIdPreviousFrame = g.HoverId;

            // Update ActiveId data
            // 1. record data related to previous frame
            // 2. reset
            if (g.ActiveIdIsAlive != g.ActiveId && g.ActiveIdPreviousFrame == g.ActiveId && g.ActiveId != 0)
            {//Clear reference to active widget if the widget isn't alive anymore
                g.SetActiveID(0, null);
            }
            g.ActiveIdPreviousFrame = g.ActiveId;
            g.ActiveIdIsAlive = 0;
            g.ActiveIdPreviousFrameIsAlive = false;
            g.ActiveIdIsJustActivated = false;

            w.NewFrame(g);

            MainForm.ForeBackGroundRenderOpen();

            // [DEBUG] Item picker tool - start with DebugStartItemPicker()
            // useful to visually select an item and break into its call-stack.
            UpdateDebugToolItemPicker();

            // Create implicit debug window - we will only render it if the user has added something to it.
            GUI.Begin("Debug##Default", Application.InitialDebugWindowRect);
        }

        internal static void EndFrame()
        {
            GUIContext g = Application.ImGuiContext;
            WindowManager w = g.WindowManager;
            Debug.Assert(g.Initialized);                       // Forgot to call ImGui::NewFrame()
            Debug.Assert(g.FrameCountEnded != g.FrameCount);   // ImGui::EndFrame() called multiple times, or forgot to call ImGui::NewFrame() again

            // Hide implicit "Debug" window if it hasn't been used
            Debug.Assert(w.WindowStack.Count == 1);    // Mismatched Begin()/End() calls
            if (w.CurrentWindow != null && !w.CurrentWindow.Accessed)
            {
                w.CurrentWindow.Active = false;
            }
            GUI.End();//end of the implicit "Debug" window

            w.CurrentViewport = null;
            
            //TODO drag and drop
            
            MainForm.ForeBackGroundRenderClose();

            // End frame
            g.FrameCountEnded = g.FrameCount;

            // Initiate moving window + handle left-click and right-click focus
            UpdateMouseMovingWindowEndFrame();

            // Update user-facing viewport list (g.Viewports -> g.PlatformIO.Viewports after filtering out some)
            UpdateViewportsEndFrame();

            // Update windows
            // TODO merge UpdateViewportsEndFrame to w.EndFrame
            w.EndFrame(g);

            // Clear Input data for next frame
            Mouse.Instance.MouseWheel = 0;
            Ime.ImeBuffer.Clear();

        }

        internal static void Render()
        {
            GUIContext g = Application.ImGuiContext;
            if (MainForm.Closed)
            {
                return;
            }
            WindowManager w = g.WindowManager;

            Debug.Assert(g.Initialized);   // Make sure that NewFrame() is called.

            if (g.FrameCountEnded != g.FrameCount)
            {
                EndFrame();
            }
            g.FrameCountRendered = g.FrameCount;

            Metrics.VertexNumber = 0;
            Metrics.IndexNumber = 0;
            Metrics.RenderWindows = 0;
            
            //reset MeshBuffer and MeshList
            {
                foreach (var form in w.Viewports)
                {
                    form.MeshBuffer.Clear();
                    form.MeshBuffer.Init();
                    form.backgroundMeshList.Clear();
                    form.foregroundMeshList.Clear();
                }
                foreach (var window in w.Windows)
                {
                    window.MeshList.Clear();
                }
            }

            //build MeshList of all forms from their RenderTree
            {
                foreach (var form in w.Viewports)
                {
                    form.RenderToForegroundList();
                }

                foreach (var window in w.Windows)
                {
                    if (!window.Active && !Utility.HasAllFlags(window.Flags, WindowFlags.ChildWindow))
                    {
                        continue;
                    }

                    window.RenderToMeshList();
                }
                
                foreach (var form in w.Viewports)
                {
                    form.RenderToForegroundList();
                }
            }

            //append all MeshLists to form's MeshBuffer
            {
                foreach (var form in w.Viewports)
                {
                    form.MeshBuffer.Append(form.backgroundMeshList);
                }
                foreach (var window in w.Windows)
                {
                    var meshBuffer = window.Viewport.MeshBuffer;
                    meshBuffer.Append(window.MeshList);
                    if (window.Name != Metrics.WindowName)
                    {
                        Metrics.VertexNumber += meshBuffer.ShapeMesh.VertexBuffer.Count
                            + meshBuffer.ImageMesh.VertexBuffer.Count
                            + meshBuffer.TextMesh.VertexBuffer.Count;
                        Metrics.IndexNumber += meshBuffer.ShapeMesh.IndexBuffer.Count
                            + meshBuffer.ImageMesh.IndexBuffer.Count
                            + meshBuffer.TextMesh.IndexBuffer.Count;
                        Metrics.RenderWindows++;
                    }
                }
                foreach (var form in w.Viewports)
                {
                    form.MeshBuffer.Append(form.foregroundMeshList);
                }
            }

            //render each form's MeshBuffer to back-buffer
            foreach (var form in w.Viewports)
            {
                form.renderer.Clear(MainForm.BackgroundColor);
                var meshBuffer = form.MeshBuffer;
                form.renderer.DrawMeshes(
                    (int)MainForm.ClientSize.Width, (int)MainForm.ClientSize.Height,
                    (meshBuffer.ShapeMesh, meshBuffer.ImageMesh, meshBuffer.TextMesh));
            }

            //swap front and back-buffer
            MainForm.renderer.SwapBuffers();
        }

        internal static void Log()
        {
            if (ImGui.Log.Enabled && ImGui.Log.LogStatus)
            {
                //Status logging is time consuming, so we lower down the fps by sleeping here.
                Thread.Sleep(40);
            }
            
            GUIContext g = Application.ImGuiContext;
            MainForm.Platform_SetWindowTitle(
                $"fps:{g.fps,5:0.0}," +
                $" mouse pos: {Mouse.Instance.Position}," +
                $" deltaTime: {g.DeltaTime}ms," +
                $" {g.DevOnly_GetNodeName(g.HoverId)}");

            var l = Application.Logger;
            if (ImGui.Log.Enabled && ImGui.Log.LogStatus)
            {
                WindowManager w = g.WindowManager;
                l.Clear();
                l.Msg("fps:{0,5:0.0}, mouse pos: {1}, deltaTime: {2}ms", g.fps, Mouse.Instance.Position, g.DeltaTime);
                l.Msg("Input");
                l.Msg("    LeftButtonState {0}", Mouse.Instance.LeftButtonState);
                l.Msg("    LeftButtonDownDuration {0}ms", Mouse.Instance.LeftButtonDownDuration);
                l.Msg("    LeftButtonPressed {0}, {1} times", Mouse.Instance.LeftButtonPressed, Mouse.Instance.LeftButtonPressedTimes);
                l.Msg("    LeftButtonReleased {0}, {1} times", Mouse.Instance.LeftButtonReleased, Mouse.Instance.LeftButtonReleasedTimes);
                l.Msg("    LeftButtonDoubleClicked {0}, {1} times", Mouse.Instance.LeftButtonDoubleClicked, Mouse.Instance.LeftButtonDoubleClickedTimes);

                l.Msg("ActiveId: {0}, ActiveIdIsAlive: {1}", g.ActiveId, g.ActiveIdIsAlive);
                l.Msg("HoverId: {0}", g.HoverId);

                l.Msg("Window:");
                l.Msg("    HoveredWindow: {0}", (w.HoveredWindow != null) ? w.HoveredWindow.ID.ToString() : "<none>");
                l.Msg("    MovingWindow: {0}", (w.MovingWindow != null) ? w.MovingWindow.ID.ToString() : "<none>");
                l.Msg("    Window List:");
                for (int i = 0; i < w.Windows.Count; i++)
                {
                    var window = w.Windows[i];
                    l.Msg($"        [{i}]:{window.Name}, active: {window.Active}, rect: {window.Rect}");
                }
            }
        }

        // [DEBUG] Item picker tool - start with DebugStartItemPicker()
        // useful to visually select an item and break into its call-stack.
        private static void UpdateDebugToolItemPicker()
        {
            var g = GUILayout.GetCurrentContext();
            g.DebugItemPickerBreakID = 0;
            if (g.DebugItemPickerActive)
            {
                var hovered_id = g.HoveredIdPreviousFrame;
                //Mouse.Instance.Cursor = Cursor.Hand;
                if (Input.Keyboard.Instance.KeyPressed(Key.Escape))
                    g.DebugItemPickerActive = false;
                if (Input.Mouse.Instance.LeftButtonPressed && hovered_id != 0)
                {
                    g.DebugItemPickerBreakID = hovered_id;
                    g.DebugItemPickerActive = false;
                }
                //TODO draw tooltip to display hovered item info
            }
        }

        private static void UpdateViewportsNewFrame()
        {
            var g = ImGuiContext;
            var w = g.WindowManager;

            //update minimized status
            foreach (var form in w.Viewports)
            {
                if (!form.PlatformWindowCreated)
                {
                    continue;
                }
                if (form.IsMinimized)
                {
                    form.Flags |= ImGuiViewportFlags.Minimized;
                }
                else
                {
                    form.Flags &= ImGuiViewportFlags.Minimized;
                }
            }

            // fetch latest viewport info via platform APIs
            var mainForm = w.MainForm;
            //TODO size should be only adjusted by
            //* dragging #Move node
            //* user-specified size in GUI.Begin
            var mainFormPosition = mainForm.PlatformPosition;
            var mainFormSize = mainForm.Size;
            if (Utility.HasAllFlags(mainForm.Flags, ImGuiViewportFlags.Minimized))
            {// Preserve last pos/size when minimized
                mainFormPosition = mainForm.Pos;
                mainFormSize = mainForm.Size;
            }

            AddUpdateViewport(null, IMGUI_VIEWPORT_DEFAULT_ID, mainFormPosition, mainFormSize,
                ImGuiViewportFlags.CanHostOtherWindows);

            w.CurrentViewport = null;
            w.MouseViewport = null;
            
            // Erase unused viewports
            for (int n = 0; n < w.Viewports.Count; n++)
            {
                var viewport = w.Viewports[n];
                viewport.Idx = n;
                
                if (n > 0 && viewport.LastFrameActive < g.FrameCount - 2)
                {
                    // Clear references to this viewport in windows (window->ViewportId becomes the master data)
                    foreach (var window in w.Windows)
                    {
                        if (window.Viewport == viewport)
                        {
                            window.Viewport = null;
                            window.ViewportOwned = false;
                        }
                    }

                    if (viewport == g.MouseLastHoveredViewport)
                    {
                        g.MouseLastHoveredViewport = null;
                    }
                    w.Viewports.RemoveAt(n);

                    // Destroy
                    Debug.WriteLine($"Delete Viewport {viewport.ID} ({viewport.debugName})");
                    viewport.Close();// In most circumstances the platform window will already be destroyed here.
                    //Debug.Assert(g.PlatformIO.Viewports.Contains(viewport) == false);//TODO purpose of PlatformIO.Viewports
                    n--;
                    continue;
                }
                
                // Update Position and Size (from Platform Window to ImGui) if requested.
                // We do it early in the frame instead of waiting for UpdatePlatformWindows() to avoid a frame of lag when moving/resizing using OS facilities.
                if (!Utility.HasAllFlags(viewport.Flags, ImGuiViewportFlags.Minimized)
                    && viewport.PlatformWindowCreated)
                {
                    if (viewport.PlatformRequestMove)
                        viewport.Pos = viewport.LastPlatformPos = viewport.PlatformPosition;
                    if (viewport.PlatformRequestResize)
                        viewport.Size = viewport.LastPlatformSize = viewport.PlatformSize;
                }

                // Reset alpha every frame. Users of transparency (docking) needs to request a lower alpha back.
                viewport.Alpha = 1.0f;
                
                // Translate imgui windows when a Host Viewport has been moved
                // (This additionally keeps windows at the same place when ImGuiConfigFlags_ViewportsEnable is toggled!)
                var viewport_delta_pos = viewport.Pos - viewport.LastPos;
                if (Utility.HasAllFlags(viewport.Flags, ImGuiViewportFlags.CanHostOtherWindows) &&
                    (viewport_delta_pos.X != 0.0f || viewport_delta_pos.Y != 0.0f))
                {
                    TranslateWindowsInViewport(viewport, viewport.LastPos, viewport.Pos);
                }

            }
            
            // Mouse handling: decide on the actual mouse viewport for this frame between the active/focused viewport and the hovered viewport.
            // Note that 'viewport_hovered' should skip over any viewport that has the ImGuiViewportFlags.NoInputs flags set.
            Form viewport_hovered = null;
            if (Utility.HasAllFlags(IO.BackendFlags, ImGuiBackendFlags.HasMouseHoveredViewport))
            {
                viewport_hovered = Input.Mouse.Instance.MouseHoveredViewportId > 0 ? 
                    w.FindViewportById(Input.Mouse.Instance.MouseHoveredViewportId) : null;
                if (viewport_hovered!=null && Utility.HasAllFlags(viewport_hovered.Flags, ImGuiViewportFlags.NoInputs))
                {
                    Debug.Fail("Backend failed at honoring its contract if it returned a viewport with the NoInputs flag.");
                    viewport_hovered = FindHoveredViewportFromPlatformWindowStack(Input.Mouse.Instance.Position);
                }
            }
            else
            {
                // If the backend doesn't know how to honor ImGuiViewportFlags.NoInputs, we do a search ourselves. Note that this search:
                // A) won't take account of the possibility that non-imgui windows may be in-between our dragged window and our target window.
                // B) uses LastFrameAsRefViewport as a flawed replacement for the last time a window was focused (we could/should fix that by introducing Focus functions in PlatformIO)
                viewport_hovered = FindHoveredViewportFromPlatformWindowStack(Input.Mouse.Instance.Position);
            }
            if (viewport_hovered != null)
                g.MouseLastHoveredViewport = viewport_hovered;
            else if (g.MouseLastHoveredViewport == null)
                g.MouseLastHoveredViewport = w.MainForm;

            // Update mouse reference viewport
            // (when moving a window we aim at its viewport, but this will be overwritten below if we go in drag and drop mode)
            if (w.MovingWindow != null)
            {
                w.MouseViewport = w.MovingWindow.Viewport;
            }
            else
            {
                w.MouseViewport = g.MouseLastHoveredViewport;
            }

            Debug.Assert(w.MouseViewport != null);
        }
        
        // Update user-facing viewport list (g.Viewports -> g.PlatformIO.Viewports after filtering out some)
        private static void UpdateViewportsEndFrame()
        {
            var g = ImGuiContext;
            var w = g.WindowManager;

            //TODO understand g.PlatformIO.Viewports vs g.Viewports

            for (int i = 0; i < w.Viewports.Count; i++)
            {
                Form viewport = w.Viewports[i];
                viewport.LastPos = viewport.Pos;
                if (viewport.LastFrameActive < g.FrameCount
                    || viewport.Size.Width <= 0.0f || viewport.Size.Height <= 0.0f)
                    if (i > 0) // Always include main viewport in the list
                        continue;
                if (viewport.Window != null && !viewport.Window.ActiveAndVisible)
                    continue;
                if (i > 0)
                    Debug.Assert(viewport.Window != null);
                //TODO understand g.PlatformIO.Viewports vs g.Viewports
            }
            MainForm.ClearRequestFlags(); // Clear main viewport flags because UpdatePlatformWindows() won't do it and may not even be called
        }
        
        // If the backend doesn't set MouseLastHoveredViewport or doesn't honor ImGuiViewportFlags_NoInputs, we do a search ourselves.
        // A) It won't take account of the possibility that non-imgui windows may be in-between our dragged window and our target window.
        // B) It requires Platform_GetWindowFocus to be implemented by backend.
        static Form FindHoveredViewportFromPlatformWindowStack(Point mouse_platform_pos)
        {
            var g = ImGuiContext;
            var w = g.WindowManager;
            Form best_candidate = null;
            for (int n = 0; n < w.Viewports.Count; n++)
            {
                Form viewport = w.Viewports[n];
                if (!Utility.HasAllFlags(viewport.Flags, 
                        ImGuiViewportFlags.NoInputs | ImGuiViewportFlags.Minimized)
                    && viewport.Rect/*TODO use client rect?*/.Contains(mouse_platform_pos))
                {
                    if (best_candidate == null ||
                        best_candidate.LastFrontMostStampCount < viewport.LastFrontMostStampCount)
                    {
                        best_candidate = viewport;
                    }
                }
            }
            return best_candidate;
        }

        // Translate imgui windows when a Host Viewport has been moved
        // (This additionally keeps windows at the same place when ImGuiConfigFlags_ViewportsEnable is toggled!)
        private static void TranslateWindowsInViewport(Form viewport, Point old_pos, Point new_pos)
        {
            Debug.Assert(viewport.Window == null
                && Utility.HasAllFlags(viewport.Flags, ImGuiViewportFlags.CanHostOtherWindows));
            var g = ImGuiContext;
            var w = g.WindowManager;

            // 1) We test if ImGuiConfigFlags_ViewportsEnable was just toggled, which allows us to conveniently
            // translate imgui windows from OS-window-local to absolute coordinates or vice-versa.
            // 2) If it's not going to fit into the new size, keep it at same absolute position.
            // One problem with this is that most Win32 applications doesn't update their render while dragging,
            // and so the window will appear to teleport when releasing the mouse.
            bool translate_all_windows =
                (g.ConfigFlagsCurrFrame & ImGuiConfigFlags.ViewportsEnable)
                !=
                (g.ConfigFlagsLastFrame & ImGuiConfigFlags.ViewportsEnable);
            Rect test_still_fit_rect = new Rect(old_pos, old_pos + (Vector)viewport.Size);
            Vector delta_pos = new_pos - old_pos;
            foreach (var window in w.Windows)
            {
                if (translate_all_windows)
                {
                    window.Position += delta_pos;
                }
                else if (window.Viewport == viewport &&
                    test_still_fit_rect.Contains(window.Rect))
                {
                    window.Position += delta_pos;
                }
            }
        }

        private static Form AddUpdateViewport(Window window, int id, Point pos, Size size,
            ImGuiViewportFlags flags)
        {
            var g = ImGuiContext;
            var w = g.WindowManager;
            Debug.Assert(id != 0);

            if (window != null)
            {
                if (w.MovingWindow?.RootWindow == window)
                    flags |= ImGuiViewportFlags.NoInputs | ImGuiViewportFlags.NoFocusOnAppearing;
                if (Utility.HasAllFlags(window.Flags, WindowFlags.NoMouseInputs | WindowFlags.NoNavInputs))
                    flags |= ImGuiViewportFlags.NoInputs;
                if (Utility.HasAllFlags(window.Flags, WindowFlags.NoFocusOnAppearing))
                    flags |= ImGuiViewportFlags.NoFocusOnAppearing;
            }

            var viewport = w.FindViewportById(id);
            if (viewport != null)
            {
                if (!viewport.PlatformRequestMove)
                    viewport.Pos = pos;
                if (!viewport.PlatformRequestResize)
                    viewport.Size = size;
                viewport.Flags = flags | (viewport.Flags & ImGuiViewportFlags.Minimized); // Preserve existing flags
            }
            else
            {
                // New viewport
                viewport = new Form(pos, size);
                viewport.ID = id;
                viewport.Idx = w.Viewports.Count;
                viewport.Pos = viewport.LastPos = pos;
                viewport.Size = size;
                viewport.Flags = flags;
                //UpdateViewportPlatformMonitor(viewport);//TODO
                w.Viewports.Add(viewport);
                Debug.WriteLine($"Add Viewport {id} ({window?.Name})");//TODO why use window name?

                #if TODO 
                // We normally setup for all viewports in NewFrame() but here need to handle the mid-frame creation of a new viewport.
                // We need to extend the fullscreen clip rect so the OverlayDrawList clip is correct for that the first frame
                g.DrawListSharedData.ClipRectFullscreen.x = ImMin(g.DrawListSharedData.ClipRectFullscreen.x, viewport.Pos.x);
                g.DrawListSharedData.ClipRectFullscreen.y = ImMin(g.DrawListSharedData.ClipRectFullscreen.y, viewport.Pos.y);
                g.DrawListSharedData.ClipRectFullscreen.z = ImMax(g.DrawListSharedData.ClipRectFullscreen.z, viewport.Pos.x + viewport.Size.x);
                g.DrawListSharedData.ClipRectFullscreen.w = ImMax(g.DrawListSharedData.ClipRectFullscreen.w, viewport.Pos.y + viewport.Size.y);
                #endif
            }

            viewport.Window = window;
            viewport.LastFrameActive = g.FrameCount;
            Debug.Assert(window == null || viewport.ID == window.ID);

            if (window != null)
                window.ViewportOwned = true;

            return viewport;
        }
        
        // Initiate moving window when clicking on empty space or title bar.
        // Handle left-click and right-click focus.
        private static void UpdateMouseMovingWindowEndFrame()
        {
            var g = ImGuiContext;
            if (g.ActiveId != 0 || g.HoverId != 0)
                return;

            var w = g.WindowManager;

            // Click on void to focus window and start moving
            // (after we're done with all our widgets, so e.g. clicking on docking tab-bar which have set HoveredId already and not get us here!)
            if (Mouse.Instance.LeftButtonPressed)
            {
                //TODO Remove logic about not-implemented features like pop-up, modal and docking.
                Window root_window = w.HoveredWindow;

                if (root_window != null)
                {
                    StartMouseMovingWindow(w.HoveredWindow);

                    // Cancel moving if clicked outside of title bar
                    if (IO.ConfigWindowsMoveFromTitleBarOnly)
                        if (!Utility.HasAllFlags(root_window.Flags, WindowFlags.NoTitleBar))
                            if (!root_window.TitleBarRect.Contains(Input.Mouse.Instance.LeftButtonPressedPosition))
                                w.MovingWindow = null;
                }
                else if (root_window == null)
                {
                    // Clicking on void disable focus
                    w.FocusWindow(null);
                }
            }
        }
        
        private static void StartMouseMovingWindow(Window window)
        {
            // Set ActiveId even if the WindowFlags.NoMove flag is set. Without it, dragging away from a window with _NoMove would activate hover on other windows.
            // We _also_ call this when clicking in a window empty space when ConfigWindowsMoveFromTitleBarOnly is set, but clear w.MovingWindow afterward.
            // This is because we want ActiveId to be set even when the window is not permitted to move.
            var g = ImGuiContext;
            var w = g.WindowManager;
            w.FocusWindow(window);
            g.SetActiveID(window.MoveId, window);
            g.ActiveIdNoClearOnFocusLoss = true;
            g.ActiveIdClickOffset = Input.Mouse.Instance.LeftButtonPressedPosition - window.RootWindow.Position;

            bool canMoveWindow = !(
                Utility.HasAllFlags(window.Flags, WindowFlags.NoMove)
                || Utility.HasAllFlags(window.RootWindow.Flags, WindowFlags.NoMove)
                );

            if (canMoveWindow)
            {
                w.MovingWindow = window;
            }
        }
    }
}
