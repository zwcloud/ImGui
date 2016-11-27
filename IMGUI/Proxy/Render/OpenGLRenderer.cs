using System;
using System.Collections.Generic;
using CSharpGL;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ImGui
{
    partial class OpenGLRenderer : IRenderer
    {
        class Material
        {
            string vertexShaderSource;
            string fragmentShaderSource;

            public readonly uint[] buffers = { 0, 0 };
            public uint positionVboHandle/*buffers[0]*/, elementsHandle/*buffers[1]*/;

            public readonly uint[] vertexArray = { 0 };
            public uint vaoHandle;

            public uint attributePositon, attributeUV, attributeColor;

            public CSharpGL.Objects.Shaders.ShaderProgram program;
            public Dictionary<uint, string> attributeMap;

            public readonly uint[] textures = { 0 };
            
            public Material(string vertexShader, string fragmentShader)
            {
                this.vertexShaderSource = vertexShader;
                this.fragmentShaderSource = fragmentShader;
            }

            public void Init()
            {
                this.CreateShaders();
                this.CreateVBOs();
            }

            public void ShutDown()
            {
                this.DeleteShaders();
                this.DeleteVBOs();
            }

            private void CreateShaders()
            {
                program = new CSharpGL.Objects.Shaders.ShaderProgram();
                attributePositon = 0;
                attributeUV = 1;
                attributeColor = 2;
                attributeMap = new Dictionary<uint, string>
                {
                    {0, "Position"},
                    {1, "UV"},
                    {2, "Color" }
                };
                program.Create(vertexShaderSource, fragmentShaderSource, attributeMap);

                CheckError();
            }

            private void CreateVBOs()
            {
                GL.GenBuffers(2, buffers);
                positionVboHandle = buffers[0];
                elementsHandle = buffers[1];
                GL.BindBuffer(BufferTarget.ArrayBuffer, positionVboHandle);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementsHandle);

                GL.GenVertexArrays(1, vertexArray);
                vaoHandle = vertexArray[0];
                GL.BindVertexArray(vaoHandle);

                GL.BindBuffer(GL.GL_ARRAY_BUFFER, positionVboHandle);

                GL.EnableVertexAttribArray(attributePositon);
                GL.EnableVertexAttribArray(attributeUV);
                GL.EnableVertexAttribArray(attributeColor);

                GL.VertexAttribPointer(attributePositon, 2, GL.GL_FLOAT, false, Marshal.SizeOf<DrawVertex>(), Marshal.OffsetOf<DrawVertex>("pos"));
                GL.VertexAttribPointer(attributeUV, 2, GL.GL_FLOAT, false, Marshal.SizeOf<DrawVertex>(), Marshal.OffsetOf<DrawVertex>("uv"));
                GL.VertexAttribPointer(attributeColor, 4, GL.GL_FLOAT, true, Marshal.SizeOf<DrawVertex>(), Marshal.OffsetOf<DrawVertex>("color"));

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

        }

        Material m = new Material(
            vertexShader:@"
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
};
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

        Material mExtra = new Material(@"
#version 330
uniform mat4 ProjMtx;

in vec4 Position;
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
            @"
#version 330
in vec2 Frag_UV;
in vec4 Frag_Color;

out vec4 Out_Color;

float inCurve(vec2 uv)
{
	return uv.x * uv.x - uv.y;
}

void main()
{
	float x = inCurve(Frag_UV);

	if(!gl_FrontFacing)
	{
		if (x > 0.) discard;
	}
	else
	{
		if (x < 0.) discard;
	}

	Out_Color = Frag_Color;
}
"
            );

        public void Init(object windowHandle)
        {
            CreateOpenGLContext((IntPtr)windowHandle);
            InitGLEW();

            m.Init();
            mExtra.Init();

            // Other state
            GL.ClearColor(1, 1, 1, 1);

            CheckError();
        }

        public void Clear()
        {
            GL.Clear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT);
        }

        private static void DoRender(Material m,
            ImGui.Internal.List<DrawCommand> commandBuffer, ImGui.Internal.List<DrawIndex> indexBuffer, ImGui.Internal.List<DrawVertex> vertexBuffer,
            int width, int height)
        {
            // Setup render state: alpha-blending enabled, no face culling, no depth testing, scissor enabled
            GL.Enable(GL.GL_BLEND);
            GL.BlendEquation(GL.GL_FUNC_ADD_EXT);
            GL.BlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
            GL.Disable(GL.GL_CULL_FACE);
            GL.Disable(GL.GL_DEPTH_TEST);
            //GL.Enable(GL.GL_SCISSOR_TEST);
            //GL.ActiveTexture(GL.GL_TEXTURE0);

            GL.BindVertexArray(m.vaoHandle);
            GL.BindBuffer(GL.GL_ARRAY_BUFFER, m.positionVboHandle);
            GL.BufferData(GL.GL_ARRAY_BUFFER,
                vertexBuffer.Count * Marshal.SizeOf<DrawVertex>(),
                vertexBuffer.Pointer, GL.GL_STREAM_DRAW);

            GL.BindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, m.elementsHandle);
            GL.BufferData(GL.GL_ELEMENT_ARRAY_BUFFER,
                indexBuffer.Count * Marshal.SizeOf<DrawIndex>(),
                indexBuffer.Pointer, GL.GL_STREAM_DRAW);


            // Setup viewport, orthographic projection matrix
            GL.Viewport(0, 0, width, height);
            GLM.mat4 ortho_projection = GLM.glm.ortho(0.0f, width, height, 0.0f, -5.0f, 5.0f);
            m.program.Bind();
            m.program.SetUniformMatrix4("ProjMtx", ortho_projection.to_array());

            var indexBufferOffset = IntPtr.Zero;
            foreach (var drawCmd in commandBuffer)
            {
                //GL.BindTexture(...)
                //GL.Scissor(...)
                GL.DrawElements(GL.GL_TRIANGLES, drawCmd.ElemCount, GL.GL_UNSIGNED_INT, indexBufferOffset);
                indexBufferOffset = IntPtr.Add(indexBufferOffset, drawCmd.ElemCount);
            }

            CheckError();
        }

        public void RenderDrawList(DrawList drawList, int width, int height)
        {
            DoRender(m, drawList.DrawBuffer.CommandBuffer, drawList.DrawBuffer.IndexBuffer, drawList.DrawBuffer.VertexBuffer, width, height);
            DoRender(mExtra, drawList.BezierBuffer.CommandBuffer, drawList.BezierBuffer.IndexBuffer, drawList.BezierBuffer.VertexBuffer, width, height);
        }

        public void ShutDown()
        {
            m.ShutDown();
            mExtra.ShutDown();
        }

        private static void CheckError(
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
