using System;

namespace ImGui.OSImplementation.Shared
{
    internal partial class OpenGLMaterial
    {
        private static bool CommonMaterialInitialized = false;

        public static void InitCommonMaterials()
        {
            if (CommonMaterialInitialized)
            {
                return;
            }

            OpenGLMaterial.shapeMaterial.Init();
            OpenGLMaterial.imageMaterial.Init();
            OpenGLMaterial.glyphMaterial.Init();
            OpenGLMaterial.textMaterial.Init();

            CommonMaterialInitialized = true;
        }

        public static void ShutDownCommonMaterials()
        {
            if (!CommonMaterialInitialized)
            {
                return;
            }

            OpenGLMaterial.shapeMaterial.ShutDown();
            OpenGLMaterial.imageMaterial.ShutDown();
            OpenGLMaterial.glyphMaterial.ShutDown();
            OpenGLMaterial.textMaterial.ShutDown();
        }
        

        private static readonly string shapeVertexShaderText;
        private static readonly string shapeFragmentShader;

        private static readonly string imageVertexShader;
        private static readonly string imageFragmentShader;

        private static readonly string glyphVertexShader;
        private static readonly string glyphFragmentShader;

        private static readonly string textVertexShader;
        private static readonly string textFragmentShader;

        static OpenGLMaterial()
        {
            //On linux we use EGL to create OpenGLES 3.x context instead of OpenGL 3.x context.
            //See LinuxOpenGLRenderer.CreateEGLContext()
            //So the shader version should be "300 es" for linux OS.
            string versionHeader = (OperatingSystem.IsAndroid() || OperatingSystem.IsLinux())
                ? "#version 300 es" : "#version 330";
            shapeVertexShaderText = versionHeader + @"
precision mediump float;
uniform mat4 ProjMtx;
uniform mat4 ViewMtx;
in vec2 Position;
in vec2 UV;
in vec4 Color;
out vec2 Frag_UV;
out vec4 Frag_Color;
void main()
{
	Frag_UV = UV;
	Frag_Color = Color;
	gl_Position = ProjMtx * ViewMtx * vec4(Position.xy,0,1);
}
";
            shapeFragmentShader = versionHeader + @"
precision mediump float;
uniform sampler2D Texture;
in vec2 Frag_UV;
in vec4 Frag_Color;
out vec4 Out_Color;
void main()
{
	Out_Color = Frag_Color;
}
";


            imageVertexShader = versionHeader + @"
precision mediump float;
uniform mat4 ProjMtx;
uniform mat4 ViewMtx;
in vec2 Position;
in vec2 UV;
in vec4 Color;
out vec2 Frag_UV;
out vec4 Frag_Color;
void main()
{
	Frag_UV = UV;
	Frag_Color = Color;
	gl_Position = ProjMtx * ViewMtx * vec4(Position.xy,0,1);
}
";
            textVertexShader = versionHeader + @"
precision mediump float;
in vec2 Position;
in vec2 UV;
in vec4 Color;
out vec2 Frag_UV;
out vec4 Frag_Color;
void main()
{
	Frag_UV = UV;
    Frag_Color = Color;
    //directly assigned to gl_Position: the clip-space output position of the current vertex.
	gl_Position = vec4(Position, 0.0, 1.0);
}
";
            imageFragmentShader = versionHeader + @"
precision mediump float;
uniform sampler2D Texture;
in vec2 Frag_UV;
in vec4 Frag_Color;
out vec4 Out_Color;
void main()
{
	Out_Color = Frag_Color * texture( Texture, Frag_UV.st);
}
";
            glyphVertexShader = versionHeader + @"
precision mediump float;
uniform mat4 ProjMtx;
uniform mat4 ViewMtx;
in vec2 Position;
in vec2 UV;
in vec4 Color;
out vec2 Frag_UV;
out vec4 Frag_Color;
uniform vec2 offset;
void main()
{
	Frag_UV = UV;
	Frag_Color = Color;//dummy Frag_Color is useless
	gl_Position = ProjMtx * ViewMtx * vec4(offset+Position.xy,0,1);
}
";
            glyphFragmentShader = versionHeader + @"
precision mediump float;
in vec2 Frag_UV;
in vec4 Frag_Color;
out vec4 Out_Color;
uniform vec4 color;
void main()
{
	if (Frag_UV.s * Frag_UV.s - Frag_UV.t > 0.0)
	{
		discard;
	}

	// Upper 4 bits: front faces
	// Lower 4 bits: back faces
	//Out_Color = Frag_Color + color *0.001;
    Out_Color = Frag_Color *0.001 + color * (gl_FrontFacing ? 16.0 / 255.0 : 1.0 / 255.0);
}
";
            textFragmentShader = versionHeader + @"
precision mediump float;
in vec2 Frag_UV;
in vec4 Frag_Color;
out vec4 Out_Color;
uniform sampler2D Texture;
uniform vec4 color;
void main()
{
	// Get samples for -2/3 and -1/3
	vec2 valueL = texture(Texture, vec2(Frag_UV.x + dFdx(Frag_UV.x), Frag_UV.y)).yz * 255.0;
	vec2 lowerL = mod(valueL, 16.0);
	vec2 upperL = (valueL - lowerL) / 16.0;
	vec2 alphaL = min(abs(upperL - lowerL), 2.0);

	// Get samples for 0, +1/3, and +2/3
	vec3 valueR = texture(Texture, Frag_UV).xyz * 255.0;
	vec3 lowerR = mod(valueR, 16.0);
	vec3 upperR = (valueR - lowerR) / 16.0;
	vec3 alphaR = min(abs(upperR - lowerR), 2.0);

	// Average the energy over the pixels on either side
	vec4 rgba = vec4(
		(alphaR.x + alphaR.y + alphaR.z) / 6.0,
		(alphaL.y + alphaR.x + alphaR.y) / 6.0,
		(alphaL.x + alphaL.y + alphaR.x) / 6.0,
		0.0);

	Out_Color =   0.0001 * Frag_Color
                + 0.0001 * (color.a == 0.0 ? 1.0 - rgba : color * rgba)
                + 1.0 - rgba;

    //TODO This approach only supports black colored text
    //NOTE Out_Color's alpha channel is the transparency of the drawn text.
    //NOTE A clear color with alpha 0 will make the drawn text invisible!!
    //actual code:
    //Out_Color = 1.0 - rgba;
}
";
            shapeMaterial = new OpenGLMaterial(
                vertexShader: shapeVertexShaderText,
                fragmentShader: shapeFragmentShader
            );
            imageMaterial = new OpenGLMaterial(
                vertexShader: imageVertexShader,
                fragmentShader: imageFragmentShader
            );
            glyphMaterial = new OpenGLMaterial(
                vertexShader: glyphVertexShader,
                fragmentShader: glyphFragmentShader
            );
            textMaterial = new OpenGLMaterial(
                vertexShader: textVertexShader,
                fragmentShader: textFragmentShader
            );
        }

        public static readonly OpenGLMaterial shapeMaterial;

        public static readonly OpenGLMaterial imageMaterial;

        public static readonly OpenGLMaterial glyphMaterial;

        public static readonly OpenGLMaterial textMaterial;
    }
}
