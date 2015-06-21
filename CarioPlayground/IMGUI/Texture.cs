using System.Collections.Generic;
using Cairo;

namespace IMGUI
{
    public class Texture
    {
        internal ImageSurface _surface;

        public int Width
        {
            get { return _surface.Width; }
        }
        
        public int Height
        {
            get { return _surface.Height; }
        }

        internal static Dictionary<string, Texture> _presets;

        public Texture(ImageSurface imageSurface)
        {
            _surface = imageSurface;
        }

        static Texture()
        {
            _presets = new Dictionary<string, Texture>
            {
                {"Toggle.Off", new Texture( new ImageSurface("W:/VS2013/CarioPlayground/Resources/Toggle.Off.png") )},
                {"Toggle.On", new Texture( new ImageSurface("W:/VS2013/CarioPlayground/Resources/Toggle.On.png") )},
            };
        }
            
    }
}