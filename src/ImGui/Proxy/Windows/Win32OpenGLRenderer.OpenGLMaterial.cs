using System.Collections.Generic;
using CSharpGL;
using System.Runtime.InteropServices;

namespace ImGui
{
    internal partial class Win32OpenGLRenderer : IRenderer
    {
        class OpenGLMaterial
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
            
            public OpenGLMaterial(string vertexShader, string fragmentShader)
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

                Utility.CheckGLError();
            }

            private void CreateVBOs()
            {
                GL.GenBuffers(2, buffers);
                positionVboHandle = buffers[0];
                elementsHandle = buffers[1];
                GL.BindBuffer(GL.GL_ARRAY_BUFFER, positionVboHandle);
                GL.BindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, elementsHandle);

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

                Utility.CheckGLError();
            }

            private void DeleteShaders()
            {
                if (program != null)
                {
                    program.Unbind();
                    Utility.CheckGLError();
                    program.Delete();
                    Utility.CheckGLError();
                    program = null;
                }
            }

            private void DeleteVBOs()
            {
                GL.DeleteBuffers(1, buffers);
                Utility.CheckGLError();

                GL.BindBuffer(GL.GL_ARRAY_BUFFER, 0);
                Utility.CheckGLError();
            }

        }
    }
}
