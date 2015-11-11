using Cairo;
using System.Diagnostics;

namespace IMGUI
{
    class Window : Control
    {
        sealed class ImmediateForm : BorderlessForm
        {
            private GUI.WindowFunction Func { get; set; }

            public bool Actived { private get; set; }
            public ImmediateForm(Rect rect, GUI.WindowFunction func)
                : base((int)rect.Width, (int)rect.Height)
            {
                Position = rect.TopLeft;//NOTE Consider move this to constructor of SFMLForm

                Func = func;
            }

            protected override void OnGUI(GUI gui)
            {
                if(Actived)
                {
                    this.Show();
                    Actived = false;
                }
                else
                {
                    this.Hide();
                    return;
                }
                if(Func!= null)
                {
                    Func(gui);
                }
            }
        }

        private readonly ImmediateForm innerForm;

        public Window(string name, BaseForm form, Rect rect, GUI.WindowFunction func)
            : base(name, form)
        {
            var tmp = Utility.ClientToScreen(new Point(), form);//offset the window's position relative to parent window 
            rect.Offset(tmp.X, tmp.Y);
            innerForm = new ImmediateForm(rect, func);
            Application.Forms.Add(innerForm);
        }

        #region Overrides of Control

        public override void OnUpdate()
        {
        }

        public override void OnRender(Context g)
        {
        }

        public override void Dispose()
        {
        }

        #endregion

        //TODO Control-less DoControl overload (without name parameter)
        internal static void DoControl(Context g, BaseForm form, Rect rect, GUI.WindowFunction func, string name)
        {
            if (!form.Controls.ContainsKey(name))
            {
                var window = new Window(name, form, rect, func);
                Debug.Assert(window != null);
                window.OnUpdate();
                window.OnRender(g);
            }

            var control = form.Controls[name] as Window;
            Debug.Assert(control != null);

            control.innerForm.Actived = true;
        }
    }
}