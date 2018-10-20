using System;
using System.Runtime.InteropServices;
using WebAssembly;

namespace ImGui.OSImplentation.Web
{
    /// <summary>
    /// WebGL function bindings
    /// </summary>
    public static partial class GL
    {
        static JSObject _gl;

        public static void Init(JSObject webGLRenderingContext)
        {
            GL._gl = webGLRenderingContext;
        }

        public const int VERTEX_SHADER = 35633;
        public const int FRAGMENT_SHADER = 35632;

        public static JSObject CreateBuffer()
        {
            return _gl.Invoke("createBuffer") as JSObject;
        }

        public const int ARRAY_BUFFER = 34962;

        public const int STATIC_DRAW = 35044;

        public const int FLOAT = 5126;

        public const int COLOR_BUFFER_BIT = 16384;

        public const int TRIANGLE_STRIP = 5;

        public const int COMPILE_STATUS = 35713;

        private const string GLESv2 = "GLESv2";

        //no glGenBuffers existed in WebGL

        //[DllImport(GLESv2, EntryPoint = "glBindBuffer")]
        public static void BindBuffer(int target, JSObject buffer)
        {
            _gl.Invoke("bindBuffer", target, buffer);
        }

        [DllImport(GLESv2, EntryPoint = "glScissor")]
        public static extern void Scissor(int x, int y, int width, int height);

        [DllImport(GLESv2, EntryPoint = "glGenVertexArrays")]
        public static extern void GenVertexArrays(int n, uint[] arrays);

        [DllImport(GLESv2, EntryPoint = "glBindVertexArray")]
        public static extern void BindVertexArray(uint array);

        //[DllImport(GLESv2, EntryPoint = "glEnableVertexAttribArray")]
        public static void EnableVertexAttribArray(int attributeIndex)
        {
            _gl.Invoke("enableVertexAttribArray", attributeIndex);
        }

        //[DllImport(GLESv2, EntryPoint = "glVertexAttribPointer")]
        public static void VertexAttribPointer(int attribute, int size, int type, bool normalized, int stride, int offset)
        {
            _gl.Invoke("vertexAttribPointer", attribute, size, type, normalized, stride, offset);
        }

        [DllImport(GLESv2, EntryPoint = "glDeleteBuffers")]
        public static extern void DeleteBuffers(int n, uint[] buffers);

        //[DllImport(GLESv2, EntryPoint = "glClearColor")]
        public static void ClearColor(float red, float green, float blue, float alpha)
        {
            _gl.Invoke("clearColor", red, green, blue, alpha);
        }

        //[DllImport(GLESv2, EntryPoint = "glClear")]
        public static void Clear(int mask)
        {
            _gl.Invoke("clear", mask);
        }

        [DllImport(GLESv2, EntryPoint = "glGetIntegerv")]
        public static extern void GetIntegerv(uint pname, int[] params_notkeyword);

        [DllImport(GLESv2, EntryPoint = "glGetFloatv")]
        public static extern void GetFloatv(uint pname, float[] params_notkeyword);

        [DllImport(GLESv2, EntryPoint = "glIsEnabled")]
        public static extern byte IsEnabled(uint cap);

        [DllImport(GLESv2, EntryPoint = "glEnable")]
        public static extern void Enable(uint cap);

        [DllImport(GLESv2, EntryPoint = "glBlendEquation")]
        public static extern void BlendEquation(uint mode);

        [DllImport(GLESv2, EntryPoint = "glBlendFunc")]
        public static extern void BlendFunc(uint sfactor, uint dfactor);

        [DllImport(GLESv2, EntryPoint = "glDisable")]
        public static extern void Disable(uint cap);

        [DllImport(GLESv2, EntryPoint = "glViewport")]
        public static extern void Viewport(int x, int y, int width, int height);

        //[DllImport(GLESv2, EntryPoint = "glBufferData")]
        public static void BufferData(int target, float[] srcData, int usage)
        {
            _gl.Invoke("bufferData", target, srcData, usage);
        }

        [DllImport(GLESv2, EntryPoint = "glActiveTexture")]
        public static extern void ActiveTexture(uint texture);

        [DllImport(GLESv2, EntryPoint = "glBindTexture")]
        public static extern void BindTexture(uint target, uint texture);

        [DllImport(GLESv2, EntryPoint = "glGenTextures")]
        public static extern void GenTextures(int n, uint[] textures);

        [DllImport(GLESv2, EntryPoint = "glTexImage2D")]
        public static extern void TexImage2D(uint target, int level, uint internalformat, int width, int height, int border, uint format, uint type, IntPtr pixels);

        [DllImport(GLESv2, EntryPoint = "glTexParameteri")]
        public static extern void TexParameteri(uint target, uint pname, int param);

        [DllImport(GLESv2, EntryPoint = "glDeleteTexture")]
        public static extern void DeleteTextures(int n, uint[] textures);

        [DllImport(GLESv2, EntryPoint = "glDrawElements")]
        public static extern void DrawElements(uint mode, int count, uint type, IntPtr indices);

        public static void DrawArrays(int mode, int first, int size)
        {
            _gl.Invoke("drawArrays", mode, first, size);
        }

        //[DllImport(GLESv2, EntryPoint = "glUseProgram")]
        public static void UseProgram(JSObject prog)
        {
            _gl.Invoke("useProgram", prog);
        }

        [DllImport(GLESv2, EntryPoint = "glBlendEquationSeparate")]
        public static extern void BlendEquationSeparate(uint modeRgb, uint modeAlpha);

        [DllImport(GLESv2, EntryPoint = "glStencilOp")]
        public static extern void StencilOp(uint fail, uint zfail, uint zpass);

        [DllImport(GLESv2, EntryPoint = "glStencilFunc")]
        public static extern void StencilFunc(uint func, int refNotkeword, uint mask);

        [DllImport(GLESv2, EntryPoint = "glColorMask")]
        public static extern void ColorMask(bool red, bool green, bool blue, bool alpha);

        [DllImport(GLESv2, EntryPoint = "glDepthFunc")]
        public static extern void DepthFunc(uint func);

        [DllImport(GLESv2, EntryPoint = "glReadPixels")]
        public static extern void ReadPixels(int x, int y, int width, int height, uint format, uint type, byte[] pixels);

        //shader
        //[DllImport(GLESv2, EntryPoint = "glCreateProgram")]
        public static JSObject CreateProgram()
        {
            return _gl.Invoke("createProgram") as JSObject;
        }

        //[DllImport(GLESv2, EntryPoint = "glAttachShader")]
        public static void AttachShader(JSObject program, JSObject shaderObject)
        {
            _gl.Invoke("attachShader", program, shaderObject);
        }

        [DllImport(GLESv2, EntryPoint = "glBindAttribLocation")]
        public static extern void BindAttribLocation(uint program, uint index, string name);

        //[DllImport(GLESv2, EntryPoint = "glLinkProgram")]
        public static void LinkProgram(JSObject program)
        {
            _gl.Invoke("linkProgram", program);
        }

        [DllImport(GLESv2, EntryPoint = "glDetachShader")]
        public static extern void DetachShader(uint program, uint shader);

        [DllImport(GLESv2, EntryPoint = "glDeleteProgram")]
        public static extern void DeleteProgram(uint program);

        //[DllImport(GLESv2, EntryPoint = "glGetAttribLocation")]
        public static int GetAttribLocation(JSObject prog, string attrName)
        {
            return (int)_gl.Invoke("getAttribLocation", prog, attrName);
        }

        [DllImport(GLESv2, EntryPoint = "glGetProgramiv")]
        public static extern void GetProgram(uint program, uint pname, int[] parameters);
        
        public static bool GetShaderParameter(JSObject shader, int pname)
        {
            return (bool)_gl.Invoke("getShaderParameter", shader, pname);
        }

        [DllImport(GLESv2, EntryPoint = "glGetProgramInfoLog")]
        public static extern void GetProgramInfoLog(uint program, int bufSize, IntPtr length, System.Text.StringBuilder infoLog);


        [DllImport(GLESv2, EntryPoint = "glUniform1i")]
        public static extern void Uniform1(int location, int v0);
        [DllImport(GLESv2, EntryPoint = "glUniform2i")]
        public static extern void Uniform2(int location, int v0, int v1);
        [DllImport(GLESv2, EntryPoint = "glUniform3i")]
        public static extern void Uniform3(int location, int v0, int v1, int v2);
        [DllImport(GLESv2, EntryPoint = "glUniform4i")]
        public static extern void Uniform4(int location, int v0, int v1, int v2, int v3);

        [DllImport(GLESv2, EntryPoint = "glUniform1f")]
        public static extern void Uniform1(int location, float v0);
        [DllImport(GLESv2, EntryPoint = "glUniform2f")]
        public static extern void Uniform2(int location, float v0, float v1);
        //[DllImport(GLESv2, EntryPoint = "glUniform3f")]
        public static void Uniform3(JSObject location, float v0, float v1, float v2)
        {
            _gl.Invoke("uniform3f", location, v0, v1, v2);
        }
        [DllImport(GLESv2, EntryPoint = "glUniform4f")]
        public static extern void Uniform4(int location, float v0, float v1, float v2, float v3);

        [DllImport(GLESv2, EntryPoint = "glUniformMatrix3fv")]
        public static extern void UniformMatrix3(int location, int count, bool transpose, float[] value);
        [DllImport(GLESv2, EntryPoint = "glUniformMatrix4fv")]
        public static extern void UniformMatrix4(int location, int count, bool transpose, float[] value);

        //[DllImport(GLESv2, EntryPoint = "glGetUniformLocation")]
        public static JSObject GetUniformLocation(JSObject program, string name)
        {
            return _gl.Invoke("getUniformLocation", program, name) as JSObject;
        }

        //[DllImport(GLESv2, EntryPoint = "glCreateShader")]
        public static JSObject CreateShader(int type)
        {
            return _gl.Invoke("createShader", type) as JSObject;
        }

        //[DllImport(GLESv2, EntryPoint = "glShaderSource")]
        public static void ShaderSource(JSObject shaderObject, string source)
        {
            _gl.Invoke("shaderSource", shaderObject, source);
        }

        //[DllImport(GLESv2, EntryPoint = "glCompileShader")]
        public static void CompileShader(JSObject shaderObject)
        {
            _gl.Invoke("compileShader", shaderObject);
        }

        [DllImport(GLESv2, EntryPoint = "glDeleteShader")]
        public static extern void DeleteShader(uint shader);

        [DllImport(GLESv2, EntryPoint = "glGetShaderiv")]
        public static extern void GetShader(uint shader, uint pname, int[] parameters);

        //[DllImport(GLESv2, EntryPoint = "glGetShaderInfoLog")]
        public static string GetShaderInfoLog(JSObject shader)
        {
            return _gl.Invoke("getShaderInfoLog", shader) as string;
        }

        [DllImport(GLESv2, EntryPoint = "glGetError")]
        public static extern uint GetError();
    }
}