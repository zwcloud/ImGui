using System.Runtime.CompilerServices;
using CSharpGL;

namespace ImGui
{
    class GUIRenderer
    {
        private Size SurfaceSize { get; set; }

        private string vertexShaderSource330 = @"
#version 330 core
in vec4 in_Position;
in vec2 in_TexCoord;
out vec2 TexCoord;
void main()
{
	gl_Position = in_Position;
	TexCoord = in_TexCoord;
}";

		private string fragmentShaderSource330 = @"
#version 330 core
uniform sampler2D mysampler;
in vec2 TexCoord;
out vec4 out_Color;
void main()
{
	vec2 st = TexCoord.st;
	out_Color = texture2D(mysampler,vec2(st.s, 1- st.t));
}";

		private string vertexShaderSource120 = @"
#version 120
attribute vec4 in_Position;
attribute vec2 in_TexCoord0;
varying vec2 out_TexCoord;
void main()
{
    gl_Position = in_Position;
    out_TexCoord  = in_TexCoord0;
}";

		private string fragmentShaderSource120 = @"
#version 120
uniform sampler2D mysampler;
varying vec2 out_TexCoord;
void main()
{
	vec2 st = out_TexCoord.st;
	gl_FragColor = texture2D(mysampler,vec2(st.s, 1- st.t));
}";

        private uint positionVboHandle, textureHandle;
        private uint attributePos, attributeTexCoord;

        private readonly float[] vertexData = new float[]
        {
            -1, -1, 0,	0,0,
            -1, 1, 0,	0,1,
            1, 1, 0,	1,1,
            1, -1, 0,	1,0
        };

        private CSharpGL.Objects.Shaders.ShaderProgram program;
        private System.Collections.Generic.Dictionary<uint, string> attributeMap;

        public GUIRenderer(Size surfaceSize)
        {
            SurfaceSize = surfaceSize;
        }

        public void OnLoad()
        {
            CreateShaders();
            CreateVBOs();
            CreateTexture();

            // Other state
            GL.Enable(GL.GL_DEPTH_TEST);
            GL.ClearColor(0, 0, 0.9f, 1);
        }
        
        private void CreateShaders()
        {
			string vertexShaderSource, fragmentShaderSource;
			if (Utility.CurrentOS.IsWindows)
			{
				vertexShaderSource = vertexShaderSource330;
				fragmentShaderSource = fragmentShaderSource330;
			}
			else
			{
				vertexShaderSource = vertexShaderSource120;
				fragmentShaderSource = fragmentShaderSource120;
			}

            program = new CSharpGL.Objects.Shaders.ShaderProgram();
            attributePos = 0;
            attributeTexCoord = 1;
            attributeMap = new System.Collections.Generic.Dictionary<uint, string>
            {
                {0, "in_Position"},
                {1, "in_TexCoord0"}
            };
            program.Create(vertexShaderSource, fragmentShaderSource, attributeMap);
            program.Bind();

			CheckEroor();
        }

        private void CreateVBOs()
        {
            uint[] buffers = {0};
            GL.GenBuffers(1, buffers);
            positionVboHandle = buffers[0];
            GL.BindBuffer(BufferTarget.ArrayBuffer, positionVboHandle);

            var dataHandle = System.Runtime.InteropServices.GCHandle.Alloc(vertexData,
                System.Runtime.InteropServices.GCHandleType.Pinned);
            GL.BufferData(GL.GL_ARRAY_BUFFER,
                vertexData.Length*System.Runtime.InteropServices.Marshal.SizeOf(typeof (GLM.vec3)),
                dataHandle.AddrOfPinnedObject(), GL.GL_STATIC_DRAW);
            dataHandle.Free();

            GL.VertexAttribPointer(attributePos, 3, GL.GL_FLOAT, false,
                5*System.Runtime.InteropServices.Marshal.SizeOf(typeof (float)), System.IntPtr.Zero);
            GL.VertexAttribPointer(attributeTexCoord, 2, GL.GL_FLOAT, false,
                5*System.Runtime.InteropServices.Marshal.SizeOf(typeof (float)),
                new System.IntPtr(3*System.Runtime.InteropServices.Marshal.SizeOf(typeof (float))));

            GL.EnableVertexAttribArray(attributePos);
            GL.EnableVertexAttribArray(attributeTexCoord);
            
            CheckEroor();
        }

        private void CreateTexture()
        {
            GL.ActiveTexture(GL.GL_TEXTURE0);
            GL.Enable(GL.GL_TEXTURE_2D);

            uint[] textures = {0};
            GL.GenTextures(1, textures);
            textureHandle = textures[0];
            GL.BindTexture(GL.GL_TEXTURE_2D, textureHandle);
            var textureData =
                System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Repeat((byte) 255,
                    4*(int) SurfaceSize.Width*(int) SurfaceSize.Height));
            var handle = System.Runtime.InteropServices.GCHandle.Alloc(textureData,
                System.Runtime.InteropServices.GCHandleType.Pinned);
            GL.TexImage2D(GL.GL_TEXTURE_2D, 0, GL.GL_RGBA, (int) SurfaceSize.Width, (int) SurfaceSize.Height, 0,
                GL.GL_BGRA,
                GL.GL_UNSIGNED_BYTE, handle.AddrOfPinnedObject());
            handle.Free();

            //sampler settings
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_S, (int) GL.GL_CLAMP);
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_WRAP_T, (int) GL.GL_CLAMP);
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, (int) GL.GL_LINEAR);
            GL.TexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, (int) GL.GL_LINEAR);

            CheckEroor();
        }

        public void OnUpdateTexture(Rect rect, System.IntPtr data)
        {
            GL.BindTexture(GL.GL_TEXTURE_2D, textureHandle);

            GL.TexSubImage2D(GL.GL_TEXTURE_2D, 0, (int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height, GL.GL_BGRA, GL.GL_UNSIGNED_BYTE, data);
			CheckEroor();
        }

        public void OnRenderFrame()
        {
            GL.Clear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT);
            
            GL.BindBuffer(GL.GL_ARRAY_BUFFER, positionVboHandle);
            
            GL.BindTexture(GL.GL_TEXTURE_2D, textureHandle);

            GL.DrawArrays(GL.GL_TRIANGLE_FAN, 0, 6);

			CheckEroor();
        }

		private void CheckEroor(
			[CallerFilePath] string fileName = null,
			[CallerLineNumber] int lineNumber = 0,
			[CallerMemberName] string memberName = null)
		{
			var error = GL.GetError();
			if (error != GL.GL_NO_ERROR)
			{
				System.Diagnostics.Debug.WriteLine("{0}({1}) in {2}: glError: 0x{3:X}",
					fileName, lineNumber, memberName, error);
			}
		}
    }
}