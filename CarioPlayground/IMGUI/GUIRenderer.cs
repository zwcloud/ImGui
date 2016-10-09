using System.Diagnostics;
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

        readonly uint[] buffers = { 0 };

        readonly uint[] textures = { 0 };

        public void OnLoad(Size surfaceSize)
        {
            this.SurfaceSize = surfaceSize;

            string version = GL.GetString(CSharpGL.GL.GL_VERSION);
            Debug.WriteLine("OpenGL version info: " + version);
            int[] tmp = { 0 };
            GL.GetIntegerv(CSharpGL.GL.GL_MAX_TEXTURE_SIZE, tmp);
            int max_texture_size = tmp[0];
            Debug.WriteLine("Max texture size: " + max_texture_size);

            DeleteShaders();
            DeleteVBOs();
            DeleteTexture();

            CreateShaders();
            CreateVBOs();
            CreateTexture();

            // Other state
            GL.Enable(GL.GL_DEPTH_TEST);
            GL.ClearColor(0, 0, 0.9f, 1);
            GL.Viewport(0, 0, (int)surfaceSize.Width, (int)surfaceSize.Height);
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

			CheckError();
        }

        private void CreateVBOs()
        {
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

            CheckError();
        }

        private void CreateTexture()
        {
            GL.ActiveTexture(GL.GL_TEXTURE0);
            GL.Enable(GL.GL_TEXTURE_2D);

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

            CheckError();
        }

        private void DeleteShaders()
        {
            if (program != null)
            {
                program.Unbind(); CheckError();
                program.Delete(); CheckError();
                program = null;
            }
        }

        private void DeleteVBOs()
        {
            GL.DeleteBuffers(1, buffers); CheckError();

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0); CheckError();
        }

        private void DeleteTexture()
        {
            GL.DeleteTextures(1, textures); CheckError();
        }

        public void OnUpdateTexture(Rect rect, System.IntPtr data)
        {
            GL.BindTexture(GL.GL_TEXTURE_2D, textureHandle); CheckError();

            GL.TexSubImage2D(GL.GL_TEXTURE_2D, 0, (int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height, GL.GL_BGRA, GL.GL_UNSIGNED_BYTE, data); CheckError();
        }

        public void OnRenderFrame()
        {
            GL.Clear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT);

            GL.BindBuffer(GL.GL_ARRAY_BUFFER, positionVboHandle);

            GL.BindTexture(GL.GL_TEXTURE_2D, textureHandle);

            GL.DrawArrays(GL.GL_TRIANGLE_FAN, 0, 6);

			CheckError();
        }

		private void CheckError(
			[CallerFilePath] string fileName = null,
			[CallerLineNumber] int lineNumber = 0,
			[CallerMemberName] string memberName = null)
		{
			var error = GL.GetError();
            string errorStr = "GL_NO_ERROR";
		    switch (error)
		    {
                case GL.GL_NO_ERROR:
		            errorStr = "GL_NO_ERROR";
                    break;
                case GL.GL_INVALID_ENUM:
                    errorStr = "GL_INVALID_ENUM";
                    break;
                case GL.GL_INVALID_VALUE:
                    errorStr = "GL_INVALID_VALUE";
                    break;
                case GL.GL_INVALID_OPERATION:
                    errorStr = "GL_INVALID_OPERATION";
                    break;
                case GL.GL_STACK_OVERFLOW:
                    errorStr = "GL_STACK_OVERFLOW";
                    break;
                case GL.GL_STACK_UNDERFLOW:
                    errorStr = "GL_STACK_UNDERFLOW";
                    break;
                case GL.GL_OUT_OF_MEMORY:
                    errorStr = "GL_OUT_OF_MEMORY";
                    break;
                case GL.GL_INVALID_FRAMEBUFFER_OPERATION:
                    errorStr = "GL_INVALID_FRAMEBUFFER_OPERATION";
                    break;
                case GL.GL_CONTEXT_LOST:
                    errorStr = "GL_CONTEXT_LOST";
                    break;
		    }

			if (error != GL.GL_NO_ERROR)
			{
                Debug.WriteLine("{0}({1}): glError: 0x{2:X} ({3}) in {4}",
					fileName, lineNumber, error, errorStr, memberName);
			}
		}
    }
}