using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using CSharpGLES;
using ImGui.Common;
using ImGui.Common.Primitive;
using ImGui.OSAbstraction.Graphics;

namespace ImGui.OSImplentation.Linux
{
    internal partial class LinuxOpenGLRenderer : IRenderer
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

        #region Materials

        Material m = new Material(
            vertexShader: @"
#version 300 es
uniform mat4 ProjMtx;
in vec2 Position;
in vec2 UV;
in vec4 Color;
out vec2 Frag_UV;
out vec4 Frag_Color;
void main()
{
	Frag_UV = UV;
	Frag_Color = Color;
	gl_Position = ProjMtx * vec4(Position.xy,0,1);
}
",
            fragmentShader: @"
#version 300 es
precision mediump float;
uniform sampler2D Texture;
in vec2 Frag_UV;
in vec4 Frag_Color;
out vec4 Out_Color;
void main()
{
	Out_Color = Frag_Color;
}
"
            );

        Material mExtra = new Material(
            vertexShader: @"
#version 300 es
uniform mat4 ProjMtx;

in vec4 Position;
in vec2 UV;
in vec4 Color;

out vec2 Frag_UV;
out vec4 Frag_Color;

void main()
{
	Frag_UV = UV;
	Frag_Color = Color;
	gl_Position = ProjMtx * vec4(Position.xy,0,1);
}
",
            fragmentShader: @"
#version 300 es
precision mediump float;
in vec2 Frag_UV;
in vec4 Frag_Color;

out vec4 Out_Color;

float inCurve(vec2 uv)
{
	return uv.x * uv.x - uv.y;
}

void main()
{
	float x = inCurve(Frag_UV);

	if(!gl_FrontFacing)
	{
		if (x > 0.) discard;
	}
	else
	{
		if (x < 0.) discard;
	}

	Out_Color = Frag_Color;
}
"
            );

        Material mImage = new Material(
            vertexShader: @"
#version 300 es
uniform mat4 ProjMtx;
in vec2 Position;
in vec2 UV;
in vec4 Color;
out vec2 Frag_UV;
out vec4 Frag_Color;
void main()
{
	Frag_UV = UV;
	Frag_Color = Color;
	gl_Position = ProjMtx * vec4(Position.xy,0,1);
}
",
            fragmentShader: @"
#version 300 es
precision mediump float;
uniform sampler2D Texture;
in vec2 Frag_UV;
in vec4 Frag_Color;
out vec4 Out_Color;
void main()
{
	Out_Color = Frag_Color * texture( Texture, Frag_UV.st);
}
"
            );

        #endregion

        //Helper for some GL functions
        private static readonly int[] IntBuffer = { 0, 0, 0, 0 };

        public void Init(IntPtr windowHandle, Size size)
        {
            if (!CreateEGLContext())
            {
                Debug.WriteLine("Failed to create EGL context.");
                return;
            }

            var nativeWindow = windowHandle;
            if (!CreateEGLSurface(nativeWindow))
            {
                Debug.WriteLine("Failed to create EGL surface.");
                return;
            }

            this.m.Init();
            this.mExtra.Init();
            this.mImage.Init();

            // Other state
            GL.ClearColor(1, 1, 1, 1);

            Utility.CheckGLESError();
        }

        public void Clear()
        {
            GL.Clear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT);
        }

        private static void DoRender(Material material,
            List<DrawCommand> commandBuffer, UnsafeList<DrawIndex> indexBuffer, UnsafeList<DrawVertex> vertexBuffer,
            int width, int height)
        {
            // Backup GL state
            GL.GetIntegerv(GL.GL_CURRENT_PROGRAM, IntBuffer); int last_program = IntBuffer[0];
            Utility.CheckGLESError();
            GL.GetIntegerv(GL.GL_TEXTURE_BINDING_2D, IntBuffer); int last_texture = IntBuffer[0];
            Utility.CheckGLESError();
            GL.GetIntegerv(GL.GL_ACTIVE_TEXTURE, IntBuffer); int last_active_texture = IntBuffer[0];
            Utility.CheckGLESError();
            GL.GetIntegerv(GL.GL_ARRAY_BUFFER_BINDING, IntBuffer); int last_array_buffer = IntBuffer[0];
            Utility.CheckGLESError();
            GL.GetIntegerv(GL.GL_ELEMENT_ARRAY_BUFFER_BINDING, IntBuffer); int last_element_array_buffer = IntBuffer[0];
            Utility.CheckGLESError();
            GL.GetIntegerv(GL.GL_VERTEX_ARRAY_BINDING, IntBuffer); int last_vertex_array = IntBuffer[0];
            Utility.CheckGLESError();
            GL.GetIntegerv(GL.GL_BLEND_SRC_RGB, IntBuffer); int last_blend_src_rgb = IntBuffer[0];
            Utility.CheckGLESError();
            GL.GetIntegerv(GL.GL_BLEND_SRC_ALPHA, IntBuffer); int last_blend_src_alpha = IntBuffer[0];
            Utility.CheckGLESError();
            GL.GetIntegerv(GL.GL_BLEND_DST_RGB, IntBuffer); int last_blend_dst_rgb = IntBuffer[0];
            Utility.CheckGLESError();
            GL.GetIntegerv(GL.GL_BLEND_DST_ALPHA, IntBuffer); int last_blend_dst_alpha = IntBuffer[0];
            Utility.CheckGLESError();
            GL.GetIntegerv(GL.GL_BLEND_EQUATION_RGB, IntBuffer); int last_blend_equation_rgb = IntBuffer[0];
            Utility.CheckGLESError();
            GL.GetIntegerv(GL.GL_BLEND_EQUATION_ALPHA, IntBuffer); int last_blend_equation_alpha = IntBuffer[0];
            Utility.CheckGLESError();
            GL.GetIntegerv(GL.GL_VIEWPORT, IntBuffer); Rect last_viewport = new Rect(IntBuffer[0], IntBuffer[1], IntBuffer[2], IntBuffer[3]);
            Utility.CheckGLESError();
            uint last_enable_blend = GL.IsEnabled(GL.GL_BLEND);
            Utility.CheckGLESError();
            uint last_enable_cull_face = GL.IsEnabled(GL.GL_CULL_FACE);
            Utility.CheckGLESError();
            uint last_enable_depth_test = GL.IsEnabled(GL.GL_DEPTH_TEST);
            Utility.CheckGLESError();
            uint last_enable_scissor_test = GL.IsEnabled(GL.GL_SCISSOR_TEST);
            Utility.CheckGLESError();

            // Setup render state: alpha-blending enabled, no face culling, no depth testing, scissor enabled
            GL.Enable(GL.GL_BLEND);
            GL.BlendEquation(GL.GL_FUNC_ADD_EXT);
            GL.BlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
            GL.Disable(GL.GL_CULL_FACE);
            GL.Disable(GL.GL_DEPTH_TEST);
            GL.Enable(GL.GL_SCISSOR_TEST);

            // Setup viewport, orthographic projection matrix
            GL.Viewport(0, 0, width, height);
            GLM.mat4 ortho_projection = GLM.glm.ortho(0.0f, width, height, 0.0f, -5.0f, 5.0f);
            material.program.Bind();
            material.program.SetUniformMatrix4("ProjMtx", ortho_projection.to_array());//FIXME GLM.mat4.to_array() will generate new array on the heap! This should not be done

            // Send vertex and index data
            GL.BindVertexArray(material.vaoHandle);
            GL.BindBuffer(GL.GL_ARRAY_BUFFER, material.positionVboHandle);
            GL.BufferData(GL.GL_ARRAY_BUFFER, vertexBuffer.Count * Marshal.SizeOf<DrawVertex>(), vertexBuffer.Pointer, GL.GL_STREAM_DRAW);
            GL.BindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, material.elementsHandle);
            GL.BufferData(GL.GL_ELEMENT_ARRAY_BUFFER, indexBuffer.Count * Marshal.SizeOf<DrawIndex>(), indexBuffer.Pointer, GL.GL_STREAM_DRAW);

            Utility.CheckGLESError();

            // Draw
            var indexBufferOffset = IntPtr.Zero;
            foreach (var drawCmd in commandBuffer)
            {
                var clipRect = drawCmd.ClipRect;
                if (drawCmd.TextureData != null)
                {
                    GL.ActiveTexture(GL.GL_TEXTURE0);
                    GL.BindTexture(GL.GL_TEXTURE_2D, (uint)drawCmd.TextureData.GetNativeTextureId());
                }
                GL.Scissor((int) clipRect.X, (int) (height - clipRect.Height - clipRect.Y), (int) clipRect.Width, (int) clipRect.Height);
                GL.DrawElements(GL.GL_TRIANGLES, drawCmd.ElemCount, GL.GL_UNSIGNED_INT, indexBufferOffset);
                indexBufferOffset = IntPtr.Add(indexBufferOffset, drawCmd.ElemCount * Marshal.SizeOf<DrawIndex>());
            }

            Utility.CheckGLESError();

            // Restore modified GL state
            GL.UseProgram((uint)last_program);
            GL.ActiveTexture((uint)last_active_texture);
            GL.BindTexture(GL.GL_TEXTURE_2D, (uint)last_texture);
            GL.BindVertexArray((uint)last_vertex_array);
            GL.BindBuffer(GL.GL_ARRAY_BUFFER, (uint)last_array_buffer);
            GL.BindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, (uint)last_element_array_buffer);
            GL.BlendEquationSeparate((uint)last_blend_equation_rgb, (uint)last_blend_equation_alpha);
            GL.BlendFunc((uint)last_blend_src_rgb, (uint)last_blend_dst_rgb);
            GL.BlendFunc((uint)last_blend_src_alpha, (uint)last_blend_dst_alpha);
            if (last_enable_blend == GL.GL_TRUE) GL.Enable(GL.GL_BLEND); else GL.Disable(GL.GL_BLEND);
            if (last_enable_cull_face == GL.GL_TRUE) GL.Enable(GL.GL_CULL_FACE); else GL.Disable(GL.GL_CULL_FACE);
            if (last_enable_depth_test == GL.GL_TRUE) GL.Enable(GL.GL_DEPTH_TEST); else GL.Disable(GL.GL_DEPTH_TEST);
            if (last_enable_scissor_test == GL.GL_TRUE) GL.Enable(GL.GL_SCISSOR_TEST); else GL.Disable(GL.GL_SCISSOR_TEST);
            GL.Viewport((int)last_viewport.X, (int)last_viewport.Y, (int)last_viewport.Width, (int)last_viewport.Height);
        }

        public void RenderDrawList(DrawList drawList, int width, int height)
        {
            DoRender(this.m, drawList.ShapeMesh.CommandBuffer, drawList.ShapeMesh.IndexBuffer, drawList.ShapeMesh.VertexBuffer, width, height);
            //DoRender(mExtra, drawList.BezierBuffer.CommandBuffer, drawList.BezierBuffer.IndexBuffer, drawList.BezierBuffer.VertexBuffer, width, height);
            DoRender(this.mImage, drawList.ImageMesh.CommandBuffer, drawList.ImageMesh.IndexBuffer, drawList.ImageMesh.VertexBuffer, width, height);
        }

        public void SwapBuffers()
        {
            eglSwapBuffers(this.display, this.surface);
        }

        public void ShutDown()
        {
            this.m.ShutDown();
            this.mExtra.ShutDown();

            DestroyEGL();
        }
    }
}
