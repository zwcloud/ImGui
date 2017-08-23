using ImGui;
using ImGui.Common;
using ImGui.Common.Primitive;
using ImGui.OSAbstraction.Graphics;

public class Demo
{
    double f = 0.0f;
    Color clearColor = Color.Argb(255, 114, 144, 154);
    private bool showAnotherWindow = true;

    #region Demo
    bool showDemoWindow = false;

    #region Help
    bool helpOn;
    #endregion

    #region Window Options
    bool windowsOptionsOn;
    static bool no_titlebar = false;
    static bool no_border = true;
    static bool no_resize = false;
    static bool no_move = false;
    static bool no_scrollbar = false;
    static bool no_collapse = false;
    static double bg_alpha = 0.65;
    #endregion

    #region Widgets
    bool widgetsOn = true;
    bool a = false;
    ITexture image;
    int pressed_count = 0;
    bool open1, open2, open3, open4, open5, open6, open7, open8, open9, open10, open11, open12, open13, open14;
    bool[] openChild = new bool[5]{ false, false, false, false, false};
    bool closable_group = true;

    #region Filtered Text Input
    string defaultText = "";
    string decimalText = "";
    string hexadecimalText = "";
    string uppercaseText = "";
    string noBlankText = "";
    string password = "";
    #endregion

    string buf = "日本語";
    bool read_only = false;
    string text = @"/*\n
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

    #region Sliders
    double sliderValue = 0.01;
    double vSliderValue = 0.01;
    #endregion

    #endregion

    #region Graphics Widgets
    bool graphicsWidgetsOn;
    #endregion

    #region Layout
    bool layoutOn = false;
    #endregion

    #endregion

    public void OnGUI()
    {
        // 1. Show a simple window
        // Tip: if we don't call GUI.Begin()/GUI.End() the widgets appears in a window automatically called "Debug"
        {
            GUILayout.Label("Hello, world!");
            f = GUILayout.Slider("float", f, 0, 1);
            clearColor = GUILayout.ColorField("clear color", clearColor);
            if (GUILayout.Button("Show Demo Window")) showDemoWindow = !showDemoWindow;
            if (GUILayout.Button("Show Another Window")) showAnotherWindow = !showAnotherWindow;
            //var fps = Form.current.uiContext.fps;
            //GUILayout.Label(string.Format("Application average {0:F3} ms/frame ({1:F1} FPS)", 1000.0f / fps, fps)); //removed, will be added again when textmesh cache is solved
        }

        // 2. Show another simple window, this time using an explicit Begin/End pair
        if (showAnotherWindow)
        {
            GUI.Begin("Another Window", ref showAnotherWindow, (70, 450), (400, 100));
            GUILayout.Label("Hello");
            GUI.End();
        }

        // 3. Show the ImGui demo window. Most of the sample code is in demoUI.ShowTestWindow()
        if (showDemoWindow)
        {
            ShowTestWindow(ref showDemoWindow);
        }

        Form.current.BackgroundColor = clearColor;
    }

    private void ShowTestWindow(ref bool open)
    {
        if(image == null)
        {
            image = GUI.CreateTexture("./Image/trees.jpg");
        }

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
            GUILayout.BeginHorizontal("HGroup");
                GUILayout.Space("HGroup_indient", 60);
                GUILayout.BeginVertical("VGroup");
                    no_titlebar = GUILayout.Toggle("no titlebar", no_titlebar);
                    no_border = GUILayout.Toggle("no border", no_border);
                    no_resize = GUILayout.Toggle("no resize", no_resize);
                    no_move = GUILayout.Toggle("no move", no_move);
                    no_scrollbar = GUILayout.Toggle("no scrollbar", no_scrollbar);
                    bg_alpha = GUILayout.Slider("background alpha", bg_alpha, 0.0, 1.0);
                    ShowStyleEditor();
                    //TODO logging
                GUILayout.EndVertical();
            GUILayout.EndHorizontal();
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
                        }
                        GUILayout.TreePop();
                    }
                }
                GUILayout.TreePop();
            }
            GUILayout.TreePop();

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
            }
            GUILayout.TreePop();

            if (GUILayout.TreeNode("Bullets", ref open6))
            {
                GUILayout.BulletText("Bullet point 1");
                GUILayout.BulletText("Bullet point 2\nOn multiple lines");
                GUILayout.PushHCellSpacing(0);//remove horizontal cell spacing of following groups.
                GUILayout.BeginHorizontal("HGroup~1"); GUILayout.Bullet("_Bullet"); GUILayout.Text("Bullet point 3 (two calls)"); GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal("HGroup~2"); GUILayout.Bullet("_Bullet"); GUILayout.Button("Button"); GUILayout.EndHorizontal();
                GUILayout.PopStyleVar();
            }
            GUILayout.TreePop();

            if (GUILayout.TreeNode("Colored Text", ref open7))
            {
                // Using shortcut. You can use GUILayout.PushFontColor() for more flexibility.
                GUILayout.Label(new Color(1.0f, 0.0f, 1.0f, 1.0f), "Pink");
                GUILayout.Label(new Color(1.0f, 1.0f, 0.0f, 1.0f), "Yellow");
                GUILayout.LabelDisabled("Disabled");
            }
            GUILayout.TreePop();

            if (GUILayout.TreeNode("Word Wrapping", ref open8))
            {
                //TODO
                GUILayout.Label("TODO");
            }
            GUILayout.TreePop();

            if (GUILayout.TreeNode("Non-ASCII Text", ref open9))
            {
                //TODO wrapped Label
                GUILayout.Label("Hiragana: カククケコ (kakikukeko)");
                GUILayout.Label("Kanjis: 日本語 (nihongo)");
                buf = GUILayout.TextBox("Unicode input", 200, buf);
            }
            GUILayout.TreePop();

            if (GUILayout.TreeNode("Images", ref open10))
            {
                GUILayout.Image("Image/trees.jpg");
                GUILayout.Text("Some textured buttons:");
                GUILayout.BeginHorizontal("HGroup~1");
                for (int i = 0; i < 8; i++)
                {
                    GUILayout.PushID(i);
                    if (GUILayout.ImageButton("Image/trees.jpg", new Size(32, 32), new Point(32.0f * i / 256, 0), new Point(32.0f * (i + 1) / 256, 32.0f / 256), Color.White))
                        pressed_count += 1;
                    GUILayout.PopID();
                }
                GUILayout.EndHorizontal();
                GUILayout.Label("Pressed {0} times.", pressed_count);
            }
            GUILayout.TreePop();

            if (GUILayout.TreeNode("Selectables", ref open11))
            {
                //TODO
                GUILayout.Label("TODO");
            }
            GUILayout.TreePop();

            if (GUILayout.TreeNode("Filtered Text Input", ref open12))
            {
                GUILayout.Label("TODO");
#if false
                GUILayout.TextBox("default", 64,     defaultText     );
                GUILayout.TextBox("decimal", 64,     decimalText,     TextBoxFlags.CharsDecimal);
                GUILayout.TextBox("hexadecimal", 64, hexadecimalText, TextBoxFlags.CharsHexadecimal | TextBoxFlags.CharsUppercase);
                GUILayout.TextBox("uppercase", 64,   uppercaseText,   TextBoxFlags.CharsUppercase);
                GUILayout.TextBox("no blank", 64,    noBlankText,     TextBoxFlags.CharsNoBlank);
                //TODO callback-based filters

                GUILayout.Text("Password input");
                GUILayout.TextBox("password", 64, password, TextBoxFlags.Password | TextBoxFlags.CharsNoBlank);
                GUILayout.TextBox("password (clear)", 64, password, TextBoxFlags.CharsNoBlank);
                //TODO
#endif
            }
            GUILayout.TreePop();

            if (GUILayout.TreeNode("Multi-line Text Input", ref open13))
            {
                GUILayout.PushPadding((0,0,0,0));
                read_only = GUILayout.CheckBox("Read-only", read_only);
                GUILayout.PopStyleVar(4);
                GUILayout.PushHStretchFactor(1);
                if(read_only)
                {
                    GUILayout.TextBox("Text Box", new Size(120, 200), text);
                }
                else
                {
                    text = GUILayout.TextBox("Text Box", new Size(120, 200), text);
                }
                GUILayout.PopStyleVar();
            }
            GUILayout.TreePop();

            GUILayout.BeginHorizontal("HGroup~button_show_text");
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
            GUILayout.EndHorizontal();

            check = GUILayout.CheckBox("checkbox", check);

            // TODO radio
            //GUILayout.BeginHorizontal("HGroup~radios");
            //{
            //    GUILayout.Radio("Radio 0", ref active_id, "radio_b_0");
            //    GUILayout.Radio("Radio 1", ref active_id, "radio_b_1");
            //    GUILayout.Radio("Radio 2", ref active_id, "radio_b_2");
            //}
            //GUILayout.EndHorizontal();

            // Color buttons, demonstrate using PushID() to add unique identifier in the ID stack, and changing style.
            GUILayout.BeginHorizontal("HGroup~click buttons");
            {
                for (int i = 0; i < 7; i++)
                {
                    GUILayout.PushID(i);
                    GUILayout.PushStyleColor(GUIStyleName.BackgroundColor, Color.HSV(i / 7.0f, 0.6f, 0.6f), GUIState.Normal);
                    GUILayout.PushStyleColor(GUIStyleName.BackgroundColor, Color.HSV(i / 7.0f, 0.7f, 0.7f), GUIState.Hover);
                    GUILayout.PushStyleColor(GUIStyleName.BackgroundColor, Color.HSV(i / 7.0f, 0.8f, 0.8f), GUIState.Active);
                    GUILayout.Button("Click");
                    GUILayout.PopStyleVar(3);
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

            if (GUILayout.TreeNode("Sliders", ref open14))
            {
                GUILayout.Label("Horizontal Slider");
                sliderValue = GUILayout.Slider("slider", sliderValue, 0.0, 1.0);
                GUILayout.Label("Vertical Slder");
                vSliderValue = GUILayout.VSlider("vslider", vSliderValue, 0.0, 1.0);
            }
            GUILayout.TreePop();

            GUILayout.PopID();
        }

        if (GUILayout.CollapsingHeader("Graphs widgets", ref graphicsWidgetsOn))
        {
            GUILayout.PushID("_GraphsWidgets");
            //TODO
            GUILayout.Label("TODO");
            GUILayout.PopID();
        }

        if (GUILayout.CollapsingHeader("Layout", ref layoutOn))
        {
            GUILayout.PushID("_Layout");
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
                GUILayout.PopStyleVar(2);
            }
            GUILayout.EndHorizontal();
            GUILayout.Label("Three stretched sized buttons with 1/2/3 stretch factor.");
            GUILayout.BeginHorizontal("H~~~3");
            {
                GUILayout.PushHStretchFactor(1);
                GUILayout.Button("1");
                GUILayout.PopStyleVar();
                GUILayout.PushHStretchFactor(2);
                GUILayout.Button("2");
                GUILayout.PopStyleVar();
                GUILayout.PushHStretchFactor(3);
                GUILayout.Button("3");
                GUILayout.PopStyleVar();
            }
            GUILayout.EndHorizontal();
            GUILayout.PopID();
        }

        GUI.End();
    }

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

        var bgColor = GUILayout.GetCurrentWindow().Style.BackgroundColor;

        GUILayout.BeginHorizontal("HGroup~1");
        bgColor = GUILayout.ColorField("Background Color", bgColor); //FIXME
        GUILayout.EndHorizontal();

        GUILayout.GetCurrentWindow().Style.BackgroundColor = bgColor;

        GUILayout.PopID();
    }
}
