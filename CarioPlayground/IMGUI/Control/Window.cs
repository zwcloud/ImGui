using Cairo;
using System.Diagnostics;

namespace IMGUI
{
    class Window : Control, IRect
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

        public Rect Rect { get; private set; }

        public Window(string name, BaseForm form, Rect rect, GUI.WindowFunction func)
            : base(name, form)
        {
            Rect = rect;

            var tmp = Utility.ClientToScreen(new Point(), form);//offset the window's position relative to parent window 
            Rect = Rect.Offset(this.Rect, tmp.X, tmp.Y);
            innerForm = new ImmediateForm(Rect, func);
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

        public override void OnClear(Context g)
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
            }

            var control = form.Controls[name] as Window;
            Debug.Assert(control != null);
            control.Active = true;

            control.innerForm.Actived = true;
        }
    }
}