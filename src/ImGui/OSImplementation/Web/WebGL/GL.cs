using System;
using WebAssembly;

namespace ImGui.OSImplementation.Web
{
    /// <summary>
    /// WebGL function bindings
    /// </summary>
    public static partial class WebGL
    {
        static JSObject gl;

        public static void Init(JSObject webGLRenderingContext)
        {
            WebGL.gl = webGLRenderingContext;
        }

        public static JSObject CreateBuffer()
        {
            return gl.Invoke("createBuffer") as JSObject;
        }

        public static void BindBuffer(uint target, JSObject buffer)
        {
            gl.Invoke("bindBuffer", target, buffer);
        }

        public static void Scissor(int x, int y, int width, int height)
        {
            gl.Invoke("scissor", x, y, width, height);
        }

        public static JSObject CreateVertexArray()
        {
            return gl.Invoke("createVertexArray") as JSObject;
        }

        public static void BindVertexArray(JSObject array)
        {
            gl.Invoke("bindVertexArray", array);
        }

        public static void EnableVertexAttribArray(uint attributeIndex)
        {
            gl.Invoke("enableVertexAttribArray", attributeIndex);
        }

        public static void VertexAttribPointer(uint attribute, int size, uint type, bool normalized, int stride, int offset)
        {
            gl.Invoke("vertexAttribPointer", attribute, size, type, normalized, stride, offset);
        }

        public static void DeleteBuffer(JSObject buffer)
        {
            gl.Invoke("deleteBuffer", buffer);
        }

        public static void ClearColor(float red, float green, float blue, float alpha)
        {
            gl.Invoke("clearColor", red, green, blue, alpha);
        }

        public static void Clear(uint mask)
        {
            gl.Invoke("clear", mask);
        }

        public static object GetParameter(uint pname)
        {
            return gl.Invoke("getParameter", pname);
        }

        public static bool IsEnabled(uint cap)
        {
            return (bool)gl.Invoke("isEnabled", cap);
        }

        public static void Enable(uint cap)
        {
            gl.Invoke("enable", cap);
        }

        public static void BlendEquation(uint mode)
        {
            gl.Invoke("blendEquation", mode);
        }

        public static void BlendFunc(uint sfactor, uint dfactor)
        {
            gl.Invoke("blendFunc", sfactor, dfactor);
        }
        
        public static void BlendFuncSeparate(uint srcRGB, uint dstRGB, uint srcAlpha,uint dstAlpha)
        {
            gl.Invoke("blendFuncSeparate", srcRGB, dstRGB, srcAlpha, dstAlpha);
        }

        public static void Disable(uint cap)
        {
            gl.Invoke("disable", cap);
        }

        public static void Viewport(int x, int y, int width, int height)
        {
            gl.Invoke("viewport", x, y, width, height);
        }

        public static void BufferData(uint target, float[] srcData, uint usage)
        {
            gl.Invoke("bufferData", target, srcData, usage);
        }

        public static void ActiveTexture(uint textureUnit)
        {
            gl.Invoke("activeTexture", textureUnit);
        }

        public static void BindTexture(uint target, JSObject textureObject)
        {
            gl.Invoke("bindTexture", target, textureObject);
        }

        public static JSObject CreateTexture()
        {
            return gl.Invoke("createTexture") as JSObject;
        }

        public static void TexImage2D(uint target, int level, uint internalformat, int width, int height,
            int border, uint format, uint type, byte[] pixels)
        {
            gl.Invoke("texImage2D", target, level, internalformat, width, height,
                border, format, type, pixels);
        }
        
        public static void TexParameteri(uint target, uint pname, int param)
        {
            gl.Invoke("texParameteri", target, pname, param);
        }

        public static void TexParameterf(uint target, uint pname, float param)
        {
            gl.Invoke("texParameterf", target, pname, param);
        }

        public static object GetTexParameter(uint target, uint pname)
        {
            return gl.Invoke("getTexParameter", target, pname);
        }

        public static void DeleteTexture(JSObject textureObject)
        {
            gl.Invoke("deleteTexture", textureObject);
        }

        public static void DrawElements(uint mode, int count, uint type, IntPtr offset)
        {
            gl.Invoke("drawElements", mode, count, type, offset);
        }

        public static void DrawArrays(int mode, int first, int size)
        {
            gl.Invoke("drawArrays", mode, first, size);
        }

        public static void UseProgram(JSObject prog)
        {
            gl.Invoke("useProgram", prog);
        }

        public static void BlendEquationSeparate(uint modeRgb, uint modeAlpha)
        {
            gl.Invoke("blendEquationSeparate", modeRgb, modeAlpha);
        }

        public static void StencilOp(uint fail, uint zfail, uint zpass)
        {
            gl.Invoke("stencilOp", fail, zfail, zpass);
        }

        public static void StencilFunc(uint func, int @ref, uint mask)
        {
            gl.Invoke("stencilFunc", func, @ref, mask);
        }

        public static void ColorMask(bool red, bool green, bool blue, bool alpha)
        {
            gl.Invoke("colorMask", red, green, blue, alpha);
        }

        public static void DepthFunc(uint func)
        {
            gl.Invoke("depthFunc", func);
        }

        public static void ReadPixels(int x, int y, int width, int height, uint format, uint type, byte[] pixels)
        {
            gl.Invoke("readPixels", x, y, width, height, format, type, pixels);
        }

        public static JSObject CreateProgram()
        {
            return gl.Invoke("createProgram") as JSObject;
        }

        public static void AttachShader(JSObject program, JSObject shaderObject)
        {
            gl.Invoke("attachShader", program, shaderObject);
        }

        public static void BindAttribLocation(JSObject program, uint index, string name)
        {
            gl.Invoke("bindAttribLocation", program, index, name);
        }

        public static void LinkProgram(JSObject program)
        {
            gl.Invoke("linkProgram", program);
        }

        public static void DetachShader(JSObject program, JSObject shader)
        {
            gl.Invoke("detachShader", program, shader);
        }

        public static void DeleteProgram(JSObject program)
        {
            gl.Invoke("deleteProgram", program);
        }

        public static int GetAttribLocation(JSObject prog, string attrName)
        {
            return (int)gl.Invoke("getAttribLocation", prog, attrName);
        }

        public static uint GetProgramParameter(JSObject program, uint pname)
        {
            return (uint)gl.Invoke("getProgramParameter", program, pname);
        }

        public static uint GetShaderParameter(JSObject shader, uint pname)
        {
            return (uint)gl.Invoke("getShaderParameter", shader, pname);
        }

        public static string GetProgramInfoLog(JSObject program)
        {
            return gl.Invoke("getProgramInfoLog", program) as string;
        }

        public static void Uniform1(JSObject location, int v0)
        {
            gl.Invoke("uniform1i", location, v0);
        }

        public static void Uniform2(JSObject location, int v0, int v1)
        {
            gl.Invoke("uniform2i", location, v0, v1);
        }

        public static void Uniform3(JSObject location, int v0, int v1, int v2)
        {
            gl.Invoke("uniform3i", location, v0, v1, v2);
        }

        public static void Uniform4(JSObject location, int v0, int v1, int v2, int v3)
        {
            gl.Invoke("uniform4i", location, v0, v1, v2, v3);
        }

        public static void Uniform1(JSObject location, float v0)
        {
            gl.Invoke("uniform1f", location, v0);
        }

        public static void Uniform2(JSObject location, float v0, float v1)
        {
            gl.Invoke("uniform2f", location, v0, v1);
        }

        public static void Uniform3(JSObject location, float v0, float v1, float v2)
        {
            gl.Invoke("uniform3f", location, v0, v1, v2);
        }

        public static void Uniform4(JSObject location, float v0, float v1, float v2, float v3)
        {
            gl.Invoke("uniform4f", location, v0, v1, v2, v3);
        }

        public static void UniformMatrix3(JSObject location, bool transpose, float[] value)
        {
            gl.Invoke("uniformMatrix3fv", location, transpose, value);
        }

        public static void UniformMatrix4(JSObject location, bool transpose, float[] value)
        {
            gl.Invoke("uniformMatrix4fv", location, transpose, value);
        }

        public static JSObject GetUniformLocation(JSObject program, string name)
        {
            return gl.Invoke("getUniformLocation", program, name) as JSObject;
        }

        public static JSObject CreateShader(uint type)
        {
            return gl.Invoke("createShader", type) as JSObject;
        }

        public static void ShaderSource(JSObject shaderObject, string source)
        {
            gl.Invoke("shaderSource", shaderObject, source);
        }

        public static void CompileShader(JSObject shaderObject)
        {
            gl.Invoke("compileShader", shaderObject);
        }

        public static void DeleteShader(JSObject shaderObject)
        {
            gl.Invoke("deleteShader", shaderObject);
        }

        public static string GetShaderInfoLog(JSObject shader)
        {
            return gl.Invoke("getShaderInfoLog", shader) as string;
        }

        public static uint GetError()
        {
            return (uint) gl.Invoke("getError");
        }
    }
}