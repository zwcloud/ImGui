namespace IMGUI
{
    class GUIRenderer
    {
        private Size SurfaceSize { get; set; }

        private string vertexShaderSource = @"
#version 330 core
in vec4 in_Position;
in vec2 in_TexCoord;
out vec2 TexCoord;
void main()
{
	gl_Position = in_Position;
	TexCoord = in_TexCoord;
}";

        private string fragmentShaderSource = @"
#version 330 core
uniform sampler2D mysampler;
in vec2 TexCoord;
out vec4 out_Color;
void main()
{
	vec2 st = TexCoord.st;
	out_Color = texture2D(mysampler,vec2(st.s, 1- st.t));
}";

        private uint vertexShaderHandle,
            fragmentShaderHandle,
            shaderProgramHandle,
            positionVboHandle,
            textureHandle;

        private uint attributePos, attributeTexCoord;

        private readonly float[] vertexData = new float[]
        {
            -1, -1, 0,	0,0,
            -1, 1, 0,	0,1,
            1, 1, 0,	1,1,
            1, -1, 0,	1,0
        };

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
            CSharpGL.GL.Enable(CSharpGL.GL.GL_DEPTH_TEST);
            CSharpGL.GL.ClearColor(0, 0, 0.9f, 1);
        }

        private void CreateShaders()
        {
            vertexShaderHandle = CSharpGL.GL.CreateShader(CSharpGL.GL.GL_VERTEX_SHADER);
            fragmentShaderHandle = CSharpGL.GL.CreateShader(CSharpGL.GL.GL_FRAGMENT_SHADER);

            CSharpGL.GL.ShaderSource(vertexShaderHandle, vertexShaderSource);
            CSharpGL.GL.ShaderSource(fragmentShaderHandle, fragmentShaderSource);

            CSharpGL.GL.CompileShader(vertexShaderHandle);
            CSharpGL.GL.CompileShader(fragmentShaderHandle);

            //System.Diagnostics.Debug.WriteLine(GL.GetShaderInfoLog(vertexShaderHandle));
            //System.Diagnostics.Debug.WriteLine(GL.GetShaderInfoLog(fragmentShaderHandle));

            // Create program
            shaderProgramHandle = CSharpGL.GL.CreateProgram();

            CSharpGL.GL.AttachShader(shaderProgramHandle, vertexShaderHandle);
            CSharpGL.GL.AttachShader(shaderProgramHandle, fragmentShaderHandle);

            CSharpGL.GL.LinkProgram(shaderProgramHandle);
            //System.Diagnostics.Debug.WriteLine(GL.GetProgramInfoLog(shaderProgramHandle));
            CSharpGL.GL.UseProgram(shaderProgramHandle);

            //Release unused shader object
            CSharpGL.GL.DeleteShader(vertexShaderHandle);
            CSharpGL.GL.DeleteShader(fragmentShaderHandle);

            attributePos = (uint)CSharpGL.GL.GetAttribLocation(shaderProgramHandle, "in_Position");
            attributeTexCoord = (uint)CSharpGL.GL.GetAttribLocation(shaderProgramHandle, "in_TexCoord");

        }

        private void CreateVBOs()
        {
            {
                uint[] buffers = { 0 };
                CSharpGL.GL.GenBuffers(1, buffers);
                positionVboHandle = buffers[0];
                CSharpGL.GL.BindBuffer(CSharpGL.BufferTarget.ArrayBuffer, positionVboHandle);

                var dataHandle = System.Runtime.InteropServices.GCHandle.Alloc(vertexData, System.Runtime.InteropServices.GCHandleType.Pinned);
                CSharpGL.GL.BufferData(CSharpGL.GL.GL_ARRAY_BUFFER, vertexData.Length * System.Runtime.InteropServices.Marshal.SizeOf(typeof(GLM.vec3)), dataHandle.AddrOfPinnedObject(), CSharpGL.GL.GL_STATIC_DRAW);
                dataHandle.Free();
            }

            CSharpGL.GL.VertexAttribPointer(attributePos, 3, CSharpGL.GL.GL_FLOAT, false, 5 * System.Runtime.InteropServices.Marshal.SizeOf(typeof(float)), System.IntPtr.Zero);
            CSharpGL.GL.VertexAttribPointer(attributeTexCoord, 2, CSharpGL.GL.GL_FLOAT, false, 5 * System.Runtime.InteropServices.Marshal.SizeOf(typeof(float)),
                new System.IntPtr(3 * System.Runtime.InteropServices.Marshal.SizeOf(typeof(float))));

            CSharpGL.GL.EnableVertexAttribArray(attributePos);
            CSharpGL.GL.EnableVertexAttribArray(attributeTexCoord);
        }

        private void CreateTexture()
        {
            CSharpGL.GL.ActiveTexture(CSharpGL.GL.GL_TEXTURE0);
            CSharpGL.GL.Enable(CSharpGL.GL.GL_TEXTURE_2D);

            uint[] textures = { 0 };
            CSharpGL.GL.GenTextures(1, textures);
            textureHandle = textures[0];
            CSharpGL.GL.BindTexture(CSharpGL.GL.GL_TEXTURE_2D, textureHandle);

            {
                var textureData = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Repeat(new byte(), 4 * (int)SurfaceSize.Width * (int)SurfaceSize.Height));
                var handle = System.Runtime.InteropServices.GCHandle.Alloc(textureData, System.Runtime.InteropServices.GCHandleType.Pinned);
                CSharpGL.GL.TexImage2D(CSharpGL.GL.GL_TEXTURE_2D, 0, CSharpGL.GL.GL_RGBA, (int)SurfaceSize.Width, (int)SurfaceSize.Height, 0, CSharpGL.GL.GL_BGRA,
                    CSharpGL.GL.GL_UNSIGNED_BYTE, handle.AddrOfPinnedObject());
                handle.Free();
            }

            //sampler settings
            CSharpGL.GL.TexParameteri(CSharpGL.GL.GL_TEXTURE_2D, CSharpGL.GL.GL_TEXTURE_WRAP_S, (int)CSharpGL.GL.GL_CLAMP);
            CSharpGL.GL.TexParameteri(CSharpGL.GL.GL_TEXTURE_2D, CSharpGL.GL.GL_TEXTURE_WRAP_T, (int)CSharpGL.GL.GL_CLAMP);
            CSharpGL.GL.TexParameteri(CSharpGL.GL.GL_TEXTURE_2D, CSharpGL.GL.GL_TEXTURE_MAG_FILTER, (int)CSharpGL.GL.GL_LINEAR);
            CSharpGL.GL.TexParameteri(CSharpGL.GL.GL_TEXTURE_2D, CSharpGL.GL.GL_TEXTURE_MIN_FILTER, (int)CSharpGL.GL.GL_LINEAR);
        }

        public void OnUpdateFrame(long time)
        {
        }

        public void OnUpdateTexture(int width, int height, System.IntPtr data)
        {
            CSharpGL.GL.BindTexture(CSharpGL.GL.GL_TEXTURE_2D, textureHandle);
            CSharpGL.GL.TexSubImage2D(CSharpGL.GL.GL_TEXTURE_2D, 0, 0, 0, width, height, CSharpGL.GL.GL_BGRA, CSharpGL.GL.GL_UNSIGNED_BYTE, data);
        }

        public void OnRenderFrame()
        {
            CSharpGL.GL.Clear(CSharpGL.GL.GL_COLOR_BUFFER_BIT | CSharpGL.GL.GL_DEPTH_BUFFER_BIT);
            
            CSharpGL.GL.BindBuffer(CSharpGL.GL.GL_ARRAY_BUFFER, positionVboHandle);
            
            CSharpGL.GL.BindTexture(CSharpGL.GL.GL_TEXTURE_2D, textureHandle);

            CSharpGL.GL.DrawArrays(CSharpGL.GL.GL_TRIANGLE_FAN, 0, 6);
        }
    }
}