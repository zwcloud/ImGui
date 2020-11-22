using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ImGui.OSAbstraction.Graphics;
using WebAssembly;
using GL = ImGui.OSImplementation.Web.WebGL;

namespace ImGui.OSImplementation.Web
{
    internal partial class WebGLRenderer : IRenderer
    {
        Material shapeMaterial = new Material(
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

        Material imageMaterial = new Material(
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

        private readonly Material glyphMaterial = new Material(
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
in vec2 Frag_UV;
in vec4 Frag_Color;
out vec4 Out_Color;
void main()
{
	if (Frag_UV.s * Frag_UV.s - Frag_UV.t > 0.0)
	{
		discard;
	}
	Out_Color = Frag_Color;
}
"
    );

        //Helper for some GL functions
        private static readonly int[] IntBuffer = { 0, 0, 0, 0 };
        private static readonly float[] FloatBuffer = { 0, 0, 0, 0 };
        private static readonly uint[] UIntBuffer = { 0, 0, 0, 0 };

        public void Init(IntPtr windowHandle, Size size)
        {
            //CreateOpenGLContext((IntPtr)windowHandle);//done by the browser

            this.shapeMaterial.Init();
            this.imageMaterial.Init();
            this.glyphMaterial.Init();

            // Other state
            GL.Disable(GL.GL_CULL_FACE);
            GL.Disable(GL.GL_DEPTH_TEST);
            GL.DepthFunc(GL.GL_NEVER);
            GL.Enable(GL.GL_SCISSOR_TEST);

            Utility.CheckGLESError();
        }

        public void Bind()
        {
            //nothing to do:
            //a web browser will call `MakeCurrent` internally for each WebGLRenderingContext
        }

        public void Clear(Color clearColor)
        {
            GL.ClearColor((float)clearColor.R, (float)clearColor.G, (float)clearColor.B, (float)clearColor.A);
            GL.Clear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT | GL.GL_STENCIL_BUFFER_BIT);
        }

        public void DrawMeshes(int width, int height, (Mesh shapeMesh, Mesh imageMesh, TextMesh textMesh) meshes)
        {
            DrawMesh(this.shapeMaterial, meshes.shapeMesh, width, height);
            DrawMesh(this.imageMaterial, meshes.imageMesh, width, height);

            DrawTextMesh(meshes.textMesh, width, height);
        }

        private static void DrawMesh(Material material, Mesh mesh, int width, int height)
        {
            // Backup GL state
            JSObject last_program = (JSObject)GL.GetParameter(GL.GL_CURRENT_PROGRAM);
            JSObject last_texture = (JSObject)GL.GetParameter(GL.GL_TEXTURE_BINDING_2D);
            uint last_active_texture = (uint)GL.GetParameter(GL.GL_ACTIVE_TEXTURE);
            JSObject last_array_buffer = (JSObject)GL.GetParameter(GL.GL_ARRAY_BUFFER_BINDING);
            JSObject last_element_array_buffer = (JSObject)GL.GetParameter(GL.GL_ELEMENT_ARRAY_BUFFER_BINDING);
            JSObject last_vertex_array = (JSObject)GL.GetParameter(GL.GL_VERTEX_ARRAY_BINDING);
            uint last_blend_src_alpha = (uint)GL.GetParameter(GL.GL_BLEND_SRC_ALPHA);
            uint last_blend_src_rgb = (uint)GL.GetParameter(GL.GL_BLEND_SRC_RGB);
            uint last_blend_dst_alpha = (uint)GL.GetParameter(GL.GL_BLEND_DST_ALPHA);
            uint last_blend_dst_rgb = (uint)GL.GetParameter(GL.GL_BLEND_DST_RGB);
            uint last_blend_equation_rgb = (uint)GL.GetParameter(GL.GL_BLEND_EQUATION_RGB);
            uint last_blend_equation_alpha = (uint)GL.GetParameter(GL.GL_BLEND_EQUATION_ALPHA);
            float[] float4_clear_color = (float[])GL.GetParameter(GL.GL_COLOR_CLEAR_VALUE);
            Debug.Assert(float4_clear_color.Length == 4);
            float last_clear_color_r = float4_clear_color[0];
            float last_clear_color_g = float4_clear_color[1];
            float last_clear_color_b = float4_clear_color[2];
            float last_clear_color_a = float4_clear_color[3];
            int[] int4_viewport = (int[])GL.GetParameter(GL.GL_VIEWPORT);
            Rect last_viewport = new Rect(int4_viewport[0], int4_viewport[1], int4_viewport[2], int4_viewport[3]);
            bool last_enable_blend = GL.IsEnabled(GL.GL_BLEND);
            bool last_enable_cull_face = GL.IsEnabled(GL.GL_CULL_FACE);
            bool last_enable_depth_test = GL.IsEnabled(GL.GL_DEPTH_TEST);
            bool last_enable_scissor_test = GL.IsEnabled(GL.GL_SCISSOR_TEST);
            int[] int4_sessor = (int[])GL.GetParameter(GL.GL_SCISSOR_BOX);
            int last_sessor_rect_x = int4_sessor[0];
            int last_sessor_rect_y = int4_sessor[1];
            int last_sessor_rect_width = int4_sessor[2];
            int last_sessor_rect_height = int4_sessor[3];

            // Setup render state: alpha-blending enabled, no face culling, no depth testing, scissor enabled
            GL.Enable(GL.GL_BLEND);
            GL.BlendEquation(GL.GL_FUNC_ADD_EXT);
            GL.BlendFunc(GL.GL_SRC_ALPHA, GL.GL_ONE_MINUS_SRC_ALPHA);
            GL.Disable(GL.GL_CULL_FACE);
            GL.Disable(GL.GL_DEPTH_TEST);
            GL.Enable(GL.GL_SCISSOR_TEST);

            // Setup viewport, orthographic projection matrix
            GL.Viewport(0, 0, width, height);
            float L = 0;
            float R = 0 + width;
            float T = 0;
            float B = 0 + height;
            float[] ortho_projection = new float[16]
            {
                2.0f/(R-L),   0.0f,         0.0f,   0.0f,
                0.0f,         2.0f/(T-B),   0.0f,   0.0f,
                0.0f,         0.0f,        -1.0f,   0.0f,
                (R+L)/(L-R),  (T+B)/(B-T),  0.0f,   1.0f,
            };
            material.program.Bind();
            material.program.SetUniformMatrix4("ProjMtx", ortho_projection);

            // Send vertex and index data
            List<DrawCommand> commandBuffer = mesh.CommandBuffer;
            VertexBuffer vertexBuffer = mesh.VertexBuffer;
            IndexBuffer indexBuffer = mesh.IndexBuffer;
            GL.BindVertexArray(material.vaoHandle);
            GL.BindBuffer(GL.GL_ARRAY_BUFFER, material.positionVboHandle);
            //GL.BufferData(GL.GL_ARRAY_BUFFER, vertexBuffer.data, GL.GL_STREAM_DRAW);
            GL.BindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, material.elementsHandle);
            //GL.BufferData(GL.GL_ELEMENT_ARRAY_BUFFER, indexBuffer.data, GL.GL_STREAM_DRAW);

            Utility.CheckGLESError();

            // Draw
            var indexBufferOffset = IntPtr.Zero;
            foreach (var drawCmd in commandBuffer)
            {
                var clipRect = drawCmd.ClipRect;
                if (drawCmd.TextureData != null)
                {
                    GL.ActiveTexture(GL.GL_TEXTURE0);
                    GL.BindTexture(GL.GL_TEXTURE_2D, (JSObject)drawCmd.TextureData.GetNativeTextureObject());
                }
                GL.Scissor((int) clipRect.X, (int) (height - clipRect.Height - clipRect.Y), (int) clipRect.Width, (int) clipRect.Height);
                GL.DrawElements(GL.GL_TRIANGLES, drawCmd.ElemCount, GL.GL_UNSIGNED_INT, indexBufferOffset);
                indexBufferOffset = IntPtr.Add(indexBufferOffset, drawCmd.ElemCount * Marshal.SizeOf<DrawIndex>());
            }

            Utility.CheckGLESError();

            // Restore modified GL state
            GL.UseProgram(last_program);
            GL.ActiveTexture(last_active_texture);
            GL.BindTexture(GL.GL_TEXTURE_2D, last_texture);
            GL.BindVertexArray(last_vertex_array);
            GL.BindBuffer(GL.GL_ARRAY_BUFFER, last_array_buffer);
            GL.BindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, last_element_array_buffer);
            GL.BlendEquationSeparate((uint)last_blend_equation_rgb, (uint)last_blend_equation_alpha);
            GL.BlendFuncSeparate(last_blend_src_rgb, last_blend_dst_rgb, last_blend_src_alpha, last_blend_dst_alpha);
            if (last_enable_blend) GL.Enable(GL.GL_BLEND); else GL.Disable(GL.GL_BLEND);
            if (last_enable_cull_face) GL.Enable(GL.GL_CULL_FACE); else GL.Disable(GL.GL_CULL_FACE);
            if (last_enable_depth_test) GL.Enable(GL.GL_DEPTH_TEST); else GL.Disable(GL.GL_DEPTH_TEST);
            if (last_enable_scissor_test) GL.Enable(GL.GL_SCISSOR_TEST); else GL.Disable(GL.GL_SCISSOR_TEST);
            GL.ClearColor(last_clear_color_r, last_clear_color_g, last_clear_color_b, last_clear_color_a);
            GL.Viewport((int)last_viewport.X, (int)last_viewport.Y, (int)last_viewport.Width, (int)last_viewport.Height);
            GL.Scissor(last_sessor_rect_x, last_sessor_rect_y, last_sessor_rect_width, last_sessor_rect_height);
        }

        private void DrawTextMesh(TextMesh textMesh, int width, int height)
        {
            // Backup GL state
            JSObject last_program = (JSObject)GL.GetParameter(GL.GL_CURRENT_PROGRAM);
            JSObject last_texture = (JSObject)GL.GetParameter(GL.GL_TEXTURE_BINDING_2D);
            uint last_active_texture = (uint)GL.GetParameter(GL.GL_ACTIVE_TEXTURE);
            JSObject last_array_buffer = (JSObject)GL.GetParameter(GL.GL_ARRAY_BUFFER_BINDING);
            JSObject last_element_array_buffer = (JSObject)GL.GetParameter(GL.GL_ELEMENT_ARRAY_BUFFER_BINDING);
            JSObject last_vertex_array = (JSObject)GL.GetParameter(GL.GL_VERTEX_ARRAY_BINDING);
            uint last_blend_src_alpha = (uint)GL.GetParameter(GL.GL_BLEND_SRC_ALPHA);
            uint last_blend_src_rgb = (uint)GL.GetParameter(GL.GL_BLEND_SRC_RGB);
            uint last_blend_dst_alpha = (uint)GL.GetParameter(GL.GL_BLEND_DST_ALPHA);
            uint last_blend_dst_rgb = (uint)GL.GetParameter(GL.GL_BLEND_DST_RGB);
            uint last_blend_equation_rgb = (uint)GL.GetParameter(GL.GL_BLEND_EQUATION_RGB);
            uint last_blend_equation_alpha = (uint)GL.GetParameter(GL.GL_BLEND_EQUATION_ALPHA);
            float[] float4_clear_color = (float[])GL.GetParameter(GL.GL_COLOR_CLEAR_VALUE);
            Debug.Assert(float4_clear_color.Length == 4);
            float last_clear_color_r = float4_clear_color[0];
            float last_clear_color_g = float4_clear_color[1];
            float last_clear_color_b = float4_clear_color[2];
            float last_clear_color_a = float4_clear_color[3];
            int[] int4_viewport = (int[])GL.GetParameter(GL.GL_VIEWPORT);
            Rect last_viewport = new Rect(int4_viewport[0], int4_viewport[1], int4_viewport[2], int4_viewport[3]);
            bool last_enable_blend = GL.IsEnabled(GL.GL_BLEND);
            bool last_enable_cull_face = GL.IsEnabled(GL.GL_CULL_FACE);
            bool last_enable_depth_test = GL.IsEnabled(GL.GL_DEPTH_TEST);
            bool last_enable_scissor_test = GL.IsEnabled(GL.GL_SCISSOR_TEST);
            int[] int4_sessor = (int[])GL.GetParameter(GL.GL_SCISSOR_BOX);
            int last_sessor_rect_x = int4_sessor[0];
            int last_sessor_rect_y = int4_sessor[1];
            int last_sessor_rect_width = int4_sessor[2];
            int last_sessor_rect_height = int4_sessor[3];

            float L = 0;
            float R = 0 + width;
            float T = 0;
            float B = 0 + height;
            float[] ortho_projection = new float[16]
            {
                2.0f/(R-L),   0.0f,         0.0f,   0.0f,
                0.0f,         2.0f/(T-B),   0.0f,   0.0f,
                0.0f,         0.0f,        -1.0f,   0.0f,
                (R+L)/(L-R),  (T+B)/(B-T),  0.0f,   1.0f,
            };
            GL.Viewport(0, 0, width, height);

            var material = this.glyphMaterial;
            var vertexBuffer = textMesh.VertexBuffer;
            var indexBuffer = textMesh.IndexBuffer;

            material.program.Bind();
            material.program.SetUniformMatrix4("ProjMtx", ortho_projection);

            // Send vertex data
            GL.BindVertexArray(material.vaoHandle);
            GL.BindBuffer(GL.GL_ARRAY_BUFFER, material.positionVboHandle);
            //var vertexBuf = new Memory<float>(vertexBuffer.Data);
            //GL.BufferData(GL.GL_ARRAY_BUFFER,  vertexBuf, GL.GL_STREAM_DRAW);
            GL.BindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, material.elementsHandle);
            //GL.BufferData(GL.GL_ELEMENT_ARRAY_BUFFER, indexBuffer.data, GL.GL_STREAM_DRAW);

            Utility.CheckWebGLError();

            GL.Enable(GL.GL_STENCIL_TEST);
            var indexBufferOffset = IntPtr.Zero;
            foreach (var drawCmd in textMesh.Commands)
            {
                var clipRect = drawCmd.ClipRect;
                {
                    // Draw text mesh to stencil buffer
                    GL.StencilOp(GL.GL_KEEP, GL.GL_KEEP, GL.GL_INVERT);
                    GL.StencilFunc(GL.GL_ALWAYS, 1, 1);

                    GL.ColorMask(false, false, false, false);//only draw to stencil buffer
                    GL.Clear(GL.GL_STENCIL_BUFFER_BIT); //clear stencil buffer to 0

                    GL.Scissor((int)clipRect.X, (int)(height - clipRect.Height - clipRect.Y), (int)clipRect.Width, (int)clipRect.Height);
                    GL.DrawElements(GL.GL_TRIANGLES, drawCmd.ElemCount, GL.GL_UNSIGNED_INT, indexBufferOffset);

                    Utility.CheckWebGLError();

                    // Draw text mesh againest stencil buffer
                    GL.StencilFunc(GL.GL_EQUAL, 1, 1);
                    GL.StencilOp(GL.GL_KEEP, GL.GL_KEEP, GL.GL_KEEP);
                    GL.ColorMask(true, true, true, true);

                    GL.DrawElements(GL.GL_TRIANGLES, drawCmd.ElemCount, GL.GL_UNSIGNED_INT, indexBufferOffset);
                }
                indexBufferOffset = IntPtr.Add(indexBufferOffset, drawCmd.ElemCount * Marshal.SizeOf<DrawIndex>());
            }
            GL.Disable(GL.GL_STENCIL_TEST);

            // Restore modified GL state
            GL.UseProgram(last_program);
            GL.ActiveTexture(last_active_texture);
            GL.BindTexture(GL.GL_TEXTURE_2D, last_texture);
            GL.BindVertexArray(last_vertex_array);
            GL.BindBuffer(GL.GL_ARRAY_BUFFER, last_array_buffer);
            GL.BindBuffer(GL.GL_ELEMENT_ARRAY_BUFFER, last_element_array_buffer);
            GL.BlendEquationSeparate((uint)last_blend_equation_rgb, (uint)last_blend_equation_alpha);
            GL.BlendFuncSeparate(last_blend_src_rgb, last_blend_dst_rgb, last_blend_src_alpha, last_blend_dst_alpha);
            if (last_enable_blend) GL.Enable(GL.GL_BLEND); else GL.Disable(GL.GL_BLEND);
            if (last_enable_cull_face) GL.Enable(GL.GL_CULL_FACE); else GL.Disable(GL.GL_CULL_FACE);
            if (last_enable_depth_test) GL.Enable(GL.GL_DEPTH_TEST); else GL.Disable(GL.GL_DEPTH_TEST);
            if (last_enable_scissor_test) GL.Enable(GL.GL_SCISSOR_TEST); else GL.Disable(GL.GL_SCISSOR_TEST);
            GL.ClearColor(last_clear_color_r, last_clear_color_g, last_clear_color_b, last_clear_color_a);
            GL.Viewport((int)last_viewport.X, (int)last_viewport.Y, (int)last_viewport.Width, (int)last_viewport.Height);
            GL.Scissor(last_sessor_rect_x, last_sessor_rect_y, last_sessor_rect_width, last_sessor_rect_height);
        }

        public void Unbind()
        {
            //a web browser will call `MakeCurrent` internally for each WebGLRenderingContext
        }

        public void ShutDown()
        {
            this.shapeMaterial.ShutDown();
            this.imageMaterial.ShutDown();
            this.glyphMaterial.ShutDown();
        }

        public byte[] GetRawBackBuffer(out int width, out int height)
        {
            Int32[] buffer4 = (Int32[])GL.GetParameter(GL.GL_VIEWPORT);
            int viewportX = buffer4[0];
            int viewportY = buffer4[1];
            int viewportWidth = width = buffer4[2];
            int viewportHeight = height = buffer4[3];
            var pixels = new byte[viewportWidth * viewportHeight * 4];
            GL.ReadPixels(viewportX, viewportY, viewportWidth, viewportHeight, GL.GL_RGBA, GL.GL_UNSIGNED_BYTE, pixels);
            return pixels;
        }
    }
}
