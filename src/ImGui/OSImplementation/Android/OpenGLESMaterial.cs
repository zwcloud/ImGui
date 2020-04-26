using System.Collections.Generic;
using System.Runtime.InteropServices;
using CSharpGL;

namespace ImGui.OSImplementation.Android
{
    internal partial class OpenGLESMaterial
    {
        private readonly string vertexShaderSource;
        private readonly string fragmentShaderSource;

        private readonly uint[] buffers = { 0, 0 };
        private uint vboHandle;/*buffers[0]*/
        private uint eboHandle;/*buffers[1]*/

        private readonly uint[] vertexArray = { 0 };
        private uint vaoHandle;

        private uint attributePositon;
        private uint attributeUV;
        private uint attributeColor;

        public CSharpGL.Objects.Shaders.ShaderProgram program;
        private Dictionary<uint, string> attributeMap;

        public OpenGLESMaterial(string vertexShader, string fragmentShader)
        {
            this.vertexShaderSource = vertexShader;
            this.fragmentShaderSource = fragmentShader;
        }

        public uint VboHandle => this.vboHandle;

        public uint EboHandle => this.eboHandle;

        public uint VaoHandle => this.vaoHandle;

        public void Init()
        {
            CreateShaders();
            CreateObjects();
        }

        public void ShutDown()
        {
            DeleteShaders();
            DeleteObjects();
        }

        private void CreateShaders()
        {
            this.program = new CSharpGL.Objects.Shaders.ShaderProgram();
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

            Utility.CheckGLError();
        }

        private void CreateObjects()
        {
            GL.GenBuffers(2, this.buffers);
            this.vboHandle = this.buffers[0];
            this.eboHandle = this.buffers[1];
            GL.BindBuffer(GL.GL_ARRAY_BUFFER, this.VboHandle);
            GL.BindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, this.EboHandle);

            GL.GenVertexArrays(1, this.vertexArray);
            this.vaoHandle = this.vertexArray[0];
            GL.BindVertexArray(this.VaoHandle);

            GL.BindBuffer(GL.GL_ARRAY_BUFFER, this.VboHandle);

            GL.EnableVertexAttribArray(this.attributePositon);
            GL.EnableVertexAttribArray(this.attributeUV);
            GL.EnableVertexAttribArray(this.attributeColor);

            GL.VertexAttribPointer(this.attributePositon, 2, GL.GL_FLOAT, false, Marshal.SizeOf<DrawVertex>(), Marshal.OffsetOf<DrawVertex>("pos"));
            GL.VertexAttribPointer(this.attributeUV, 2, GL.GL_FLOAT, false, Marshal.SizeOf<DrawVertex>(), Marshal.OffsetOf<DrawVertex>("uv"));
            GL.VertexAttribPointer(this.attributeColor, 4, GL.GL_FLOAT, true, Marshal.SizeOf<DrawVertex>(), Marshal.OffsetOf<DrawVertex>("color"));

            GL.BindVertexArray(0);

            Utility.CheckGLError();
        }

        private void DeleteShaders()
        {
            if (this.program != null)
            {
                this.program.Unbind();
                Utility.CheckGLError();
                this.program.Delete();
                Utility.CheckGLError();
                this.program = null;
            }
        }

        private void DeleteObjects()
        {
            GL.DeleteBuffers(2, this.buffers);
            Utility.CheckGLError();

            GL.BindBuffer(GL.GL_ARRAY_BUFFER, 0);
            Utility.CheckGLError();
        }

    }
}
