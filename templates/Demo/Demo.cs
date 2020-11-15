using ImGui;

public class Demo
{
    double f = 0.0f;
    Color clearColor = Color.Argb(255, 114, 144, 154);
    private bool showAnotherWindow = false;
    private bool showMetricsWindow = false;

    #region Demo
    bool showDemoWindow = false;

    System.Diagnostics.Stopwatch watch;
    private static long startTime;
    private static long deltaTime;

    public Demo()
    {
        watch = new System.Diagnostics.Stopwatch();
        watch.Start();
    }

    #region Help
    bool helpOn;
    #endregion

    #region Window Options
    bool windowsOptionsOn;
    bool no_titlebar = false;
    bool no_border = false;
    bool no_resize = false;
    bool no_move = false;
    bool no_scrollbar = false;
    bool no_collapse = false;
    double bg_alpha = 0.65;
    bool styleEditorOpen = false;
    bool loggingOpen = false;
    #endregion

    #region Widgets
    bool widgetsOn = false;
    bool a = false;
    int pressed_count = 0;
    bool open1, open2, open3, open4, open5, open6, open7, open8, open9, open10, open11, open12, open13, open14;
    bool[] openChild = new bool[5]{ false, false, false, false, false};
    bool closable_group = true;

    #region Selectable
    bool basicSelectableOpen = false;
    bool[] selected = { false, true, false, false };
    #endregion

    #region Filtered Text Input
    string defaultText = "";
    string decimalText = "";
    string hexadecimalText = "";
    string uppercaseText = "";
    string customText = "";
    string noBlankText = "";
    string password = "password123";
    #endregion

    string buf = "日本語";
    bool read_only = false;
    string multiLineText = @"/*\n
 The Pentium F00F bug, shorthand for F0 0F C7 C8,
 the hexadecimal encoding of one offending instruction,
 more formally, the invalid operand with locked CMPXCHG8B
 instruction bug, is a design flaw in the majority of
 Intel Pentium, Pentium MMX, and Pentium OverDrive
 processors (all in the P5 microarchitecture).
*/

label:
\tlock cmpxchg8b eax
";
    bool check;
    int active_id = 0;

    string str0 = "Hello, world!";
    int i0 = 123;
    float f0 = 0.001f;
    Color col1 = new Color(1.0f, 0.0f, 0.2f);
    Color col2 = new Color(0.4f, 0.7f, 0.0f, 0.5f);
    string[] listBoxItems = { "Apple", "Banana", "Cherry", "Kiwi", "Mango", "Orange", "Pineapple", "Strawberry", "Watermelon" };
    int currentListBoxItem = 0;

    #region Sliders
    double sliderValue = 0.01;
    double vSliderValue = 0.01;
    #endregion

    #endregion

    #region Graphics Widgets
    bool graphicsWidgetsOn;
    bool animate = true;
    double progress = 0.0, progress_dir = 1.0;
    #endregion

    #region Layout
    bool layoutOn = false;
    bool childRegionsOpen = false;
    int line = 50;
    bool stackLayoutOpen = false;
    bool layoutScopesOpen = false;
    #endregion

    #region Style & Skin
    bool skinOn = false;
    LayoutOptions smallRed = new LayoutOptions().FontColor(Color.Red).FontSize(20);
    bool defaultSkinOpen = false;
    bool dearImGuiSkinOpen = false;
    bool win10SkinOpen = false;

    #endregion

    #endregion
    
    private string text = "Quick brown fox";
    private double value = 0.6;

    public void OnGUI()
    {
        deltaTime = watch.ElapsedMilliseconds - startTime;
        startTime = watch.ElapsedMilliseconds;

        // 1. Show a simple window
        // Tip: if we don't call GUI.Begin()/GUI.End() the widgets appears in a window automatically called "Debug"
        {
            GUILayout.Label("Hello, world!");
            f = GUILayout.Slider("float", f, 0, 1);
            clearColor = GUILayout.ColorField("clear color", clearColor);
            if (GUILayout.Button("Show Demo Window")) showDemoWindow = !showDemoWindow;
            if (GUILayout.Button("Show Another Window")) showAnotherWindow = !showAnotherWindow;
            if (GUILayout.Button("Show Metrics Window")) showMetricsWindow = !showMetricsWindow;
            //var fps = Form.current.uiContext.fps;
            //GUILayout.Label(string.Format("Application average {0:F3} ms/frame ({1:F1} FPS)", 1000.0f / fps, fps));//FIXME Showing text that keeps updating is slow.
        }

        // 2. Show another simple window, this time using an explicit Begin/End pair
        if (showAnotherWindow)
        {
            GUI.Begin("Another Window", ref showAnotherWindow, (80, 340), (400, 150));
            GUILayout.Text("Hello, ImGui {0}", 123);

            if (GUILayout.Button("OK"))
            {
                // do stuff
            }

            text = GUILayout.TextBox("string", 256, text);
            value = GUILayout.Slider("float", value, 0, 1);
            GUI.End();
        }

        // 3. Show the ImGui demo window. Most of the sample code is in demoUI.ShowTestWindow()
        if (showDemoWindow)
        {
            ShowTestWindow(ref showDemoWindow);
        }

        if (showMetricsWindow)
        {
            ImGui.Development.Metrics.ShowWindow(ref showMetricsWindow);
        }

        Form.current.BackgroundColor = clearColor;
    }

    private void ShowTestWindow(ref bool open)
    {
        WindowFlags window_flags = WindowFlags.VerticalScrollbar;
        if (no_titlebar) window_flags |= WindowFlags.NoTitleBar;
        if (!no_border) window_flags |= WindowFlags.ShowBorders;
        if (no_resize) window_flags |= WindowFlags.NoResize;
        if (no_move) window_flags |= WindowFlags.NoMove;
        if (no_scrollbar) window_flags |= WindowFlags.NoScrollbar;
        if (no_collapse) window_flags |= WindowFlags.NoCollapse;

        if(!GUI.Begin("ImGui Demo", ref open, (650, 20), (550, 680), bg_alpha, window_flags))
        {
            // Early out if the window is collapsed, as an optimization.
            GUI.End();
            return;
        }
        GUILayout.Label("ImGui says hello.");
        GUILayout.Space("Space~1", 5);
        if (GUILayout.CollapsingHeader("Help", ref helpOn))
        {
            ShowUserGuide();
        }

        if (GUILayout.CollapsingHeader("Window options", ref windowsOptionsOn))
        {
            GUILayout.PushID("_WindowOptions");
            no_titlebar = GUILayout.Toggle("no titlebar", no_titlebar);
            no_border = GUILayout.Toggle("no border", no_border);
            no_resize = GUILayout.Toggle("no resize", no_resize);
            no_move = GUILayout.Toggle("no move", no_move);
            no_scrollbar = GUILayout.Toggle("no scrollbar", no_scrollbar);
            bg_alpha = GUILayout.Slider("background alpha", bg_alpha, 0.0, 1.0);
            if (GUILayout.TreeNode("Style", ref styleEditorOpen))
            {
                ShowStyleEditor();
                GUILayout.TreePop();
            }
            if (GUILayout.TreeNode("Logging", ref loggingOpen))
            {
                GUILayout.Text("TODO");
                GUILayout.TreePop();
            }
            //TODO logging
            GUILayout.PopID();
        }

        if (GUILayout.CollapsingHeader("Widgets", ref widgetsOn))
        {
            GUILayout.PushID("_Widgets");
            if (GUILayout.TreeNode("Trees", ref open1))
            {
                if(GUILayout.TreeNode("Basic trees", ref open2))
                {
                    for (int i = 0; i < openChild.Length; i++)
                    {
                        if (GUILayout.TreeNode(string.Format("Child {0}", i), ref openChild[i]))
                        {
                            GUILayout.BeginHorizontal("HGroup");
                            GUILayout.Label("blah blah");
                            if (GUILayout.Button("print")) System.Console.WriteLine("Child {0} pressed", i);
                            GUILayout.EndHorizontal();
                            GUILayout.TreePop();
                        }
                    }
                    GUILayout.TreePop();
                }
                GUILayout.TreePop();
            }

            if (GUILayout.TreeNode("Collapsing Headers", ref open4))
            {
                if (GUILayout.CollapsingHeader("Header", ref open5))
                {
                    closable_group = GUILayout.Toggle("Enable extra group", closable_group);
                    for (int i = 0; i < 5; i++)
                    {
                        GUILayout.Label("Some content {0}", i);
                    }
                }
                if (GUILayout.CollapsingHeader("Header with a close button", ref closable_group))
                {
                    for (int i = 0; i < 5; i++)
                    {
                        GUILayout.Label("More content {0}", i);
                    }
                }
                GUILayout.TreePop();
            }

            if (GUILayout.TreeNode("Bullets", ref open6))
            {
                GUILayout.BulletText("Bullet point 1");
                GUILayout.BulletText("Bullet point 2\nOn multiple lines");
                GUILayout.PushStyle(StylePropertyName.CellSpacingHorizontal, 0);//remove horizontal cell spacing of following groups.
                GUILayout.BeginHorizontal("HGroup~1"); GUILayout.Bullet("_Bullet"); GUILayout.Text("Bullet point 3 (two calls)"); GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal("HGroup~2"); GUILayout.Bullet("_Bullet"); GUILayout.Button("Button"); GUILayout.EndHorizontal();
                GUILayout.PopStyle();
                GUILayout.TreePop();
            }

            if (GUILayout.TreeNode("Colored Text", ref open7))
            {
                // Using shortcut. You can use GUILayout.PushFontColor() for more flexibility.
                GUILayout.Label(new Color(1.0f, 0.0f, 1.0f, 1.0f), "Pink");
                GUILayout.Label(new Color(1.0f, 1.0f, 0.0f, 1.0f), "Yellow");
                GUILayout.LabelDisabled("Disabled");
                GUILayout.TreePop();
            }

            if (GUILayout.TreeNode("Word Wrapping", ref open8))
            {
                //TODO
                GUILayout.Label("TODO");
                GUILayout.TreePop();
            }

            if (GUILayout.TreeNode("Non-ASCII Text", ref open9))
            {
                //TODO wrapped Label
                GUILayout.Label("Hiragana: カククケコ (kakikukeko)");
                GUILayout.Label("Kanjis: 日本語 (nihongo)");
                buf = GUILayout.InputText("Unicode input", buf);
                GUILayout.TreePop();
            }

            if (GUILayout.TreeNode("Images", ref open10))
            {
                GUILayout.Image("images/trees.jpg");
                GUILayout.Text("Some textured buttons:");
                GUILayout.BeginHorizontal("HGroup~1");
                for (int i = 0; i < 8; i++)
                {
                    GUILayout.PushID(i);
                    if (GUILayout.ImageButton("images/trees.jpg", new Size(32, 32),
                        new Vector(32.0f * i, 0)))
                    {
                        pressed_count += 1;
                    }
                    GUILayout.PopID();
                }
                GUILayout.EndHorizontal();
                GUILayout.Label("Pressed {0} times.", pressed_count);
                GUILayout.TreePop();
            }

            if (GUILayout.TreeNode("Selectables", ref open11))
            {
                if (GUILayout.TreeNode("Basic", ref basicSelectableOpen))
                {
                    selected[0] = GUILayout.Selectable("1. I am selectable", selected[0]);
                    selected[1] = GUILayout.Selectable("2. I am selectable", selected[1]);
                    GUILayout.Text("3. I am not selectable");
                    selected[2] = GUILayout.Selectable("4. I am selectable", selected[2]);
                    GUILayout.TreePop();
                }
                GUILayout.Label("more TODO");
                GUILayout.TreePop();
            }

            if (GUILayout.TreeNode("Filtered Text Input", ref open12))
            {
                defaultText = GUILayout.InputText("default", defaultText);
                decimalText = GUILayout.InputText("decimal", decimalText, InputTextFlags.CharsDecimal);
                hexadecimalText = GUILayout.InputText("hexadecimal", hexadecimalText, InputTextFlags.CharsHexadecimal | InputTextFlags.CharsUppercase);
                uppercaseText = GUILayout.InputText("uppercase", uppercaseText, InputTextFlags.CharsUppercase);
                noBlankText = GUILayout.InputText("no blank", noBlankText, InputTextFlags.CharsNoBlank);
                customText = GUILayout.InputText("\"imgui\" letters", customText, 0, (c) => "imguiIMGUI".IndexOf(c) >= 0);
                GUILayout.Text("Password input");
                password = GUILayout.InputText("password", password, InputTextFlags.Password | InputTextFlags.CharsNoBlank);
                password = GUILayout.InputText("password (clear)", password, InputTextFlags.CharsNoBlank);
                GUILayout.TreePop();
            }

            if (GUILayout.TreeNode("Multi-line Text Input", ref open13))
            {
                GUILayout.PushPadding((0,0,0,0));
                read_only = GUILayout.CheckBox("Read-only", read_only);
                GUILayout.PopStyle(4);
                GUILayout.PushStyle(StylePropertyName.HorizontalStretchFactor, 1);
                if(read_only)
                {
                    GUILayout.InputTextMultiline("Text Box", new Size(120, 200), multiLineText);
                }
                else
                {
                    multiLineText = GUILayout.InputTextMultiline("Text Box", new Size(120, 200), multiLineText);
                }
                GUILayout.PopStyle();
                GUILayout.TreePop();
            }

            using (GUILayout.HScope("HGroup~button_show_text"))
            {
                if (GUILayout.Button("Button"))
                {
                    System.Console.WriteLine("Clicked");
                    a ^= true;
                }
                if (a)
                {
                    GUILayout.Label("Thanks for clicking me!");
                }
            }

            check = GUILayout.CheckBox("checkbox", check);

            GUILayout.BeginHorizontal("HGroup~radios");
            {
                GUILayout.RadioButton("Radio 0", ref active_id, 0);
                GUILayout.RadioButton("Radio 1", ref active_id, 1);
                GUILayout.RadioButton("Radio 2", ref active_id, 2);
            }
            GUILayout.EndHorizontal();

            // Color buttons, demonstrate using PushID() to add unique identifier in the ID stack, and changing style.
            GUILayout.BeginHorizontal("HGroup~click buttons");
            {
                for (int i = 0; i < 7; i++)
                {
                    GUILayout.PushID(i);
                    GUILayout.PushStyle(StylePropertyName.BackgroundColor, Color.HSV(i / 7.0f, 0.6f, 0.6f), GUIState.Normal);
                    GUILayout.PushStyle(StylePropertyName.BackgroundColor, Color.HSV(i / 7.0f, 0.7f, 0.7f), GUIState.Hover);
                    GUILayout.PushStyle(StylePropertyName.BackgroundColor, Color.HSV(i / 7.0f, 0.8f, 0.8f), GUIState.Active);
                    GUILayout.Button("Click");
                    GUILayout.PopStyle(3);
                    GUILayout.PopID();
                }
            }
            GUILayout.EndHorizontal();

            //TODO tooltip
            //GUILayout.BeginHorizontal("HGroup~tooltips");
            //GUILayout.Text("Hover over me");
            //if (GUI.IsItemHovered())
            //    GUI.SetTooltip("I am a tooltip");

            //if (GUI.IsItemHovered())
            //{
            //    GUILayout.BeginTooltip();
            //    //...
            //    GUILayout.EndTooltip();
            //}
            //GUILayout.EndHorizontal();

            GUILayout.Separator("Separator~1");

            GUILayout.LabelText("label", "Value");

            //TODO combo

            str0 = GUILayout.InputText("input text", str0);
            i0 = GUILayout.InputInt("input int", i0);
            f0 = GUILayout.InputFloat("input float", f0);
            col1 = GUILayout.ColorField("color 1", col1);//TODO no alpha color field
            col2 = GUILayout.ColorField("color 2", col2);
            currentListBoxItem = GUILayout.ListBox<string>("listbox\n(single select)", listBoxItems, currentListBoxItem);

            if (GUILayout.TreeNode("Sliders", ref open14))
            {
                GUILayout.Label("Horizontal Slider");
                sliderValue = GUILayout.Slider("slider", sliderValue, 0.0, 1.0);
                GUILayout.Label("Vertical Slder");
                vSliderValue = GUILayout.VSlider("vslider", vSliderValue, 0.0, 1.0);
                GUILayout.TreePop();
            }

            GUILayout.PopID();
        }

        if (GUILayout.CollapsingHeader("Graphs widgets", ref graphicsWidgetsOn))
        {
            GUILayout.PushID("_GraphsWidgets");
            animate = GUILayout.CheckBox("Animate", animate);
            GUILayout.PushStyle(StylePropertyName.HorizontalStretchFactor, 1);//+1
            if (animate)
            {
                progress += progress_dir * 0.4f * deltaTime / 1000.0;
                if (progress >= +1.1f) { progress = +1.1f; progress_dir *= -1.0f; }
                if (progress <= -0.1f) { progress = -0.1f; progress_dir *= -1.0f; }
            }
            double progress_saturated = (progress < 0.0) ? 0.0 : (progress > 1.0f) ? 1.0 : progress;

            var percentText = string.Format("{0}%", (int)(progress_saturated * 100));
            GUILayout.BeginHorizontal("HGroup~1");
            GUILayout.BeginVertical("ProgressBars");
            GUILayout.ProgressBar("ProgressBar %", progress, (400, 20), percentText);

            const int total = 1753;
            var progressText = string.Format("{0}/{1}", (int)(progress_saturated * total), total);
            GUILayout.ProgressBar("ProgressBar /", progress, (400, 20), progressText);
            GUILayout.EndVertical();
            GUILayout.PushFixedWidth(100);//+2
            GUILayout.Text("Progress");
            GUILayout.PopStyle(2);//-2
            GUILayout.EndHorizontal();

            GUILayout.PopStyle();//-1
            GUILayout.PopID();
        }

        if (GUILayout.CollapsingHeader("Layout", ref layoutOn))
        {
            GUILayout.PushID("_Layout");

            if (GUILayout.TreeNode("Child regions", ref childRegionsOpen))
            {
                GUILayout.Text("Without border");
                bool goto_line = GUILayout.Button("Goto");
                GUILayout.PushFixedWidth(100);//+2
                var newLine = GUILayout.InputInt("##Line", line);
                if (newLine != line)
                {
                    goto_line = true;
                }
                GUILayout.PopStyle(2);//-2

                using (GUILayout.HScope("HGroup~1"))
                {
                    if (GUILayout.BeginChild("Sub1", GUILayout.Height(300).ExpandWidth(true)))
                    {
                        for (int i = 0; i < 50; i++)
                        {
                            GUILayout.Text("{0,4}: scrollable region", i);
                            if (goto_line && line == i)
                            {
                                //SetScrollHere();//TODO
                            }
                        }
                        if (goto_line && line >= 10)
                        {
                            //SetScrollHere();//TODO
                        }
                        GUILayout.EndChild();
                    }

                    if (GUILayout.BeginChild("Sub2", GUILayout.Height(300).ExpandWidth(true)))
                    {
                        GUILayout.Text("With border");
                        for (int i = 0; i < 50; i++)
                        {
                            GUILayout.Button(string.Format("0x{0:X8}", i * 5731));
                        }
                        GUILayout.EndChild();
                    }
                }
                GUILayout.TreePop();
            }

            if (GUILayout.TreeNode("Stack layout", ref stackLayoutOpen))
            {
                GUILayout.Label("Three buttons of default size.");
                GUILayout.BeginHorizontal("H~~~1");
                {
                    GUILayout.Button("1~");
                    GUILayout.Button("2~~");
                    GUILayout.Button("3~~~");
                }
                GUILayout.EndHorizontal();
                GUILayout.Label("Three fixed-width (100 pixels) buttons.");
                GUILayout.BeginHorizontal("H~~~2");
                {
                    GUILayout.PushFixedWidth(100);
                    GUILayout.Button("1");
                    GUILayout.Button("2");
                    GUILayout.Button("3");
                    GUILayout.PopStyle(2);
                }
                GUILayout.EndHorizontal();
                GUILayout.Label("Three stretched sized buttons with 1/2/3 stretch factor.");
                GUILayout.BeginHorizontal("H~~~3");
                {
                    GUILayout.PushStyle(StylePropertyName.HorizontalStretchFactor, 1);
                    GUILayout.Button("1");
                    GUILayout.PopStyle();
                    GUILayout.PushStyle(StylePropertyName.HorizontalStretchFactor, 2);
                    GUILayout.Button("2");
                    GUILayout.PopStyle();
                    GUILayout.PushStyle(StylePropertyName.HorizontalStretchFactor, 3);
                    GUILayout.Button("3");
                    GUILayout.PopStyle();
                }
                GUILayout.EndHorizontal();
                GUILayout.TreePop();
            }

            if (GUILayout.TreeNode("Layout scopes", ref layoutScopesOpen))
            {
                /*
                 * H(V)Scope is a wrapper of BeginHorizontal(Vertical)/EndHorizontal(Vertical) API.
                 * It employs IDisposable so we don't need to call EndHorizontal(Vertical),
                 * because it is called automatically when H(V)Scope goes out of scope.
                 */
                using (GUILayout.HScope("HorizontalScope~", GUILayout.ExpandWidth(false)))
                {
                    GUILayout.Button("H1");
                    using (GUILayout.VScope("VerticalScope~", GUILayout.ExpandWidth(false)))
                    {
                        GUILayout.Button("V1");
                        GUILayout.Button("V2");
                    }
                    GUILayout.Button("H2");
                    GUILayout.Button("H3");
                }
                GUILayout.TreePop();
            }

            GUILayout.PopID();
        }

        if (GUILayout.CollapsingHeader("Style & Skin", ref skinOn))
        {
            GUILayout.PushID("_Skin");

            GUILayout.Button("MyButton1", this.smallRed);
            GUILayout.Button("MyButton2", new LayoutOptions().FontColor(Color.Blue).FontSize(40));

            GUILayout.PushBorder((10,20,30,40));
            GUILayout.PushPadding((40,30,20,10));
            GUILayout.PushStyle(StylePropertyName.BorderTopColor, Color.Red);
            GUILayout.PushStyle(StylePropertyName.BorderRightColor, Color.Green);
            GUILayout.PushStyle(StylePropertyName.BorderBottomColor, Color.Blue);
            GUILayout.PushStyle(StylePropertyName.BorderLeftColor, Color.Yellow);
            GUILayout.Button("Box Model", GUILayout.Width(161).Height(100));
            GUILayout.PopStyle(4+4+4);

            if (GUILayout.TreeNode("Default", ref defaultSkinOpen))
            {
                using (GUILayout.HScope("HorizontalScope~", GUILayout.ExpandWidth(false)))
                {
                    GUILayout.Button("Button 1");
                    GUILayout.Button("Button 2");
                    GUILayout.Button("Button 3");
                }
                GUILayout.TreePop();
            }

            if (GUILayout.TreeNode("dear imgui", ref dearImGuiSkinOpen))
            {
                GUI.SetSkin(dearImGuiSkinRules);
                using (GUILayout.HScope("HorizontalScope~", GUILayout.ExpandWidth(false)))
                {
                    GUILayout.Button("Button 1");
                    GUILayout.Button("Button 2");
                    GUILayout.Button("Button 3");
                }
                GUILayout.TreePop();
            }

            GUI.SetDefaultSkin();

            if (GUILayout.TreeNode("Windows 10", ref win10SkinOpen))
            {
                GUI.SetSkin(win10SkinRules);
                using (GUILayout.HScope("HorizontalScope~", GUILayout.ExpandWidth(false)))
                {
                    GUILayout.Button("Button 1");
                    GUILayout.Button("Button 2");
                    GUILayout.Button("Button 3");
                }
                GUILayout.TreePop();
            }

            GUI.SetDefaultSkin();

            GUILayout.PopID();
        }

        if (GUILayout.CollapsingHeader("Header", ref headerOn))
        {
            GUILayout.Button("Button A");
            GUILayout.Button("Button B");
            GUILayout.Button("Button C");
        }
        GUI.End();
    }
    bool headerOn = false;
    private void ShowUserGuide()
    {
        GUILayout.PushID("UserGuide");
        GUILayout.Label("Double-click on title bar to collapse window.");
        GUILayout.Label("Click and drag on lower right corner to resize window.");
        GUILayout.Label("Click and drag on any empty space to move window.");
        GUILayout.Label("Mouse Wheel to scroll.");
        GUILayout.Label("TAB/SHIFT+TAB to cycle thru keyboard editable fields.");
        GUILayout.Label("CTRL+Click on a slider to input text.");
        GUILayout.Label(
            @"While editing text:
- Hold SHIFT or use mouse to select text
- CTRL+Left/Right to word jump
- CTRL+A select all
- CTRL+X,CTRL+C,CTRL+V clipboard
- CTRL+Z,CTRL+Y undo/redo
- ESCAPE to revert\n
- You can apply arithmetic operators +,*,/ on numerical values.
  Use +- to subtract.");
        GUILayout.PopID();
    }

    private void ShowStyleEditor()
    {
        GUILayout.PushID("StyleEditor");

        var bgColor = Form.current.BackgroundColor;

        GUILayout.BeginHorizontal("HGroup~1");
        bgColor = GUILayout.ColorField("Background Color", bgColor);
        GUILayout.EndHorizontal();

        Form.current.BackgroundColor = bgColor;

        GUILayout.PopID();
    }

    private static ImGui.Style.CustomSkin dearImGuiSkinRules;
    private static ImGui.Style.CustomSkin win10SkinRules;

    private static void InitDearImGuiSkin()
    {
        dearImGuiSkinRules = new ImGui.Style.CustomSkin();
        var button = dearImGuiSkinRules[GUIControlName.Button];
        button.Border = (1, 1, 1, 1);
        button.Padding = (5, 5, 5, 5);
        button.SetBorderColor(new Color(0.70f, 0.70f, 0.70f, 0.65f));
        button.SetBackgroundColor(new Color(0.67f, 0.40f, 0.40f, 0.60f), GUIState.Normal);
        button.SetBackgroundColor(new Color(0.67f, 0.40f, 0.40f, 1.00f), GUIState.Hover);
        button.SetBackgroundColor(new Color(0.80f, 0.50f, 0.50f, 1.00f), GUIState.Active);
    }

    private static void InitWin10Skin()
    {
        win10SkinRules = new ImGui.Style.CustomSkin();
        var button = win10SkinRules[GUIControlName.Button];
        button.Border = (2, 2, 2, 2);
        button.Padding = (5, 5, 5, 5);
        button.SetBorderColor(Color.Rgb(204), GUIState.Normal);
        button.SetBorderColor(Color.Rgb(122), GUIState.Hover);
        button.SetBorderColor(Color.Rgb(153), GUIState.Active);
        button.SetBackgroundColor(Color.Rgb(225), GUIState.Normal);
        button.SetBackgroundColor(Color.Rgb(225), GUIState.Hover);
        button.SetBackgroundColor(Color.Rgb(153), GUIState.Active);
    }

    static Demo()
    {
        InitDearImGuiSkin();
        InitWin10Skin();
    }
}
