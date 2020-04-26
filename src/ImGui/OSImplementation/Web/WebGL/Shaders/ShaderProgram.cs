using System;
using System.Collections.Generic;
using System.Text;
using WebAssembly;
using GL = ImGui.OSImplementation.Web.WebGL;

namespace ImGui.OSImplementation.Web
{
    public class ShaderProgram
    {
        private readonly Shader vertexShader = new Shader();
        private readonly Shader fragmentShader = new Shader();

        /// <summary>
        /// Creates the shader program.
        /// </summary>
        /// <param name="vertexShaderSource">The vertex shader source.</param>
        /// <param name="fragmentShaderSource">The fragment shader source.</param>
        /// <param name="attributeLocations">The attribute locations. This is an optional array of
        /// uint attribute locations to their names.</param>
        /// <exception cref="ShaderCompilationException"></exception>
        public void Create(string vertexShaderSource, string fragmentShaderSource,
            Dictionary<uint, string> attributeLocations)
        {
            //  Create the shaders.
            vertexShader.Create(GL.GL_VERTEX_SHADER, vertexShaderSource);
            fragmentShader.Create(GL.GL_FRAGMENT_SHADER, fragmentShaderSource);

            //  Create the program, attach the shaders.
            ShaderProgramObject = GL.CreateProgram();
            GL.AttachShader(ShaderProgramObject, vertexShader.ShaderObject);
            GL.AttachShader(ShaderProgramObject, fragmentShader.ShaderObject);

            //  Before we link, bind any vertex attribute locations.
            if (attributeLocations != null)
            {
                foreach (var vertexAttributeLocation in attributeLocations)
                    GL.BindAttribLocation(ShaderProgramObject, vertexAttributeLocation.Key, vertexAttributeLocation.Value);
            }

            //  Now we can link the program.
            GL.LinkProgram(ShaderProgramObject);

            //  Now that we've compiled and linked the shader, check it's link status. If it's not linked properly, we're
            //  going to throw an exception.
            if (GetLinkStatus() == false)
            {
                string log = this.GetInfoLog();
                throw new ShaderCompilationException(
                    string.Format("Failed to link shader program with ID {0}. Log: {1}", ShaderProgramObject, log), 
                    log);
            }
            if (vertexShader.GetCompileStatus() == false)
            {
                string log = vertexShader.GetInfoLog();
                throw new Exception(log);
            }
            if (fragmentShader.GetCompileStatus() == false)
            {
                string log = fragmentShader.GetInfoLog();
                throw new Exception(log);
            }

            GL.DetachShader(ShaderProgramObject, vertexShader.ShaderObject);
            GL.DetachShader(ShaderProgramObject, fragmentShader.ShaderObject);
            vertexShader.Delete();
            fragmentShader.Delete();
        }

        public void Delete()
        {
            //GL.DetachShader(ShaderProgramObject, vertexShader.ShaderObject);
            //GL.DetachShader(ShaderProgramObject, fragmentShader.ShaderObject);
            //vertexShader.Delete();
            //fragmentShader.Delete();
            GL.DeleteProgram(ShaderProgramObject);
            ShaderProgramObject = null;
        }

        public uint GetAttributeLocation(string attributeName)
        {
            //  If we don't have the attribute name in the dictionary, get it's
            //  location and add it.
            if (attributeNamesToLocations.ContainsKey(attributeName) == false)
            {
                int location = GL.GetAttribLocation(ShaderProgramObject, attributeName);
                attributeNamesToLocations[attributeName] = (uint)location;
            }

            //  Return the attribute location.
            return attributeNamesToLocations[attributeName];
        }

        public void BindAttributeLocation(uint location, string attribute)
        {
            GL.BindAttribLocation(ShaderProgramObject, location, attribute);
        }

        public void Bind()
        {
            GL.UseProgram(ShaderProgramObject);
        }

        public void Unbind()
        {
            GL.UseProgram(null);
        }

        public bool GetLinkStatus()
        {
            return GL.GetProgramParameter(ShaderProgramObject, GL.GL_LINK_STATUS)== GL.GL_TRUE;
        }

        public string GetInfoLog()
        {
            return GL.GetProgramInfoLog(ShaderProgramObject);
        }
        
        public void SetUniform(string uniformName, int v1)
        {
            GL.Uniform1(GetUniformLocation(uniformName), v1);
        }
        
        public void SetUniform(string uniformName, int v1, int v2)
        {
            GL.Uniform2(GetUniformLocation(uniformName), v1, v2);
        }

        public void SetUniform(string uniformName, int v1, int v2, int v3)
        {
            GL.Uniform3(GetUniformLocation(uniformName), v1, v2, v3);
        }
        
        public void SetUniform(string uniformName, int v1, int v2, int v3, int v4)
        {
            GL.Uniform4(GetUniformLocation(uniformName), v1, v2, v3, v4);
        }

        public void SetUniform(string uniformName, float v1)
        {
            GL.Uniform1(GetUniformLocation(uniformName), v1);
        }
        
        public void SetUniform(string uniformName, float v1, float v2)
        {
            GL.Uniform2(GetUniformLocation(uniformName), v1, v2);
        }

        public void SetUniform(string uniformName, float v1, float v2, float v3)
        {
            GL.Uniform3(GetUniformLocation(uniformName), v1, v2, v3);
        }

        public void SetUniform(string uniformName, float v1, float v2, float v3, float v4)
        {
            GL.Uniform4(GetUniformLocation(uniformName), v1, v2, v3, v4);
        }
        
        public void SetUniformMatrix3(string uniformName, float[] m)
        {
            GL.UniformMatrix3(GetUniformLocation(uniformName), false, m);
        }

        public void SetUniformMatrix4(string uniformName, float[] m)
        {
            GL.UniformMatrix4(GetUniformLocation(uniformName), false, m);
        }

        public JSObject GetUniformLocation(string uniformName)
        {
            //  If we don't have the uniform name in the dictionary, get it's
            //  location and add it.
            if (uniformNamesToLocations.ContainsKey(uniformName) == false)
            {
                uniformNamesToLocations[uniformName] = GL.GetUniformLocation(ShaderProgramObject, uniformName);
                //  TODO: if it's not found, we should probably throw an exception.
            }

            //  Return the uniform location.
            return uniformNamesToLocations[uniformName];
        }

        /// <summary>
        /// Gets the shader program object.
        /// </summary>
        /// <value>
        /// The shader program object.
        /// </value>
        public JSObject ShaderProgramObject { get; protected set; }


        /// <summary>
        /// A mapping of uniform names to locations. This allows us to very easily specify
        /// uniform data by name, quickly looking up the location first if needed.
        /// </summary>
        private readonly Dictionary<string, JSObject> uniformNamesToLocations = new Dictionary<string, JSObject>();

        /// <summary>
        /// A mapping of attribute names to locations. This allows us to very easily specify
        /// attribute data by name, quickly looking up the location first if needed.
        /// </summary>
        private readonly Dictionary<string, uint> attributeNamesToLocations = new Dictionary<string, uint>();
    }
}
