using System;
using System.Collections.Generic;
using System.Text;

namespace CSharpGL.Objects.Shaders
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
            this.vertexShader.Create(GL.GL_VERTEX_SHADER, vertexShaderSource);
            this.fragmentShader.Create(GL.GL_FRAGMENT_SHADER, fragmentShaderSource);

            //  Create the program, attach the shaders.
            this.ShaderProgramObject = GL.CreateProgram();
            GL.AttachShader(this.ShaderProgramObject, this.vertexShader.ShaderObject);
            GL.AttachShader(this.ShaderProgramObject, this.fragmentShader.ShaderObject);

            //  Before we link, bind any vertex attribute locations.
            if (attributeLocations != null)
            {
                foreach (var vertexAttributeLocation in attributeLocations)
                    GL.BindAttribLocation(this.ShaderProgramObject, vertexAttributeLocation.Key, vertexAttributeLocation.Value);
            }

            //  Now we can link the program.
            GL.LinkProgram(this.ShaderProgramObject);

            //  Now that we've compiled and linked the shader, check it's link status. If it's not linked properly, we're
            //  going to throw an exception.
            if (this.GetLinkStatus() == false)
            {
                string log = this.GetInfoLog();
                throw new ShaderCompilationException(
                    $"Failed to link shader program with ID {this.ShaderProgramObject}. Log: {log}",
                    log);
            }
            if (this.vertexShader.GetCompileStatus() == false)
            {
                string log = this.vertexShader.GetInfoLog();
                throw new Exception(log);
            }
            if (this.fragmentShader.GetCompileStatus() == false)
            {
                string log = this.fragmentShader.GetInfoLog();
                throw new Exception(log);
            }

            // Now that the shaders have been linked to the program, they can be released.
            GL.DetachShader(this.ShaderProgramObject, this.vertexShader.ShaderObject);
            GL.DetachShader(this.ShaderProgramObject, this.fragmentShader.ShaderObject);
            this.vertexShader.Delete();
            this.fragmentShader.Delete();
        }

        public void Delete()
        {
            GL.DeleteProgram(this.ShaderProgramObject);
            this.ShaderProgramObject = 0;
        }

        public uint GetAttributeLocation(string attributeName)
        {
            //  If we don't have the attribute name in the dictionary, get it's
            //  location and add it.
            if (this.attributeNamesToLocations.ContainsKey(attributeName) == false)
            {
                int location = GL.GetAttribLocation(this.ShaderProgramObject, attributeName);
                if (location < 0) { throw new Exception(); }

                this.attributeNamesToLocations[attributeName] = (uint)location;
            }

            //  Return the attribute location.
            return this.attributeNamesToLocations[attributeName];
        }

        public void BindAttributeLocation(uint location, string attribute)
        {
            GL.BindAttribLocation(this.ShaderProgramObject, location, attribute);
        }

        public void Bind()
        {
            GL.UseProgram(this.ShaderProgramObject);
        }

        public void Unbind()
        {
            GL.UseProgram(0);
        }

        public bool GetLinkStatus()
        {
            int[] parameters = new int[] { 0 };
            GL.GetProgram(this.ShaderProgramObject, GL.GL_LINK_STATUS, parameters);
            return parameters[0] == GL.GL_TRUE;
        }

        public string GetInfoLog()
        {
            //  Get the info log length.
            int[] infoLength = new int[] { 0 };
            GL.GetProgram(this.ShaderProgramObject, GL.GL_INFO_LOG_LENGTH, infoLength);
            int bufSize = infoLength[0];

            //  Get the compile info.
            StringBuilder il = new StringBuilder(bufSize);
            GL.GetProgramInfoLog(this.ShaderProgramObject, bufSize, IntPtr.Zero, il);

            string log = il.ToString();
            return log;
        }

        /// <summary>
        /// Set a uniform of int
        /// </summary>
        public void SetUniform(string uniformName, int v1)
        {
            GL.Uniform1(this.GetUniformLocation(uniformName), v1);
        }

        /// <summary>
        /// Set a uniform of vec2
        /// </summary>
        public void SetUniform(string uniformName, int v1, int v2)
        {
            GL.Uniform2(this.GetUniformLocation(uniformName), v1, v2);
        }

        /// <summary>
        /// Set a uniform of vec3
        /// </summary>
        public void SetUniform(string uniformName, int v1, int v2, int v3)
        {
            GL.Uniform3(this.GetUniformLocation(uniformName), v1, v2, v3);
        }

        /// <summary>
        /// Set a uniform of vec4
        /// </summary>
        public void SetUniform(string uniformName, int v1, int v2, int v3, int v4)
        {
            GL.Uniform4(this.GetUniformLocation(uniformName), v1, v2, v3, v4);
        }

        /// <summary>
        /// Set a uniform of float
        /// </summary>
        public void SetUniform(string uniformName, float v1)
        {
            GL.Uniform1(this.GetUniformLocation(uniformName), v1);
        }
        
        /// <summary>
        /// Set a uniform of float2
        /// </summary>
        public void SetUniform(string uniformName, float v1, float v2)
        {
            GL.Uniform2(this.GetUniformLocation(uniformName), v1, v2);
        }
        
        /// <summary>
        /// Set a uniform of float3
        /// </summary>
        public void SetUniform(string uniformName, float v1, float v2, float v3)
        {
            GL.Uniform3(this.GetUniformLocation(uniformName), v1, v2, v3);
        }

        /// <summary>
        /// Set a uniform of float4
        /// </summary>
        public void SetUniform(string uniformName, float v1, float v2, float v3, float v4)
        {
            GL.Uniform4(this.GetUniformLocation(uniformName), v1, v2, v3, v4);
        }
        
        /// <summary>
        /// Set a uniform of mat3
        /// </summary>
        public void SetUniformMatrix3(string uniformName, float[] m)
        {
            GL.UniformMatrix3(this.GetUniformLocation(uniformName), 1, false, m);
        }
        
        /// <summary>
        /// Set a uniform of mat4
        /// </summary>
        public void SetUniformMatrix4(string uniformName, float[] m)
        {
            GL.UniformMatrix4(this.GetUniformLocation(uniformName), 1, false, m);
        }
        
        /// <summary>
        /// Get the location index of a uniform
        /// </summary>
        public int GetUniformLocation(string uniformName)
        {
            //  If we don't have the uniform name in the dictionary, get it's
            //  location and add it.
            if (this.uniformNamesToLocations.ContainsKey(uniformName) == false)
            {
                var location = GL.GetUniformLocation(this.ShaderProgramObject, uniformName);
                if (location < 0)
                {
                    throw new Exception($"Uniform<{uniformName}> is not found.");
                }

                this.uniformNamesToLocations[uniformName] = location;
            }

            //  Return the uniform location.
            return this.uniformNamesToLocations[uniformName];
        }

        /// <summary>
        /// Gets the shader program object.
        /// </summary>
        /// <value>
        /// The shader program object.
        /// </value>
        public uint ShaderProgramObject { get; protected set; }

        /// <summary>
        /// A mapping of uniform names to locations. This allows us to very easily specify
        /// uniform data by name, quickly looking up the location first if needed.
        /// </summary>
        private readonly Dictionary<string, int> uniformNamesToLocations = new Dictionary<string, int>();

        /// <summary>
        /// A mapping of attribute names to locations. This allows us to very easily specify
        /// attribute data by name, quickly looking up the location first if needed.
        /// </summary>
        private readonly Dictionary<string, uint> attributeNamesToLocations = new Dictionary<string, uint>();
    }
}
