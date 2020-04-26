using System.Collections.Generic;
using System.Runtime.InteropServices;
using WebAssembly;
using GL = ImGui.OSImplementation.Web.WebGL;

namespace ImGui.OSImplementation.Web
{
    internal partial class WebGLRenderer
    {
        class Material
        {
            string vertexShaderSource;
            string fragmentShaderSource;

            public JSObject positionVboHandle, elementsHandle;

            public JSObject vaoHandle;

            public uint attributePositon, attributeUV, attributeColor;

            public ShaderProgram program;
            private Dictionary<uint, string> attributeMap;

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

                Utility.CheckWebGLError();
            }

            private void CreateVBOs()
            {
                this.positionVboHandle = GL.CreateBuffer();
                this.elementsHandle =  GL.CreateBuffer();
                GL.BindBuffer(GL.GL_ARRAY_BUFFER, this.positionVboHandle);
                GL.BindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, this.elementsHandle);

                this.vaoHandle = GL.CreateVertexArray();
                GL.BindVertexArray(this.vaoHandle);

                GL.BindBuffer(GL.GL_ARRAY_BUFFER, this.positionVboHandle);

                GL.EnableVertexAttribArray(this.attributePositon);
                GL.EnableVertexAttribArray(this.attributeUV);
                GL.EnableVertexAttribArray(this.attributeColor);

                GL.VertexAttribPointer(this.attributePositon, 2, GL.GL_FLOAT, false, Marshal.SizeOf<DrawVertex>(), Marshal.OffsetOf<DrawVertex>("pos").ToInt32());
                GL.VertexAttribPointer(this.attributeUV, 2, GL.GL_FLOAT, false, Marshal.SizeOf<DrawVertex>(), Marshal.OffsetOf<DrawVertex>("uv").ToInt32());
                GL.VertexAttribPointer(this.attributeColor, 4, GL.GL_FLOAT, true, Marshal.SizeOf<DrawVertex>(), Marshal.OffsetOf<DrawVertex>("color").ToInt32());

                Utility.CheckWebGLError();
            }

            private void DeleteShaders()
            {
                if (this.program != null)
                {
                    this.program.Unbind();
                    Utility.CheckWebGLError();
                    this.program.Delete();
                    Utility.CheckWebGLError();
                    this.program = null;
                }
            }

            private void DeleteVBOs()
            {
                GL.DeleteBuffer(this.positionVboHandle);
                GL.DeleteBuffer(this.elementsHandle);
                Utility.CheckWebGLError();

                GL.BindBuffer(GL.GL_ARRAY_BUFFER, null);
                Utility.CheckWebGLError();
            }

        }
    }
}
