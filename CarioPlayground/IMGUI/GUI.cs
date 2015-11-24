using Cairo;
using TinyIoC;

namespace IMGUI
{
    public class GUI
    {
        public delegate void WindowFunction(GUI gui);

        private static class GUIState
        {
            public const string Normal = "Normal";
            public const string HorizontalLayout = "HorizontalLayout";
            public const string VerticalLayout = "VerticalLayout";
        }

        private readonly Context g;
        private readonly BaseForm form;
        private readonly GUILayout layout;

        private string state = GUIState.Normal;
        private int lastX;
        private int lastY;

        public GUI(Context context, BaseForm form)
        {
            this.g = context;
            this.form = form;
            this.layout = new GUILayout();
        }

        public bool Button(Rect rect, string text, string name)
        {
            rect = DoLayout(rect);
            return IMGUI.Button.DoControl(g, form, rect, text, name);
        }

        public void Label(Rect rect, string text, string name)
        {
            rect = DoLayout(rect);
            IMGUI.Label.DoControl(g, form, rect, text, name);
        }

        public bool Toggle(Rect rect, string text, bool value, string name)
        {
            rect = DoLayout(rect);
            return IMGUI.Toggle.DoControl(g, form, rect, text, value, name);
        }

        public int CombolBox(Rect rect, string[] text, int selectedIndex, string name)
        {
            rect = DoLayout(rect);
            return ComboBox.DoControl(g, null /*FIXME ComboBox subitems should be shown in a new form*/, form, rect, text,
                selectedIndex, name);
        }

        public void Image(Rect rect, Texture image, string name)
        {
            rect = DoLayout(rect);
            IMGUI.Image.DoControl(g, form, rect, image, name);
        }

        public bool Radio(Rect rect, string text, string groupName, bool value, string name)
        {
            rect = DoLayout(rect);
            return IMGUI.Radio.DoControl(g, form, rect, text, groupName, value, name);
        }

        public string TextBox(Rect rect, string text, string name)
        {
            rect = DoLayout(rect);
            return IMGUI.TextBox.DoControl(g, form, rect, text, name);
        }

        public bool PolygonButton(Point[] points, string text, string name)
        {
            return IMGUI.PolygonButton.DoControl(g, form, points, text, name);
        }

        public bool ToggleButton(Rect rect, string text, bool value, string name)
        {
            rect = DoLayout(rect);
            return IMGUI.ToggleButton.DoControl(g, form, rect, text, value, name);
        }

        public bool HoverButton(Rect rect, string text, string name)
        {
            rect = DoLayout(rect);
            return IMGUI.HoverButton.DoControl(g, form, rect, text, name);
        }

        public bool RadioButton(Rect rect, string text, string groupName, bool value, string name)
        {
            rect = DoLayout(rect);
            return IMGUI.RadioButton.DoControl(g, form, rect, text, groupName, value, name);
        }

        public void Window(Rect rect, WindowFunction func, string name)
        {
            rect = DoLayout(rect);
            IMGUI.Window.DoControl(g, form, rect, func, name);
        }

        private Rect DoLayout(Rect rect)
        {
            if (state == GUIState.Normal)
            {
                lastX = 0;
                lastY = 0;
            }
            else if (state == GUIState.HorizontalLayout)
            {
                rect.Offset(lastX, 0);
                lastX += (int) rect.Width;
                rect.Y = lastY;
            }
            else if (state == GUIState.VerticalLayout)
            {
                rect.Offset(0, lastY);
                rect.X = lastX;
                lastY += (int) rect.Height;
            }
            return rect;
        }

        #region Layout-featured controls
        public bool LayoutButton(string text, string name)
        {
            var font = Skin.current.Button["Normal"].Font;
            var tmpFormat = Application.IocContainer.Resolve<ITextFormat>(
                new NamedParameterOverloads
                    {
                        {"fontFamilyName", font.FontFamily},
                        {"fontWeight", font.FontWeight},
                        {"fontStyle", font.FontStyle},
                        {"fontStretch", font.FontStretch},
                        {"fontSize", (float) font.Size}
                    });
            var textStyle = Skin.current.Button["Normal"].TextStyle;
            tmpFormat.Alignment = textStyle.TextAlignment;
            var tmpLayout = Application.IocContainer.Resolve<ITextLayout>(
                new NamedParameterOverloads
                    {
                        {"text", text},
                        {"textFormat", tmpFormat},
                        {"maxWidth", 1024},
                        {"maxHeight", 1024}
                    });
            var boxSize = CairoEx.MeasureBoxModel(new Content(tmpLayout), Skin.current.Button["Normal"]);
            //System.Diagnostics.Debug.WriteLine("<{0}> {1}", name, boxSize);
            tmpFormat.Dispose();
            tmpLayout.Dispose();
            var rect = layout.AddRect(new Rect(boxSize));
            return IMGUI.Button.DoControl(g, form, rect, text, name);
        }

        #endregion
        public void BeginH()
        {
            layout.BeginGroup(GUILayout.LayoutMode.Horizontal);
        }

        public void EndH()
        {
            layout.EndGroup(GUILayout.LayoutMode.Horizontal);
        }

        public void BeginV()
        {
            layout.BeginGroup(GUILayout.LayoutMode.Vertical);
        }

        public void EndV()
        {
            layout.EndGroup(GUILayout.LayoutMode.Vertical);
        }


        #region Simple layout
        
        public void BeginHorizontal(Rect rect)
        {
            if (rect.IsEmpty)
            {
                throw new System.ArgumentOutOfRangeException("rect", "rect is empty");
            }
            if (state != GUIState.Normal)
            {
                throw new System.InvalidOperationException("Layout has began!");
            }
            lastX = (int) rect.X;
            lastY = (int) rect.Y;
            state = GUIState.HorizontalLayout;
        }

        public void EndHorizontal()
        {
            if(state != GUIState.HorizontalLayout)
            {
                throw new System.InvalidOperationException("BeginHorizontal hasn't been called!");
            }
            state = GUIState.Normal;
        }

        public void BeginVertical(int x, int y)
        {
            if (state != GUIState.Normal)
            {
                throw new System.InvalidOperationException("Layout has began!");
            }
            lastX = x;
            lastY = y;
            state = GUIState.VerticalLayout;
        }

        public void BeginVertical(Rect rect)
        {
            if (rect.IsEmpty)
            {
                throw new System.ArgumentOutOfRangeException("rect", "rect is empty");
            }
            if (state != GUIState.Normal)
            {
                throw new System.InvalidOperationException("Layout has began!");
            }
            lastX = (int)rect.X;
            lastY = (int)rect.Y;
            state = GUIState.VerticalLayout;
        }

        public void EndVertical()
        {
            if (state != GUIState.VerticalLayout)
            {
                throw new System.InvalidOperationException("BeginVertical hasn't been called!");
            }
            state = GUIState.Normal;
        }
        
        #endregion
    }

}
