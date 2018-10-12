using System;
using System.Runtime.InteropServices;

namespace CSharpGLES
{
    public static partial class GL
    {
        private const string GLESv2 = "GLESv2";

        [DllImport(GLESv2, EntryPoint = "glGenBuffers")]
        public static extern void GenBuffers(int v, uint[] buffers);

        [DllImport(GLESv2, EntryPoint = "glBindBuffer")]
        public static extern void BindBuffer(uint target, uint buffer);

        [DllImport(GLESv2, EntryPoint = "glScissor")]
        public static extern void Scissor(int x, int y, int width, int height);

        [DllImport(GLESv2, EntryPoint = "glGenVertexArrays")]
        public static extern void GenVertexArrays(int n, uint[] arrays);

        [DllImport(GLESv2, EntryPoint = "glBindVertexArray")]
        public static extern void BindVertexArray(uint array);

        [DllImport(GLESv2, EntryPoint = "glEnableVertexAttribArray")]
        public static extern void EnableVertexAttribArray(uint index);

        [DllImport(GLESv2, EntryPoint = "glVertexAttribPointer")]
        public static extern void VertexAttribPointer(uint index, int size, uint type, bool normalized, int stride, IntPtr pointer);

        [DllImport(GLESv2, EntryPoint = "glDeleteBuffers")]
        public static extern void DeleteBuffers(int n, uint[] buffers);

        [DllImport(GLESv2, EntryPoint = "glClearColor")]
        public static extern void ClearColor(float red, float green, float blue, float alpha);

        [DllImport(GLESv2, EntryPoint = "glClear")]
        public static extern void Clear(uint mask);

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

        [DllImport(GLESv2, EntryPoint = "glBufferData")]
        public static extern void BufferData(uint target, int size, IntPtr data, uint usage);

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
        
        [DllImport(GLESv2, EntryPoint = "glUseProgram")]
        public static extern void UseProgram(uint program);

        [DllImport(GLESv2, EntryPoint = "glBlendEquationSeparate")]
        public static extern void BlendEquationSeparate(uint modeRGB, uint modeAlpha);

        [DllImport(GLESv2, EntryPoint = "glStencilOp")]
        public static extern void StencilOp(uint fail, uint zfail, uint zpass);

        [DllImport(GLESv2, EntryPoint = "glStencilFunc")]
        public static extern void StencilFunc(uint func, int ref_notkeword, uint mask);

        [DllImport(GLESv2, EntryPoint = "glColorMask")]
        public static extern void ColorMask(bool red, bool green, bool blue, bool alpha);

        [DllImport(GLESv2, EntryPoint = "glDepthFunc")]
        public static extern void DepthFunc(uint func);

        [DllImport(GLESv2, EntryPoint = "glReadPixels")]
        public static extern void ReadPixels(int x, int y, int width, int height, uint format, uint type, byte[] pixels);

        //shader
        [DllImport(GLESv2, EntryPoint = "glCreateProgram")]
        public static extern uint CreateProgram();

        [DllImport(GLESv2, EntryPoint = "glAttachShader")]
        public static extern void AttachShader(uint program, uint shader);
        
        [DllImport(GLESv2, EntryPoint = "glBindAttribLocation")]
        public static extern void BindAttribLocation(uint program, uint index, string name);

        [DllImport(GLESv2, EntryPoint = "glLinkProgram")]
        public static extern void LinkProgram(uint program);

        [DllImport(GLESv2, EntryPoint = "glDetachShader")]
        public static extern void DetachShader(uint program, uint shader);
        
        [DllImport(GLESv2, EntryPoint = "glDeleteProgram")]
        public static extern void DeleteProgram(uint program);

        [DllImport(GLESv2, EntryPoint = "glGetAttribLocation")]
        public static extern int GetAttribLocation(uint program, string name);

        [DllImport(GLESv2, EntryPoint = "glGetProgramiv")]
        public static extern void GetProgram(uint program, uint pname, int[] parameters);

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
        [DllImport(GLESv2, EntryPoint = "glUniform3f")]
        public static extern void Uniform3(int location, float v0, float v1, float v2);
        [DllImport(GLESv2, EntryPoint = "glUniform4f")]
        public static extern void Uniform4(int location, float v0, float v1, float v2, float v3);

        [DllImport(GLESv2, EntryPoint = "glUniformMatrix3fv")]
        public static extern void UniformMatrix3(int location, int count, bool transpose, float[] value);
        [DllImport(GLESv2, EntryPoint = "glUniformMatrix4fv")]
        public static extern void UniformMatrix4(int location, int count, bool transpose, float[] value);

        [DllImport(GLESv2, EntryPoint = "glGetUniformLocation")]
        public static extern int GetUniformLocation(uint program, string name);
        
        [DllImport(GLESv2, EntryPoint = "glCreateShader")]
        public static extern uint CreateShader(uint type);

        [DllImport(GLESv2, EntryPoint = "glShaderSource")]
        public static extern void ShaderSource(uint shader, int count, string[] str, int[] length);//possible problem: paramter str marshal?

        [DllImport(GLESv2, EntryPoint = "glCompileShader")]
        public static extern void CompileShader(uint shader);

        [DllImport(GLESv2, EntryPoint = "glDeleteShader")]
        public static extern void DeleteShader(uint shader);

        [DllImport(GLESv2, EntryPoint = "glGetShaderiv")]
        public static extern void GetShader(uint shader, uint pname, int[] parameters);

        [DllImport(GLESv2, EntryPoint = "glGetShaderInfoLog")]
        public static extern void GetShaderInfoLog(uint shader, int bufSize, IntPtr length, System.Text.StringBuilder infoLog);

        [DllImport(GLESv2, EntryPoint = "glGetError")]
        public static extern uint GetError();
    }
}
