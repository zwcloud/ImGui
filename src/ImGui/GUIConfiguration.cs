namespace ImGui
{
    internal class GUIConfiguration
    {
        // See ImGuiConfigFlags_ enum. Set by user/application. Gamepad/keyboard navigation options, etc.
        public ImGuiConfigFlags ConfigFlags;// = 0
        public ImGuiBackendFlags  BackendFlags;            // = 0              // See ImGuiBackendFlags_ enum. Set by backend (imgui_impl_xxx files or custom backend) to communicate features supported by the backend.
        public bool ConfigWindowsMoveFromTitleBarOnly { get; set; } = false;
    }
}