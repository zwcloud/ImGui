using System.Diagnostics;
namespace ImGui
{
    internal class ToggleButton : Toggle
    {
        public ToggleButton(string name, Form form, bool value, string text, Rect rect)
            : base(name, form, value, text, rect)
        {

        }
        
        public override void OnRender(Cairo.Context g)
        {
            g.DrawBoxModel(Rect, new Content(TextContext),
                Result ? Skin.current.Button["Active"] : Skin.current.Button["Normal"]);
            this.RenderRects.Add(Rect);
        }

        public static new bool DoControl(Form form, Rect rect, string text, bool value, string name)
        {
            if (!form.Controls.ContainsKey(name))
            {
                var toggle = new ToggleButton(name, form, value, text, rect);
            }

            var control = form.Controls[name] as ToggleButton;
            Debug.Assert(control != null);
            control.Active = true;

            return control.Result;
        }
    }
}