using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
