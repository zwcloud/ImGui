using Cairo;
using System;
using System.Diagnostics;

namespace IMGUI
{
    class Window : Control
    {
        sealed class ImmediateForm : BorderlessForm
        {
            private GUI.WindowFunction Func { get; set; }

            public bool Actived { private get; set; }
            public ImmediateForm(Rect rect, GUI.WindowFunction func, BaseForm parentForm)
                : base((int)rect.Width, (int)rect.Height)
            {
                var handle = internalForm.SystemHandle;
                var parentHandle = ((SFML.Window.Window) (parentForm.InternalForm)).SystemHandle;

                //TODO Move these to Form(SFMLForm) and try to abstract these for multiple platform
                //Remove WS_POPUP style and add WS_CHILD style
                const uint WS_POPUP = 0x80000000;
                const uint WS_CHILD = 0x40000000;
                const int GWL_STYLE = -16;
                const int GWL_EXSTYLE = -20;
                const long WS_EX_TOOLWINDOW = 0x00000080L;
                var handleRef = new System.Runtime.InteropServices.HandleRef(this, handle);
                System.IntPtr exStyle = Native.GetWindowLong(handleRef, (int) Native.GWL.GWL_EXSTYLE);
                var error = Native.GetLastError();
                //style = (style & ~(WS_POPUP)) | WS_CHILD;
                if (IntPtr.Size == 4)
                {
                    exStyle = new IntPtr(exStyle.ToInt32() | WS_EX_TOOLWINDOW);
                }
                else
                {
                    exStyle = new IntPtr(exStyle.ToInt64() | ((long)WS_EX_TOOLWINDOW));
                }
                Native.SetWindowLongPtr(handleRef, (int)Native.GWL.GWL_EXSTYLE, exStyle);
                error = Native.GetLastError();

                Func = func;
                Position = rect.TopLeft;//NOTE Consider move this to constructor of SFMLForm
            }

            protected override void OnGUI(GUI gui)
            {
                if (ForceHide)
                {
                    this.Hide();
                    return;
                }

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
                    if(Func(gui))
                    {
                        Actived = false;
                        ForceHide = true;
                    }
                }
            }

            public bool ForceHide { get; set; }
        }

        private readonly ImmediateForm innerForm;
        
        public Window(string name, BaseForm form, Rect rect, GUI.WindowFunction func)
            : base(name, form)
        {
            Rect = rect;

            var tmp = Utility.ClientToScreen(new Point(), form);//offset the window's position relative to parent window 
            Rect = Rect.Offset(this.Rect, tmp.X, tmp.Y);
            innerForm = new ImmediateForm(Rect, func, form);
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