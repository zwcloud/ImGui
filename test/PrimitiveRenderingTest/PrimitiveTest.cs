using ImGui;
using System;
using Xunit;

namespace PrimitiveRenderingTest
{
    public class Primitive
    {
        public Primitive()
        {
            Application.InitSysDependencies();
        }

        ITexture image;
        GUIStyle style;

        [Fact]
        public void ShouldRenderAnImage()
        {
            style = new GUIStyle();
            style.BorderImageSlice = (83, 54, 54, 54);

            Application.Run(new Form1(() => {
                if (image == null)
                {
                    image = GUI.CreateTexture(@"D:\My documents\My Pictures\素材\crystal_button.png");
                }

                GUILayout.Image(image, style, "image0", GUILayout.Width(image.Width+50), GUILayout.Height(image.Height+100));
            }));
        }
    }
}
