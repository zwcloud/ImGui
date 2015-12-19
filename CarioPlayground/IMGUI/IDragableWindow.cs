namespace ImGui
{
    interface IDragableWindow
    {
        void OnMouseButtonPressed(object sender, SFML.Window.MouseButtonEventArgs e);

        void OnMouseMoved(object sender, SFML.Window.MouseMoveEventArgs e);

        void OnMouseButtonReleased(object sender, SFML.Window.MouseButtonEventArgs e);
    }
}
