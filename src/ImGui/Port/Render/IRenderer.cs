namespace ImGui
{
    interface IRenderer
    {
        void Init(object windowHandle);
        void Clear();
        void RenderDrawList(DrawList drawList, int width, int height);
        void SwapBuffers();
        void ShutDown();
    }
}
