using System;
using WebAssembly;
using GL = ImGui.OSImplementation.Web.WebGL;

namespace ImGui.OSImplementation.Web
{
    /// <summary>
    /// This is the base class for all shaders (vertex and fragment). It offers functionality
    /// which is core to all shaders, such as file loading and binding.
    /// </summary>
    public class Shader
    {
        public void Create(uint shaderType, string source)
        {
            //  Create the OpenGL shader object.
            ShaderObject = GL.CreateShader(shaderType);
            if (ShaderObject == null)
            {
                throw new InvalidOperationException("GL.CreateShader failed.");
            }

            //  Set the shader source.
            GL.ShaderSource(ShaderObject, source);

            //  Compile the shader object.
            GL.CompileShader(ShaderObject);

            //  Now that we've compiled the shader, check it's compilation status. If it's not compiled properly, we're
            //  going to throw an exception.
            if (GetCompileStatus() == false)
            {
                string log = GetInfoLog();
                throw new ShaderCompilationException(
                    $"Failed to compile shader with ID {this.ShaderObject}. Log: {log}", log);
            }
        }

        public void Delete()
        {
            GL.DeleteShader(ShaderObject);
            ShaderObject = null;
        }

        public bool GetCompileStatus()
        {
            return GL.GetShaderParameter(ShaderObject, GL.GL_COMPILE_STATUS) == GL.GL_TRUE;
        }

        /// <summary>
        /// Get the compile info log.
        /// </summary>
        /// <returns></returns>
        public string GetInfoLog()
        {
            return GL.GetShaderInfoLog(ShaderObject);
        }

        /// <summary>
        /// Gets the shader object.
        /// </summary>
        public JSObject ShaderObject { get; protected set; }
    }
}
