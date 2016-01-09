using Cairo;
using System.Diagnostics;
namespace ImGui
{
    class RadioButton : Radio
    {
        public RadioButton(string name, BaseForm form, string text, Rect rect, string groupName)
            : base(name, form, text, rect, groupName)
        {
        }

        public override void OnRender(Context g)
        {
            g.DrawBoxModel(Rect, new Content(TextContext),
                Actived ? Skin.current.Button["Active"] : Skin.current.Button["Normal"]); ;
            this.RenderRects.Add(Rect);
        }

        public static new bool DoControl(BaseForm form, Rect rect, string text, string groupName, bool value, string name)
        {
            if (!form.Controls.ContainsKey(name))
            {
                var radioButton = new RadioButton(name, form, text, rect, groupName);
                Debug.Assert(radioButton != null);
            }

            var control = form.Controls[name] as RadioButton;
            Debug.Assert(control != null);
            control.Active = true;

            return control.Actived;
        }

    }
}