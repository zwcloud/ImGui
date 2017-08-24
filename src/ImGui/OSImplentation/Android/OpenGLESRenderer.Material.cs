using System.Collections.Generic;
using System.Runtime.InteropServices;
using CSharpGLES;

namespace ImGui.OSImplentation.Android
{
    internal partial class OpenGLESRenderer
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

            public ShaderProgram program;
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
                this.program = new ShaderProgram();
                this.attributePositon = 0;
                this.attributeUV = 1;
                this.attributeColor = 2;
                this.attributeMap = new Dictionary<uint, string>
                {
                    {0, "Position"},
                    {1, "UV"},
                    {2, "Color" }
                };
                this.program.Create(this.vertexShaderSource, this.fragmentShaderSource, this.attributeMap);

                Utility.CheckGLESError();
            }

            private void CreateVBOs()
            {
                GL.GenBuffers(2, this.buffers);
                this.positionVboHandle = this.buffers[0];
                this.elementsHandle = this.buffers[1];
                GL.BindBuffer(GL.GL_ARRAY_BUFFER, this.positionVboHandle);
                GL.BindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, this.elementsHandle);

                GL.GenVertexArrays(1, this.vertexArray);
                this.vaoHandle = this.vertexArray[0];
                GL.BindVertexArray(this.vaoHandle);

                GL.BindBuffer(GL.GL_ARRAY_BUFFER, this.positionVboHandle);

                GL.EnableVertexAttribArray(this.attributePositon);
                GL.EnableVertexAttribArray(this.attributeUV);
                GL.EnableVertexAttribArray(this.attributeColor);

                GL.VertexAttribPointer(this.attributePositon, 2, GL.GL_FLOAT, false, Marshal.SizeOf<DrawVertex>(), Marshal.OffsetOf<DrawVertex>("pos"));
                GL.VertexAttribPointer(this.attributeUV, 2, GL.GL_FLOAT, false, Marshal.SizeOf<DrawVertex>(), Marshal.OffsetOf<DrawVertex>("uv"));
                GL.VertexAttribPointer(this.attributeColor, 4, GL.GL_FLOAT, true, Marshal.SizeOf<DrawVertex>(), Marshal.OffsetOf<DrawVertex>("color"));

                Utility.CheckGLESError();
            }

            private void DeleteShaders()
            {
                if (this.program != null)
                {
                    this.program.Unbind();
                    Utility.CheckGLESError();
                    this.program.Delete();
                    Utility.CheckGLESError();
                    this.program = null;
                }
            }

            private void DeleteVBOs()
            {
                GL.DeleteBuffers(1, this.buffers);
                Utility.CheckGLESError();

                GL.BindBuffer(GL.GL_ARRAY_BUFFER, 0);
                Utility.CheckGLESError();
            }

        }
    }
}
