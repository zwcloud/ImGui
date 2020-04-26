namespace ImGui.OSImplementation.Android
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
#version 300 es
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
#version 300 es
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
#version 300 es
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
#version 300 es
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
#version 300 es
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
#version 300 es
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
#version 300 es
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
#version 300 es
in vec2 Frag_UV;
in vec4 Frag_Color;
out vec4 Out_Color;
void main()
{
	if (Frag_UV.s * Frag_UV.s - Frag_UV.t > 0.0)
	{
		discard;
	}
	Out_Color = Frag_Color;
}
"
    );

    }
}
