using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IMGUI
{
    public class GUI
    {
        Cairo.Context g;
        public GUI(Cairo.Context context)
        {
            g = context;
        }

        public bool Button(Rect rect, string text)
        {
            bool active = Input.LeftButtonState == InputState.Down && rect.Contains(Input.MousePos);
            bool hover = Input.LeftButtonState == InputState.Up && rect.Contains(Input.MousePos);
            StyleStateType styleStateType = StyleStateType.Normal;
            if(active)
                styleStateType = StyleStateType.Active;
            else if(hover)
                styleStateType = StyleStateType.Hover;
            else
                styleStateType = StyleStateType.Normal;
            g.DrawBoxModel(rect, new Content(text), Skin._current.Button, Skin._current.Font, styleStateType);

            bool clicked = Input.LeftButtonClicked && rect.Contains(Input.MousePos);
            return clicked;
        }

        public void Label(Rect rect, string text)
        {
            bool active = Input.LeftButtonState == InputState.Down && rect.Contains(Input.MousePos);
            bool hover = Input.LeftButtonState == InputState.Up && rect.Contains(Input.MousePos);
            StyleStateType styleStateType = StyleStateType.Normal;
            if(active)
                styleStateType = StyleStateType.Active;
            else if(hover)
                styleStateType = StyleStateType.Hover;
            else
                styleStateType = StyleStateType.Normal;

            g.DrawBoxModel(rect, new Content(text), Skin._current.Label, Skin._current.Font, styleStateType);
        }
    }
}
