namespace ImGui.OSImplementation.Shared
{
    internal partial class OpenGLMaterial
    {
        public static void InitCommonMaterials()
        {
            OpenGLMaterial.shapeMaterial.Init();
            OpenGLMaterial.imageMaterial.Init();
            OpenGLMaterial.glyphMaterial.Init();
            OpenGLMaterial.textMaterial.Init();
        }

        public static void ShutDownCommonMaterials()
        {
            OpenGLMaterial.shapeMaterial.ShutDown();
            OpenGLMaterial.imageMaterial.ShutDown();
            OpenGLMaterial.glyphMaterial.ShutDown();
            OpenGLMaterial.textMaterial.ShutDown();
        }

        public static readonly OpenGLMaterial shapeMaterial = new OpenGLMaterial(
            vertexShader: @"
#version 330
#extension GL_ARB_explicit_uniform_location : require
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
",
            fragmentShader: @"
#version 330
uniform sampler2D Texture;
in vec2 Frag_UV;
in vec4 Frag_Color;
out vec4 Out_Color;
void main()
{
	Out_Color = Frag_Color;
}
"
            );

        public static readonly OpenGLMaterial imageMaterial = new OpenGLMaterial(
            vertexShader: @"
#version 330
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
",
            fragmentShader: @"
#version 330
uniform sampler2D Texture;
in vec2 Frag_UV;
in vec4 Frag_Color;
out vec4 Out_Color;
void main()
{
	Out_Color = Frag_Color * texture( Texture, Frag_UV.st);
}
"
            );

        public static readonly OpenGLMaterial glyphMaterial = new OpenGLMaterial(
    vertexShader: @"
#version 330
uniform mat4 ProjMtx;
uniform mat4 ViewMtx;
in vec2 Position;
in vec2 UV;
in vec4 Color;
out vec2 Frag_UV;
out vec4 Frag_Color;
uniform vec2 offset;
uniform vec4 color;
void main()
{
	Frag_UV = UV;
	Frag_Color = color;//dummy Frag_Color is useless
	gl_Position = ProjMtx * ViewMtx * vec4(offset+Position.xy,0,1);
}
",
    fragmentShader: @"
#version 330
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
"
    );
        public static readonly OpenGLMaterial textMaterial = new OpenGLMaterial(
    vertexShader: @"
#version 330
in vec2 Position;
in vec2 UV;
in vec4 Color;
out vec2 Frag_UV;
out vec4 Frag_Color;
void main()
{
	Frag_UV = UV;
    Frag_Color = Color;
	gl_Position = vec4(Position, 0.0, 1.0);
}
",
    fragmentShader: @"
#version 330
in vec2 Frag_UV;
in vec4 Frag_Color;
out vec4 Out_Color;
uniform sampler2D Texture;
uniform vec4 color;
void main()
{
	// Get samples for -2/3 and -1/3
	vec2 valueL = texture2D(Texture, vec2(Frag_UV.x + dFdx(Frag_UV.x), Frag_UV.y)).yz * 255.0;
	vec2 lowerL = mod(valueL, 16.0);
	vec2 upperL = (valueL - lowerL) / 16.0;
	vec2 alphaL = min(abs(upperL - lowerL), 2.0);

	// Get samples for 0, +1/3, and +2/3
	vec3 valueR = texture2D(Texture, Frag_UV).xyz * 255.0;
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
"
    );

    }
}
