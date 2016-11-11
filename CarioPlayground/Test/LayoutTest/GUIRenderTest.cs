using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ImGui;
using SFML.System;
using SFML.Window;

namespace Test
{
    [TestClass]
    public class GUIRenderTest
    {

        private GUIRenderer r;
        private Window form;

        [TestInitialize]
        public void TestInit()
        {
            form = new SFML.Window.Window(
                new SFML.Window.VideoMode((uint)400, (uint)300),
                "DummyWindowTitle",
                SFML.Window.Styles.None, new SFML.Window.ContextSettings
                {
                    DepthBits = 0,
                    StencilBits = 0,
                    AntialiasingLevel = 0,
                    MajorVersion = 2,
                    MinorVersion = 1
                });
            form.SetVisible(false);//not show form on creating
            form.SetVerticalSyncEnabled(true);

            r = new GUIRenderer();
        }


        [TestMethod]
        public void Test()
        {
            this.form.SetActive(true);
            this.form.Display();
            this.form.SetActive(true);
            byte[] data = new byte[400 * 300 * 4];
            r.OnLoad(new Size(400, 300));
            r.OnUpdateTexture(new Rect(400, 300), 
                                   System.Runtime.InteropServices.Marshal.UnsafeAddrOfPinnedArrayElement(data, 0));
            r.OnRenderFrame();

            form.Size = new Vector2u(512, 512);

            this.form.Display();
            this.form.SetActive(true);
            byte[] data1 = new byte[512 * 512 * 4];
            r.OnLoad(new Size(512, 512));
            r.OnUpdateTexture(new Rect(512, 512),
                                   System.Runtime.InteropServices.Marshal.UnsafeAddrOfPinnedArrayElement(data1, 0));
            r.OnRenderFrame();
        }
    }
}
