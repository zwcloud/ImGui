namespace ImGui.OSImplentation.Android
{
    internal partial class OpenGLESMaterial
    {
        public static void InitCommonMaterials()
        {
            OpenGLESMaterial.shapeMaterial.Init();
            OpenGLESMaterial.imageMaterial.Init();
            OpenGLESMaterial.glyphMaterial.Init();
            OpenGLESMaterial.textMaterial.Init();
        }

        public static void ShutDownCommonMaterials()
        {
            OpenGLESMaterial.shapeMaterial.ShutDown();
            OpenGLESMaterial.imageMaterial.ShutDown();
            OpenGLESMaterial.glyphMaterial.ShutDown();
            OpenGLESMaterial.textMaterial.ShutDown();
        }

        public static readonly OpenGLESMaterial shapeMaterial = new OpenGLESMaterial(
            vertexShader: @"
#version 330
uniform mat4 ProjMtx;
in vec2 Position;
in vec2 UV;
in vec4 Color;
out vec2 Frag_UV;
out vec4 Frag_Color;
void main()
{
	Frag_UV = UV;
	Frag_Color = Color;
	gl_Position = ProjMtx * vec4(Position.xy,0,1);
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

        public static readonly OpenGLESMaterial imageMaterial = new OpenGLESMaterial(
            vertexShader: @"
#version 330
uniform mat4 ProjMtx;
in vec2 Position;
in vec2 UV;
in vec4 Color;
out vec2 Frag_UV;
out vec4 Frag_Color;
void main()
{
	Frag_UV = UV;
	Frag_Color = Color;
	gl_Position = ProjMtx * vec4(Position.xy,0,1);
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

        public static readonly OpenGLESMaterial glyphMaterial = new OpenGLESMaterial(
    vertexShader: @"
#version 330
uniform mat4 ProjMtx;
in vec2 Position;
in vec2 UV;
in vec4 Color;
out vec2 Frag_UV;
out vec4 Frag_Color;
uniform vec2 offset;
void main()
{
	Frag_UV = UV;
	Frag_Color = Color;
	gl_Position = ProjMtx * vec4(offset+Position.xy,0,1);
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
	Out_Color = Frag_Color* color *0.001 + vec4(0.125, 0, 0, 0); /*Frag_Color* color * (gl_FrontFacing ? 16.0 / 255.0 : 1.0 / 255.0);*/
    Out_Color.a = 1;
}
"
    );
        public static readonly OpenGLESMaterial textMaterial = new OpenGLESMaterial(
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

	Out_Color = vec4(rgba.rgb,1) + Frag_Color*0.001;// + vec4(0,1,0,1);
}
"
    );

    }
}
