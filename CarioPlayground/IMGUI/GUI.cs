using Cairo;

namespace IMGUI
{
    public class GUI
    {
        Context g;
        public GUI(Context context)
        {
            g = context;
        }

        public bool Button(Rect rect, string text)
        {
            string state;
            bool active = Input.LeftButtonState == InputState.Down && rect.Contains(Input.MousePos);
            bool hover = Input.LeftButtonState == InputState.Up && rect.Contains(Input.MousePos);
            if(active)
                state = "Active";
            else if (hover)
                state = "Hover";
            else
                state = "Normal";
            
            g.DrawBoxModel(rect, new Content(text), Skin._current.Button[state]);

            bool clicked = Input.LeftButtonClicked && rect.Contains(Input.MousePos);
            return clicked;
        }

        public void Label(Rect rect, string text)
        {
            string state;
            bool active = Input.LeftButtonState == InputState.Down && rect.Contains(Input.MousePos);
            bool hover = Input.LeftButtonState == InputState.Up && rect.Contains(Input.MousePos);
            if (active)
                state = "Active";
            else if (hover)
                state = "Hover";
            else
                state = "Normal";

            g.DrawBoxModel(rect, new Content(text), Skin._current.Label[state]);
        }

        public bool Toggle(Rect rect, string text, bool value)
        {
            string state;
            bool active = Input.LeftButtonState == InputState.Down && rect.Contains(Input.MousePos);
            bool hover = Input.LeftButtonState == InputState.Up && rect.Contains(Input.MousePos);
            if (active)
                state = "Active";
            else if (hover)
                state = "Hover";
            else
                state = "Normal";

            bool changed = Input.LeftButtonClicked && rect.Contains(Input.MousePos);
            bool on = changed ? !value : value;
            
            var toggleBoxRect = new Rect(rect.TopLeft, new Size(20, 20));
            g.DrawBoxModel(toggleBoxRect,
                new Content(Texture._presets[on?"Toggle.On":"Toggle.Off"]),
                Skin._current.Toggle[state]);

            var toggleTextRect = new Rect(toggleBoxRect.TopRight, rect.BottomRight);
            g.DrawBoxModel(toggleTextRect,
                new Content(text),
                Skin._current.Toggle[state]);

            return on;
        }

    }
}
